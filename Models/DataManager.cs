using GlobalModule;
using System;
using System.Collections.Generic;
using System.IO;

namespace Models
{
    public interface IDataManager
    {
        void LoadUserData(string aFilePath, ref List<string> aLoadErrors);  // Завантаження данних користувачів
        void Save_UserData(string aUserID, string aArchetype);              // ЗБЕРЕЖЕННЯ ЗМІНЕНИХ ДАНИХ користувачів
        void Save_Questions_FromWeb(Dictionary<string, string> aQuestions_FromWeb, string aPathToFile);  // ЗБЕРЕЖЕННЯ списку запитань завантажених з WEB
        int UserData_FinishTest_Count { get; }                              // Отримання кількості завантажених користувачів з пройденим тестом
        List<UserData> UsersData { get; }                                   // Отримання даних користувачів
    }
    // Менеджер роботи з даними
    public class DataManager : IDataManager
    {
        #region ---=== ПОЛЯ ===---
        private readonly List<UserData> _UsersData;      // Списки ДАНИХ користувачів      
        private string dUserData_FilePath;      // Шлях до файлу з завантаженими даними користувача.
        #endregion ---=== ПОЛЯ ===---



        #region  ---=== Реалізація інтерфесів ===---

        // Завантаження данних користувачів (Відповіді на запитання повинні міститись міх сптовпцями ІДЕНТИФІКАТОР КОРИСТУВАЧА та Result)
        public void LoadUserData(string aFilePath, ref List<string> aLoadErrors) // aLoadErrors - Помилки при завантаженні файлу з даними
        {
            // Якщо файл існує
            if (File.Exists(aFilePath))
            {
                // Завантажуєм дані з файлу
                if (_UsersData != null) // Якщо списки створенні
                {
                    _UsersData.Clear();                 // Очищаєм списки перед заповненням. 
                                                        //                    bool vUsersData_AddRecord = true;   // Якщо запис без помилок тоді додаєм до списку даних користувачів                   

                    int vLineNum = -1;                              // Порядковий номер зядка в файлі з даними
                                                                    //                    int vColumnNum = 0;                             // Порядковий номер стовпчика в файлі даних

                    List<string> vAllQuestions = new List<string>();  // Заголовки(ЗПАИТАННЯ) - перший рядок в Файлі

                    string vUserID;                                     // ІДЕНТИФІКАТОР КОРИСТУВАЧА;
                    Dictionary<string, AnswerQuestion> vUserAnswers;    // Відповіді користувача.
                    string vTestRezult;                                 // Результат тесту (якщо тест було пройдено) // назва основного АРХЕТИПУ

                    int vColumnNum_UserID = -1;          // Номер стовпця з ІДЕНТИФІКАТОРОМ КОРИСТУВАЧА
                    int vColumnNum_Rezult = -1;          // Номер стовпця з РЕЗУЛЬТАТОМ ТЕСТУ


                    // ---=== ЗАВАНТАЖУЄМ ДАНІ КОРИСТУВАЧІВ ===---                    
                    string[] vLines = File.ReadAllLines(aFilePath); // ЗАВАНТАЖУЄМ ФАЙЛ 
                    foreach (string vLine in vLines)  // Проходим по всіх рядках файлу та записуєм дані
                    {
                        string[] vValues = vLine.Split(C_Const.CSV_File_Separator); // Отримуэм ЗНАЧЕННЯ ПОТОЧНОГО РЯДКА

                        vUserID = "";                                               // ІДЕНТИФІКАТОР КОРИСТУВАЧА;
                        vTestRezult = "";                                           // Результат тесту (якщо тест було пройдено) //  назва основного АРХЕТИПУ
                        vUserAnswers = new Dictionary<string, AnswerQuestion>();    // Відповіді користувача.

                        bool vUsersData_AddRecord = true;    // Додаєм запис до списку даних користувачів (якщо нема помилок)                        


                        vLineNum++; // Порядковий номер зядка в файлі з даними Номер стовпця з ІДЕНТИФІКАТОР КОРИСТУВАЧА та результатом
                                    // Заповнюэм СПИСОК ЗАГОЛОВКІВ (колонок) + 

                        int vColumnNum = 0; // Обнуляєм порядковий номер стовпчика в файлі даних при початку читання кожного рядка.
                        // Проходим по всіх значеннях в рядку та записуєм
                        foreach (string vValue in vValues)
                        {
                            // Якщо в файлі запис(рядок) перший - Записуєм ЗНАЧЕННЯ ЗАГОЛОВКІВ (ЗАПИТАННЯ)
                            // ---=== 1 рядок - ЗАПИТАННЯ ===---
                            if (vLineNum == 0)
                            {
                                // Якщо поле ЗАПОВНЕНЕ(значення не порожнє)
                                if (!String.IsNullOrEmpty(vValue))
                                {
                                    // Якщо стовпець НЕ ІДЕНТИФІКАТОР КОРИСТУВАЧА,  НЕ  РЕЗУЛЬТАТОМ ТЕСТУ  та  НЕ  Позначка часу
                                    if ((vValue != C_Const.UserID_Text) && (vValue != "Result") && (vValue != "Позначка часу"))
                                    {
                                        string vQuestion = vValue.Split('.')[0].Trim(); // Обрізаєм Ураїнську та російську версію відповіді
                                        vAllQuestions.Add(vQuestion);                     // Записуєм ЗНАЧЕННЯ ЗАГОЛОВКА (Запитання)
                                    }
                                    if (vValue == C_Const.UserID_Text)      // Якщо стовпець з ІДЕНТИФІКАТОР КОРИСТУВАЧА
                                        vColumnNum_UserID = vColumnNum;     // Номер стовпця з ІДЕНТИФІКАТОРОМ КОРИСТУВАЧА
                                    if (vValue == "Result")                 // Якщо стовпець з результатом тесту
                                        vColumnNum_Rezult = vColumnNum;     // Номер стовпця з РЕЗУЛЬТАТОМ ТЕСТУ
                                }
                                // Заголовок не може бути пустим
                                else
                                {
                                    // Якщо хоч одне значення в заголову є Пустим записуэм в помилки та завершуэм завантаження.
                                    aLoadErrors.Add("\nВ заголовках № стовпця: \"" + vColumnNum + "\" відсутні дані!");
                                    aLoadErrors.Add("\nДані користувачів НЕ ЗАВАНТАЖЕНО !!!");
                                    return; // Завершуэм завантаження.
                                }
                            }
                            // Якщо в файлі запис(рядок) не перший - Заповнюэм ДАНІ КОРИСТУВАЧА та ВІДПОВІДІ 
                            // ---=== ДАНІ КОРИСТУВАЧА та ВІДПОВІДІ  ===---
                            else
                            {
                                // TODO 99 Зберігаєм всі ЗАПИТАНЯ з файлу завантаження. (щоб порівняти з збереженими з WEB "Save_Questions_FromWeb" )                               
                                //File.WriteAllLines(Directory.GetCurrentDirectory() + "\\Questions_FromData_" + DateTime.Now.ToString("yyyy.MM.dd") + ".tsv", vAllQuestions);

                                // Якщо поле ЗАПОВНЕНЕ(значення не порожнє)
                                if (!String.IsNullOrEmpty(vValue))
                                {
                                    // Якщо стовпець ІДЕНТИФІКАТОР КОРИСТУВАЧА і він не порожній                                        
                                    if (vColumnNum == vColumnNum_UserID)
                                        vUserID = vValue;  // Зберігаєм  ІДЕНТИФІКАТОР КОРИСТУВАЧА;
                                    // Якщо поточне значення стовбця між ІДЕНТИФІКАТОР КОРИСТУВАЧА та Result
                                    if (vColumnNum_UserID > -1 && vColumnNum_Rezult > -1)
                                    {   // між ІДЕНТИФІКАТОР КОРИСТУВАЧА та Result можуть бути лише 
                                        // ---=== ВІДПОВІДІ на ЗАПИТАННЯ ===--- 
                                        if (vColumnNum > vColumnNum_UserID && vColumnNum < vColumnNum_Rezult)
                                        {
                                            string vAnsver = vValue.Split('/')[0].Trim(); // Обрізаєм Ураїнську та російську версію відповіді
                                            //string vAnsver = vValue;

                                            // Якщо відповідь є в списку
                                            if (GetAnswerQuestion(vAnsver.Split('/')[0].Trim()) != AnswerQuestion.Err)
                                                vUserAnswers.Add(vAllQuestions[vColumnNum - (vColumnNum_UserID + 1)], GetAnswerQuestion(vAnsver));   // Зберігаєм ЗАПИТАННЯ та ВІДПОВІДІ користувача
                                                                                                                                                     // --(vColumnNum_UserID+1) відповіді починаються після ідентифікатора користувача а СПИСОК ЗАПИТАНЬ(vAllQuestions) з 0
                                            else
                                            {
                                                // Помилка відповіді в рядок № ..... стовбець № ....
                                                aLoadErrors.Add("\nВ записі № " + vLineNum + ", стовбець № " + vColumnNum + " помилкова ВІДПОВІДЬ: ");
                                                aLoadErrors.Add("\n\"" + vAnsver + "\"");
                                                aLoadErrors.Add("\nНа запитання:");
                                                aLoadErrors.Add("\n\"" + vAllQuestions[vColumnNum - (vColumnNum_UserID + 1)] + "\".");
                                                aLoadErrors.Add("\nДані користувача \"" + vUserID + "\" НЕ ЗАВАНТАЖЕНО !!!");
                                                aLoadErrors.Add(C_Const.Info_Block_Separator);
                                                vUsersData_AddRecord = false;  // НЕ ДОДАЄМ запис до списку даних користувачів
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        aLoadErrors.Add("\nСтовпці з відповідями на запитання повинні знаходитись між \"" + C_Const.UserID_Text + "\" та \"Result\" !");
                                        if (vColumnNum_UserID == -1)
                                            aLoadErrors.Add("\nНажать стовпець \"" + C_Const.UserID_Text + "\" не знайдено.");
                                        if (vColumnNum_Rezult == -1)
                                            aLoadErrors.Add("\nНажать стовпець \"Result\" не знайдено.");
                                        aLoadErrors.Add("\nДані користувачів НЕ ЗАВАНТАЖЕНО !!!");
                                        aLoadErrors.Add(C_Const.Info_Block_Separator);
                                        return; // Завершуэм завантаження.
                                    }
                                    // Якщо стовпець результату і він НЕ ПОРОЖНІЙ                                        
                                    if (vColumnNum == vColumnNum_Rezult)
                                        vTestRezult = vValue;       // Зберігаєм Результат тесту (якщо тест було пройдено) // назва основного АРХЕТИПУ
                                }
                                // Якщо поле НЕ ЗАПОВНЕНЕ (значення не порожнє) - аналізуєм, та записуєм помилку
                                else
                                {
                                    // Якщо стовпець ІДЕНТИФІКАТОР КОРИСТУВАЧА ПОРОЖНІЙ
                                    if (vColumnNum == vColumnNum_UserID)
                                    {
                                        aLoadErrors.Add("\nВ записі № \"" + vLineNum + "\" відсутнє поле \"" + C_Const.UserID_Text + "\", запис НЕ ЗАВАНТАЖЕНО !");
                                        aLoadErrors.Add(C_Const.Info_Block_Separator);
                                        vUsersData_AddRecord = false;  // НЕ ДОДАЄМ запис до списку даних користувачів
                                        break;
                                    }
                                    // Якщо стовпець з будь якою ВІДПОВІДЮ ПОРОЖНІЙ
                                    // Якщо поточне значення стовбця між ІДЕНТИФІКАТОР КОРИСТУВАЧА та Result
                                    if (vColumnNum_UserID > -1 && vColumnNum_Rezult > 1 && vColumnNum > vColumnNum_UserID && vColumnNum < vColumnNum_Rezult)
                                    {
                                        aLoadErrors.Add("\nВ записі № \"" + vLineNum + "\", стовбець № \"" + vColumnNum + "\" Не знайдено ВІДПОВІДЬ !");
                                        aLoadErrors.Add("\nНа запитання:");
                                        aLoadErrors.Add("\n\"" + vAllQuestions[vColumnNum - (vColumnNum_UserID + 1)] + "\".");
                                        aLoadErrors.Add("\nДані користувача \"" + vUserID + "\" НЕ ЗАВАНТАЖЕНО !!!");
                                        aLoadErrors.Add(C_Const.Info_Block_Separator);
                                        vUsersData_AddRecord = false;  // НЕ ДОДАЄМ запис до списку даних користувачів            
                                        break;
                                    }
                                }
                            }
                            vColumnNum++;                       // Порядковий номер стовпчика в файлі даних
                        }
                        // Якщо нема помилок та кількість відповідей дорівнює кількості запитань
                        if (vUsersData_AddRecord && vUserAnswers.Count == (vColumnNum_Rezult - vColumnNum_UserID - 1))
                            _UsersData.Add(new UserData(vUserID, vUserAnswers, vTestRezult)); // Додаєм запис до списку даних користувачів                                                
                    }
                    dUserData_FilePath = aFilePath;
                }
            }
            else
            {
                aLoadErrors.Add("\nПомилка завантаження файлу \"" + aFilePath + "\" : ФАЙЛ НЕ ЗНАЙДЕНО");
                aLoadErrors.Add(C_Const.Info_Block_Separator);
            }
            //!!!!!!!!!!!!!!!!!!!!!!
            GC.Collect();// Очищаєм память
            GC.WaitForPendingFinalizers();
            GC.Collect();// Очищаєм память
            //!!!!!!!!!!!!!!!!!!!!!!
        }

        // ЗБЕРЕЖЕННЯ ЗМІНЕНИХ ДАНИХ користувачів
        public void Save_UserData(string aUserID, string aArchetype)
        {
            // ---=== Зберігаєм дані користувача в змінній ===---
            if (!String.IsNullOrEmpty(aUserID) && !String.IsNullOrEmpty(aArchetype))
                for (int i = 0; i < _UsersData.Count; i++)
                {
                    if (_UsersData[i].UserID == aUserID)
                    {
                        UserData vUserData = _UsersData[i];
                        vUserData.UserArchetype = aArchetype;
                        _UsersData[i] = vUserData;
                        break;
                    }
                }
            // ---=== Зберігаєм дані користувача в файл ===---
            if (File.Exists(dUserData_FilePath)) // Зберігаєм в файл з якого завантажили
            {
                bool vHeaderRecorded = false;
                List<string> aText = new List<string>(); // Текст для запису в файл                                

                foreach (UserData vUserData in _UsersData)
                {
                    string vLine = "";
                    if (!vHeaderRecorded) // Записуэм  заголовки (ЗАПИТАННЯ) та відповіді першого користувача
                    {
                        vLine = "Електронна адреса";
                        vLine += this.GetLineQuestion(vUserData); // Рядок Запитань
                        vLine += C_Const.CSV_File_Separator + "Result";
                        vHeaderRecorded = true;

                        aText.Add(vLine); // Додаєм рядок в текст 
                        vLine = "";

                        vLine = vUserData.UserID;
                        vLine += this.GetLineAnswer(vUserData); // Рядок відповідей
                        vLine += C_Const.CSV_File_Separator + vUserData.UserArchetype;
                    }
                    else
                    {
                        vLine = vUserData.UserID;
                        vLine += this.GetLineAnswer(vUserData); // Рядок відповідей
                        vLine += C_Const.CSV_File_Separator + vUserData.UserArchetype;
                    }
                    aText.Add(vLine); // Додаэм рядок в текст                     
                }
                File.WriteAllLines(dUserData_FilePath, aText); // Запис тексту в файл
            }
        }

        //ЗБЕРЕЖЕННЯ списку запитань завантажених з WEB
        public void Save_Questions_FromWeb(Dictionary<string, string> aQuestions_FromWeb, string aPathToFile)
        {
            // Зберігаєм отриманий список запитань в файл
            string vData = "";
            foreach (KeyValuePair<string, string> vRec in aQuestions_FromWeb)
            {
                vData += (vRec.Key + "\r\n");
                //vData += (vRec.Value + C_Const.CSV_File_Separator + vRec.Key + "\r\n");
            }

            File.WriteAllText(aPathToFile, vData);
        }

        // Отримання кількості завантажених користувачів з пройденим тестом
        public int UserData_FinishTest_Count
        {
            get
            {
                int vCount = 0;
                foreach (UserData vUserData in _UsersData)
                    if (!string.IsNullOrEmpty(vUserData.UserArchetype))
                        vCount++;
                return vCount;
            }
        }
        // Отримання даних користувачів
        public List<UserData> UsersData
        {
            get
            {
                return _UsersData;
            }
        }


        #endregion  ---=== Реалізація інтерфесів ===---


        #region  ---=== Власний код  ===---
        //КОНСТРУКТОР
        public DataManager()
        {
            _UsersData = new List<UserData>();                  // Списки ДАНИХ користувачів
        }

        // Конвертуєм відповідь користувача в тип enum "МОЖЛИВІ ВІДПОВІДІ КОРИСТУВАЧА"
        private AnswerQuestion GetAnswerQuestion(string aAnswer)
        {
            switch (aAnswer)
            {
                case "Disagree strongly"://"Катигорично НЕ ЗГОДЕН":
                    return AnswerQuestion.Disagree_strongly;
                case "Disagree": //"НЕ ЗГОДЕН":
                    return AnswerQuestion.Disagree;
                case "Disagree slightly"://"Трохи НЕ ЗГОДЕН":
                    return AnswerQuestion.Disagree_slightly;
                case "Neither agree nor disagree"://"Ні погоджуюсь, ні не погоджуюсь":
                    return AnswerQuestion.Neither;
                case "Agree slightly"://"Трохи ЗГОДЕН":
                    return AnswerQuestion.Agree_slightly;
                case "Agree"://"ЗГОДЕН":
                    return AnswerQuestion.Agree;
                case "Agree strongly"://"Повністю ЗГОДЕН":
                    return AnswerQuestion.Agree_strongly;
                default:
                    return AnswerQuestion.Err;
            }
        }

        // Отримання списку запитань з даних Користувача
        private string GetLineQuestion(UserData aUserData)
        {
            string vResult = "";
            foreach (string vQuestion in aUserData.UserAnswers.Keys)
                vResult = vResult + C_Const.CSV_File_Separator + vQuestion;
            return vResult;
        }

        // Отримання списку відповідей з даних Користувача
        private string GetLineAnswer(UserData aUserData)
        {
            string vResult = "";
            foreach (AnswerQuestion vAnswer in aUserData.UserAnswers.Values)
                vResult = vResult + C_Const.CSV_File_Separator + StaticMetod.GetDescription(vAnswer);
            return vResult;
        }

        #endregion  ---=== Власний код  ===---



    }
}
