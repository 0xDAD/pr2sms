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
using System.Diagnostics;
using MyDebug;


namespace PR2_SMS
{
    public  delegate void ChangeProgressNotifier(int progress);
    internal delegate void AddItem(TargetInfo ti);
    internal delegate void ColumnAdd(EnhancedListView lv,string colName);
    internal delegate void StringTransl(string str);
    internal delegate void ReportTransl(string num, string field, string msg, string state);
    internal delegate void StringPass(ref string str);
    internal delegate void AddLvi(ListViewItem lvi);
    internal delegate List<ListViewItem> GetCollection();
    internal delegate void VoidMethod();
    

    public partial class MassSendControl : Form
    {
        public static int BytesPerChar = 2;

        ExcelMessageList curES = null;
        MassMsgStorage massMsg = null;
        MySqlKannelManager msm = null;
        KannelThreadSession kannelThreadSession = null;
        string templateFilePath = "";
        string massmsgFilePath = "";
        SMSSettings smsSetts;

        bool isWorking = false;

        public MassSendControl()
        {
            InitializeComponent();
            InitInterface();
            InitSettings();

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            ResetAll();
            TraceHelper.Report( "Приложение загружено успешно");

        }
        private void InitSettings()
        {
            Settings.Default.Reload();
            smsSetts = SMSSettings.LoadConfig(Settings.Default.kannel_setts);
            if (smsSetts == null)
            {
                smsSetts = new SMSSettings();
                smsSetts.ConfigName = "Default";
            }          

        }
        private ExcelMessageList LoadMessageList(string fileName)
        {
            if (!File.Exists(fileName))
                return null;
            templateFilePath = fileName;
            if (curES != null)
            {
                curES.CloseDocument();               
            }
            curES = new ExcelMessageList(fileName);            
            TraceHelper.Report(fileName + " загружен");
            return curES;
        }
        private bool LoadDoc(string fileName)
        {

            if (!File.Exists(fileName))
                return false;
            templateFilePath = fileName;

            if (curES == null)
            {
                curES = new ExcelMessageList(templateFilePath);
            }
            else
            {
                if (DialogResult.Yes == MessageBox.Show("Вы хотите создать новую рассылку?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    ResetAll();
                    curES = new ExcelMessageList(templateFilePath);
                }
            }
            TraceHelper.Report( templateFilePath + " загружен");            
            return true;
        }
        private void CloseDocument()
        {
            if (curES != null)
            {
                curES.CloseDocument();
            }

        }
        private void ResetAll()
        {
            CloseDocument();
            
            docView.VirtualListSize = 0;
            docView.Columns.Clear();
            ///
            logSB = new StringBuilder();

            mainProgressBar.Value = 0;
            massMsg = new MassMsgStorage();
            msm = new MySqlKannelManager();
            massMsg.TranslitEnabled = smsSetts.NeedTransliteration;
            //massMsg.progressChanged += new ChangeProgress(ProgressChangedEventHandler);
            //massMsg.reportAdded+=new ReportTransl(addStringToLog);
            
            docView.ClearCache();
            UpdateWinCap();
            
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (curES != null)
                curES.CloseDocument();
            if (!massMsg.IsEmpty)
                MassMsgStorage.Save(massmsgFilePath,massMsg);
            TraceHelper.Report("Приложение завершило работу");
            TraceHelper.Release();
            Settings.Default.Save();
        }
        private void ParseExcelDoc()
        {
            bool errorOccured = false;
            if (curES != null && curES.DocOpened)
            {
                try
                {
                    
                    massMsg.AddMessageList(curES, new ChangeProgressNotifier(this.ProgressChangedEventHandler));
                    TraceHelper.Report( String.Format("База обработана успешно! Файл = {0}; Записей = {1};Параметров = {2};",templateFilePath,massMsg.RcvList.Count,massMsg.ParamList.Count));
                    
                }
                catch(InvalidCastException ex)
                {
                    errorOccured = true;
                    TraceHelper.Report( "Исключительная ситуация при просмотре документа {ParseExcelDoc}: " + ex.Message);
                    InfoMsg("Невозможно открыть документ. Файл не соответствует настройкам таблицы");
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    errorOccured = true;
                    TraceHelper.Report( "Исключительная ситуация при просмотре документа {ParseExcelDoc}: " + ex.Message);
                    InfoMsg("Невозможно открыть документ. Файл не соответствует настройкам таблицы");
                }
                catch (Exception e)
                {
                    errorOccured = true;
                    TraceHelper.Report( "Исключительная ситуация {ParseExcelDoc}: " + e.Message);
                    InfoMsg( e.Message );
                }
                finally
                {
                    ProgressChangedEventHandler(0);
                    CloseDocument();
                    if (errorOccured) 
                        templateFilePath = string.Empty;
                    UpdateDocView();
                    UpdateWinCap();
                }
            }

        }
        private void CreateMassMsg()
        {
            bool errorOccured = false;
            try
            {
                if (curES != null && curES.DocOpened)
                {
                    if (!massMsg.Create(curES, Path.GetFileNameWithoutExtension(massmsgFilePath)))
                    {
                        InfoMsg("Не удалось создать рассылку");
                        return;
                    }
                    massMsg.AddMessageList(curES, new ChangeProgressNotifier(this.ProgressChangedEventHandler));

                    TraceHelper.Report(String.Format("База обработана успешно! Файл = {0}; Записей = {1};Параметров = {2};", templateFilePath, massMsg.RcvList.Count, massMsg.ParamList.Count));
                }
            }
            catch (InvalidCastException ex)
            {
                errorOccured = true;
                TraceHelper.Report("Исключительная ситуация при просмотре документа {ParseExcelDoc}: " + ex.Message);
                InfoMsg("Невозможно открыть документ. Файл не соответствует настройкам таблицы");
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                errorOccured = true;
                TraceHelper.Report("Исключительная ситуация при просмотре документа {ParseExcelDoc}: " + ex.Message);
                InfoMsg("Невозможно открыть документ. Файл не соответствует настройкам таблицы");
            }
            catch (Exception e)
            {
                errorOccured = true;
                TraceHelper.Report("Исключительная ситуация {ParseExcelDoc}: " + e.Message);
                InfoMsg(e.Message);
            }
            finally
            {
                ProgressChangedEventHandler(0);
                CloseDocument();
                if (errorOccured)
                    templateFilePath = string.Empty;
                UpdateDocView();
                UpdateWinCap();
            }
            

        }
        private void prop_Click(object sender, EventArgs e)
        {
            OptionsForm of = new OptionsForm();
            of.Options = smsSetts;
            if (of.ShowDialog() == DialogResult.OK)
            {
                smsSetts = (SMSSettings)of.Options;
                //need rescan                
                if(of.CodingChanged && File.Exists(templateFilePath)&&curES!=null)
                {
                    ResetAll(); 
                    if(DialogResult.Yes ==  MessageBox.Show("Режим транслитерации был изменен. Прочитать шаблон заново?", "Менеджер рассылки", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {                        
                        curES.OpenDocument(templateFilePath);
                        Thread th = new Thread(new ThreadStart(ParseExcelDoc));
                        th.Start();        
                    }
                    
                   
                }
                massMsg.TranslitEnabled = smsSetts.NeedTransliteration;

            }
        }
        private void start_Click(object sender, EventArgs e)
        {
            InitKS();
            if (massMsg != null && massMsg.RcvList.Count > 0)
                if (!isWorking)
                    kannelThreadSession.Enqueue(massMsg.RcvList.ToArray());
                else
                    kannelThreadSession.StopActivity();
            else
            {
                InfoMsg("Необходимо подготовить список рассылки");
            }
        }
        private void InitKS()
        {
            if (kannelThreadSession == null)
                kannelThreadSession = new KannelThreadSession(this.smsSetts);
            else return;
             kannelThreadSession.answerReceived += new ChangeProgressNotifier(ks_answerReceived);
             kannelThreadSession.statusMessageReceived += new StringTransl(ks_messageReceived);
             kannelThreadSession.progressChanged += new ChangeProgressNotifier(ProgressChangedEventHandler);
             kannelThreadSession.threadSessionStateChanged += new ChangeState(ks_threadSessionStateChanged);                      
        }
        void ks_threadSessionStateChanged(SenderState state)
        {
            switch(state)
            {
                case SenderState.Started:
                    toolStrip2.Invoke(new VoidMethod(SetStartedState));
                    break;
                case SenderState.Stopped:
                    toolStrip2.Invoke(new VoidMethod(SetStoppedState));
                    break;
                default:break;
            }
        }
        void SetStartedState()
        {
            isWorking = true;
            this.toolStripButton4.Image = global::PR2_SMS.Properties.Resources.pause;
            this.toolStripButton4.Text = "Стоп";

        }
        void SetStoppedState()
        {
            isWorking = false;
            this.toolStripButton4.Image = global::PR2_SMS.Properties.Resources.start;
            this.toolStripButton4.Text = "Старт";
        }
        void ks_messageReceived(string str)
        {
            TraceHelper.Report(str);
        }
        void ks_answerReceived(int idx)
        {
            VoidMethod vm = delegate(){
                if (idx < 0 || idx >= massMsg.RcvList.Count)
                    return;
                massMsg.RcvList[idx].UpdateStatus();
                docView.RedrawItems(idx,idx,true);
            };
            docView.Invoke(vm);

        }
        private ListViewItem GetItem(int p)
        {
            massMsg.RcvList[p].Index = p;
            massMsg.RcvList[p].Lvi.Text = (p+1).ToString();
            return massMsg.RcvList[p].Lvi;
        }
        private ListViewItem GetSqlItem(int p)
        {
            try
            {
                return msm.GetLVItem(p);
            }
            catch (System.Exception e)
            {
                InfoMsg(e.Message);
                return null;
            }  
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            SingleMessageForm smf = new SingleMessageForm();
            smf.To = smsSetts.DefaultTo;
            if(DialogResult.OK == smf.ShowDialog(this))
            {
                TargetInfo ti = new TargetInfo();
                ti.message = smsSetts.NeedTransliteration ? Translit.Convert(smf.Msg):smf.Msg;
                ti.phoneNumber = smf.To;
                InitKS();
                kannelThreadSession.EnqueueSingle(ti);
            }
        }

        private void sqlBoxQuery_click(object sender, EventArgs e)
        {
            CreateTab("MySQL", "sqlView", GetSqlItem, msm.GetLVCollection);

            Thread th = null;
            th = new Thread(new ThreadStart(InitSqlBoxConnection));
            th.Start(); 
            
        }

        private void InitSqlBoxConnection()
        {
            try
            {
                if (msm == null)
                {
                    msm = new MySqlKannelManager();
                }
                if (smsSetts.MySqlServerAddress == null ||
                    smsSetts.MySqlUser == null ||
                    smsSetts.SqlBoxDbName == null )
                {
                    InfoMsg("Не заполнены все параметры подключения к серверу MySql");
                    return;
                }
                if (msm.Connect(smsSetts.MySqlServerAddress, smsSetts.MySqlUser, smsSetts.MySqlPass, smsSetts.SqlBoxDbName))
                {
                    TraceHelper.Report(string.Format("Подключение к MySql-серверу {0} прошло успешно", smsSetts.MySqlServerAddress));
                    massMsg.UpdateStatuses(msm, new ChangeProgressNotifier(this.ProgressChangedEventHandler));
                    msm.GetFullReport();
                }
            }
            catch (System.Exception e)
            {
                InfoMsg(e.Message);
            }
            UpdateSqlView();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            SqlBoxXLSWrite sw = new SqlBoxXLSWrite();            
            sw.list = GetTabLv("MySQL","sqlView");
            if (sw.list == null)
                return;
            SaveFileDialog sd = new SaveFileDialog();
            if(sd.ShowDialog()== DialogResult.OK)
            {
                sw.WriteDateToExcel(sd.FileName, msm.LviList, "A1", "H1");
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (massMsg.IsEmpty)
            {
                MessageBox.Show("Не создано ни одной рассылки");
                return;
            }                        
            SaveFileDialog sd = new SaveFileDialog();
            if (sd.ShowDialog() == DialogResult.OK)
            {
                SqlBoxXLSWrite sw = new SqlBoxXLSWrite();
                sw.list = currentView;
                sw.WriteDateToExcel(sd.FileName, msm.LviList, "A1", "H1");
            }

        }

        private void addMsgList_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Title = "Загрузить шаблон рассылки";
            opf.Filter = "Файлы MS Excel|*.xlsx;*.xls|Файлы MS Excel 2007 (*.xlsx)|*.xlsx|Файлы MS Excel 97-03 (*.xls)|*.xls";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                LoadMessageList(opf.FileName);
            }
            Thread th = null;
            th = new Thread(new ThreadStart(ParseExcelDoc));
            th.Start();            
        }
        /// <summary>
        /// 
        /// </summary>
        StringBuilder logSB;
        private void addStringToLog(string num, string field, string msg, string state)
        {
            logSB.AppendLine(string.Format("{0};{1};{2};{3}",num,field,msg,state));
        }

        private void createNewMassMsg_Click(object sender, EventArgs e)
        {            
            OpenFileDialog opf = new OpenFileDialog();
            opf.Title = "Загрузить шаблон рассылки";
            opf.Filter = "Файлы MS Excel|*.xlsx;*.xls|Файлы MS Excel 2007 (*.xlsx)|*.xlsx|Файлы MS Excel 97-03 (*.xls)|*.xls";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                if (!massMsg.IsEmpty)
                {
                    if (DialogResult.Yes == MessageBox.Show("Сохранить изменения?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {                        
                       MassMsgStorage.Save(massmsgFilePath,massMsg);
                    }                                        
                }
                massmsgFilePath = Path.ChangeExtension(opf.FileName, "pr2");
                ResetAll();
                LoadMessageList(opf.FileName);
                
                Thread th = null;
                th = new Thread(new ThreadStart(CreateMassMsg));
                th.Start();   
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Title = "Открыть существующую рассылку";
            opf.Filter = "Файлы рассылки PR2|*.pr2";            
            if (opf.ShowDialog() == DialogResult.OK)
            {
                if (!massMsg.IsEmpty)
                {
                    switch (MessageBox.Show("Сохранить изменения?", "Внимание", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            MassMsgStorage.Save(massmsgFilePath, massMsg);
                            break;
                        case DialogResult.Cancel:
                            return;
                        default:
                            break;
                    }        
                }
                massmsgFilePath = opf.FileName;

                try
                {
                    MassMsgStorage.Load(massmsgFilePath,ref massMsg);
                }
                catch (System.Exception ex)
                {
                	
                }
                finally
                {
                    UpdateDocView();
                    UpdateWinCap();
                }
            }
        }

        private void createReport_Click(object sender, EventArgs e)
        {
            SqlBoxXLSWrite sw = new SqlBoxXLSWrite();
            sw.list = (EnhancedListView)tabControl1.SelectedTab.Controls[0];
            if (sw.list == null)
                return;
            SaveFileDialog sd = new SaveFileDialog();
            if (sd.ShowDialog() == DialogResult.OK)
            {
                //List<ListViewItem> collection = ((GetCollection)sw.list.Tag)();
                sw.WriteDataToExcel(sd.FileName, massMsg.LviList);
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {

        }


  
}
}