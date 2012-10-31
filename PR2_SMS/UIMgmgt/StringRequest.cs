using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PR2_SMS
{
    public partial class StringRequest : Form
    {
        string initialText = "";

        public string InitialText
        {
            get { return initialText; }
            set { initialText = value; }
        }     
       
        public StringRequest()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            initialText = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void StringRequest_Load(object sender, EventArgs e)
        {
            textBox1.Text = initialText;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Ok_Click(null, null);
            }
        }


    }
}