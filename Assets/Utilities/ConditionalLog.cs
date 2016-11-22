using System.Diagnostics;

public static class CDebug
{
    [Conditional("DEBUG")]
    public static void Log (string message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("DEBUG")]
    public static void LogFormat (string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    [Conditional("DEBUG")]
    public static void LogWarning (string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("DEBUG")]
    public static void LogError (string message)
    {
        UnityEngine.Debug.LogError(message);
    }
}
