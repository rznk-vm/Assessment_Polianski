using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace Views
{
    public interface IUC_WB
    {
        event Action Event_Save_Questions_FromWeb;          // Подія ЗБЕРЕЖЕННЯ списку запитань завантажених з WEB
        event Action<string> Event_LoadDataFromFile;        // Подія натискання кнопки "Завантажити з дані з файлу" 
        event Action Event_Start;                           // Подія натискання кнопки "Розпочати" 

        void ShowInfo(List<string> aRecords);           // Додати рядки до інформаційної панелі
        void ClearLinesInfo();                          // Очистка інформаційної панелі
        bool Button_Start_Visble { set; }               // Зробити кнопку "Розпочати" (видимою / схованою)
        bool Button_Load_Visble { set; }                // Зробити кнопку "Завантажити" (видимою / схованою)
        IProcessorWB ProcessorWB { get; }                //  Обєкт "WebBrouser" (обробка даних) 


    }
    public partial class UC_WB : UserControl, IUC_WB
    {

        #region ---=== ПОЛЯ ===---
        private readonly IProcessorWB _ProcessorWB;
        #endregion ---=== ПОЛЯ ===---

        #region ---=== Прокидання подій ===---
        //-------------------------------------------------------------------------       
        public event Action Event_Save_Questions_FromWeb;            // Подія ЗБЕРЕЖЕННЯ списку запитань завантажених з WEB        
        public event Action<string> Event_LoadDataFromFile;         // Подія натискання кнопки "Завантажити з дані з файлу"
        public event Action Event_Start;                            // Подія натискання кнопки "Розпочати"

        //-------------------------------------------------------------------------                       

        #endregion ---=== Прокидання подій ===---

        #region ---=== Реалізація інтерфесу ===---

        // Додати рядки до інформаційної панелі
        public void ShowInfo(List<string> aRecords)
        {
            foreach (string vLine in aRecords)
            {
                int vEndIndexLine = RTB_History.Lines.Count() - 1;

                if (vEndIndexLine == -1 && vLine[0] == '\n')
                    RTB_History.AppendText(vLine.Remove(0, 1));
                else
                    RTB_History.AppendText(vLine);
                RTB_History.SelectionStart = RTB_History.TextLength;
                RTB_History.ScrollToCaret();
            }
            this.RTB_Stylization(); // Стілизация останнього рядка історії історії
        }

        // Очистка інформаційної панелі
        public void ClearLinesInfo()
        {
            RTB_History.Clear();
        }

        // Зробити кнопку "Розпочати" (видимою / схованою)
        public bool Button_Start_Visble
        {
            set => B_Start.Visible = value;
        }

        // Зробити кнопку "Завантажити" (видимою / схованою)
        public bool Button_Load_Visble
        {
            set => B_LoadDataFromFile.Visible = value;
        }

        //  Обєкт "WebBrouser" (обробка даних) 
        public IProcessorWB ProcessorWB
        {
            get
            {
                return _ProcessorWB;
            }
        }

        #endregion ---=== Реалізація інтерфесу ===---

        #region ---=== Власний код  ===---
        //КОНСТРУКТОР
        public UC_WB()
        {
            InitializeComponent();
            // === ЗНАЧЕНЯ ПО ЗАМОВЧУВАНЮ ===        
            _ProcessorWB = new ProcessorWB(this.WebV);                      // Створюєм клас обробки подій Веб сторінки                                                                            

            // == Видимість КНОПОК ===            
            /*
            this.B_LoadQuestionsFromWeb.Location = new Point(107, 18);
            this.B_LoadQuestionsFromWeb.Visible = true;
            */
            // == КНОПКИ та СУБМЕНЮ ===
            this.B_LoadQuestionsFromWeb.Click += B_LoadQuestionsFromWeb_Click;  // Натискання кнопки "Load" завантаження списку запитань
            this.B_LoadDataFromFile.Click += B_LoadDataFromFile_Click;          // Натискання кнопки "Завантажити" (Вхідні дані)                     
            this.B_Start.Click += B_Start_Click;                            // Натискання кнопки "Розпочати" 
        }

        // ---=== КНОПКИ ===---

        // Натискання кнопки "Load Questions"/ "Завантажити список запитань"
        private void B_LoadQuestionsFromWeb_Click(object sender, EventArgs e)
        {
            Event_Save_Questions_FromWeb?.Invoke();
        }

        // Натискання КНОПКИ "Завантажити" (Вхідні дані)
        private void B_LoadDataFromFile_Click(object sender, EventArgs e)
        {
            OFD_LoadDataFromFile.InitialDirectory = Environment.CurrentDirectory;

            OFD_LoadDataFromFile.Filter = "tsv з род.\"tab\" (*.tsv)|*.tsv"; // Встановлюєм фільтр для завантаження даних з файлу 
            if (OFD_LoadDataFromFile.ShowDialog() == DialogResult.OK)
            {
                // Якщо файл вибрано
                Button vBOF = (sender as Button);

                // Вдале завершення вибору файлу після натискання кнопки "Завантажити дані з файлу" викликає подію завантаження з фалу
                Event_LoadDataFromFile?.Invoke(OFD_LoadDataFromFile.FileName);
            }
        }

        // Натискання КНОПКИ "Розпочати" (ApplicationController-> this.Start_EnteringUsersData())
        private void B_Start_Click(object sender, EventArgs e)
        {
            // Після натискання кнопки "РОЗПОЧАТИ" викликаємо подію з головного модуля             
            Event_Start?.Invoke();
        }


        //--------------============ ============--------------
        // TODO _!!!!! Стілизация інформаційної панелі
        private void RTB_Stylization()
        {
            int vStartIndex_InCurLine = 0; // Індекс першого символу поточного рядка
            //Розфарбування Історії за Ключовими словами(Критеріями)
            for (int i = 0; i < RTB_History.Lines.Count(); i++)
            {
                string vLine = RTB_History.Lines[i]; // Отримуэм текст рядка                
                //int vStartIndex_InCurLine = RTB_History.GetFirstCharIndexFromLine(i); // Індекс першого символу поточного рядка

                // Якщо в рядку є запис 
                string vFindText = "НЕ ЗАВАНТАЖЕНО !!!";
                if (vLine.Contains(vFindText)) // "НЕ ЗАВАНТАЖЕНО"
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Red;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }
                vFindText = "Дані користувача";
                if (vLine.Contains(vFindText))
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Red;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }

                vFindText = "НЕ знайдено ЗАПИТАННЯ";
                if (vLine.Contains(vFindText))
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Red;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }

                vFindText = "ЗУПИНЕНО";
                if (vLine.Contains(vFindText))
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Red;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }

                /*
                // Якщо в рядку є запис 
                vFindText = "ВІДПОВІДЬ:";
                if (vLine.Contains(vFindText)) 
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Blue;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }
                */
                /*
                vFindText = "На запитання:";
                if (vLine.Contains(vFindText)) 
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Blue;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }
                */
                vFindText = "Дані користувачів завантажені вдало !";
                if (vLine.Contains(vFindText))
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Blue;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }
                vFindText = "Дані користувачів завантажені з помилками !";
                if (vLine.Contains(vFindText))
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Red;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }
                vFindText = "Обробка запису";
                if (vLine.Contains(vFindText))
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Blue;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }
                vFindText = "Tест пройдено !";
                if (vLine.Contains(vFindText))
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Green;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }
                vFindText = "Архетип:";
                if (vLine.Contains(vFindText))
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Blue;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }
                vFindText = "Error in metod:";
                if (vLine.Contains(vFindText))
                {
                    RTB_History.Select(vStartIndex_InCurLine + vLine.IndexOf(vFindText), vFindText.Length);// Виділяєм текст в рядку
                    if (RTB_History.SelectedText == vFindText)
                    {
                        RTB_History.SelectionColor = Color.Red;// Колір виділеного тексту
                        RTB_History.SelectionFont = new Font(RTB_History.SelectionFont.FontFamily, RTB_History.SelectionFont.Size, FontStyle.Bold);// Стиль шрифта(Жирний)
                    }
                }

                vStartIndex_InCurLine = vStartIndex_InCurLine + vLine.Length + 1; // Індекс першого символу поточного рядка (для наступного рядка)                
            }
            RTB_History.SelectionLength = 0; // Знімаєм виділення            
        }
    }
    #endregion ---=== Власний код  ===---
}





