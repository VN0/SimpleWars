using UnityEngine;
using HardShellStudios.InputManager;

///<summary>
///Main class containing all functions that can be used at runtime within your game.
///</summary>
public class hardInput : MonoBehaviour
{
    ///<summary>
    ///These are different layouts that allow you to change the way controller inputs are showen.
    ///</summary>
    public enum controllerType { PS3, PS4, XBOX1, XBOX360 };

    ///<summary>
    ///Returns true if the key is being held down.
    ///</summary>
    public static bool GetKey(string keyName)
    {
        return hardManager.singleton.GetKey(keyName);
    }

    ///<summary>
    ///Returns true if the key has been pressed once, but not held down.
    ///</summary>
    public static bool GetKeyDown(string keyName)
    {
        return hardManager.singleton.GetKeyDown(keyName);
    }

    ///<summary>
    ///Returns a float value between two keys (a positive and a negative) when held down, between -1 and 1.
    ///</summary>
    public static float GetAxis(string keyName, string keyName2, float gravity)
    {
        return hardManager.singleton.GetAxis(keyName, keyName2, gravity);
    }

    ///<summary>
    ///Returns a float value for a core axis chosen. Use this for things which are "Mouse Axis", or "Controller Axis" in the InputController prefab.
    ///</summary>
    public static float GetAxis(string keyName, float gravity)
    {
        return hardManager.singleton.GetAxis(keyName, gravity);
    }

    ///<summary>
    ///Returns a string of what button is currently bound to the input specified. You can also request the secondary input by using the bool set to true.
    ///</summary>
    public static string GetKeyName(string keyName, bool useSecond)
    {
        return hardManager.singleton.GetKeyName(keyName, useSecond);
    }

    ///<summary>
    ///DONT USE! Use "hardInput.ForceRebind()" (This is used by the "hardInputUI.cs" component that you can drag onto buttons within the editor)
    ///</summary>
    public static void HardStartRebind(string keyNameGET, bool wantSecond, hardInputUI inputFrom)
    {
        hardManager.singleton.HardStartRebind(keyNameGET, wantSecond, inputFrom);
    }

    ///<summary>
    ///Allows you to forcefully rebind a key. Good for things like preset's etc.
    ///</summary>
    public static void ForceRebind(string keyName, bool useSecondaryKey, KeyCode keyCode)
    {
        hardManager.singleton.HardStartRebind(keyName, useSecondaryKey, keyCode);
    }

    ///<summary>
    ///Can be used to lock the mouse to the middle of the screen, or allow free movement. True = Locked, False = Unlocked.
    ///</summary>
    public static void MouseLock(bool setMouseLock)
    {
        hardManager.singleton.MouseLock(setMouseLock);
    }

    ///<summary>
    ///Can be used to set the visibility of the cursor. True = Visible, False = Hidden.
    ///</summary>
    public static void MouseVisible(bool setMouseVisible)
    {
        hardManager.singleton.MouseVisble(setMouseVisible);
    }

    ///<summary>
    ///Used to set the controller layout show to the user. This will also change "GetKeyName" to show the layout chosen from a controller.
    ///</summary>
    public static void SetControllerType(controllerType type)
    {
        hardManager.singleton.setControllerType(type);
    }

    ///<summary>
    ///Returns the controller layout currently being used.
    ///</summary>
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

    ///<summary>
    ///Returns the index of the controller layout being used. PS3 = 0, PS4 = 1, XBOX1 = 2, XBOX360 = 3.
    ///</summary>
    public static int GetControllerTypeIndex()
    {
        return hardManager.singleton.controllerType;
    }

    ///<summary>
    ///Returns KeyCode for the specified key.
    ///</summary>
    public static KeyCode GetKeyCode(string keyName, bool useSecondaryKey)
    {
        return hardManager.singleton.GetKeyCode(keyName, useSecondaryKey);
    }

    ///<summary>
    ///Can be used to reset all saved bindings to their default values set by the InputController prefab.
    ///</summary>
    public static void ResetAllBindings()
    {
        hardManager.singleton.resetSavedKeys();
    }

    ///<summary>
    ///Can be used to reset a specific value to its default, set by the InputController prefab.
    ///</summary>
    public static void ResetBinding(string bindingName)
    {
        hardManager.singleton.resetKey(bindingName);
    }
}