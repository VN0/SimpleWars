using UnityEngine;

public class hardInput : MonoBehaviour
{

    public enum controllerType {PS3, PS4, XBOX1, XBOX360};

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

    public static float GetAxis(string keyName, float gravity)
    {
        return hardManager.singleton.GetAxis(keyName, gravity);
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

    public static void SetControllerType(controllerType type)
    {
        hardManager.singleton.setControllerType(type);
    }

    public static controllerType GetControllerType()
    {
        switch (hardManager.singleton.controllerType)
        {
            default:
                return hardInput.controllerType.PS3;
            case 1:
                return hardInput.controllerType.PS4;
            case 2:
                return hardInput.controllerType.XBOX1;
            case 3:
                return hardInput.controllerType.XBOX360;
        }
    }

    public static int GetControllerTypeIndex()
    {
        return hardManager.singleton.controllerType;
    }
}