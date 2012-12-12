using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PR2_SMS
{
    enum SenderState {
        Started,
        Breaked,
        Stopped,
        QueueReady,
        Paused
    }
    internal delegate void ChangeState (SenderState state);
    class KannelThreadSession
    {

        internal event ChangeProgressNotifier progressChanged;
        internal event ChangeProgressNotifier answerReceived;
        internal event StringTransl statusMessageReceived;
        internal event ChangeState threadSessionStateChanged;


        public KannelThreadSession(SMSSettings setts)
        {
            this.settings = setts;
        }
        private KannelThreadSession(){}
        SMSSettings settings;
        Queue<TargetInfo> internalQueue = new Queue<TargetInfo>();
        bool canWork = true;
        Thread mainThread;
        internal void EnqueueSingle(TargetInfo ti)
        {
            Enqueue(new TargetInfo[] { ti });
        }
        public bool Enqueue(TargetInfo[] range)
        {
                Monitor.Enter(internalQueue);
                foreach (TargetInfo ti in range)
                {
                    internalQueue.Enqueue(ti);
                }
                Monitor.Exit(internalQueue);
                ThreadStart ts = new ThreadStart(startTh);

                threadSessionStateChanged.Invoke(SenderState.QueueReady);
                if (mainThread==null || mainThread.ThreadState != ThreadState.Running)
                { 
                    mainThread = new Thread(ts); 
                    mainThread.Start(); 
                }
                threadSessionStateChanged.Invoke(SenderState.Started);
                return true;
        }
        private void startTh()
        {
            progressChanged(0);
            int firstCnt = internalQueue.Count;
            int counter = 1000;
            while (counter > 0 && internalQueue.Count > 0 && canWork)
            {
                Monitor.Enter(internalQueue);
                TargetInfo ti = internalQueue.Dequeue();
                Monitor.Exit(internalQueue);
                if (ti.preStatus == MessageStatus.ReadyToSend)
                {
                    string req = settings.GetKannelRequest(string.Format(ti.message,ti.paramList.ToArray()), ti.phoneNumber);
                    ti.answer = HttpReq.Send(req);
                    ti.preStatus = AnalyzeOutput(ti.answer);               

                progressChanged(100 - (int)(((Double)internalQueue.Count / (Double)firstCnt)*100));                
                answerReceived(ti.Index);
                if (ti.preStatus == MessageStatus.SendError)
                {
                    statusMessageReceived(String.Format("Произошла ошибка при отправке сообщения. Отсылка прекращена. WES={0} Stat={1}", HttpReq.wes,HttpReq.status));
                    Monitor.Enter(internalQueue);
                    internalQueue.Clear();
                    Monitor.Exit(internalQueue);
                    threadSessionStateChanged.Invoke(SenderState.Breaked);
                    break;
                }else
                statusMessageReceived(String.Format("Сообщение для номера {0} отправлено. Статус={1}",ti.phoneNumber,ti.preStatus));
                }
            }
            threadSessionStateChanged.Invoke(SenderState.Stopped);
            progressChanged(0);
            counter--;
        }

        private MessageStatus AnalyzeOutput(string p)
        {
            if (HttpReq.status == KannelReqCode.SuccessfullyAccepted && p[0] == '0')
                return MessageStatus.Accepted;
            else
                return MessageStatus.SendError;

        }


        internal void StopActivity()
        {
            canWork = false;
        }

    }
}
