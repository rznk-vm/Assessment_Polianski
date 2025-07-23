using GlobalModule;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Views;

// !!! ОСНОВНИЙ МОДУЛЬ ПРОГРАМИ !!!
namespace AppMainModule
{
    class ApplicationController
    {
        #region ---=== ПОЛЯ ===---

        private readonly IFormMain _ViewMain;                   // Обєкт головної форми програми
        private readonly IDataManager _DataManager;             // Менеджер роботи з даними
        private bool _Stop_WB;                                  // Признак зупинки програми
        #endregion ---=== ПОЛЯ ===---

        #region ---=== Власний код  ===---
        // КОНСТРУКТОР (створення потрібних модулів)
        public ApplicationController()
        {
            _ViewMain = new FormMain();         // СТВОРЕННЯ Головної Форми
            _DataManager = new DataManager();   // СТВОРЕННЯ Менеджера для роботи з даними
            _Stop_WB = false;                   // Признак зупинки програми

            // Події закладки "WB"
            _ViewMain.UC_WB.Event_Save_Questions_FromWeb += UC_WB_Event_Save_Questions_FromWeb; // Подія ЗБЕРЕЖЕННЯ списку запитань завантажених з WEB 

            _ViewMain.UC_WB.Event_LoadDataFromFile += UC_WB_Event_LoadDataFromFile;             // Подія ЗАВАНТАЖЕННЯ ДАНИХ КОРИСТУВАЧІВ З ФАЙЛУ
            _ViewMain.UC_WB.Event_Start += UC_WB_Event_Start;                                   // Подія Натискання КНОПКИ "Start"

            // Події ГОЛОВНОЇ ФОРМИ
            _ViewMain.Event_FormClosing += MainForm_Close;

            _ViewMain.Run();                // ПОЧАТОК ПРОГРАММИ - Запуск Головної форми     
        }


        #endregion ---=== Власний код  ===---

