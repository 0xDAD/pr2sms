using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PR2_SMS
{
    public partial class SMSSettingsForm : Form
    {
        SMSSettings selectedObject;
        bool modified = false;
        public int scoding = -1;
        public bool Modified
        {
            get { return modified; }
            
        }
        internal SMSSettings SelectedObject
        {
            get { return selectedObject; }
            set {
                selectedObject = value;
                Init();
            }
        }

        private void Init()
        {
            if(selectedObject!=null)
            {
                hostTBss.Text = selectedObject.HostName;
                portTBss.Text = selectedObject.Port.ToString();
                userTBss.Text = selectedObject.User;
                paTBss.Text = selectedObject.Password;
                checkBox1ss.Checked = selectedObject.Coding == 0;
                scoding = selectedObject.Coding;
            }
            modified = false;
        }


        public SMSSettingsForm()
        {
            InitializeComponent();
        }
        private void hostTB_TextChanged(object sender, EventArgs e)
        {
            modified = true;
            selectedObject.HostName = hostTBss.Text;
        }

        private void userTB_TextChanged(object sender, EventArgs e)
        {
            modified = true;
            selectedObject.User = userTBss.Text;
        }

        private void passTB_TextChanged(object sender, EventArgs e)
        {
            modified = true;
            selectedObject.Password = paTBss.Text;
        }

        private void portTB_TextChanged(object sender, EventArgs e)
        {
            modified = true;
            
            int portval = -1;
            Int32.TryParse(portTBss.Text, out portval);
            selectedObject.Port = portval > 0 ? portval : selectedObject.Port;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1ss.Checked)
            {
                selectedObject.Coding = 0;
                selectedObject.Charset = "";
            }
            else
            {
                selectedObject.Coding = 2;
                selectedObject.Charset = "utf8";
            }
        }



    }
}