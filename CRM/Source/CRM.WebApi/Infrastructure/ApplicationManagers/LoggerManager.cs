using System;
using System.IO;
using System.Net.Http;
using NLog;
using NLog.Targets;

namespace CRM.WebApi.Infrastructure.ApplicationManagers
{
    public class LoggerManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public LoggerManager()
        {
            var loggerTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            loggerTarget.DeleteOldFileOnStartup = false;
        }
        public void LogInfo(HttpMethod request, Uri uri)
        {
            Logger.Info($"Request: [ {request} ] | URL [ {uri} ]");
        }
        public void LogError(Exception ex, HttpMethod request, Uri uri)
        {
            Logger.Error($"\nRequest: [ {request} ] | URL [ {uri} ]\nErr: [ {ex.Message} ] Inner: [ {ex.InnerException?.Message} ]\n" + new string('-', 120));
        }
        public void LogException(Exception ex)
        {
            Logger.Log(LogLevel.Fatal, ex, $"\nErr: {ex.Message}\nInner: {ex.InnerException?.Message}\n");
        }
        public string ReadLogData()
        {
            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");
            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
            var fileName = fileTarget.FileName.Render(logEventInfo);
            if (!File.Exists(fileName))
                File.Create($"{logEventInfo.TimeStamp}.log");
            var data = File.ReadAllLines(fileName);
            var path = System.Web.HttpContext.Current?.Request.MapPath("~//Templates//logs.html");
            var html = File.ReadAllText(path);
            var res = "";
            foreach (var s in data)
                res += s + "</br>";
            var t = html.Replace("{data}", res).Replace("{filename}", fileName);
            return t;
        }
    }
}