using System;
using System.Threading;
using System.Windows.Forms;

namespace Views
{
    public interface IFormMain
    {
        void Run();                             // Метод запуску програми          
        IUC_WB UC_WB { get; }                   //  Обєкт вкладки "WebBrouser"
        IProcessorWB ProcessorWB { get; }       //  Обєкт "WebBrouser" (обробка даних)
        // ---=== Події ===---
        event Action Event_FormClosing;               // Подія закриття форми(программи)
    }
    public partial class FormMain : Form, IFormMain
    {
        #region ---=== ПОЛЯ ===---
        private readonly IUC_WB _UC_WB;                  // Обєкт вкладки "WebBrouser"
        private readonly IProcessorWB _ProcessorWB;      // Обєкт "WebBrouser" (обробка даних)
        #endregion ---=== ПОЛЯ ===---

        #region ---=== Прокидання подій ===---
        //-------------------------------------------------------------------------       
        public event Action Event_FormClosing;               // Подія закриття форми(программи)
        //-------------------------------------------------------------------------                       

        #endregion ---=== Прокидання подій ===---

        #region ---=== Реалізація інтерфесу IFormMain ===---
        // ПОЧАТОК ПРОГРАММИ - Запуск Головної форми 
        public void Run()
        {
            Application.Run(this);
        }

        //  Повернення обєкта(UserControl) вкладки "WebBrouser"
        public IUC_WB UC_WB
        {
            get { return _UC_WB; }
        }

        //  Повернення обєкта"WebBrouser" (обробка даних)
        public IProcessorWB ProcessorWB
        {
            get { return _ProcessorWB; }
        }

        #endregion ---=== Реалізація інтерфесу IMainForm ===---

        #region ---=== Власний код  ===---
        // ---=== КОНСТРУКТОР (створення Головної Форми)===---
        public FormMain()
        {
            InitializeComponent();
            // ---=== Прикріпляєм та розміщаєм контроли на вкладках ===---
            _UC_WB = new UC_WB();                               // Вкладка "WebBrouser"

            this.TP_WB.Controls.Add((Control)_UC_WB);
            this.TP_WB.Controls[0].Dock = DockStyle.Fill;
            _ProcessorWB = _UC_WB.ProcessorWB;
            _ProcessorWB.Event_ProcesStop += Event_ProcesStop;
        }

        #endregion ---=== Власний код  ===---

        // Закриття форми
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Event_FormClosing?.Invoke();
        }


        // Подія зупинення програми (відображення секунд)
        private void Event_ProcesStop(int aTimeSecond_ToClose)
        {
            this.Text = this.Text + " " + aTimeSecond_ToClose.ToString();
            System.Windows.Forms.Timer vTimer = new System.Windows.Forms.Timer // Таймер
            {
                Interval = 1000,    // Інтервал спрацювання таймера
                Enabled = true      // Таймер включений
            };
            vTimer.Tick += Timer_Tick;               // Метод який відпрацьовує з вказаним інтервалом при включеному таймері
        }
        private void Timer_Tick(object sender, System.EventArgs e)
        {
            string vFormText = this.Text.Split(' ')[0];
            int vLabel_Second = Int32.Parse(this.Text.Split(' ')[1]);
            Interlocked.Decrement(ref vLabel_Second); // Зменшуэм секунди.
            this.Text = vFormText + " " + vLabel_Second.ToString();
        }

    }
}
