using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace HardShellStudios.InputManager
{

    ///<summary>
    ///Core class that handles everything input related. 
    ///</summary>
    [AddComponentMenu("Hard Shell Studios/Input Manager/Input Manager")]
    [System.Serializable]
    public class hardManager : MonoBehaviour
    {

        [SerializeField]
        public givenInputs[] inputs = new givenInputs[] { };

        Dictionary<string, hardKey> keyMaps = new Dictionary<string, hardKey>();
        string currentRebind = "";
        bool replaceSecond = false;
        hardInputUI currentBindFrom;
        public bool saveControllerType = false;

        public bool useController = false;
        public int controllerType = 0;

        public static hardManager singleton;

        float lastX;
        float lastY;


        // Start the class
        void Awake()
        {
            singleton = this;
        }

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

                hardKey addInput = new hardKey(inputs[i].keyName,
                                                    translateController(inputs[i].primaryKeycode, inputs[i].controllerOne, axisCode),
                                                    translateController(inputs[i].secondaryKeycode, inputs[i].controllerTwo, axisCode2),
                                                    getAxisFromController(inputs[i].controllerOne, axisCode),
                                                    getAxisFromController(inputs[i].controllerTwo, axisCode2),
                                                    inputs[i].saveKey);

                keyMaps.Add(addInput.keyName, addInput);
                //print("1 " + addInput.keyInput + " / " + addInput.keyName);
                //print("2 " + addInput.keyInput + " / " + addInput.keyName);
            }

            loadBindings();
        }

        // Make controller to normall keymaps
        KeyCode translateController(KeyCode keyName, hardKey.controllerMap inputName, int axis)
        {
            if (axis == 5 && useController)
            {

                KeyCode returnCode = getFromController(inputName);
                return returnCode;

            }
            if (axis >= 10 && axis <= 13)
            {
                keyName = KeyCode.None;
                if (!useController)
                    axis = 0;
            }

            return keyName;

        }

        int getAxisFromController(hardKey.controllerMap inputName, int axis)
        {
            if (!useController)
            {
                if (axis >= 10 && axis <= 13)
                    return 0;
                else
                    return axis;
            }
            else
            {
                switch (inputName)
                {
                    default:
                        return axis;
                    case hardKey.controllerMap.DPadUp:
                        return 10;
                    case hardKey.controllerMap.DPadDown:
                        return 11;
                    case hardKey.controllerMap.DPadLeft:
                        return 13;
                    case hardKey.controllerMap.DPadRight:
                        return 12;
                }
            }
        }

        KeyCode getFromController(hardKey.controllerMap inputName)
        {
            switch (inputName)
            {
                default:
                    return KeyCode.None;
                case hardKey.controllerMap.Square:
                    return KeyCode.Joystick1Button0;
                case hardKey.controllerMap.Cross:
                    return KeyCode.Joystick1Button1;
                case hardKey.controllerMap.Circle:
                    return KeyCode.Joystick1Button2;
                case hardKey.controllerMap.Triangle:
                    return KeyCode.Joystick1Button3;
                case hardKey.controllerMap.L1:
                    return KeyCode.Joystick1Button4;
                case hardKey.controllerMap.R1:
                    return KeyCode.Joystick1Button5;
                case hardKey.controllerMap.L2:
                    return KeyCode.Joystick1Button6;
                case hardKey.controllerMap.R2:
                    return KeyCode.Joystick1Button7;
                case hardKey.controllerMap.Share:
                    return KeyCode.Joystick1Button8;
                case hardKey.controllerMap.Options:
                    return KeyCode.Joystick1Button9;
                case hardKey.controllerMap.LeftStick:
                    return KeyCode.Joystick1Button10;
                case hardKey.controllerMap.RightStick:
                    return KeyCode.Joystick1Button11;
                case hardKey.controllerMap.PSHome:
                    return KeyCode.Joystick1Button12;
                case hardKey.controllerMap.Trackpad:
                    return KeyCode.Joystick1Button13;
                case hardKey.controllerMap.DPadUp:
                    return KeyCode.None;
                case hardKey.controllerMap.DPadDown:
                    return KeyCode.None;
                case hardKey.controllerMap.DPadLeft:
                    return KeyCode.None;
                case hardKey.controllerMap.DPadRight:
                    return KeyCode.None;

            }
        }

        [Serializable]
        public struct givenInputs
        {
            public string keyName;
            public KeyCode primaryKeycode;
            public KeyCode secondaryKeycode;
            public int axisType;
            public int axisType2;
            public hardKey.controllerMap controllerOne;
            public hardKey.controllerMap controllerTwo;
            public bool saveKey;
        }

        // Core Functions
        public bool GetKeyDown(string keyName)
        {
            bool isPressed = false;

            // Check primary key
            if (keyMaps[keyName].keyWheelState == 0 || keyMaps[keyName].keyWheelState == 5)
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
            else if (keyMaps[keyName].keyWheelState == 10)
            {
                if (Input.GetAxis("DPADVER") == 1 && Input.GetAxis("DPADVER") != keyMaps[keyName].keyValue)
                    isPressed = true;

                keyMaps[keyName].keyValue = Input.GetAxis("DPADVER");
            }
            else if (keyMaps[keyName].keyWheelState == 11)
            {
                if (Input.GetAxis("DPADVER") == -1 && Input.GetAxis("DPADVER") != keyMaps[keyName].keyValue)
                    isPressed = true;

                keyMaps[keyName].keyValue = Input.GetAxis("DPADVER");
            }
            else if (keyMaps[keyName].keyWheelState == 12)
            {
                if (Input.GetAxis("DPADHOR") == 1 && Input.GetAxis("DPADHOR") != keyMaps[keyName].keyValue)
                    isPressed = true;

                keyMaps[keyName].keyValue = Input.GetAxis("DPADHOR");
            }
            else if (keyMaps[keyName].keyWheelState == 13)
            {
                if (Input.GetAxis("DPADHOR") == -1 && Input.GetAxis("DPADHOR") != keyMaps[keyName].keyValue)
                    isPressed = true;

                keyMaps[keyName].keyValue = Input.GetAxis("DPADHOR");
            }
            // End first

            // Check secondary key
            if (keyMaps[keyName].keyWheelState2 == 0 || keyMaps[keyName].keyWheelState2 == 5)
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
            else if (keyMaps[keyName].keyWheelState2 == 10 && useController)
            {
                if (Input.GetAxis("DPADVER") == 1 && Input.GetAxis("DPADVER") != keyMaps[keyName].keyValue)
                    isPressed = true;

                keyMaps[keyName].keyValue = Input.GetAxis("DPADVER");
            }
            else if (keyMaps[keyName].keyWheelState2 == 11 && useController)
            {
                if (Input.GetAxis("DPADVER") == -1 && Input.GetAxis("DPADVER") != keyMaps[keyName].keyValue)
                    isPressed = true;

                keyMaps[keyName].keyValue = Input.GetAxis("DPADVER");
            }
            else if (keyMaps[keyName].keyWheelState2 == 12 && useController)
            {
                if (Input.GetAxis("DPADHOR") == 1 && Input.GetAxis("DPADHOR") != keyMaps[keyName].keyValue)
                    isPressed = true;

                keyMaps[keyName].keyValue = Input.GetAxis("DPADHOR");
            }
            else if (keyMaps[keyName].keyWheelState2 == 13 && useController)
            {
                if (Input.GetAxis("DPADHOR") == -1 && Input.GetAxis("DPADHOR") != keyMaps[keyName].keyValue)
                    isPressed = true;

                keyMaps[keyName].keyValue = Input.GetAxis("DPADHOR");
            }
            // End all

            return isPressed;

        }

        public bool GetKey(string keyName)
        {
            bool isPressed = false;

            // Check primary key
            if (keyMaps[keyName].keyWheelState == 0 || keyMaps[keyName].keyWheelState == 5)
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
            else if (keyMaps[keyName].keyWheelState == 10 && useController)
            {
                if (Input.GetAxis("DPADVER") == 1)
                    isPressed = true;
            }
            else if (keyMaps[keyName].keyWheelState == 11 && useController)
            {
                if (Input.GetAxis("DPADVER") == -1)
                    isPressed = true;
            }
            else if (keyMaps[keyName].keyWheelState == 12 && useController)
            {
                if (Input.GetAxis("DPADHOR") == -1)
                    isPressed = true;
            }
            else if (keyMaps[keyName].keyWheelState == 13 && useController)
            {
                if (Input.GetAxis("DPADHOR") == 1)
                    isPressed = true;
            }
            // End first

            // Check secondary key
            if (keyMaps[keyName].keyWheelState2 == 0 || keyMaps[keyName].keyWheelState2 == 5)
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
            else if (keyMaps[keyName].keyWheelState2 == 10 && useController)
            {
                if (Input.GetAxis("DPADVER") == 1)
                    isPressed = true;
            }
            else if (keyMaps[keyName].keyWheelState2 == 11 && useController)
            {
                if (Input.GetAxis("DPADVER") == -1)
                    isPressed = true;
            }
            else if (keyMaps[keyName].keyWheelState2 == 12 && useController)
            {
                if (Input.GetAxis("DPADHOR") < 0)
                    isPressed = true;
            }
            else if (keyMaps[keyName].keyWheelState2 == 13 && useController)
            {
                if (Input.GetAxis("DPADHOR") < 0)
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

            return keyMaps[keyName].keyValue;
        }

        public float GetAxis(string keyName, float gravity)
        {

            if (GetKey(keyName))
                keyMaps[keyName].keyValue += gravity * Time.deltaTime;

            if (!GetKey(keyName))
                keyMaps[keyName].keyValue = Mathf.MoveTowards(keyMaps[keyName].keyValue, 0, gravity * Time.deltaTime);

            keyMaps[keyName].keyValue = Mathf.Clamp(keyMaps[keyName].keyValue, -1, 1);

            if (keyMaps[keyName].keyWheelState == 3 || keyMaps[keyName].keyWheelState2 == 3)
            {
                return keyMaps[keyName].keyValue = Input.GetAxis("Mouse X") * gravity;
            }
            else if (keyMaps[keyName].keyWheelState == 4 || keyMaps[keyName].keyWheelState2 == 4)
            {
                return keyMaps[keyName].keyValue = Input.GetAxis("Mouse Y") * gravity;
            }
            else if (keyMaps[keyName].keyWheelState == 6 || keyMaps[keyName].keyWheelState2 == 6 && useController)
            {
                if (useController)
                    keyMaps[keyName].keyValue = Input.GetAxis("STICKLHOR") * gravity;
            }
            else if (keyMaps[keyName].keyWheelState == 7 || keyMaps[keyName].keyWheelState2 == 7 && useController)
            {
                if (useController)
                    keyMaps[keyName].keyValue = Input.GetAxis("STICKLVER") * gravity;
            }
            else if (keyMaps[keyName].keyWheelState == 8 || keyMaps[keyName].keyWheelState2 == 8 && useController)
            {
                if (useController)
                    keyMaps[keyName].keyValue = Input.GetAxis("STICKRHOR") * gravity;
            }
            else if (keyMaps[keyName].keyWheelState == 9 || keyMaps[keyName].keyWheelState2 == 9 && useController)
            {
                if (useController)
                    keyMaps[keyName].keyValue = Input.GetAxis("STICKRVER") * gravity;
            }

            return Mathf.Clamp(keyMaps[keyName].keyValue, -1, 1);
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
                else if (keyWheel == 5)
                {
                    if (controllerType == 0)
                    {
                        if (keyString == "Joystick1Button0")
                            keyString = "Square";
                        else if (keyString == "Joystick1Button1")
                            keyString = "Cross";
                        else if (keyString == "Joystick1Button2")
                            keyString = "Circle";
                        else if (keyString == "Joystick1Button3")
                            keyString = "Triangle";
                        else if (keyString == "Joystick1Button4")
                            keyString = "L1";
                        else if (keyString == "Joystick1Button5")
                            keyString = "R1";
                        else if (keyString == "Joystick1Button6")
                            keyString = "L2";
                        else if (keyString == "Joystick1Button7")
                            keyString = "R2";
                        else if (keyString == "Joystick1Button8")
                            keyString = "Share";
                        else if (keyString == "Joystick1Button9")
                            keyString = "Options";
                        else if (keyString == "Joystick1Button10")
                            keyString = "Left Stick Click";
                        else if (keyString == "Joystick1Button11")
                            keyString = "Right Stick Click";
                        else if (keyString == "Joystick1Button12")
                            keyString = "PS Home";
                        else if (keyString == "JoystickButton13")
                            keyString = "Trackpad";
                    }
                    else if (controllerType == 1)
                    {
                        if (keyString == "Joystick1Button0")
                            keyString = "Square";
                        else if (keyString == "Joystick1Button1")
                            keyString = "Cross";
                        else if (keyString == "Joystick1Button2")
                            keyString = "Circle";
                        else if (keyString == "Joystick1Button3")
                            keyString = "Triangle";
                        else if (keyString == "Joystick1Button4")
                            keyString = "L1";
                        else if (keyString == "Joystick1Button5")
                            keyString = "R1";
                        else if (keyString == "Joystick1Button6")
                            keyString = "L2";
                        else if (keyString == "Joystick1Button7")
                            keyString = "R2";
                        else if (keyString == "Joystick1Button8")
                            keyString = "Select";
                        else if (keyString == "Joystick1Button9")
                            keyString = "Start";
                        else if (keyString == "Joystick1Button10")
                            keyString = "Left Stick Click";
                        else if (keyString == "Joystick1Button11")
                            keyString = "Right Stick Click";
                        else if (keyString == "Joystick1Button12")
                            keyString = "PS Home";
                    }
                    else if (controllerType == 2)
                    {
                        if (keyString == "Joystick1Button0")
                            keyString = "X";
                        else if (keyString == "Joystick1Button1")
                            keyString = "A";
                        else if (keyString == "Joystick1Button2")
                            keyString = "B";
                        else if (keyString == "Joystick1Button3")
                            keyString = "Y";
                        else if (keyString == "Joystick1Button4")
                            keyString = "LB";
                        else if (keyString == "Joystick1Button5")
                            keyString = "RB";
                        else if (keyString == "Joystick1Button6")
                            keyString = "LT";
                        else if (keyString == "Joystick1Button7")
                            keyString = "RT";
                        else if (keyString == "Joystick1Button8")
                            keyString = "View";
                        else if (keyString == "Joystick1Button9")
                            keyString = "Menu";
                        else if (keyString == "Joystick1Button10")
                            keyString = "Left Stick Click";
                        else if (keyString == "Joystick1Button11")
                            keyString = "Right Stick Click";
                        else if (keyString == "Joystick1Button12")
                            keyString = "Xbox Home";
                    }
                    else if (controllerType == 3)
                    {
                        if (keyString == "Joystick1Button0")
                            keyString = "X";
                        else if (keyString == "Joystick1Button1")
                            keyString = "A";
                        else if (keyString == "Joystick1Button2")
                            keyString = "B";
                        else if (keyString == "Joystick1Button3")
                            keyString = "Y";
                        else if (keyString == "Joystick1Button4")
                            keyString = "LB";
                        else if (keyString == "Joystick1Button5")
                            keyString = "RB";
                        else if (keyString == "Joystick1Button6")
                            keyString = "LT";
                        else if (keyString == "Joystick1Button7")
                            keyString = "RT";
                        else if (keyString == "Joystick1Button8")
                            keyString = "Back";
                        else if (keyString == "Joystick1Button9")
                            keyString = "Start";
                        else if (keyString == "Joystick1Button10")
                            keyString = "Left Stick Click";
                        else if (keyString == "Joystick1Button11")
                            keyString = "Right Stick Click";
                        else if (keyString == "Joystick1Button12")
                            keyString = "Xbox Home";
                    }

                }
                else if (keyWheel == 10)
                    keyString = "D-Pad Up";
                else if (keyWheel == 11)
                    keyString = "D-Pad Down";
                else if (keyWheel == 12)
                    keyString = "D-Pad Right";
                else if (keyWheel == 13)
                    keyString = "D-Pad Left";
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
                if (PlayerPrefs.HasKey("settings_bindings_" + e.Current.Value.keyName) && keyMaps[e.Current.Value.keyName].saveKey)
                {
                    string[] parsed = PlayerPrefs.GetString("settings_bindings_" + e.Current.Value.keyName).Split('^');
                    int axis = int.Parse(parsed[1]);

                    if (useController || (!useController && (axis <= 10 && axis >= 13)))
                    {
                        keyMaps[e.Current.Value.keyName].keyInput = (KeyCode)System.Enum.Parse(typeof(KeyCode), parsed[0]);
                        keyMaps[e.Current.Value.keyName].keyWheelState = int.Parse(parsed[1]);
                    }

                    //else
                    //    keyMaps[e.Current.Value.keyName].keyWheelState = 0;

                    //print("LOADED: " + e.Current.Value.keyName + " / " + keyMaps[e.Current.Value.keyName].keyWheelState);

                }
                else
                {
                    //print("Primary Binding either not found or ignored for: " + e.Current.Value.keyName);
                }
            }

            // Load Secondary Keys
            for (var e = keyMaps.GetEnumerator(); e.MoveNext();)
            {
                if (PlayerPrefs.HasKey("settings_bindings_sec_" + e.Current.Value.keyName) && keyMaps[e.Current.Value.keyName].saveKey)
                {
                    string[] parsed2 = PlayerPrefs.GetString("settings_bindings_sec_" + e.Current.Value.keyName).Split('^');
                    int axis = int.Parse(parsed2[1]);

                    if (useController || (!useController && (axis <= 10 && axis >= 13)))
                    {
                        keyMaps[e.Current.Value.keyName].keyInput2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), parsed2[0]);
                        keyMaps[e.Current.Value.keyName].keyWheelState2 = int.Parse(parsed2[1]);
                    }
                    //else
                    //    keyMaps[e.Current.Value.keyName].keyWheelState2 = 0;

                    //print("FETCHED: " + keyMaps[e.Current.Value.keyName].keyName + " STATE: " + int.Parse(parsed2[1]));
                }
                else
                {
                    //print("Secondary Binding either not found or ignored for: " + e.Current.Value.keyName);
                }
            }

            if (PlayerPrefs.HasKey("settings_bindings_controller_type") && saveControllerType)
                controllerType = PlayerPrefs.GetInt("settings_bindings_controller_type");

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

        public void HardStartRebind(string keyNameGET, bool wantSecond, KeyCode inputFrom)
        {
            //print("Rebinding: " + keyNameGET);
            currentBindFrom = null;
            replaceSecond = wantSecond;
            currentRebind = keyNameGET;
            //StartCoroutine(waitForKeyPress());
            int axis = 0;
            if (inputFrom.ToString().Contains("Joystick1Button") && useController)
            {
                axis = 5;

                //print(inputFrom);
                hardRebind(currentRebind, inputFrom, axis);
            }
            else if (!inputFrom.ToString().Contains("Joystick"))
            {
                hardRebind(currentRebind, inputFrom, axis);
            }
        }

        IEnumerator waitForKeyPress()
        {
            yield return new WaitForEndOfFrame();

            if (!useController)
            {
                while (!Input.anyKeyDown && Input.mouseScrollDelta.y == 0)
                {
                    yield return null;
                }
            }
            else
            {
                while (!Input.anyKeyDown && Input.mouseScrollDelta.y == 0 && Input.GetAxis("DPADHOR") == 0 && Input.GetAxis("DPADVER") == 0)
                {
                    yield return null;
                }
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                if (Input.mouseScrollDelta.y > 0)
                    hardRebind(currentRebind, KeyCode.None, 1);
                else
                    hardRebind(currentRebind, KeyCode.None, 2);
            }
            else if (Input.GetAxis("DPADVER") != 0)
            {
                if (Input.GetAxis("DPADVER") > 0)
                    hardRebind(currentRebind, KeyCode.None, 10);
                else
                    hardRebind(currentRebind, KeyCode.None, 11);
            }
            else if (Input.GetAxis("DPADHOR") != 0)
            {
                if (Input.GetAxis("DPADHOR") > 0)
                    hardRebind(currentRebind, KeyCode.None, 12);
                else
                    hardRebind(currentRebind, KeyCode.None, 13);
            }
            else
            {
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kcode))
                    {
                        int axis = 0;
                        if (kcode.ToString().Contains("Joystick1Button") && useController)
                        {
                            axis = 5;

                            //print(kcode);
                            hardRebind(currentRebind, kcode, axis);
                        }
                        else if (!kcode.ToString().Contains("Joystick"))
                        {
                            hardRebind(currentRebind, kcode, axis);
                        }
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



            if(currentBindFrom != null)
                currentBindFrom.beingBound = false;

            SaveBindings();
        }

        public void setControllerType(hardInput.controllerType type)
        {
            switch (type)
            {
                case hardInput.controllerType.PS3:
                    controllerType = 0;
                    PlayerPrefs.SetInt("settings_bindings_controller_type", 0);
                    break;
                case hardInput.controllerType.PS4:
                    controllerType = 1;
                    PlayerPrefs.SetInt("settings_bindings_controller_type", 1);
                    break;
                case hardInput.controllerType.XBOX1:
                    controllerType = 2;
                    PlayerPrefs.SetInt("settings_bindings_controller_type", 2);
                    break;
                case hardInput.controllerType.XBOX360:
                    controllerType = 3;
                    PlayerPrefs.SetInt("settings_bindings_controller_type", 3);
                    break;
            }

            PlayerPrefs.Save();
        }

        public void resetSavedKeys()
        {
            for (int i = 0; i < inputs.Length; i++)
            {

                if (PlayerPrefs.HasKey("settings_bindings_" + inputs[i].keyName))
                    PlayerPrefs.DeleteKey("settings_bindings_" + inputs[i].keyName);

                if (PlayerPrefs.HasKey("settings_bindings_sec_" + inputs[i].keyName))
                    PlayerPrefs.DeleteKey("settings_bindings_sec_" + inputs[i].keyName);

                keyMaps[inputs[i].keyName].keyInput = translateController(inputs[i].primaryKeycode, inputs[i].controllerOne, inputs[i].axisType);
                keyMaps[inputs[i].keyName].keyInput2 = translateController(inputs[i].secondaryKeycode, inputs[i].controllerTwo, inputs[i].axisType2);

                keyMaps[inputs[i].keyName].keyWheelState = getAxisFromController(inputs[i].controllerOne, inputs[i].axisType);
                keyMaps[inputs[i].keyName].keyWheelState2 = getAxisFromController(inputs[i].controllerTwo, inputs[i].axisType2);
            }

            if (PlayerPrefs.HasKey("settings_bindings_controller_type"))
                PlayerPrefs.DeleteKey("settings_bindings_controller_type");


            Debug.Log("All bindings reset to default values. PlayerPrefs have been removed for each key.");
            //loadBindings();
        }

        public void resetKey(string name)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i].keyName.Equals(name))
                {
                    if (PlayerPrefs.HasKey("settings_bindings_" + inputs[i].keyName))
                        PlayerPrefs.DeleteKey("settings_bindings_" + inputs[i].keyName);

                    if (PlayerPrefs.HasKey("settings_bindings_sec_" + inputs[i].keyName))
                        PlayerPrefs.DeleteKey("settings_bindings_sec_" + inputs[i].keyName);

                    keyMaps[inputs[i].keyName].keyInput = translateController(inputs[i].primaryKeycode, inputs[i].controllerOne, inputs[i].axisType);
                    keyMaps[inputs[i].keyName].keyInput2 = translateController(inputs[i].secondaryKeycode, inputs[i].controllerTwo, inputs[i].axisType2);

                    keyMaps[inputs[i].keyName].keyWheelState = getAxisFromController(inputs[i].controllerOne, inputs[i].axisType);
                    keyMaps[inputs[i].keyName].keyWheelState2 = getAxisFromController(inputs[i].controllerTwo, inputs[i].axisType2);
                    break;
                }
            }




            Debug.Log("The binding '" + name + "' has been reset to the fault value");
            //loadBindings();
        }

        public KeyCode GetKeyCode(string keyName, bool wantSecond)
        {
            try
            {
                if (!wantSecond)
                    return keyMaps[keyName].keyInput;
                else
                    return keyMaps[keyName].keyInput2;
            }
            catch
            {
                Debug.LogWarning("Failed to 'GetKeyCode' for key: " + keyName);
                return KeyCode.None;
            }
        }

    }
}