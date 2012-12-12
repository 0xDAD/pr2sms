using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using PR2_SMS.Properties;
using MyDebug;
using System.Diagnostics;
namespace PR2_SMS
{
    partial class MassSendControl
    {
        EnhancedListView currentView = null;
        void CreateTab(string tabname, string lvname, GetItemDelegate gidel,GetCollection gcdel)
        {
            if (tabControl1.TabPages.ContainsKey(tabname))
            {
                currentView = (EnhancedListView)tabControl1.Controls[lvname];
            }
            else
            {
                if (gidel == null)
                    return;
                TabPage newTP = new TabPage(tabname);
                newTP.Name = tabname;
                currentView = new EnhancedListView();
                currentView.FullRowSelect = true;
                currentView.GridLines = true;
                currentView.Name = lvname;
                currentView.TabIndex = 0;
                currentView.UseCompatibleStateImageBehavior = false;
                currentView.View = View.Details;
                currentView.VirtualMode = true;
                currentView.Dock = DockStyle.Fill;
                currentView.SetCallback(gidel);
                currentView.Tag = gcdel;
                currentView.ContextMenuStrip = contextMenuStrip1;
                newTP.Controls.Add(currentView);
                tabControl1.TabPages.Add(newTP);
            }
            tabControl1.SelectTab(tabname);
        }
        EnhancedListView GetTabLv(string tabname, string lvname)
        {
            if (!tabControl1.TabPages.ContainsKey(tabname))
                return null;
            if (!tabControl1.TabPages[tabname].Controls.ContainsKey(lvname))
                return null;
            return (EnhancedListView)tabControl1.TabPages[tabname].Controls[lvname];
        }
        private void InitInterface()
        {
            TraceHelper.InitializeTrace(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/pr2sms.log");
            TraceHelper.InitializeStatusDelegate(UpdateStatus);
            docView.SetCallback(GetItem);
        }
        private void UpdateWinCap()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            AssemblyName assname = ass.GetName();
            Version v = assname.Version;
            string workingFileString = massMsg.IsEmpty ? "" : String.Format(" - {0}", massMsg.MassMsgName);
            string caption = String.Format("Менеджер массовой рассылки PR2 (v {0}.{1}.{2}){3}", v.Major, v.Minor, v.Build, workingFileString);
            StringTransl del = delegate(String cap)
            {
                this.Text = cap;
            };
            if (!this.InvokeRequired)
            {
                del(caption);
            }
            else
            {
                this.Invoke(del, caption);
            }
        }
        
        void UpdateDocViewColumns()
        {
            if (docView.Columns.Count == 0)
            {
                docView.AddColumn("№");
                docView.AddColumn("Номер");
                docView.AddColumn("Сообщение");
                docView.AddColumn("Символы");
                docView.AddColumn("Части");
                docView.AddColumn("Статус");
                docView.AddColumn("Результат");
            }
        }
        void ProgressChangedEventHandler(int progress)
        {
            toolStrip1.Invoke((ChangeProgressNotifier)delegate(int p)
            {
                mainProgressBar.Value = p;
            }, progress );

        }
        private void UpdateDocView()
        {
        //    if (ep.ParamList != null && ep.ParamList.Count > 0)
          //  {
                UpdateDocViewColumns();
                docView.Invoke((ChangeProgressNotifier)delegate(int cnt)
                {
                    docView.VirtualListSize = cnt;
                    docView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                }, massMsg.RcvList.Count);

            //}
        }
        private void UpdateSqlView()
        {
            if (msm.MsgList != null && msm.MsgList.Count > 0)
            {
                currentView = GetTabLv("MySQL", "sqlView");
                currentView.VirtualListSize = 0;
                UpdateSqlViewColumns();
                currentView.Invoke((ChangeProgressNotifier)delegate(int cnt)
                 {
                     currentView.ClearCache();
                     currentView.VirtualListSize = cnt;
                     currentView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                     currentView.Refresh();
                 }, msm.MsgList.Count);

            }
        }
        void UpdateSqlViewColumns()
        {
            if (currentView.Columns.Count == 0)
            {
                currentView.AddColumn("№");
                currentView.AddColumn("Тип");
                currentView.AddColumn("Отправитель");
                currentView.AddColumn("Получатель");
                currentView.AddColumn("Сообщение");
                currentView.AddColumn("Время");
                currentView.AddColumn("Символы");
                currentView.AddColumn("Части");
                currentView.AddColumn("Кодирование");                
            }
        }
        private void InfoMsg(string p)
        {
            TraceHelper.Report("Сообщение: " + p);
            MessageBox.Show(p, "Менеджер рассылки", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        internal void UpdateStatus(ref string value)
        {
            value = String.Format("[{0}]:{1}", DateTime.Now.ToString("G"), value);
            if (!toolStrip1.InvokeRequired)
            {
                toolStripComboBox1.SelectedIndex = toolStripComboBox1.Items.Add(value);
            }
            else
            {
                StringTransl st = delegate(string m) { toolStripComboBox1.SelectedIndex = toolStripComboBox1.Items.Add(m); };
                toolStrip1.Invoke(st, value);
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.AssemblyCompany = "\"Студия спецпроектов \"PR квадрат\"";
            ab.AssemblyCopyright = "Copyright © 2011  Алексей Засенко";
            ab.AssemblyTitle = "О программе \"Менеджер массовой рассылки PR2\"";
            
            Assembly ass = Assembly.GetExecutingAssembly();
            AssemblyName assname = ass.GetName();
            Version v = assname.Version;

            ab.AssemblyVersion = String.Format("Версия {0}.{1} build {2}", v.Major, v.Minor, v.Build);
            ab.AssemblyDescription = "Внимание! Данная программа защищена законами об авторских правах и международными соглашениями. Незаконное воспроизведение или распространение данной программы или любой ее части влечет гражданскую и уголовную ответственность.";

            ab.ShowDialog();

        }
        private void MassSendControl_Resize(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case FormWindowState.Minimized:

                    this.ShowInTaskbar = false;
                    break;
                case FormWindowState.Maximized:
                case FormWindowState.Normal:
                    this.ShowInTaskbar = true;
                    break;
            }
        }
        private void MassSendControl_Load(object sender, EventArgs e)
        {
            notifyIcon1.Icon = Resources.Icon_137;
            notifyIcon1.Visible = true;
        }
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            else
            {
                this.WindowState = FormWindowState.Minimized;
            }


        }

    }
}