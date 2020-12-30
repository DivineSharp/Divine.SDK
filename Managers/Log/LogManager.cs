using System;
using System.Collections.Generic;
using System.Reflection;

namespace Divine.SDK.Managers.Log
{
    public static class LogManager
    {
        private static readonly Dictionary<int, Log> Loggers = new Dictionary<int, Log>();

        private static readonly Log Log = GetAssemblyLogger(Assembly.GetExecutingAssembly());

        public static Log GetCurrentClassLogger()
        {
            return GetAssemblyLogger(Assembly.GetCallingAssembly());
        }

        public static Log GetAssemblyLogger(Assembly assembly)
        {
            var id = assembly.GetHashCode();
            if (!Loggers.TryGetValue(id, out var logger))
            {
                /*var metadata = assembly.GetMetadata();
                var name = assembly.GetName();

                var sentryClient = new SentryClient(metadata.SentryProject, name.Name, name.Version.ToString(), metadata.Channel);
                sentryClient.Tags["Id"] = () => metadata.Id;
                sentryClient.Tags["Channel"] = () => metadata.Channel;
                sentryClient.Tags["Version"] = () => metadata.Version;
                sentryClient.Tags["Build"] = () => metadata.Build;
                sentryClient.Tags["Commit"] = () => metadata.Commit;

                logger = new Log(sentryClient);
                Loggers[id] = logger;*/
            }

            logger = new Log();

            return logger;
        }

        public static void Debug(string message)
        {
            Log.Debug(message);
        }

        public static void Info(string message)
        {
            Log.Info(message);
        }

        public static void Warn(Exception e)
        {
            Log.Warn(e);
        }

        public static void Warn(string message)
        {
            Log.Warn(message);
        }

        public static void Error(Exception e)
        {
            Log.Error(e);
        }

        public static void Error(string message)
        {
            Log.Error(message);
        }

        public static void Fatal(Exception e)
        {
            Log.Fatal(e);
        }

        public static void Fatal(string message)
        {
            Log.Fatal(message);
        }

        public static void SentryError(Exception e)
        {
            Log.SentryError(e);
        }

        public static void SentryMessage(string message)
        {
            Log.SentryMessage(message);
        }
    }
}