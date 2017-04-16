using System.Diagnostics;
using UnityEngine;

public static class Unbug
{
    [Conditional("DEBUG")]
    public static void Log (object message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("DEBUG")]
    public static void LogFormat (string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    [Conditional("DEBUG")]
    public static void LogWarning (object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("DEBUG")]
    public static void LogError (object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public static void SafeDestroy (Object obj)
    {
        if (obj != null)
        {
            Object.Destroy(obj);
        }
    }

    public static TR IfNotNull<T, TR> (this T obj, System.Func<T, TR> func, System.Func<TR> ifNull = null) where T : class
    {
        return obj != null ? func(obj) : (ifNull != null ? ifNull() : default(TR));
    }

    public static TR IfObjectNotNull<T, TR> (T obj, System.Func<T, TR> func, System.Func<TR> ifNull = null)
    {
        return obj != null ? func(obj) : (ifNull != null ? ifNull() : default(TR));
    }
}
