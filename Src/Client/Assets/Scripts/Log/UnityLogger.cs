using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using log4net;

public static class UnityLogger
{

    public static void Init()
    {
        // Unity 自己开放的消息接收事件，当有日志产生的时候，就会引入到这里来
        Application.logMessageReceived += onLogMessageReceived;
        Common.Log.Init("Unity");
    }

    private static ILog log = LogManager.GetLogger("Unity");

    // 日志消息接收事件

    private static void onLogMessageReceived(string condition, string stackTrace, UnityEngine.LogType type)
    {
        // 对各消息类型进行判断，然后按照格式打印
        switch(type)
        {
            case LogType.Error:
                log.ErrorFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Assert:
                log.DebugFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Exception:
                log.FatalFormat("{0\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Warning:
                log.WarnFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            default:
                log.Info(condition);
                break;
        }
    }
}