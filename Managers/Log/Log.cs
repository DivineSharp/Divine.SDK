using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Divine.SDK.Helpers;

using Newtonsoft.Json.Linq;

namespace Divine.SDK.Managers.Log
{
    public sealed class Log
    {
        private static readonly Dictionary<int, int> Cache = new Dictionary<int, int>();

        private static readonly Dictionary<LogLevel, ConsoleColor> Colors = new Dictionary<LogLevel, ConsoleColor>
        {
            { LogLevel.Debug, ConsoleColor.Gray },
            { LogLevel.Info, ConsoleColor.White },
            { LogLevel.Warn, ConsoleColor.Yellow },
            { LogLevel.Error, ConsoleColor.Red },
            { LogLevel.Fatal, ConsoleColor.DarkRed }
        };

        /*private readonly SentryClient SentryClient;

        public Log(SentryClient sentryClient)
        {
            SentryClient = sentryClient;
            ((RavenClient)SentryClient.Client).ErrorOnCapture = (_) => { };
        }*/

        private readonly HttpClient HttpClient;

        public Log()
        {
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri("http://84.38.183.219:5003")
            };

            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }

        public void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        public void Warn(Exception e)
        {
            Warn(e.ToString());
            SentryCaptureException(e);
        }

        public void Warn(string message)
        {
            Write(LogLevel.Warn, message);
        }

        public void Error(Exception e)
        {
            Error(e.ToString());
            SentryCaptureException(e);
        }

        public void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        public void Fatal(Exception e)
        {
            Fatal(e.ToString());
            SentryCaptureException(e);
        }

        public void Fatal(string message)
        {
            Write(LogLevel.Fatal, message);
        }

        public void SentryError(Exception e)
        {
            SentryCaptureException(e);
        }

        public void SentryMessage(string message)
        {
            SentryCaptureMessage(message);
        }

        private void Write(LogLevel logLevel, string message)
        {
            Console.ForegroundColor = Colors[logLevel];
            Console.WriteLine(string.Format("{0:HH:mm:ss} | {1,-5} | {2}", DateTime.Now, logLevel.ToString().ToUpper(), message));
            Console.ResetColor();
        }

        private void SentryCaptureException(Exception e)
        {
            var key = e.ToString().GetHashCode();
            if (Cache.TryGetValue(key, out var count) && count >= 10)
            {
                return;
            }

            Cache[key] = count + 1;

            /*Task.Run(() =>
            {
                BeginInvoke(() =>
                {
                    SentryClient.CaptureAsync(e);
                });
            });*/

            var callingAssembly = Assembly.GetCallingAssembly();

            Task.Run(async () =>
            {
                var jObject = new JObject
                {
                   { "message", e.ToString() },
                   { "user", Environment.UserName },
                   { "assembly", callingAssembly.GetName().Name },
                   { "level", 4 },
                   { "data", JObject.FromObject(e.Data) }
                };

                var stringContent = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                await HttpClient.PostAsync("Log", stringContent);
            });
        }

        private void SentryCaptureMessage(string message)
        {
            var key = message.GetHashCode();
            if (Cache.TryGetValue(key, out var count) && count >= 10)
            {
                return;
            }

            Cache[key] = count + 1;

            /*Task.Run(() =>
            {
                BeginInvoke(() =>
                {
                    SentryClient.CaptureAsync(new SentryEvent(new SentryMessage(message)));
                });
            });*/

            var callingAssembly = Assembly.GetCallingAssembly();

            Task.Run(async () =>
            {
                var jObject = new JObject
                {
                   { "message", message },
                   { "user", Environment.UserName },
                   { "assembly", callingAssembly.GetName().Name },
                   { "level", 6 },
                   { "data", new JObject() },
                };

                var stringContent = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                await HttpClient.PostAsync("Log", stringContent);
            });
        }

        /*private static readonly float[] LastTimes = new float[20];

        private static async void BeginInvoke(Action action)
        {
            var time = Timer.Time;
            var maxTimer = -1f;

            for (var i = 0; i < LastTimes.Length; i++)
            {
                var lastTime = LastTimes[i];
                var timer = time - lastTime;

                if (lastTime == 0f || timer > 60f)
                {
                    LastTimes[i] = time;
                    action.Invoke();
                    return;
                }

                if (timer > maxTimer)
                {
                    maxTimer = timer;
                }
            }

            await Task.Delay((int)((61f - maxTimer) * 1000f));
            BeginInvoke(action);
        }*/
    }
}