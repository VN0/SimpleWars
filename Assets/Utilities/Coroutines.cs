using UnityEngine;
using System.Collections;

public class Coroutines
{
    public static IEnumerator ExecuteAfter (float seconds, System.Action func)
    {
        yield return new WaitForSecondsRealtime(seconds);
        func();
    }
}
