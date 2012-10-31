using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;

namespace PR2_SMS
{
    
    [Serializable]
    internal class SMSSettings
    {
        static string sPath;

        public static string SPath
        {
            get { return SMSSettings.sPath; }
        }
        static SMSSettings()
        {
            sPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/Settings/";
            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);
        }
        public static void SaveConfig(string p,SMSSettings setts)
        {
            FileStream fs = new FileStream(sPath + p + ".ksf", FileMode.Create);

            // Construct a SoapFormatter and use it 
            // to serialize the data to the stream.
            SoapFormatter formatter = new SoapFormatter();
            try
            {
                setts.ConfigName = p;
                formatter.Serialize(fs, setts);
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
        public static SMSSettings LoadConfig(string cfgName)
        {
            SMSSettings smss = null;
            if (!File.Exists(sPath + cfgName + ".ksf"))
                return null;
            // Open the file containing the data that you want to deserialize.
            FileStream fs = new FileStream(sPath + cfgName + ".ksf", FileMode.Open);
            try
            {
                SoapFormatter formatter = new SoapFormatter();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                smss = (SMSSettings)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                throw;
            }
            finally
            {
                fs.Close();
            }
            return smss;
        }    

        string sName = String.Empty;

        public string ConfigName
        {
            get { return sName; }
            set { sName = value; }
        }
        public string GetKannelRequest(string message,string to)
        {
            String request;

            message = message.Replace("@", "%40");
            message = message.Replace("+", "%2b");
            to = to.Replace("+", "%2b");

            if(dlrMask!=0)
                request = String.Format("http://{0}:{1}/cgi-bin/sendsms?user={2}&pass={3}&from={4}&to={5}&charset={6}&coding={7}&dlr-mask={9}&text={8}", hostName, port, user, password, defaultFrom, to == "" ? defaultTo : to, charset, coding, message, dlrMask);
            else
                request = String.Format("http://{0}:{1}/cgi-bin/sendsms?user={2}&pass={3}&from={4}&to={5}&charset={6}&coding={7}&text={8}", hostName, port, user, password, defaultFrom, to == "" ? defaultTo : to, charset, coding, message);
            return request;
        }

        int coding = 2;

        public int Coding
        {
            get { return coding; }
            set {
                    coding = value;
                    if (value == 0)
                    {
                        charset = "";
                    }
                    else charset = "utf8";
                }
        }
        

        string user;
        public string User
        {
            get { return user; }
            set { user = value; }
        }

        string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        string hostName;

        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }
        int port;

        public int Port
        {
            get { return port; }
            set { port = value; }
        }
        string defaultTo="";
        public string DefaultTo
        {
            get { return defaultTo; }
            set { defaultTo = value; }
        }
        string defaultFrom="any";
        public string DefaultFrom
        {
            get { return defaultFrom; }
            set { defaultFrom = value; }
        }

        string charset;
        public string Charset
        {
            get { return charset; }
            set { charset = value; }
        }
        
        internal bool NeedTransliteration
        {
            get {return coding == 0;}            
        }
        string mySqlServerAddress;

        public string MySqlServerAddress
        {
            get { return mySqlServerAddress; }
            set { mySqlServerAddress = value; }
        }
        string mySqlServerPort;

        public string MySqlServerPort
        {
            get { return mySqlServerPort; }
            set { mySqlServerPort = value; }
        }
        string sqlBoxDbName;

        public string SqlBoxDbName
        {
            get { return sqlBoxDbName; }
            set { sqlBoxDbName = value; }
        }
        string mySqlUser;

        public string MySqlUser
        {
            get { return mySqlUser; }
            set { mySqlUser = value; }
        }
        string mySqlPass;
        public string MySqlPass
        {
            get { return mySqlPass; }
            set { mySqlPass = value; }
        }
        bool requestDlr = false;

        byte dlrMask = 18;

        public byte DlrMask
        {
            get { return dlrMask; }
            set { dlrMask = value; }
        }

        bool checkNumbers = false;

        public bool CheckNumbers
        {
            get { return checkNumbers; }
            set { checkNumbers = value; }
        }

        bool filterData = false;

        public bool FilterData
        {
            get { return filterData; }
            set { filterData = value; }
        }

    }
}
