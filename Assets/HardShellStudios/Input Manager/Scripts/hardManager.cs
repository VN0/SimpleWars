using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[System.Serializable]
public class hardManager : MonoBehaviour {

    [SerializeField]
    public givenInputs[] inputs = new givenInputs[] {  };
    Dictionary<string, hardKey> keyMaps = new Dictionary<string, hardKey>();
    string currentRebind = "";
    bool replaceSecond = false;
    hardInputUI currentBindFrom;

    public static hardManager singleton;

    void Awake()
    {
        singleton = this;
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            int axisCode = inputs[i].axisType;
            int axisCode2 = inputs[i].axisType2;

            if (inputs[i].axisType > 0)
            {
                inputs[i].primaryKeycode = KeyCode.None;
            }

            if (inputs[i].axisType2 > 0)
            {
                inputs[i].secondaryKeycode = KeyCode.None;
            }

            hardKey addInput = new hardKey(inputs[i].keyName, inputs[i].primaryKeycode, inputs[i].secondaryKeycode, axisCode, axisCode2);
            keyMaps.Add(addInput.keyName, addInput);
        }

        loadBindings();
    }

    [Serializable]
    public struct givenInputs
    {
        public string keyName;
        public KeyCode primaryKeycode;
        public KeyCode secondaryKeycode;
        public int axisType;
        public int axisType2;
    }

    // Basic Functions
    public bool GetKeyDown(string keyName)
    {
        bool isPressed = false;

        // Check primary key
        if (keyMaps[keyName].keyWheelState == 0)
        {
            if (Input.GetKeyDown(keyMaps[keyName].keyInput))
                isPressed = true;
        }
        else if (keyMaps[keyName].keyWheelState == 1)
        {
            if (Input.mouseScrollDelta.y > 0)
                isPressed = true;
        }
        else if (keyMaps[keyName].keyWheelState == 2)
        {
            if (Input.mouseScrollDelta.y < 0)
                isPressed = true;
        }
        // End first

        // Check secondary key
        if (keyMaps[keyName].keyWheelState2 == 0)
        {
            if (Input.GetKeyDown(keyMaps[keyName].keyInput2))
                isPressed = true;
        }
        else if (keyMaps[keyName].keyWheelState2 == 1)
        {
            if (Input.mouseScrollDelta.y > 0)
                isPressed = true;
        }
        else if (keyMaps[keyName].keyWheelState2 == 2)
        {
            if (Input.mouseScrollDelta.y < 0)
                isPressed = true;
        }

        // End all

        return isPressed;

    }

    public bool GetKey(string keyName)
    {
        bool isPressed = false;

        // Check primary key
        if (keyMaps[keyName].keyWheelState == 0)
        {
            if (Input.GetKey(keyMaps[keyName].keyInput))
                isPressed = true;
        }
        else if (keyMaps[keyName].keyWheelState == 1)
        {
            if (Input.mouseScrollDelta.y > 0)
                isPressed = true;
        }
        else if (keyMaps[keyName].keyWheelState == 2)
        {
            if (Input.mouseScrollDelta.y < 0)
                isPressed = true;
        }
        // End first

        // Check secondary key
        if (keyMaps[keyName].keyWheelState2 == 0)
        {
            if (Input.GetKey(keyMaps[keyName].keyInput2))
                isPressed = true;
        }
        else if (keyMaps[keyName].keyWheelState2 == 1)
        {
            if (Input.mouseScrollDelta.y > 0)
                isPressed = true;
        }
        else if (keyMaps[keyName].keyWheelState2 == 2)
        {
            if (Input.mouseScrollDelta.y < 0)
                isPressed = true;
        }

        // End all

        return isPressed;


    }


    public float GetAxis(string keyName, string keyName2, float gravity)
    {
        
        if (GetKey(keyName))
            keyMaps[keyName].keyValue += gravity * Time.deltaTime;
        if (GetKey(keyName2))
            keyMaps[keyName].keyValue -= gravity * Time.deltaTime;

        if (!GetKey(keyName) && !GetKey(keyName2))
            keyMaps[keyName].keyValue = Mathf.MoveTowards(keyMaps[keyName].keyValue, 0, gravity * Time.deltaTime);

        keyMaps[keyName].keyValue = Mathf.Clamp(keyMaps[keyName].keyValue, -1, 1);
        

        if (keyMaps[keyName].keyWheelState == 3)
        {
            float xMovement = Input.GetAxisRaw("Mouse X") * gravity;
            keyMaps[keyName].keyValue = xMovement;
        }
        else if (keyMaps[keyName].keyWheelState == 4)
        {
            float yMovement = Input.GetAxisRaw("Mouse Y") * gravity;
            keyMaps[keyName].keyValue = yMovement;
        }

        return keyMaps[keyName].keyValue;
    }

    public string GetKeyName(string keyName, bool wantSecond)
    {
        string keyString = "";
        int keyWheel = 0;

        if (!wantSecond)
        {
            keyString = keyMaps[keyName].keyInput.ToString();
            keyWheel = keyMaps[keyName].keyWheelState;
        }
        else
        {
            keyString = keyMaps[keyName].keyInput2.ToString();
            keyWheel = keyMaps[keyName].keyWheelState2;
        }

        if (keyWheel == 0)
        {
            if (keyString.Contains("Alpha"))
                keyString = keyString.Replace("Alpha", "");
            else if (keyString.Contains("Keypad"))
                keyString = keyString.Replace("Keypad", "Keypad ");
            else if (keyString.Contains("Left"))
                keyString = keyString.Replace("Left", "Left ");
            else if (keyString.Contains("Right"))
                keyString = keyString.Replace("Right", "Right ");
            else if (keyString.Contains("Up"))
                keyString = keyString.Replace("Up", "Up ");
            else if (keyString.Contains("Down"))
                keyString = keyString.Replace("Down", "Down ");
            else if (keyString.Contains("Mouse0"))
                keyString = "Left Click";
            else if (keyString.Contains("Mouse1"))
                keyString = "Right Click";
            else if (keyString.Contains("Mouse2"))
                keyString = "Middle Click";
            else if (keyString.Contains("Mouse"))
                keyString = "Mouse " + keyString.Replace("Mouse", "");
        }
        else
        {
            if (keyWheel == 1)
                keyString = "Mouse Wheel Up";
            else if (keyWheel == 2)
                keyString = "Mouse Wheel Down";
            else if (keyWheel == 3)
                keyString = "Mouse X";
            else if (keyWheel == 4)
                keyString = "Mouse Y";
        }

        return keyString;
    }

    // Various mouse operations
    public void MouseLock(bool lockedOrNot)
    {
        if (lockedOrNot)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    public void MouseVisble(bool visibleOrNot)
    {
        Cursor.visible = visibleOrNot;
    }

    // Load and save bindings
    public void loadBindings()
    {
        // Load Primary Keys
        for (var e = keyMaps.GetEnumerator(); e.MoveNext();)
        {
            if (PlayerPrefs.HasKey("settings_bindings_" + e.Current.Value.keyName))
            {
                string[] parsed = PlayerPrefs.GetString("settings_bindings_" + e.Current.Value.keyName).Split('^');
                keyMaps[e.Current.Value.keyName].keyInput = (KeyCode)System.Enum.Parse(typeof(KeyCode), parsed[0]);

                if (parsed.Length >= 2)
                    keyMaps[e.Current.Value.keyName].keyWheelState = int.Parse(parsed[1]);
                else
                    keyMaps[e.Current.Value.keyName].keyWheelState = 0;

            }
            else
            {
                print("Not Found: settings_bindings_" + e.Current.Value.keyName);
            }
        }

        // Load Secondary Keys
        for (var e = keyMaps.GetEnumerator(); e.MoveNext();)
        {
            if (PlayerPrefs.HasKey("settings_bindings_sec_" + e.Current.Value.keyName))
            {
                string[] parsed2 = PlayerPrefs.GetString("settings_bindings_sec_" + e.Current.Value.keyName).Split('^');
                keyMaps[e.Current.Value.keyName].keyInput2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), parsed2[0]);

                if (parsed2.Length >= 2)
                    keyMaps[e.Current.Value.keyName].keyWheelState2 = int.Parse(parsed2[1]);
                else
                    keyMaps[e.Current.Value.keyName].keyWheelState2 = 0;
            }
            else
            {
                print("Not Found: settings_bindings_sec_" + e.Current.Value.keyName);
            }
        }

        SaveBindings();
    }

    public void SaveBindings()
    {
        // Save primary keys
        for (var e = keyMaps.GetEnumerator(); e.MoveNext();)
        {
            PlayerPrefs.SetString("settings_bindings_" + e.Current.Value.keyName, e.Current.Value.keyInput.ToString() + "^" + e.Current.Value.keyWheelState.ToString());
        }

        // Save secondary keys
        for (var e = keyMaps.GetEnumerator(); e.MoveNext();)
        {
            PlayerPrefs.SetString("settings_bindings_sec_" + e.Current.Value.keyName, e.Current.Value.keyInput2.ToString() + "^" + e.Current.Value.keyWheelState2.ToString());
        }

        PlayerPrefs.Save();
    }

    // Rebinding Keys
    public void HardStartRebind(string keyNameGET, bool wantSecond, hardInputUI inputFrom)
    {
        //print("Rebinding: " + keyNameGET);
        currentBindFrom = inputFrom;
        replaceSecond = wantSecond;
        currentRebind = keyNameGET;
        StartCoroutine(waitForKeyPress());
    }

    IEnumerator waitForKeyPress()
    {
        yield return new WaitForEndOfFrame();

        while (!Input.anyKeyDown && Input.mouseScrollDelta.y == 0)
        {
            yield return null;
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            print(Input.mouseScrollDelta.ToString());
            //replaceSecond = false;
            if (Input.mouseScrollDelta.y > 0)
            {
                hardRebind(currentRebind, KeyCode.None, 1);
            }
            else
            {
                hardRebind(currentRebind, KeyCode.None, 2);
            }
        }
        else
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    hardRebind(currentRebind, kcode, 0);
                }
            }
        }
    }

    void hardRebind(string rebindName, KeyCode inputKey, int keyWheelState)
    {
        //print("Rebinding: " + rebindName + " from " + keyMaps[rebindName].keyInput.ToString() + " to " + inputKey.ToString() + " WHEELSTATE: " + keyWheelState);

        if (inputKey == KeyCode.Delete)
        {
            inputKey = KeyCode.None;
           
        }

        if (!replaceSecond)
        {
            keyMaps[rebindName].keyInput = inputKey;
            keyMaps[rebindName].keyWheelState = keyWheelState;
        }
        else
        {
            keyMaps[rebindName].keyInput2 = inputKey;
            keyMaps[rebindName].keyWheelState2 = keyWheelState;
        }
        



        currentBindFrom.beingBound = false;
        SaveBindings();
    }
}
