using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using MyDebug;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PR2_SMS
{

 /*   struct KannelSqlBoxDBEntry
    {
        public int id;
        public MsgType type;
        public string sender;
        public string receiver;
        public string msgdata;
        public DateTime time;
        public string smscId;
        public int dlr_mask;
    }*/
    internal class MySqlKannelManager
    {
        public MySqlKannelManager()
        {
            msgList = new List<SqlDataContainer>();
        }

        List<SqlDataContainer> msgList = null;

        internal List<SqlDataContainer> MsgList
        {
            get { return msgList; }
        }
        internal List<ListViewItem> LviList
        {
            get { 
                List<ListViewItem> list = new List<ListViewItem>();
                foreach (SqlDataContainer s in msgList)
                {
                    list.Add(s.UpdateLvi());
                }
                return list;
            }
        }

        MySqlConnection connection = null;
        DataTable data = null;
        string conn_string;
        public bool Connected
        {
            get {               
                if(connection!=null)
                {
                    if (connection.State == ConnectionState.Open)
                        return true;
                    else 
                    {
                        Connect();
                        return connection.State == ConnectionState.Open;                    
                    }
                }
                else
                return false;
            }
            set{Disconnect();}
        }

        internal bool Connect(string server, string user, string pass, string rootdb)
        {            
            conn_string = String.Format("Data Source={0};User Id={1};Password={2};Database={3}",server,user,pass,rootdb);
            return Connect();
        }

        private bool Connect()
        {
            Disconnect();
            if (conn_string == null || conn_string == String.Empty)
                return false;
            connection = new MySqlConnection(conn_string);
            try
            {
                connection.Open();
            }
            catch (MySqlException sqex)
            {
                TraceHelper.Report(sqex.Message);
                Disconnect();
                return false;
            }
            catch (Exception ex)
            {
                TraceHelper.Report(ex.Message);
                return false;
            }

            return connection != null;
        }

        internal  void Disconnect()
        {
            if (connection != null)
            {                
                connection.Close();
                data = null;
                connection = null;
            }
        }
        internal void GetFullReport()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM sent_sms ORDER BY time DESC", connection);
            FetchCommand(cmd);
        }
        internal void GetDlrMessages()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM sent_sms WHERE sms_type = 3 ORDER BY time DESC", connection);
            FetchCommand(cmd);
        }
        internal void GetInboundMessages()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM sent_sms WHERE sms_type = 0 ORDER BY time DESC", connection);
            FetchCommand(cmd);
        }
        internal void FetchCommand(MySqlCommand cmd)
        {
            if(Connected)
            {
                if (cmd == null)
                    return;                
                MySqlDataReader reader = cmd.ExecuteReader();
                msgList.Clear();
                while (reader.Read())
                {
                    SqlDataContainer list = new SqlDataContainer(reader.FieldCount);
                    Encoding enc = Encoding.UTF8 ;
                    if (!reader.IsDBNull(14))
                        enc = reader.GetString(14) == "0"?Encoding.UTF8:Encoding.BigEndianUnicode;
                    for(int i=0;i<reader.FieldCount;i++)
                    {
                        try
                        {
                            string s = String.Empty;
                            if (!reader.IsDBNull(i))
                            {
                                s = reader.GetString(i);
                                if (s == null) s = string.Empty; 
                                if (i == 5)
                                    Decode(ref s,enc);
                            }
                            list[i] = s;
                        }
                        catch  { continue; }
                    
                    }
                    msgList.Add(list);
                }
                reader.Close();
            }
        }
        private void Decode(ref string s,Encoding enc)
        {
            byte[] arr = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i++)
            {
                arr[i / 2] = (byte)(arr[i / 2] << 4);
                if (s[i] >= '0' && s[i] <= '9')
                {
                    arr[i / 2] += (byte)(s[i] - 0x30);
                    continue;
                }
                if (s[i] >= 'A' && s[i] <= 'F')
                {
                    arr[i / 2] += (byte)(s[i] - 0x37);
                    continue;
                }
            }
            s = enc.GetString(arr);
        }
                internal ListViewItem GetLVItem(int p)
        {
            if(msgList!=null&&p<msgList.Count)
            {
                SqlDataContainer target = msgList[p];
                return target.UpdateLvi();              
                
            }
            return null;
        }

                public List<ListViewItem> GetLVCollection()
                {
                    List<ListViewItem> list = new List<ListViewItem>();
                    foreach (SqlDataContainer s in msgList)
                    {
                        list.Add(s.UpdateLvi());
                    }
                    return list;

                }
    }
}
