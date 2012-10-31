using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace PR2_SMS
{
    enum KannelReqCode
    {
        SuccessfullyAccepted,
        OperationTimeout,
        UnknownError
    }
    class HttpReq
    {
        public static KannelReqCode status;
        public static WebExceptionStatus wes;
        public static string Send(string request)
        {
            try
            {

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(request);
                
                req.ReadWriteTimeout = 9000;
                req.Timeout = 10000;

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                status = KannelReqCode.SuccessfullyAccepted;
                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                string answer = readStream.ReadLine();
                //MessageBox.Show(response.StatusCode.ToString() + "\n" + readStream.ReadLine(), "SMS Sender", MessageBoxButtons.OK, MessageBoxIcon.Information);

                response.Close();
                readStream.Close();
                return answer;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                    status = KannelReqCode.OperationTimeout;
                else
                    status = KannelReqCode.UnknownError;
                wes = ex.Status;
                return ex.Message;
            }
            catch(Exception ex)
            {
                wes = WebExceptionStatus.SendFailure;
                status = KannelReqCode.UnknownError;
                return ex.Message;
            }

        }
    }
}
