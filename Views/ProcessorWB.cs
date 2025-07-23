using GlobalModule;
using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Views
{
    //---=== Робота в WEB-Browser ===---
    public interface IProcessorWB
    {

        WBState GoTo_URL(string aURL);                                                  // ПЕРЕХІД  до сторінки з тестами         
        Dictionary<string, string> Get_Questions_FromWeb(ref List<string> aListErrors); // Отримання списку запитань з Web

        // Метоод який ВВОДИТЬ ВІДПОВІДІ(дані) КОРИСТУВАЧА на сайті, повертає результат та зберінає PDF з сторокою результату
        string Enter_User_Responses(UserData aUserData, string aSavePDF_FilePath, ref List<string> aListErrors);    // Введення відповідей користувача (повертаэ психотип та зберыгаэ PDF)

        // Зупинка WB (при завершенні програми)
        void Stop();
        event Action<int> Event_ProcesStop;        // Подія зупинення програми 
        //private void PageSaveAsPdf(String aFilePath); // Зберігаєм сотінку в форматі PDF

    }
    public class ProcessorWB : IProcessorWB
    {
        #region ---=== ПОЛЯ ===---

        private readonly WebView2 _WB;                              // WebBrowser для відображення та взаємодії з сайтом
        private bool _Stop_WB;                                      // Признак зупинки програми
        private bool _WB_Stoped;                                    // Процес програми зупинено
        private readonly HtmlAgilityPack.HtmlDocument _Current_HTMLDoc;      // Поточний HTML документ для аналізу.

        private bool _WB_LoadCompleted = false;     // Признак завершення завантаження сторінки
        WBState _WBState = WBState.stPage_Unknown;  // СТАН БРОУЗЕРА - не визначена сторынка

        private readonly System.Windows.Forms.Timer _Timer_TestForm;    // Таймер для прохода форми з тестом (сторінка завантажується за допомогою Ajax)
        private int _TimerSecond = 0;                           // Скільки секунд пройшло після запуску таймера
        #endregion ---=== ПОЛЯ ===---

        #region ---=== Прокидання подій ===---
        //------------------------------------
        public event Action<int> Event_ProcesStop;        // Подія зупинення програми 
        //------------------------------------
        #endregion ---=== Прокидання подій ===---

        #region ---=== Реалізація інтерфесу ===---

        // ПЕРЕХІД  до сторінки з тестами 
        public WBState GoTo_URL(string aURL)
        {
            _WBState = WBState.stPage_Unknown;
            if (!string.IsNullOrEmpty(aURL))
            {

                //---=== Признак завершення завантаження сторінки ===---
                _WB_LoadCompleted = false;
                _WB.Visible = true;
                // ПЕРЕХІД НА СТОРІНКУ(за посиланням) Якщо в броузері сторінка вже відкрита
                if (this._WB.Source != null && _WB.CoreWebView2 != null && !_Stop_WB)// 
                {
                    this._WB.CoreWebView2.Profile.ClearBrowsingDataAsync(); // Очищаэм дані профіля
                    this._WB.CoreWebView2.CookieManager.DeleteAllCookies(); // Очищаэм Cookies
                    this._WB.CoreWebView2.Navigate(aURL);                   // Переходим за посиланням
                }
                else
                    if (!_Stop_WB)
                    this._WB.Source = new System.Uri(aURL);                 // Переходим за посиланням

                //---=== Поки сторінка не завантажилась - очікуєм ===---
                while (!_WB_LoadCompleted && !_Stop_WB)
                    Application.DoEvents();
                if (_Stop_WB)
                    _WB_Stoped = true;
            }
            return _WBState;
        }

        // Отримання списку запитань з Web
        public Dictionary<string, string> Get_Questions_FromWeb(ref List<string> aListErrors) //aLoadErrors - Помилки при завантаженні запитань
        {
            Dictionary<string, string> vQuestions = new Dictionary<string, string>();   // Список запитань з нумерацією

            // ---=== РЕАЛІЗАЦІЯ ЗАВАНТАЖЕННЯ ЗАПИТАНЬ з WEB ===---
            bool vStop = false;
            // Поки стан СТАН БРОУЗЕРА != Форма ТЕСТУВАННЯ ЗАВЕРШЕНО або не зупинено користувачем
            while (_WBState != WBState.stPage_Test_End && !vStop && !_Stop_WB)
            {
                // Якщо сторінка - форма тестування без введених даних.(з ВІДКЛЮЧЕНОЮ кнопкою continue.)
                if (_WBState == WBState.stPage_Test_Clear)
                {
                    // Отримуэм Блоки з запитаннями Та ВНОСИМ ВІДПОВІДІ
                    HtmlNodeCollection vNodes_Question = _Current_HTMLDoc.DocumentNode.SelectNodes(StaticMetod.GetDescription(XPathSelect.FT_Questions));

                    // Проходим по всіх блоках з запитаннями на поточній сторінці
                    foreach (HtmlNode vNode in vNodes_Question)
                    {
                        string vQuestionNum = GetNumQuestion(vNode.Id); // Отримуэм номер елемента запитання                                                 
                        // ---=== Натискаэм  на ВІДПОВІДЬ в запитанні ===---
                        Random vRnd = new Random();
                        int vAnsverNum = vRnd.Next(0, 6);  // Випадковий № від 0 до 6 для вибору відповіді
                        Thread.Sleep(150);

                        // Натискаєм на відповідь
                        this.WB_Answer_Click(vQuestionNum, vAnsverNum);

                        // Якщо даного запитання НЕ ІСНУЄ в нашому списку
                        if (!vQuestions.ContainsKey(vNode.InnerHtml))
                            vQuestions.Add(vNode.InnerHtml, vQuestionNum);
                        else // Якщо ЗАПИТАННЯ ПОВТОРЮЄТЬСЯ і вже ічнує в нашому списку
                            aListErrors.Add("Запитання вже існує : \"" + vNode.InnerHtml + "\"");
                    }

                } // END if (_WBState == WBState.stPage_Test_Clear) (форма тестування без введених даних.(з ВІДКЛЮЧЕНОЮ кнопкою continue.))           

                // Поки не перейшли на наступну сторінку з запитаннями  АБО  не зупинено користиувачем
                bool vGoToNext_PageQuestions = false; // Перехід на наступну сторінку з запитаннями
                while (!vGoToNext_PageQuestions && !vStop)
                {
                    if (this.WB_WaitingNextPage(10, _WBState)) // Очікуєм зміни стрінки до 10 секунд
                    {
                        // Якщо сторінка змінилась на ->  Форма ТЕСТУВАННЯ З ВВЕДЕНИМИ ДАНИМИ (з АКТИВНОЮ кнопкою continue.) 
                        if (_WBState == WBState.stPage_Test_Сontinue)
                        {
                            WBState vCur_WBState = _WBState;            // Поточна сторінка до натискання кнопки continue
                            this.WB_Button_Сontinue_Click();            // Натискаєм кнопку "continue"
                            this.WB_WaitingNextPage(10, vCur_WBState);  // Очікуєм зміни стрінки до 10 секунд
                            vGoToNext_PageQuestions = true;             // Перейшли на наступну сторінку з запитаннями                            
                        }
                        else
                            if (MessageBox.Show("Довге очікування наступної сторінки.\nПродовжити очікування?\n\nOk - продовжити очікування.\nCancel - Зупинити.",
                                    "Попередження", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.Cancel)
                            vStop = true;
                    }
                    else
                    {
                        // Якщо сторінка не змінилась
                        if (MessageBox.Show("Довге очікування наступної сторінки.\nПродовжити очікування?\n\nOk - продовжити очікування.\nCancel - Зупинити.",
                                    "Попередження", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.Cancel)
                            vStop = true;
                        break;
                    }
                }

            }// END while (_WBState != WBState.stPage_Test_End && !vStop)
            if (_Stop_WB)
                _WB_Stoped = true;
            return vQuestions; // Повертаэм список запитань з нумерацією
        }

        // Введення відповідей користувача (повертаэ психотип та зберыгаэ PDF)
        // Метоод який ВВОДИТЬ ВІДПОВІДІ(дані) КОРИСТУВАЧА на сайті, повертає результат та зберінає PDF з сторікою результату
        public string Enter_User_Responses(UserData aUserData, string aSavePDF_FilePath, ref List<string> aListErrors)
        {
            // TODO 90 Програма працює до ......
            /*  Error in metod: "Enter_User_Responses". 
                Block: "ProcessorWB.cs"*/
            DateTime vEndDate = new DateTime(2025, 08, 30);
            bool vEndTrial = (vEndDate < DateTime.Now);  // Програма працює до .....

            string vResult_Archetypes = "Err";
            bool vStop = false;
            // Поки стан СТАН БРОУЗЕРА != Форма ТЕСТУВАННЯ ЗАВЕРШЕНО або не зупинено користувачем
            while (_WBState != WBState.stPage_Test_End && !vStop && !_Stop_WB && !vEndTrial)
            {
                // Якщо сторінка - форма тестування без введених даних.(з ВІДКЛЮЧЕНОЮ кнопкою continue.)
                if (_WBState == WBState.stPage_Test_Clear)
                {
                    // Отримуэм Блоки з запитаннями Та ---=== ВНОСИМ ВІДПОВІДІ ===---
                    HtmlNodeCollection vNodes_Question = _Current_HTMLDoc.DocumentNode.SelectNodes(StaticMetod.GetDescription(XPathSelect.FT_Questions));
                    // ---=== Проходим по всіх блоках з запитаннями на поточній сторінці ===---
                    foreach (HtmlNode vNode in vNodes_Question)
                    {
                        string vQuestionNum = GetNumQuestion(vNode.Id); // Отримуэм номер елемента запитання
                        string vQuestionTextWeb = vNode.InnerHtml; // Отримуэм ТЕКСТ запитання з WEB

                        // Отримуэм відповідь з ДАНИХ КОРИСТУВАЧА по даному запитанню
                        string vUserAnswer = "";
                        if (aUserData.UserAnswers.ContainsKey(vQuestionTextWeb))
                            vUserAnswer = StaticMetod.GetDescription(aUserData.UserAnswers[vQuestionTextWeb]);

                        // Якщо запитання та відповідь існують
                        if (!String.IsNullOrEmpty(vQuestionTextWeb) && !String.IsNullOrEmpty(vUserAnswer))
                        {
                            Random vRnd = new Random();
                            int vSleepMS = vRnd.Next(100, 120);  // Випадковий № від 0 до 6 для вибору відповіді 
                            // Знаходжим порядковий номер відповіді ТА ---=== Натискаэм  на ВІДПОВІДЬ в запитанні ===---
                            switch (vUserAnswer)
                            {
                                case "Disagree strongly":
                                    Thread.Sleep(vSleepMS);
                                    this.WB_Answer_Click(vQuestionNum, 0);// Натискаєм на відповідь
                                    break;
                                case "Disagree":
                                    Thread.Sleep(vSleepMS);
                                    this.WB_Answer_Click(vQuestionNum, 1);// Натискаєм на відповідь
                                    break;
                                case "Disagree slightly":
                                    Thread.Sleep(vSleepMS);
                                    this.WB_Answer_Click(vQuestionNum, 2);// Натискаєм на відповідь
                                    break;
                                case "Neither agree nor disagree":
                                    Thread.Sleep(vSleepMS);
                                    this.WB_Answer_Click(vQuestionNum, 3);// Натискаєм на відповідь
                                    break;
                                case "Agree slightly":
                                    Thread.Sleep(vSleepMS);
                                    this.WB_Answer_Click(vQuestionNum, 4);// Натискаєм на відповідь
                                    break;
                                case "Agree":
                                    Thread.Sleep(vSleepMS);
                                    this.WB_Answer_Click(vQuestionNum, 5);// Натискаєм на відповідь
                                    break;
                                case "Agree strongly":
                                    Thread.Sleep(vSleepMS);
                                    this.WB_Answer_Click(vQuestionNum, 6);// Натискаєм на відповідь
                                    break;
                            }
                        }
                        else
                        { // ПОМИЛКА -  НЕ знайдено ЗАПИТАННЯ
                            vStop = true;
                            aListErrors.Add("НЕ знайдено ЗАПИТАННЯ: \n" + vQuestionTextWeb + "\nПроцес проходженння тесту ЗУПИНЕНО !");
                            return "STOP";
                        }
                    }
                }// END if (_WBState == WBState.stPage_Test_Clear)(форма тестування без введених даних.(з ВІДКЛЮЧЕНОЮ кнопкою continue.))                                 

                // Поки не перейшли на наступну сторінку з запитаннями  АБО  не зупинено користиувачем
                bool vGoToNext_PageQuestions = false; // Перехід на наступну сторінку з запитаннями
                while (!vGoToNext_PageQuestions && !vStop)
                {
                    if (this.WB_WaitingNextPage(10, _WBState)) // Очікуєм зміни стрінки до 10 секунд
                    {
                        // Якщо сторінка змінилась на ->  Форма ТЕСТУВАННЯ З ВВЕДЕНИМИ ДАНИМИ (з АКТИВНОЮ кнопкою continue.) 
                        if (_WBState == WBState.stPage_Test_Сontinue)
                        {
                            WBState vCur_WBState = _WBState;            // Поточна сторінка до натискання кнопки continue
                            this.WB_Button_Сontinue_Click();            // Натискаєм кнопку "continue"
                            this.WB_WaitingNextPage(10, vCur_WBState);  // Очікуєм зміни стрінки до 10 секунд
                            vGoToNext_PageQuestions = true;             // Перейшли на наступну сторінку з запитаннями                            
                        }
                        else
                            if (MessageBox.Show("Довге очікування наступної сторінки.\nПродовжити очікування?\n\nOk - продовжити очікування.\nCancel - Зупинити.",
                                    "Попередження", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.Cancel)
                            vStop = true;
                    }
                    else
                    {
                        // Якщо сторінка не змінилась
                        if (MessageBox.Show("Довге очікування наступної сторінки.\nПродовжити очікування?\n\nOk - продовжити очікування.\nCancel - Зупинити.",
                                    "Попередження", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.Cancel)
                            vStop = true;
                        break;
                    }
                }// Поки не перейшли на наступну сторінку з запитаннями  АБО  не зупинено користиувачем                            \

            }// END while (_WBState != WBState.stPage_Test_End && !vStop)
            // MessageBox.Show("Завершення тестування." + "\nWBState = " + _WBState.ToString());
            if (_Stop_WB)
                _WB_Stoped = true;

            // ---=== БЛОК ЗБЕРЕЖЕННЯ РЕЗУЛЬТАТУ ===---
            // Форма ТЕСТУВАННЯ ЗАВЕРШЕНО - кнопка (Переглянути результат/ View Results) та зберегти результат
            if (_WBState == WBState.stPage_Test_End && !_Stop_WB && !vEndTrial)
            {
                WBState vCur_WBState = _WBState;    // Поточна сторінка до натискання кнопки "VIEW RESULTS"
                WB_Button_ViewResults_Click();      // Натискаєм кнопку (Переглянути результат/ View Results) 

                // Пока стан  Веб Броузера != "Сторінка з результатом для збереження"                
                while (_WBState != WBState.stPage_Result)
                {
                    // Очікуєм зміни стрінки до 10 секунд якщо сторінка змінилиася
                    if (this.WB_WaitingNextPage(10, vCur_WBState))
                    {
                        // Після натискання кнопки існує ще одна сторінка яка автоматично зникає її стан - WBState.stPage_Unknown
                        // ЯКЩО після натискання кнопки НЕВИЗНАЧЕНА СТОРІНКА Ще раз потрібно дочекатись зміни сторінки
                        if (_WBState == WBState.stPage_Unknown)
                            this.WB_WaitingNextPage(30, WBState.stPage_Unknown);

                        // ------===== ЗБЕРІГАЄМ РЕЗУЛЬТАТИ ПРОХОДЖЕННЯ ТЕСТУ =====----- 
                        if (_WBState == WBState.stPage_Result) // ТА  стан  Веб Броузера != "Сторінка з результатом для збереження"
                        {
                            // TODO 98 Отримуєм Блок з результатом "Архетип"
                            /*HtmlNode vNode_Block_Archetypes_Resul = _Current_HTMLDoc.DocumentNode.SelectSingleNode(StaticMetod.GetDescription(xPathSelect.Block_Archetypes_Result));
                            if (vNode_Block_Archetypes_Resul != null)
                                vResult_Archetypes = vNode_Block_Archetypes_Resul.InnerText;
                            */
                            HtmlNode vNode_Page_SaveResults = _Current_HTMLDoc.DocumentNode.SelectSingleNode(StaticMetod.GetDescription(XPathSelect.Page_Result));
                            if (vNode_Page_SaveResults != null)
                            {
                                string vStrHtml = vNode_Page_SaveResults.InnerHtml;
                                string vFindStr = "You are most like ";
                                vStrHtml = vStrHtml.Substring(vStrHtml.IndexOf(vFindStr));
                                vFindStr = "The ";
                                vResult_Archetypes = vStrHtml.Substring(vStrHtml.IndexOf(vFindStr), vStrHtml.IndexOf("</") - vStrHtml.IndexOf(vFindStr));
                            }
                            else
                            {
                                aListErrors.Add("НЕ знайдено РЕЗУЛЬТАТ для \"" + aUserData.UserID + "\"\nПроцес проходженння тесту ЗУПИНЕНО !");
                                vResult_Archetypes = "STOP";
                            }

                            // TODO _SAVE PDF
                            // Після завантаження сторінки зберігаєм сторінку в PDF форматі
                            this.PageSaveAsPdf(aSavePDF_FilePath); // Зберігаєм сотінку в форматі PDF                            
                            //-----------------------------------------                            
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Помилка отримання сорінки з результатом користувача.\n\nПроцес зупинено.", "Помилка", MessageBoxButtons.OK);
                            break;
                        }
                    }
                    else // Якщо стан  Веб Броуз не змінився.
                        if (MessageBox.Show("Довге очікування наступної сторінки.\nПродовжити очікування?\n\nOk - продовжити очікування.\nCancel - Зупинити.",
                            "Попередження", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.Cancel)
                        break;
                }
            } // END if (_WBState == WBState.stPage_Test_End && !_Stop_WB && !vEndTrial)            
            // Програма працює до ......
            if (vEndTrial)
            {
                _WB.Visible = false;
                this.GoTo_URL(C_Const.TestURL);
            }
            GC.Collect(); // Запуск збору мусора.
                          //            GC.Collect(GCCollectionMode.Optimized); // Оптимізована зборка мусору.

            _WBState = WBState.stPage_Unknown; // Ставим Стан Веб Броузера не визначеним післязбереження результату
            if (_Stop_WB)
                _WB_Stoped = true;
            return vResult_Archetypes;
        }

        #endregion ---=== Реалізація інтерфесу ===---

        #region ---=== Власний код  ===---
        //КОНСТРУКТОР
        public ProcessorWB(WebView2 aWB)
        {
            _WB = aWB;
            _Stop_WB = false;                                      // Признак зупинки програми
            // Процедура визначення завантаження сторінки та стану WB
            _WB.NavigationCompleted += WB_NavigationCompleted;         // Метод викликається після ЗАВЕРШЕННЯ завантаження кожної нової сторінки
            _Current_HTMLDoc = new HtmlAgilityPack.HtmlDocument();      // Поточна HTML сторінка

            _Timer_TestForm = new System.Windows.Forms.Timer// Таймер для прохода форми з тестом
            {
                Interval = 1000,                            // Інтервал спрацювання таймера
                Enabled = false                            // Таймер виключений
            };
            _Timer_TestForm.Tick += Timer_TestForm_Tick;               // Метод який відпрацьовує з вказаним інтервалом при включеному таймері
        }

        // Зупинка WB (при завершенні програми)
        public void Stop()
        {
            _Stop_WB = true;            // Признак зупинки програми
            _WB_Stoped = false;         // Якщо э незавершені асинхронні цикли очікуєм їх завершення

            _TimerSecond = 0;
            _Timer_TestForm.Enabled = true;
            if (_WB.CoreWebView2 != null && _WBState != WBState.stPage_Unknown)
            {
                int vTimeSecond_ToClose = 5;
                Event_ProcesStop?.Invoke(vTimeSecond_ToClose); // Подія зупинення програми 
                while (_TimerSecond < vTimeSecond_ToClose && !_WB_Stoped)
                    Application.DoEvents();
            }

            _WB.Dispose();
        }

        // Метод який відпрацьовує з вказаним інтервалом при включеному таймері
        private void Timer_TestForm_Tick(object sender, System.EventArgs e)
        {
            Interlocked.Increment(ref _TimerSecond); // Збыльшуэмлічильник секунд.
        }

        // Метод викликається після ЗАВЕРШЕННЯ завантаження кожної нової сторінки
        private void WB_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            // Затримка/очікування (без перезавантаження сторінки) зміни елементів форми
            // ---=== Без зупинки потоку робим затримку в 1 секунду. ===--- 
            _TimerSecond = 0;
            _Timer_TestForm.Enabled = true;
            while (_TimerSecond < 1 && !_Stop_WB)
                Application.DoEvents();
            if (_Stop_WB) _WB_Stoped = true;

            _Timer_TestForm.Enabled = false;
            // Після завантаження WebView2 викликаєм метод який з броузера завантажує в обєкт HtmlAgilityPack для подальшого аналізу та визначення стану броузера
            if (!_Stop_WB)
                this.LoadDocumen();// Перезавантажуєм дані сторінки для аналізу та 
        }

        // Очікування стану Веб Броузера певний період часу (Без перезавантаження сторінки)
        private bool WB_WaitingNextPage(int aWaitingSeconds, WBState aCur_WBState)
        {
            int vTimerSecond = 0;   // Перевіряти сторінку будем 1 раз в секунду
            _TimerSecond = 0;

            _Timer_TestForm.Enabled = true;

            // Пока не закінчився час та нема результату зміни сторінки
            bool vResult = false;
            while (_TimerSecond < aWaitingSeconds && !vResult && !_Stop_WB)
            {
                Application.DoEvents();
                if (vTimerSecond < _TimerSecond) // Якщо пройшла секунда
                {
                    vTimerSecond = _TimerSecond;
                    if (!_Stop_WB)
                        this.LoadDocumen(); // Перезавантажуєм дані сторінки для аналізу та 
                }

                if (aCur_WBState != _WBState) // Якщо сторінка змінилась (результату зміни сторінки)
                    vResult = true;
                if (_Stop_WB) _WB_Stoped = true;
            }
            _Timer_TestForm.Enabled = false;
            return vResult;
        }

        //  Після завантаження WebView2 викликаєм метод який з броузера завантажує в обєкт HtmlAgilityPack для подальшого аналізу.
        private async void LoadDocumen()
        {
            // Завантажуєм документ для аналізу
            if (_WB.CoreWebView2 != null)
            {
                string vDom = await _WB.CoreWebView2.ExecuteScriptAsync("document.body.outerHTML");
                vDom = System.Text.RegularExpressions.Regex.Unescape(vDom);

                //_Current_HTMLDoc = new HtmlAgilityPack.HtmlDocument();      // Поточна HTML сторінка
                if (_Current_HTMLDoc != null)
                    _Current_HTMLDoc.LoadHtml(vDom);

                this.Check_WB_State();          // Визначаэм стан БРОУЗЕРА                        
                _WB_LoadCompleted = true;       // Признак завершення завантаження сторінки                     
            }
        }

        // Визначаєм та стан броузера
        private void Check_WB_State()
        {
            _WBState = WBState.stPage_Unknown;  // === СТАН Невизначений

            // === СТАН WB - Форма Тестування.(АКТИВНА чи ВІДКЛЮЧЕНА різний статус WB)
            if (_Current_HTMLDoc != null)
            {
                // Шукаєм сторінку по ID
                string vXpath_Page_ID = StaticMetod.GetDescription(XPathSelect.FT_Page_ID);
                HtmlNode vNode_page = _Current_HTMLDoc.DocumentNode.SelectSingleNode(vXpath_Page_ID);
                // Шукаєм блоки з запитаннями
                // HtmlNodeCollection vNodes_Question = _Current_HTMLDoc.DocumentNode.SelectNodes(".//div[contains(@id, 'TopTextQuestion')]");                
                HtmlNodeCollection vNodes_Question = _Current_HTMLDoc.DocumentNode.SelectNodes(StaticMetod.GetDescription(XPathSelect.FT_Questions));
                // Якщо присутня сторінка з  id='page', і хоч однин блок з запитаннями
                if (vNode_page != null && vNodes_Question != null && vNodes_Question.Count > 0)
                {
                    // ---=== КНОПКА 'continue' ===---
                    // Шукаєм кнопку 'continue' яка ще НЕ АКТИВНА
                    HtmlNode vNode_button_disabled = _Current_HTMLDoc.DocumentNode.SelectSingleNode(StaticMetod.GetDescription(XPathSelect.FT_ButtonContinue_Disable));
                    // Якщо ВІДКЛЮЧЕНОЇ кнопки 'continue' немає шукаєм АКТИВНУ кнгопку
                    HtmlNode vNode_button_enabled = null;
                    if (vNode_button_disabled == null)
                        vNode_button_enabled = _Current_HTMLDoc.DocumentNode.SelectSingleNode(StaticMetod.GetDescription(XPathSelect.FT_ButtonContinue_Enable));
                    // ---=== В залежності від того що кнопка 'continue' АКТИВНА чи ВІДКЛЮЧЕНА різний статус WB
                    if (vNode_button_disabled != null && vNode_button_disabled.InnerText == "Continue")
                        _WBState = WBState.stPage_Test_Clear;
                    if (vNode_button_enabled != null && vNode_button_enabled.InnerText == "Continue")
                        _WBState = WBState.stPage_Test_Сontinue;
                    return;
                }
                // === СТАН WB - ЗАВЕРШЕННЯ ТЕСТУВАННЯ.
                // Шукаєм кнопку Переглянути результат/ View Results
                HtmlNode vNode_Button_ViewResults = _Current_HTMLDoc.DocumentNode.SelectSingleNode(StaticMetod.GetDescription(XPathSelect.FT_Button_ViewResults));
                // Якщо знайшли кнопку Переглянути результат
                if (vNode_Button_ViewResults != null && vNode_Button_ViewResults.InnerText == "View Results")
                {
                    _WBState = WBState.stPage_Test_End;
                    return;
                }
                // === СТАН WB - Сторінка з результатом                
                // Шукаєм cторінку з результатом для збереження
                HtmlNode vNode_Page_SaveResults = _Current_HTMLDoc.DocumentNode.SelectSingleNode(StaticMetod.GetDescription(XPathSelect.Page_Result));
                // Якщо знайшли cторінку з результатом для збереження
                if (vNode_Page_SaveResults != null && _WB.Source != null && _WB.Source.ToString() == C_Const.ResultURL)
                {
                    _WBState = WBState.stPage_Result;
                    return;
                }
            }
        }

        //----------========== Допоміжні методи ==========----------
        // Отримання номера з ID ЗАПИТАННЯ
        private string GetNumQuestion(string aQuestionID)
        {
            // Обрізаєм всі символи залишаючи лише № запитання
            int vCount_ = aQuestionID.Split('_').Length - 1;
            if (vCount_ > 1)
            {
                aQuestionID = aQuestionID.Remove(0, aQuestionID.IndexOf('_') + 1);
                aQuestionID = aQuestionID.Remove(aQuestionID.IndexOf('_'));
            }
            return aQuestionID;
        }

        // Натискання на відповідь 
        private void WB_Answer_Click(string aIdQuestion, int aAnsverNum)
        {
            // Генеруєм id Label (відповіді) для натискання
            string vLabelId = "answer_question_number_" + aIdQuestion + "_list_item_" + aAnsverNum.ToString();


            // Натискаєм на відповідь
            string vClickLabel = "var vLabels = document.getElementsByTagName('label');" +
            "for (var i = 0; i < vLabels.length; i++) {" +
                $"if (vLabels[i].htmlFor == '" + vLabelId + "') {" +
                 "vLabels[i].click(); " +
                 "break;" +
                "};" +
            "};";
            _WB.ExecuteScriptAsync(vClickLabel);
        }

        // Натискання на кнопку continue (при заповнені відповідей)
        private void WB_Button_Сontinue_Click()
        {
            string vButton_Сontinue_Click = "var vButtons = document.getElementsByTagName('button');" +
            "for (var i = 0; i < vButtons.length; i++) {" +
                $"if (vButtons[i].innerHTML == 'Continue') {{" +
                 "vButtons[i].click(); " +
                 "break;" +
                "};" +
            "};";

            _WB.ExecuteScriptAsync(vButton_Сontinue_Click);
            // Очікування  1 секунду після натиснення кнопки "Continue" так як сторінка повністю не перезавантажується.
            _TimerSecond = 0;
            _Timer_TestForm.Enabled = true;
            while (_TimerSecond < 1 && !_Stop_WB)
                Application.DoEvents();
            if (_Stop_WB) _WB_Stoped = true;
            _Timer_TestForm.Enabled = false;
        }

        // Натискання на кнопку View Results (ТЕСТУВАННЯ ЗАВЕРШЕНО)
        private void WB_Button_ViewResults_Click()
        {
            string vButton_ViewResults_Click = "document.getElementById('skipToResults').click();";
            _WB.ExecuteScriptAsync(vButton_ViewResults_Click);
        }

        // ---=== CODE 2.0 ПІСЛЯ ТЕСТУ ЗРОБИТИ ВНУТРІШНЬІМ ПРИВАТНИМ МЕТОДОМ 

        // Зберігаєм сотінку в форматі PDF
        private async void PageSaveAsPdf(String aFilePath)
        {
            if (_WB.CoreWebView2 != null)
            {
                //Зберігаєм сотінку(результат) в форматі PDF
                // CoreWebView2PrintSettings vPrintSettings = null;
                CoreWebView2PrintSettings vPrintSettings = _WB.CoreWebView2.Environment.CreatePrintSettings();
                if (vPrintSettings != null)
                    vPrintSettings.Orientation = CoreWebView2PrintOrientation.Portrait;
                //bool isSuccessful = 
                await _WB.CoreWebView2.PrintToPdfAsync(aFilePath, vPrintSettings);
            }
        }

        #endregion ---=== Власний код  ===---

    }
}