        #region ---=== Події Вкладки (WB) ===--- 
        // Подія ЗБЕРЕЖЕННЯ списку ЗАПИТАНЬ завантажених з WEB 
        private void UC_WB_Event_Save_Questions_FromWeb()
        {
            if (_ViewMain.ProcessorWB.GoTo_URL(C_Const.TestURL) == WBState.stPage_Test_Clear)   // ПЕРЕХІД  до сторінки з тестами
            {
                //  Дані для відображення користувачу
                List<string> vInfo = new List<string>();
                //Викликаэм процедуру збору списку запитань отриманих з Web-ресурсу 
                Dictionary<string, string> vQuestions_FromWeb = _ViewMain.ProcessorWB.Get_Questions_FromWeb(ref vInfo);

                // Зберігаєм список запитань в фал (DataManager - через подію Save_Questions_FromWeb та діалог this.SFD_SaveFile)
                string vSaveFilePath = Directory.GetCurrentDirectory() + "\\Questions_FromWeb_" + DateTime.Now.ToString("yyyy.MM.dd") + ".tsv";
                // Подія ЗБЕРЕЖЕННЯ списку запитань завантажених з WEB через ApplicationController(головний модуль)->DataManager
                _DataManager.Save_Questions_FromWeb(vQuestions_FromWeb, vSaveFilePath);

                // Відображаєм для користувача що файл з списком запитань збережено вдало чи э помилки
                vInfo.Add("\nФайл зі списком ЗАПИТАНЬ збережено: \"" + vSaveFilePath + "\"");

                // Якщо є інформація для відображення - інформуєм користувача 
                if (vInfo.Count > 0 && !_Stop_WB)
                    _ViewMain.UC_WB.ShowInfo(vInfo);
            }
            else
                MessageBox.Show("Сторінку з тестом не знайдено.\nМожливо пропало Internet зэднання.\n\nПовторіть спробу пізніше", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        // Подія ЗАВАНТАЖЕННЯ ДАНИХ КОРИСТУВАЧІВ З ФАЙЛУ
        private void UC_WB_Event_LoadDataFromFile(string aSelectedFile)
        {
            if (File.Exists(aSelectedFile)) // Якщо файл існує
            {
                // Помилки завантаження з файлу                
                List<string> vErrors = new List<string>();
                // Завантажуєм дані користувачів з файлу
                List<string> vInfo = new List<string>();                //  Дані для відображення 

                _DataManager.LoadUserData(aSelectedFile, ref vErrors);  // Передаєм вибраний файл на завантаження                
                _ViewMain.UC_WB.ClearLinesInfo();                       // Очищаєм інформаційну панель
                _ViewMain.UC_WB.Button_Start_Visble = false;            // Змінюєм видимість кнопоки  "Розпочати" 

                if (vErrors != null) //  Якщо помилок в завантажені даних користувачів нема 
                {
                    if (vErrors.Count == 0)
                        vInfo.Add("\nДані користувачів завантажені вдало !");    // Відображаєм те що вдало завантажено
                    else
                        vInfo.Add("\nДані користувачів завантажені з помилками !");    // Відображаєм те що є помилки завантаження                        
                }

                // Якщо завантажені дані користувачів існують - відображаєм
                if (_DataManager.UsersData.Count > 0)
                {
                    vInfo.Add("\n" + C_Const.Info_Іndent + "Кількість завантажених користувачів: " + _DataManager.UsersData.Count.ToString());
                    if (_DataManager.UserData_FinishTest_Count > 0)
                        vInfo.Add("\n" + C_Const.Info_Іndent + "В т.ч. з результатом пройденого тесту: " + _DataManager.UserData_FinishTest_Count.ToString());

                    vInfo.Add(C_Const.Info_Block_Separator);
                    // Якщо є завантажені дані без результату перевірки
                    if ((_DataManager.UsersData.Count - _DataManager.UserData_FinishTest_Count) > 0)
                        _ViewMain.UC_WB.Button_Start_Visble = true; // Змінюєм видимість кнопоки  "Розпочати"

                }
                if (vErrors.Count > 0) // Якщо є помилки додаєм до інформації на вмвід
                    vInfo.AddRange(vErrors);

                // Якщо є інформація для відображення
                if (vInfo.Count > 0 && !_Stop_WB)
                    _ViewMain.UC_WB.ShowInfo(vInfo);
            }

        }

        // Подія Натискання КНОПКИ "Start"        
        private void UC_WB_Event_Start()
        {
            _ViewMain.UC_WB.Button_Start_Visble = false; // Змінюєм видимість кнопоки  "Розпочати" перед початком 
            _ViewMain.UC_WB.Button_Load_Visble = false; // Змінюєм видимість кнопоки  "Завантажити" перед початком 

            // Перевіряєм чи є даних бідьше 0 - Якщо є завантажені дані без результату перевірки // Кількість записів які потрібно обробити/(ввести дані на сайті)
            int vUserDataCount_ToWork = _DataManager.UsersData.Count - _DataManager.UserData_FinishTest_Count;
            if (vUserDataCount_ToWork > 0)
            {
                // ЗАПУСКАЭМ ЦИКЛ ВВЕДЕННЯ ВІДПОВІДЕЙ для кожного користувача на сайті ДЛЯ ОТРИМАННЯ РЕЗУЛЬТАТУ     _ViewMain.UC_WB -> _ProcessorWB -> 
                int vRecord_processing = 1; // Який запис обробляэться.
                for (int i = 0; i < _DataManager.UsersData.Count; i++)
                {

                    UserData vUserData = _DataManager.UsersData[i]; // Дані ПОТОЧНОГО КОРИСТУВАЧА
                    if (String.IsNullOrEmpty(vUserData.UserArchetype)) // Якщо користувач ще НЕ МАЄ РЕЗУЛЬТАТУ/НЕ ПЕРЕВІРЕНИЙ / НЕ БУВ ВВЕДЕНИЙ НА САЙТІ
                    {
                        //  Дані для відображення користувачу
                        List<string> vInfo = new List<string>
                        {   // TODO Виведення інформації для користувача 
                            "\nОбробка запису: " + vRecord_processing.ToString() + " із " + vUserDataCount_ToWork.ToString() +
                            "  \n" + C_Const.Info_Іndent + "Користувач: \"" + vUserData.UserID + "\"\n" + C_Const.Info_Іndent + "Початок введення даних !"
                        };
                        vRecord_processing++;
                        if (!_Stop_WB)
                            _ViewMain.UC_WB.ShowInfo(vInfo);
                        vInfo.Clear();

                        // Шлях для збереження PDF файлу з результатом
                        string vFileName = "Rezult_" + DateTime.Now.ToString("yyyy.MM.dd") + "_" + vUserData.UserID + ".pdf";
                        string vSavePDF_FilePath = Environment.CurrentDirectory + "\\" + vFileName;

                        Archetypes vArchetypes = new Archetypes(); // Можливі архетипи                                                                   

                        // ПЕРЕХІД  до сторінки з тестами (Отримуєм результат по користувачу)
                        WBState vWBState = _ViewMain.ProcessorWB.GoTo_URL(C_Const.TestURL);
                        if (vWBState == WBState.stPage_Test_Clear)
                        {   // Вводим дані користувача на сайті та отримуєм результат
                            string vRezultArchetype = _ViewMain.ProcessorWB.Enter_User_Responses(vUserData, vSavePDF_FilePath, ref vInfo);
                            // TODO ЗБЕРЕЖЕННЯ РЕЗУЛЬТАТУ                            
                            // По кожному користувачу ЗБЕРІГАЄМ РЕЗУЛЬТ (ДАНІ лтримані з WEB)
                            if (vArchetypes._Types.ContainsKey(vRezultArchetype)) // Якщо результат = існуючий архетип
                            {
                                // ЗБЕРІГАЄМ ЗМІНЕНІ ДАНІ користувачів
                                _DataManager.Save_UserData(vUserData.UserID, vRezultArchetype + "_" + vArchetypes._Types[vRezultArchetype]);
                                vInfo.Add("\n" + C_Const.Info_Іndent + "Tест пройдено !");
                                vInfo.Add("\n" + C_Const.Info_Іndent + "Архетип: " + vRezultArchetype);
                                vInfo.Add("\n" + C_Const.Info_Іndent + "Файл результату: " + vFileName + " збережено.");
                                vInfo.Add(C_Const.Info_Block_Separator);
                                if (!_Stop_WB)
                                    _ViewMain.UC_WB.ShowInfo(vInfo);
                                //-----------------------------------------
                            }
                            else
                            { // Якщо тест не ПРОЙДЕНО (ПОМИЛКИ)
                                if (vRezultArchetype == "STOP" && !_Stop_WB)
                                {
                                    _ViewMain.UC_WB.ShowInfo(vInfo);                //  Виводим інформацію для відображення - інформуєм користувача 
                                    _ViewMain.UC_WB.Button_Start_Visble = false;    //  Видимість кнопоки  "Розпочати"
                                    return;
                                }
                                else
                                {
                                    vInfo.Add("\n" + C_Const.Info_Іndent + "Error in metod: \"Enter_User_Responses\". \n" + C_Const.Info_Іndent + "Block: \"ProcessorWB.cs\"");
                                    if (!_Stop_WB)
                                        _ViewMain.UC_WB.ShowInfo(vInfo);                //  Виводим інформацію для відображення - інформуєм користувача 
                                    MessageBox.Show("Помилка проходження тесту.\nКористувач: " + vUserData.UserID, "Помилка !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                            MessageBox.Show("Сторінку з тестом не знайдено.\nМожливо пропало Internet зєднання.\n\nПовторіть спробу пізніше", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
            // По завершеню змінюєм видимість кнопоки  "Розпочати" та "Завантажити"
            _ViewMain.UC_WB.Button_Start_Visble = ((_DataManager.UsersData.Count - _DataManager.UserData_FinishTest_Count) > 0);
            _ViewMain.UC_WB.Button_Load_Visble = true; // Змінюєм видимість кнопоки  "Завантажити" перед початком 
        }

        #endregion ---=== Події Вкладки (WB) ===---
        #region ---=== ГОЛОВНОЇ ФОРМИ ===--- 
        // При закриті форми Зупинка WB
        private void MainForm_Close()
        {
            _Stop_WB = true;
            _ViewMain.ProcessorWB.Stop();

            //this.Process.Kill();
        }
        #endregion ---=== ГОЛОВНОЇ ФОРМИ ===---
    }
}
