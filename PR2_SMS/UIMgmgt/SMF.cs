using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace PR2_SMS
{
    public partial class SingleMessageForm : Form
    {
        string msg;

        public string Msg
        {
            get { return msg; }
            set {
                textBox7.Text = value; msg = value;
            }
        }
        string to;

        public string To
        {
            get { return to; }
            set {
                textBox4.Text = value;
                to = value; }
        }
        public SingleMessageForm()
        {
            InitializeComponent();           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            msg = textBox7.Text;
            to = textBox4.Text;
            string num = to;
            if (!MassMsgStorage.CheckNumber(to, ref num))
            {
                MessageBox.Show("Неверный формат номера");
                return;
            }
            DialogResult = DialogResult.OK;
            this.Close();

        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            label8.Text = textBox7.Text.Length.ToString();
        }
    }
}