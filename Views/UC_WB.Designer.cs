namespace Views
{
    partial class UC_WB
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.WebV = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.RTB_History = new System.Windows.Forms.RichTextBox();
            this.Panel_T = new System.Windows.Forms.Panel();
            this.B_LoadDataFromFile = new System.Windows.Forms.Button();
            this.B_Start = new System.Windows.Forms.Button();
            this.B_LoadQuestionsFromWeb = new System.Windows.Forms.Button();
            this.OFD_LoadDataFromFile = new System.Windows.Forms.OpenFileDialog();
            this.Panel_R = new System.Windows.Forms.Panel();
            this.Panel_L = new System.Windows.Forms.Panel();
            this.SFD_SaveFile = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.WebV)).BeginInit();
            this.Panel_T.SuspendLayout();
            this.Panel_R.SuspendLayout();
            this.Panel_L.SuspendLayout();
            this.SuspendLayout();
            // 
            // WebV
            // 
            this.WebV.AllowExternalDrop = true;
            this.WebV.BackColor = System.Drawing.Color.Gold;
            this.WebV.CreationProperties = null;
            this.WebV.DefaultBackgroundColor = System.Drawing.Color.White;
            this.WebV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebV.Location = new System.Drawing.Point(0, 0);
            this.WebV.Margin = new System.Windows.Forms.Padding(2);
            this.WebV.Name = "WebV";
            this.WebV.Size = new System.Drawing.Size(991, 560);
            this.WebV.TabIndex = 1;
            this.WebV.ZoomFactor = 1D;
            // 
            // RTB_History
            // 
            this.RTB_History.BackColor = System.Drawing.SystemColors.Info;
            this.RTB_History.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RTB_History.Location = new System.Drawing.Point(0, 65);
            this.RTB_History.Margin = new System.Windows.Forms.Padding(2, 5, 2, 2);
            this.RTB_History.Name = "RTB_History";
            this.RTB_History.ReadOnly = true;
            this.RTB_History.Size = new System.Drawing.Size(209, 495);
            this.RTB_History.TabIndex = 1;
            this.RTB_History.Text = "";
            // 
            // Panel_T
            // 
            this.Panel_T.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Panel_T.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel_T.Controls.Add(this.B_LoadDataFromFile);
            this.Panel_T.Controls.Add(this.B_Start);
            this.Panel_T.Controls.Add(this.B_LoadQuestionsFromWeb);
            this.Panel_T.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel_T.Location = new System.Drawing.Point(0, 0);
            this.Panel_T.Margin = new System.Windows.Forms.Padding(2);
            this.Panel_T.Name = "Panel_T";
            this.Panel_T.Size = new System.Drawing.Size(209, 65);
            this.Panel_T.TabIndex = 0;
            // 
            // B_LoadDataFromFile
            // 
            this.B_LoadDataFromFile.BackColor = System.Drawing.Color.Green;
            this.B_LoadDataFromFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.B_LoadDataFromFile.ForeColor = System.Drawing.Color.Yellow;
            this.B_LoadDataFromFile.Location = new System.Drawing.Point(4, 18);
            this.B_LoadDataFromFile.Margin = new System.Windows.Forms.Padding(2);
            this.B_LoadDataFromFile.Name = "B_LoadDataFromFile";
            this.B_LoadDataFromFile.Size = new System.Drawing.Size(95, 27);
            this.B_LoadDataFromFile.TabIndex = 1;
            this.B_LoadDataFromFile.Text = "Завантажити";
            this.B_LoadDataFromFile.UseVisualStyleBackColor = false;
            // 
            // B_Start
            // 
            this.B_Start.BackColor = System.Drawing.Color.DarkRed;
            this.B_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.B_Start.ForeColor = System.Drawing.Color.Yellow;
            this.B_Start.Location = new System.Drawing.Point(107, 18);
            this.B_Start.Margin = new System.Windows.Forms.Padding(2);
            this.B_Start.Name = "B_Start";
            this.B_Start.Size = new System.Drawing.Size(95, 27);
            this.B_Start.TabIndex = 0;
            this.B_Start.Text = "Розпочати";
            this.B_Start.UseVisualStyleBackColor = false;
            this.B_Start.Visible = false;
            // 
            // B_LoadQuestionsFromWeb
            // 
            this.B_LoadQuestionsFromWeb.BackColor = System.Drawing.Color.Yellow;
            this.B_LoadQuestionsFromWeb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.B_LoadQuestionsFromWeb.Location = new System.Drawing.Point(4, 3);
            this.B_LoadQuestionsFromWeb.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.B_LoadQuestionsFromWeb.Name = "B_LoadQuestionsFromWeb";
            this.B_LoadQuestionsFromWeb.Size = new System.Drawing.Size(95, 25);
            this.B_LoadQuestionsFromWeb.TabIndex = 3;
            this.B_LoadQuestionsFromWeb.Text = "Load Questions";
            this.B_LoadQuestionsFromWeb.UseVisualStyleBackColor = false;
            this.B_LoadQuestionsFromWeb.Visible = false;
            // 
            // OFD_LoadDataFromFile
            // 
            this.OFD_LoadDataFromFile.FileName = "OFD";
            // 
            // Panel_R
            // 
            this.Panel_R.Controls.Add(this.RTB_History);
            this.Panel_R.Controls.Add(this.Panel_T);
            this.Panel_R.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel_R.Location = new System.Drawing.Point(991, 0);
            this.Panel_R.Margin = new System.Windows.Forms.Padding(2);
            this.Panel_R.Name = "Panel_R";
            this.Panel_R.Size = new System.Drawing.Size(209, 560);
            this.Panel_R.TabIndex = 1;
            // 
            // Panel_L
            // 
            this.Panel_L.Controls.Add(this.WebV);
            this.Panel_L.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_L.Location = new System.Drawing.Point(0, 0);
            this.Panel_L.Name = "Panel_L";
            this.Panel_L.Size = new System.Drawing.Size(991, 560);
            this.Panel_L.TabIndex = 2;
            // 
            // SFD_SaveFile
            // 
            this.SFD_SaveFile.FileName = "SFD";
            // 
            // UC_WB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.Panel_L);
            this.Controls.Add(this.Panel_R);
            this.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UC_WB";
            this.Size = new System.Drawing.Size(1200, 560);
            ((System.ComponentModel.ISupportInitialize)(this.WebV)).EndInit();
            this.Panel_T.ResumeLayout(false);
            this.Panel_R.ResumeLayout(false);
            this.Panel_L.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel Panel_T;
        private System.Windows.Forms.OpenFileDialog OFD_LoadDataFromFile;
        private System.Windows.Forms.Button B_LoadDataFromFile;
        private System.Windows.Forms.Button B_Start;
        private System.Windows.Forms.RichTextBox RTB_History;
        private Microsoft.Web.WebView2.WinForms.WebView2 WebV;
        private System.Windows.Forms.Panel Panel_R;
        private System.Windows.Forms.Panel Panel_L;
        private System.Windows.Forms.Button B_LoadQuestionsFromWeb;
        private System.Windows.Forms.SaveFileDialog SFD_SaveFile;
    }
}
