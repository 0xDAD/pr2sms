using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net.Sockets;
using System.Net;


namespace sqltextbox
{
    class Program
    {
         

        static MySqlConnection conn;
        static void Main(string[] args)
        {
            /*IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("192.168.0.128"), 3306);
            Socket tempSocket =
                new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            tempSocket.Connect(ipe);*/
            if (conn != null)
                conn.Close();
            string server = @"hex-desktop";
            string userid = "root";
            string password = "5145";

            string connStr = "Database=sqlbox_db;Data Source=hex-desktop;User Id=root;Password=5145";

            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();

                GetDatabases();
            }
            catch (MySqlException ex)
            {
                Console.Write("Error connecting to the server: " + ex.Message);
            }
            Console.Read();

        }
       static  bool code = false;
        static void GetDatabases()
        {
            MySqlDataReader reader = null;

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM sent_sms ORDER BY time DESC", conn);
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(14))
                        code = reader.GetString(14) == "0";
                    else code = true;
                    for (int i = 0; i <= reader.FieldCount; i++)
                    {
                        try
                        {                           
                            string s = reader.GetString(i);
                            if (s == null) continue;
                            if (i == 5)
                                Decode(ref s);                            
                            Console.Write("  " + s);
                        }
                        catch
                        { continue; }
                        
                    }
                    Console.WriteLine();    
                }
            }
            catch (MySqlException ex)
            {
                Console.Write("Failed to populate database list: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        private static void Decode(ref string s)
        {
            byte[] arr = new byte[s.Length/2];
            for (int i = 0; i < s.Length; i++)
            {
               arr[i / 2] =  (byte)(arr[i / 2] << 4);
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
            if(!code)
                s = Encoding.BigEndianUnicode.GetString(arr);
            else
                s = Encoding.UTF8.GetString(arr);
            //byte[] ttt = Encoding.BigEndianUnicode.GetBytes("Success");
        }


    }
}
