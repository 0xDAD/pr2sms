using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Soap;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using PR2_SMS.Properties;


namespace PR2_SMS
{
    internal partial class OptionsForm : Form
    {
        SMSSettings options;
        public SMSSettings Options
        {
            get { return options; }
            set { options = value; Init(); }
        }

        public OptionsForm()
        {
            options = new SMSSettings();
            InitializeComponent();
            InitDataGrid();
        }
        private void InitDataGrid()
        {
            dataGridView1.Rows.Clear();
            DataGridViewRow row1 = new DataGridViewRow();
            row1.CreateCells(dataGridView1, new string[] { "Шаблон текста", Settings.Default.msg_row.ToString(), Settings.Default.msg_col.ToString() });
            dataGridView1.Rows.Add(row1);
            DataGridViewRow row2 = new DataGridViewRow();
            row2.CreateCells(dataGridView1, new string[] { "Начало списка", Settings.Default.list_row.ToString(), Settings.Default.list_col.ToString() });
            dataGridView1.Rows.Add(row2);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            simpleXLSReadingModeCB.Checked = Settings.Default.simpleXLSParsingMode;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StringRequest sr = new StringRequest();
            sr.Text = "Имя конфигурации";
            sr.InitialText = options.ConfigName;
            if (sr.ShowDialog(this) == DialogResult.OK)
            {
                GetValues();
                SMSSettings.SaveConfig(sr.InitialText, options);
                ScanSettings();
                modified = false;
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            if (modified)
            {
                GetValues();
                if (MessageBox.Show("Вы хотите перезаписать набор настроек " + options.ConfigName + "?", "Pr2", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SMSSettings.SaveConfig(options.ConfigName, options);
                }
                else
                {
                    StringRequest sr = new StringRequest();
                    sr.Text = "Имя конфигурации";
                    if (sr.ShowDialog(this) == DialogResult.OK)
                    {
                        SMSSettings.SaveConfig(sr.InitialText, options);
                        ScanSettings();
                    }
                    else
                        return;
                }
            }
            SaveSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void GetValues()
        {
            int portval = -1;

            options.HostName = hostTBss.Text;
            Int32.TryParse(portTBss.Text, out portval);
            if (portval > 1000)
            {
                options.Port = portval;
            }
            else portTBss.Text = options.Port.ToString();
            portval = -1;
            options.User = userTBss.Text;
            options.Password = passTB.Text;
            options.Coding = scoding;
            options.DlrMask = (byte)(checkBox2.Checked ? 18 : 0);
            options.MySqlServerAddress = sqlAddrTB.Text;
            Int32.TryParse(sqlPortTB.Text, out portval);
            if (portval > 1000)
            {
                options.MySqlServerPort = sqlPortTB.Text;
            }
            else
                sqlPortTB.Text = options.MySqlServerPort;
            options.MySqlUser = sqlUserTB.Text;
            options.MySqlPass = sqlPassTB.Text;
            options.SqlBoxDbName = dBNameTB.Text;
            options.CheckNumbers = checkBox3.Checked;
        }



        private void ScanSettings()
        {
            comboBox1.Items.Clear();
            string[] setList = Directory.GetFiles(SMSSettings.SPath, "*.ksf", SearchOption.TopDirectoryOnly);
            foreach (string sfn in setList)
            {
                int i = this.comboBox1.Items.Add(Path.GetFileNameWithoutExtension(sfn));
            }

            if (-1 != comboBox1.Items.IndexOf(Options.ConfigName))
            {
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(Options.ConfigName);
            }
            else
            {

            }
        }

        private void OptionsForm_Shown(object sender, EventArgs e)
        {
            ScanSettings();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() != Options.ConfigName)
            {
                options = SMSSettings.LoadConfig(comboBox1.SelectedItem.ToString());
                Init();
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            modified = true;
            
        }

        private void SaveSettings()
        {
            Settings.Default.msg_row = int.Parse((string)dataGridView1.Rows[0].Cells[1].Value);
            Settings.Default.msg_col = int.Parse((string)dataGridView1.Rows[0].Cells[2].Value);
            Settings.Default.list_row = int.Parse((string)dataGridView1.Rows[1].Cells[1].Value);
            Settings.Default.list_col = int.Parse((string)dataGridView1.Rows[1].Cells[2].Value);
            Settings.Default.simpleXLSParsingMode = simpleXLSReadingModeCB.Checked;
            Settings.Default.kannel_setts = options.ConfigName;

            Settings.Default.Save();

        }
        int scoding = -1;
        public bool CodingChanged
        {
            get { return scoding != options.Coding; }
        }

        bool modified = false;
        public bool Modified
        {
            get { return modified; }
        }
        private void Init()
        {
            if (options != null)
            {
                hostTBss.Text = options.HostName;
                portTBss.Text = options.Port.ToString();
                userTBss.Text = options.User;
                passTB.Text = options.Password;
                checkBox1ss.Checked = options.Coding == 0;
                scoding = options.Coding;
                sqlAddrTB.Text = options.MySqlServerAddress;
                sqlPortTB.Text = options.MySqlServerPort;
                sqlUserTB.Text = options.MySqlUser;
                sqlPassTB.Text = options.MySqlPass;
                dBNameTB.Text = options.SqlBoxDbName;
                checkBox2.Checked = options.DlrMask == 18;
            }
            modified = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            scoding = checkBox1ss.Checked ? 0 : 2;
            modified = scoding != options.Coding;
        }
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            modified = true;
        }

        private void handler_ModifiedChanged(object sender, EventArgs e)
        {
            modified = true;
        }

        private void adduser_Click(object sender, EventArgs e)
        {

        }

        private void removeuser_Click(object sender, EventArgs e)
        {

        }
    }
}