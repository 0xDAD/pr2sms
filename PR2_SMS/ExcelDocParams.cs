using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace PR2_SMS
{
    enum MessageStatus
    {
        ReadyToSend,
        Accepted,
        IncorrectNumber,
        IncorrectParam,
        SendError,
        Sent
    }
    enum DLRStatus
    {
        Unknown = 0,
        DeliveredToPhone = 1,
        NonDeliveredToPhone =  2,
        DeliveredToSMSC = 8,
        NonDeliveredToSMSC = 16
        
    }
    [Serializable]    
    class TargetInfo
    {
        
        private int index=-1;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        public TargetInfo()
        {
            paramList = new List<string>();
        }
        public ListViewItem UpdateLvi()
        {
            if(lvi==null)
                lvi = new ListViewItem();

            lvi.Text = "";
            lvi.SubItems.Add(this.phoneNumber);
            string msg = string.Format(this.message, this.paramList.ToArray());
            lvi.SubItems.Add(msg);            
            int symb = msg.Length * MassSendControl.BytesPerChar;
            lvi.SubItems.Add(symb.ToString());
            int bits = symb * (MassSendControl.BytesPerChar == 2 ? 8 : 7);
            int msgCnt = (bits / 1120) + ((bits % 1120) == 0 ? 0 : 1);
            lvi.SubItems.Add(msgCnt.ToString());
            lvi.SubItems.Add(this.preStatus.ToString());
            lvi.SubItems.Add(this.dlr.ToString());
            lvi.Tag = this;
            return lvi;
        }
        public void UpdateStatus()
        {
            if (lvi == null)
            { UpdateLvi(); return; }
            
            lvi.SubItems[5].Text = this.preStatus.ToString();
        }
        private ListViewItem lvi;

        public ListViewItem Lvi
        {
            get {
                if (lvi == null)
                    UpdateLvi();
                return lvi;
            }
        }
        public string message;
        public string phoneNumber;
        public MessageStatus preStatus;
        public DLRStatus dlr = DLRStatus.Unknown;
        public string answer;
        public List<string> paramList;
        public void Clean() 
        {
            lvi = null;
        }
    }
}
