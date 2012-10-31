using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PR2_SMS
{
    enum MsgType { MO, MT, DLR };
    enum SqlBoxColId
    {
        sql_id = 0x0,
        momt,
        sender,
        receiver,
        udhdata,
        msgdata,
        time,
        smsc_id,
        service,
        account,
        id,
        sms_type,
        mclass,
        mwi,
        coding,
        compress,
        validity,
        deferred,
        dlr_mask,
        dlr_url,
        pid,
        alt_dcs

    }
    class SqlDataContainer
    {
        string[] storage = null;
        ListViewItem lvi = null;
        public SqlDataContainer(int cnt)
        {
            storage = new string[cnt];
        }
        public ListViewItem UpdateLvi()
        {
            if (lvi == null)
            {
                    lvi = new ListViewItem();
                    lvi.Text = this[SqlBoxColId.sql_id];
                    MsgType type;
                    if (this[SqlBoxColId.momt] == MsgType.MO.ToString())
                    {
                        lvi.BackColor = Color.FromArgb(160,235,165);
                        type = MsgType.MO;
                    }
                    else if (this[SqlBoxColId.momt] == MsgType.MT.ToString())
                    {
                        lvi.BackColor = Color.Snow;
                        type = MsgType.MT;
                    }
                    else
                    {
                        lvi.BackColor = Color.Yellow;
                        type = MsgType.DLR;
                    }
                    lvi.SubItems.Add(type.ToString());
                    lvi.SubItems.Add(this[SqlBoxColId.sender]);
                    lvi.SubItems.Add(this[SqlBoxColId.receiver]);
                    string message = this[SqlBoxColId.msgdata];
                    lvi.SubItems.Add(message);                    
                    Double dtime = Double.Parse(this[SqlBoxColId.time]);
                    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);                
                    dt = dt.AddSeconds(dtime);
                    dt += DateTime.Now - DateTime.UtcNow;
                    lvi.SubItems.Add(dt.ToString("dd-MMM-yyyy HH:mm:ss"));
                    //
                    if (type != MsgType.DLR)
                    {
                        int bytesPerChar = Int32.Parse(this[SqlBoxColId.coding]) == 2 ? 2 : 1;
                        int symb = 0;
                        if (message != null)
                            symb = this[SqlBoxColId.msgdata].Length * bytesPerChar;
                        lvi.SubItems.Add(symb.ToString());
                        int bits = symb * (bytesPerChar == 2 ? 8 : 7);
                        int msgCnt = (bits / 1120) + ((symb % 1120) == 0 ? 0 : 1);
                        lvi.SubItems.Add(msgCnt.ToString());
                        lvi.SubItems.Add(this[SqlBoxColId.coding]);
                    }
                    else
                    {
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("");
                    }
                    lvi.Tag = this;
                    
            }
           
                return lvi;
        }
        public string this[int i]
        {
            get {
                return storage[i];
            }
            set{storage[i]= value;}
        }
        public string this[SqlBoxColId ID]
        {
            get
            {
                return storage[(int)ID];
            }
            set { storage[(int)ID] = value; }
        }
    }
}
