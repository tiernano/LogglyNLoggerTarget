A Loggly target for NLogger
===========================

your NLog.config should look something like:

&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
&lt;nlog xmlns=&quot;http://www.nlog-project.org/schemas/NLog.xsd&quot;
      xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&gt;
  &lt;extensions&gt;
    &lt;add assembly=&quot;LogglyTarget&quot;/&gt;
  &lt;/extensions&gt;
  &lt;targets&gt;
    &lt;target name=&quot;Loggly&quot; xsi:type=&quot;Loggly&quot; URL=&quot;&lt;EnterYourLogglyURLHere&gt;&quot; bufferNumber=&quot;5&quot; shouldBuffer=&quot;true&quot;/&gt;
  &lt;/targets&gt;

  &lt;rules&gt;
    &lt;logger name=&quot;*&quot; minlevel=&quot;Debug&quot; writeTo=&quot;Loggly&quot; /&gt;
  &lt;/rules&gt;
&lt;/nlog&gt;

* bufferNumber is the number of items to buffer before you send HTTP requests (Currently one per line...)
* shouldBuffer can be true (wait till the bufferNumber hits the specified and then write to Loggly) or False (Always write)

You will need to configure Loggly as follows:

* Create a new Input. Name it what ever, but make sure your input is HTTP and JSON is off...
* set the URL in your NLog.config to the URL you recieve from Logggly. 
* Run some tests...

That should be it. Any questions, shout... somewhere... out the window maybe?