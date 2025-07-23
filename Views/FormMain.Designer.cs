
namespace Views
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.TC_Main = new System.Windows.Forms.TabControl();
            this.TP_WB = new System.Windows.Forms.TabPage();
            this.TC_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // TC_Main
            // 
            this.TC_Main.Controls.Add(this.TP_WB);
            this.TC_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_Main.ItemSize = new System.Drawing.Size(66, 15);
            this.TC_Main.Location = new System.Drawing.Point(0, 0);
            this.TC_Main.Margin = new System.Windows.Forms.Padding(2);
            this.TC_Main.Name = "TC_Main";
            this.TC_Main.SelectedIndex = 0;
            this.TC_Main.Size = new System.Drawing.Size(1208, 583);
            this.TC_Main.TabIndex = 0;
            // 
            // TP_WB
            // 
            this.TP_WB.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TP_WB.Location = new System.Drawing.Point(4, 19);
            this.TP_WB.Margin = new System.Windows.Forms.Padding(2);
            this.TP_WB.Name = "TP_WB";
            this.TP_WB.Padding = new System.Windows.Forms.Padding(2);
            this.TP_WB.Size = new System.Drawing.Size(1200, 560);
            this.TP_WB.TabIndex = 0;
            this.TP_WB.Text = "WB";
            this.TP_WB.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1208, 583);
            this.Controls.Add(this.TC_Main);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormMain";
            this.Text = "Archetypes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.TC_Main.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage TP_WB;
        private System.Windows.Forms.TabControl TC_Main;
    }
}