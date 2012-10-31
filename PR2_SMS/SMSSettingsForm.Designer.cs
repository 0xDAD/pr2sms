namespace PR2_SMS
{
    partial class SMSSettingsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.portTBss = new System.Windows.Forms.TextBox();
            this.label4ss = new System.Windows.Forms.Label();
            this.label2ss = new System.Windows.Forms.Label();
            this.label3ss = new System.Windows.Forms.Label();
            this.label1ss = new System.Windows.Forms.Label();
            this.paTBss = new System.Windows.Forms.TextBox();
            this.userTBss = new System.Windows.Forms.TextBox();
            this.hostTBss = new System.Windows.Forms.TextBox();
            this.checkBox1ss = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1ss);
            this.groupBox1.Controls.Add(this.portTBss);
            this.groupBox1.Controls.Add(this.label4ss);
            this.groupBox1.Controls.Add(this.label2ss);
            this.groupBox1.Controls.Add(this.label3ss);
            this.groupBox1.Controls.Add(this.label1ss);
            this.groupBox1.Controls.Add(this.paTBss);
            this.groupBox1.Controls.Add(this.userTBss);
            this.groupBox1.Controls.Add(this.hostTBss);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 136);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройка сервера";
            // 
            // portTB
            // 
            this.portTBss.Location = new System.Drawing.Point(151, 32);
            this.portTBss.Name = "portTB";
            this.portTBss.Size = new System.Drawing.Size(45, 20);
            this.portTBss.TabIndex = 2;
            this.portTBss.TextChanged += new System.EventHandler(this.portTB_TextChanged);
            // 
            // label4
            // 
            this.label4ss.AutoSize = true;
            this.label4ss.Location = new System.Drawing.Point(122, 55);
            this.label4ss.Name = "label4";
            this.label4ss.Size = new System.Drawing.Size(45, 13);
            this.label4ss.TabIndex = 1;
            this.label4ss.Text = "Пароль";
            // 
            // label2
            // 
            this.label2ss.AutoSize = true;
            this.label2ss.Location = new System.Drawing.Point(148, 16);
            this.label2ss.Name = "label2";
            this.label2ss.Size = new System.Drawing.Size(32, 13);
            this.label2ss.TabIndex = 1;
            this.label2ss.Text = "Порт";
            // 
            // label3
            // 
            this.label3ss.AutoSize = true;
            this.label3ss.Location = new System.Drawing.Point(12, 55);
            this.label3ss.Name = "label3";
            this.label3ss.Size = new System.Drawing.Size(80, 13);
            this.label3ss.TabIndex = 1;
            this.label3ss.Text = "Пользователь";
            // 
            // label1
            // 
            this.label1ss.AutoSize = true;
            this.label1ss.Location = new System.Drawing.Point(12, 16);
            this.label1ss.Name = "label1";
            this.label1ss.Size = new System.Drawing.Size(31, 13);
            this.label1ss.TabIndex = 1;
            this.label1ss.Text = "Хост";
            // 
            // paTBss
            // 
            this.paTBss.Location = new System.Drawing.Point(125, 71);
            this.paTBss.Name = "paTBss";
            this.paTBss.Size = new System.Drawing.Size(71, 20);
            this.paTBss.TabIndex = 0;
            this.paTBss.TextChanged += new System.EventHandler(this.passTB_TextChanged);
            // 
            // userTB
            // 
            this.userTBss.Location = new System.Drawing.Point(15, 71);
            this.userTBss.Name = "userTB";
            this.userTBss.Size = new System.Drawing.Size(77, 20);
            this.userTBss.TabIndex = 0;
            this.userTBss.TextChanged += new System.EventHandler(this.userTB_TextChanged);
            // 
            // hostTB
            // 
            this.hostTBss.Location = new System.Drawing.Point(15, 32);
            this.hostTBss.Name = "hostTB";
            this.hostTBss.Size = new System.Drawing.Size(130, 20);
            this.hostTBss.TabIndex = 0;
            this.hostTBss.TextChanged += new System.EventHandler(this.hostTB_TextChanged);
            // 
            // checkBox1
            // 
            this.checkBox1ss.AutoSize = true;
            this.checkBox1ss.Location = new System.Drawing.Point(15, 106);
            this.checkBox1ss.Name = "checkBox1";
            this.checkBox1ss.Size = new System.Drawing.Size(195, 17);
            this.checkBox1ss.TabIndex = 3;
            this.checkBox1ss.Text = "Автоматическая транслитерация";
            this.checkBox1ss.UseVisualStyleBackColor = true;
            this.checkBox1ss.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // SMSSettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(222, 136);
            this.Controls.Add(this.groupBox1);
            this.Name = "SMSSettingsForm";
            this.Text = "SMSSettingsForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4ss;
        private System.Windows.Forms.Label label2ss;
        private System.Windows.Forms.Label label3ss;
        private System.Windows.Forms.Label label1ss;
        private System.Windows.Forms.TextBox paTBss;
        private System.Windows.Forms.TextBox hostTBss;
        private System.Windows.Forms.TextBox portTBss;
        private System.Windows.Forms.TextBox userTBss;
        private System.Windows.Forms.CheckBox checkBox1ss;

    }
}