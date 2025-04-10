using UnityEngine;

public static class Logger
{
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(object message)
    {
        Debug.Log(message);
    }
    
    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogError(object message)
    {
        Debug.LogError(message);
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogIf(bool condition, object message)
    {
        if (condition)
        {
            Debug.Log(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogWarningIf(bool condition, object message)
    {
        if (condition)
        {
            Debug.LogWarning(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogErrorIf(bool condition, object message)
    {
        if (condition)
        {
            Debug.LogError(message);
        }
    }
}
