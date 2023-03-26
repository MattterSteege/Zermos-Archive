using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Logger : MonoBehaviour
{
    [SerializeField] TMP_Text output;
    [SerializeField, Tooltip("Which logtypes should ouput their whole stacktrace")] private LogTypes _logTypes;

    [Flags]
    enum LogTypes
    {
        Log = 1,
        Warning = 2,
        Error = 4,
        Exception = 8,
        Assert = 16
    }

    void Start() => Application.logMessageReceived += HandleLog;
    

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            if (_logTypes.HasFlag(LogTypes.Log))
            {
                output.text += $"<color=black>{logString}</color>\n";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"-----------------\n";
            }
            else
            {
                output.text += $"<color=black>{logString}</color>\n";
                output.text += $"-----------------\n";
            }
        }
        else if (type == LogType.Warning)
        {
            if (_logTypes.HasFlag(LogTypes.Warning))
            {
                output.text += $"<color=orange>{logString}</color>\n";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"-----------------\n";
            }
            else
            {
                output.text += $"<color=orange>{logString}</color>\n";
                output.text += $"-----------------\n";
            }

        }
        else if (type == LogType.Error)
        {
            if (_logTypes.HasFlag(LogTypes.Error))
            {
                output.text += $"<color=red>{logString}</color>\n";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"-----------------\n";
            }
            else
            {
                output.text += $"<color=red>{logString}</color>\n";
                output.text += $"-----------------\n";
            }
        }
        else if (type == LogType.Exception)
        {
            if (_logTypes.HasFlag(LogTypes.Exception))
            {
                output.text += $"<color=red>{logString}</color>\n";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"-----------------\n";
            }
            else
            {
                output.text += $"<color=red>{logString}</color>\n";
                output.text += $"-----------------\n";
            }
        }
        else if (type == LogType.Assert)
        {
            if (_logTypes.HasFlag(LogTypes.Assert))
            {
                output.text += $"<color=red>{logString}</color>\n";

                string stacktrace = stackTrace;
                var m1 = Regex.Matches(stacktrace, @"((([A-Za-z]+\/)+)?[A-Z-a-z]+?(.[A-Za-z]+:[0-9]+))");

                foreach (Match match in m1)
                {
                    stacktrace = stacktrace.Replace(match.Value, $"<color=blue>{match.Value}</color>");
                }

                output.text += $"{stacktrace}";
                output.text += $"-----------------\n";
            }
            else
            {
                output.text += $"<color=red>{logString}</color>\n";
                output.text += $"-----------------\n";
            }
        }
    }
}