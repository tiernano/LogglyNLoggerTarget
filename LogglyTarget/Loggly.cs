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
        readonly List<string> _buffer;
        public Loggly()
        {
            Url = "NotSet";
            BufferNumber = 5;
            ShouldBuffer = true;
            _buffer = new List<string>();
        }


        public string Url { get; set; }
        public int BufferNumber { get; set; }
        public bool ShouldBuffer { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            //if the target has not been set correctly, dont log...
            if(Url != "NotSet")
            {
                string logMessage = Layout.Render(logEvent);
                SendMessageToLoggly(logMessage);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (_buffer.Count > 0)
            {
                _buffer.ForEach(b => WriteToUrl(b));
                _buffer.Clear();
            }
            base.Dispose(disposing);
        }



        private bool WriteToUrl(string message)
        {
            var result = false;
            var req = (HttpWebRequest)WebRequest.Create(Url);
            req.Method = "POST";
            req.ContentType = "application/x-www-from-urlencoded";
            req.ContentLength = message.Length;
            using (Stream write = req.GetRequestStream())
            {
                var enc = new UTF8Encoding();
                byte[] data = enc.GetBytes(message);
                write.Write(data, 0, data.Length);
            }
            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (var respStream = resp.GetResponseStream())
                    {
                        if (respStream != null)
                            using (var reader = new StreamReader(respStream))
                            {
                                reader.ReadToEnd();
                                //todo... do something with this...
                                result = true;
                            }
                    }
                }
                return result;
            }
        }

        private void SendMessageToLoggly(string message)
        {
            //is buffering enabled?
            if (ShouldBuffer)
            {
                //write message to buffer
                _buffer.Add(message);
                //if buffer is full, send to loggly
                if (_buffer.Count == BufferNumber)
                {
                    _buffer.ForEach(b => WriteToUrl(b));     //todo: something to check that we get a true back from the writer... 
                    //todo: only remove items that have been sent, so older ones still eventually get logged...
                    //clear buffer
                    _buffer.Clear();
                }
            }
            else
            {
                WriteToUrl(message);
            }
        }
    }
}
