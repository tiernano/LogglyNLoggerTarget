using System;
using System.Collections.Generic;
using NLog.Targets;
using NLog;
using System.Net.Http;
using Flurl.Http;
using Newtonsoft.Json;

namespace LogglyTarget
{
	[Target("Loggly")]
	public class Loggly : TargetWithLayout
	{
		readonly List<string> mBuffer;

		public string Url { get; set; }
		public int BufferNumber { get; set; }
		public bool ShouldBuffer { get; set; }

		public Loggly()
		{
			Url = "NotSet";
			BufferNumber = 1;
			ShouldBuffer = false;
			mBuffer = new List<string>();
		}

		protected override void Write(LogEventInfo logEvent)
		{
			//if the target has not been set correctly, don't log...
			if (Url != "NotSet")
			{
				var logMessage = Layout.Render(logEvent);
				SendMessageToLoggly(logMessage);
			}
		}
		protected override void Dispose(bool disposing)
		{
			if (mBuffer.Count > 0)
			{
				mBuffer.ForEach(b => WriteToUrl(b));
				mBuffer.Clear();
			}
			base.Dispose(disposing);
		}


		private bool WriteToUrl(string message)
		{
			var result = Url.PostAsync(new StringContent(message)).ReceiveJson<LogglyResult>().GetAwaiter().GetResult();

			return result.Response == "ok";
		}

		

		private void SendMessageToLoggly(string message)
		{
			//is buffering enabled?
			if (ShouldBuffer)
			{
				//write message to buffer
				mBuffer.Add(message);
				//if buffer is full, send to loggly
				if (mBuffer.Count >= BufferNumber)
				{
					var items = string.Join(Environment.NewLine, mBuffer);

					if (WriteToUrl(items))
					{
						mBuffer.Clear();
					}
				}
			}
			else
			{
				WriteToUrl(message);
			}
		}
	}


	public class LogglyResult
	{
		[JsonProperty(PropertyName = "Response" )]
		public string Response { get; set; }
	}

}
