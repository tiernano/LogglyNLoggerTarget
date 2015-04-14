using System.Collections.Generic;
using System.Text;
using NLog.Targets;
using NLog;
using System.Net;
using System.IO;

namespace LogglyTarget
{
    [Target("Loggly")]
    public class Loggly : TargetWithLayout
    {

        List<string> buffer;
        public Loggly()
        {
            this.URL = "NotSet";
            bufferNumber = 1;
            shouldBuffer = false;
            buffer = new List<string>();
        }


        public string URL { get; set; }
        public int bufferNumber { get; set; }
        public bool shouldBuffer { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            //if the target has not been set correctly, dont log...
            if(URL != "NotSet")
            {
                string logMessage = this.Layout.Render(logEvent);
                sendMessageToLoggly(logMessage);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (buffer.Count > 0)
            {
                buffer.ForEach(b => WriteToURL(b));
                buffer.Clear();
            }
            base.Dispose(disposing);
        }



        private bool WriteToURL(string message)
        {
            HttpWebRequest req = null;
            req = (HttpWebRequest)WebRequest.Create(URL);
            req.Method = "POST";
            req.ContentType = "application/x-www-from-urlencoded";
            req.ContentLength = message.Length;
            using (Stream write = req.GetRequestStream())
            {
                UTF8Encoding enc = new UTF8Encoding();
                byte[] data = enc.GetBytes(message);
                write.Write(data, 0, data.Length);
            }
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                else
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(respStream))
                        {
                            string result = reader.ReadToEnd();
                            //todo... do something with this...
                            return true;
                        }
                    }
                }
            }
        }

        private void sendMessageToLoggly(string message)
        {
            //is buffering enabled?
            if (shouldBuffer)
            {
                //write message to buffer
                buffer.Add(message);
                //if buffer is full, send to loggly
                if (buffer.Count == bufferNumber)
                {
                    buffer.ForEach(b => WriteToURL(b));     //todo: something to check that we get a true back from the writer... 
                    //todo: only remove items that have been sent, so older ones still eventually get logged...
                    //clear buffer
                    buffer.Clear();
                }
            }
            else
            {
                WriteToURL(message);
            }
        }
    }
}
