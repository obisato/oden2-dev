﻿using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Common.Tools.WebSite
{
    public class EventInfoConverter : IEventInfoConverter
    {
        private readonly string pythonInterpreterPath = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python37_64\python.exe";
        private readonly string pythonScriptPath;

        public EventInfoConverter(string pyscript = @"C:\oden2-dev\Common\Tools\WebSite\GetEventInfoJson.py") {
            pythonScriptPath = pyscript;
        }

        public EventInfo ConvertEventInfo()
        {
            EventInfo convertedEventInfo = new EventInfo { date = "", artist = "", title = ""};
            if (File.Exists(pythonScriptPath))
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo(pythonInterpreterPath)
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        Arguments = pythonScriptPath
                    },
                };

                process.Start();
                var sr = process.StandardOutput;
                var jsonStr = sr.ReadLine().Replace("\'", "\"");

                process.WaitForExit();
                process.Close();

                var settings = new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                };

                var serializer = new DataContractJsonSerializer(typeof(EventInfo), settings);
                using var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr));

                var eventData = serializer.ReadObject(ms);
                if (typeof(EventInfo) == eventData.GetType()) {
                    convertedEventInfo = (EventInfo)eventData;
                }
            }
            return convertedEventInfo;
        }
    }
}
