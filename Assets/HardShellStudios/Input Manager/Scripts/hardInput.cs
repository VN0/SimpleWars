using UnityEngine;

public class hardInput : MonoBehaviour
{

    public static bool GetKey(string keyName)
    {
        return hardManager.singleton.GetKey(keyName);
    }

    public static bool GetKeyDown(string keyName)
    {
        return hardManager.singleton.GetKeyDown(keyName);
    }

    public static float GetAxis(string keyName, string keyName2, float gravity)
    {
        return hardManager.singleton.GetAxis(keyName, keyName2, gravity);
    }

    public static string GetKeyName(string keyName, bool useSecond)
    {
        return hardManager.singleton.GetKeyName(keyName, useSecond);
    }

    public static void HardStartRebind(string keyNameGET, bool wantSecond, hardInputUI inputFrom)
    {
        hardManager.singleton.HardStartRebind(keyNameGET, wantSecond, inputFrom);
    }

    public static void MouseLock(bool setMouseLock)
    {
        hardManager.singleton.MouseLock(setMouseLock);
    }

    public static void MouseVisble(bool setMouseVisible)
    {
        hardManager.singleton.MouseVisble(setMouseVisible);
    }
}