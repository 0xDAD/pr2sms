using System;
using System.Collections.Generic;
using System.Text;
using PR2_SMS.Properties;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace PR2_SMS
{
    [Serializable]
    class MassMsgStorage
    {
       // public event ChangeProgress progressChanged;
        //public event ReportTransl reportAdded;
        
        bool translitEnabled = false;
        public bool TranslitEnabled
        {
            get { return translitEnabled; }
            set {                 
                translitEnabled = value;
                MassSendControl.BytesPerChar = value ? 1 : 2;
            }
        }

        public MassMsgStorage()
        {
            ResetAll();
        }

        List<string> paramList = null;
        public List<string> ParamList
        {
            get { return paramList; }
        }

        List<TargetInfo> rcvList = null;
        internal List<TargetInfo> RcvList
        {
            get { return rcvList; }
        }
        internal List<ListViewItem> LviList
        {
            get {
                List<ListViewItem> list = new List<ListViewItem>(RcvList.Count);
                foreach (TargetInfo ti in rcvList)
                    list.Add(ti.Lvi);
                return list; 
            }
        }
        string massMsgName;
        public string MassMsgName
        {
            get { return massMsgName; }
        }
        string templateStringCS;
        string templateStringOriginal;
        internal int ParseTemplateString(string ttempl)
        {
            int i = 0;
            templateStringOriginal = ttempl;
            templateStringCS = (string)ttempl.Clone();
            while(i<ttempl.Length)
            {
                int startIdx = ttempl.IndexOf('{', i);
                if (startIdx == -1) break;
                int endIdx = ttempl.IndexOf('}', startIdx);
                paramList.Add(ttempl.Substring(startIdx + 1, endIdx - startIdx - 1));
                i = endIdx;
            }
            int cidx = 0;
            foreach(string v in paramList)
            {
                templateStringCS = templateStringCS.Replace('{' + v + '}', '{' + cidx.ToString() + '}');
                cidx++;
            }
            if(translitEnabled)
                templateStringCS = Translit.Convert(templateStringCS);
            return paramList.Count;
        }

        private void ResetAll()
        {
            paramList = null;
            templateStringCS = "";
            rcvList = null;
        }

        internal void AddMessageList(ExcelMessageList curES, ChangeProgressNotifier changeProgressDel)
        {
            int row = Settings.Default.list_row, col = Settings.Default.list_col;
            changeProgressDel(0);
            List<string> header = new List<string>();
            if (paramList.Count != 0)
                header.AddRange(curES.GetRange(row, col, row, col + (int)paramList.Count));
            else
                header.Add(curES.GetSingleValue(row, col));
            int vrow = row+1;                        
            int numIdx = (int)header.IndexOf("num")+1;
            int parIdx;            
            do {
                TargetInfo ti = new TargetInfo();
                if (!MassMsgStorage.CheckNumber(curES.GetSingleValue(vrow, numIdx), ref ti.phoneNumber))
                {
                    ti.preStatus = MessageStatus.IncorrectNumber;
                }
                if (ti.phoneNumber == String.Empty)
                    break;
                foreach(string s in paramList)
                {
                    parIdx = header.IndexOf(s);
                    if (parIdx<0)
                        throw new Exception("There is no necessary variable in the list");
                    string paramVal = curES.GetSingleValue(vrow, parIdx + 1);
                    if (translitEnabled)
                        paramVal = Translit.Convert(paramVal);
                    ti.paramList.Add(paramVal);
                }
                //ti.message = String.Format(templateStringCS, ti.paramList.ToArray());
                ti.message = templateStringCS;
                //if (ti.status == MessageStatus.IncorrectNumber)
                //    reportAdded(ti.phoneNumber, ti.paramList[0], ti.message, ti.status.ToString());
                rcvList.Add(ti);                
                vrow++;
            } while (true);
            changeProgressDel(100);
        }
        internal bool Create(ExcelMessageList initialML, string name)
        {
            if (initialML == null)
                return false;
            
            ResetAll();
            this.massMsgName = name;

            string ttempl = initialML.GetSingleValue(Settings.Default.msg_row, Settings.Default.msg_col);

            if (!(ttempl != null && ttempl != ""))
            {
                throw new InternalException("Невозможно получить доступ к шаблонному тексту. Проверьте координаты текста");                
            }
            rcvList = new List<TargetInfo>();
            paramList = new List<string>();
            ParseTemplateString(ttempl);
            //AddMessageList(initialML);

            return true;
        }

        public static bool CheckNumber(string src, ref string dst)
        {
            if(src == null)
                return false;
            dst = src;
            if(src == String.Empty || src.Length < 11)
                return false;
            int globalCodeIdx = 0, locCodeIdx = 0, numIdx = 0;
            for(int i=0; i < src.Length; i++)
            {
                if (src[i] >= '0' && src[i] <= '9')
                    continue;
                if (src[i] == '%' && i == 0)
                    continue;
                if (src[i] == '+' && i == 0)
                {
                    globalCodeIdx = 1;
                    continue;
                }
                if (src[i] == 'b' && i == 2 && src[0] == '%' && src[1] == '2')
                {
                    globalCodeIdx = 3;
                    continue;
                }
                return false;
            }
            if(src.StartsWith("810"))
                globalCodeIdx = 3;
            if (globalCodeIdx == 0)
                locCodeIdx = 2;
            else
            {
                if (src.Substring(globalCodeIdx, 3) != "375")
                    return false;
                else
                    locCodeIdx = globalCodeIdx + 3;
            }
            string localCode = src.Substring(locCodeIdx, 2);
            numIdx = locCodeIdx + 2;
            if (src.Length != numIdx + 7)
                return false;
            switch (localCode)
            {
                case "25":
                    if (src[numIdx] == '0')
                        return false;
                    break;
                case "29":
                    if (src[numIdx] == '0' || src[numIdx] == '4')
                        return false;
                    break;
                case "33":
                    if (src[numIdx] != '3' && src[numIdx] != '6')
                        return false;
                    break;
                case "44":
                    if (src[numIdx] != '4' && src[numIdx] != '5' && src[numIdx] != '7')
                        return false;
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static void Save(string massmsgFilePath, MassMsgStorage storage)
        {
            FileStream fs = new FileStream(massmsgFilePath, FileMode.Create);

            // Construct a SoapFormatter and use it 
            // to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                storage.Conserve();
                formatter.Serialize(fs, storage);
            }
            catch (SerializationException e)
            {
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public void Conserve()
        {
            foreach (TargetInfo ti in rcvList)
            {
                ti.Clean();
            }            
        }

        public bool IsEmpty {
            get { return rcvList == null; }
        }

        internal static void Load(string massmsgFilePath, ref MassMsgStorage massMsg)
        {
            using (FileStream fs = new FileStream(massmsgFilePath, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    massMsg = (MassMsgStorage)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        //internal void UpdateStatuses(MySqlKannelManager msm, ChangeProgressNotifier changeProgressDel)
        //{
        //    if (msm == null)
        //        return;
        //    if(IsEmpty)
        //        return;
        //    Dictionary<string, TargetInfo> fastStorage = new Dictionary<string,TargetInfo>(rcvList.Count);

        //    foreach(TargetInfo ti in rcvList)
        //    {                
        //        fastStorage.Add(ti.phoneNumber, ti);
        //    }
        //    msm.GetDlrMessages();
        //    int updatedCnt = 0;
        //    for (int i = 0; i < msm.MsgList.Count; i++ )
        //    {
        //        SqlDataContainer sqdc = msm.MsgList[i];
        //        int dlrmask = int.Parse(sqdc[SqlBoxColId.dlr_mask]);
        //        if (fastStorage.ContainsKey(sqdc[SqlBoxColId.receiver]))
        //        {
        //            fastStorage[sqdc[SqlBoxColId.receiver]].dlr = (DLRStatus)dlrmask;
        //            updatedCnt++;
        //        }
        //        if (changeProgressDel != null)
        //            changeProgressDel.Invoke((int)((((double)i / (double)msm.MsgList.Count)) * 100));
            
        //    }

        //}
        internal void UpdateStatuses(MySqlKannelManager msm, ChangeProgressNotifier changeProgressDel)
        {
            if (msm == null)
                return;
            if (IsEmpty)
                return;

            msm.GetDlrMessages();
            Dictionary<string, SqlDataContainer> fastStorage = new Dictionary<string, SqlDataContainer>(msm.MsgList.Count);

            foreach (SqlDataContainer ans in msm.MsgList)
            {                
                fastStorage[ans[SqlBoxColId.receiver]] = ans;
            }
            
            int updatedCnt = 0;

            for (int i = 0; i < rcvList.Count; i++)
            {
                SqlDataContainer sqdc;
                if (fastStorage.TryGetValue(rcvList[i].phoneNumber, out sqdc))
                {
                    int dlrmask = int.Parse(sqdc[SqlBoxColId.dlr_mask]);
                    rcvList[i].dlr = (DLRStatus)dlrmask;
                    updatedCnt++;
                }
                else
                {
                    if (rcvList[i].preStatus == MessageStatus.Accepted || rcvList[i].preStatus == MessageStatus.Sent)
                        rcvList[i].dlr = DLRStatus.DeliveredToPhone;
                }
                if (changeProgressDel != null)
                    changeProgressDel.Invoke((int)((((double)i / rcvList.Count)) * 100));

            }
            changeProgressDel.Invoke(0);
        }
    }
}
