using UnityEngine;
using System.Collections;
using System;


namespace HardShellStudios.InputManager
{
    ///<summary>
    ///Custom IComparable that stores key information like name, keycode etc.
    ///</summary>
    public class hardKey : IComparable<hardKey>
    {

        public enum controllerMap { None, Square, Cross, Circle, Triangle, L1, R1, L2, R2, Share, Options, LeftStick, RightStick, PSHome, Trackpad, DPadUp, DPadDown, DPadLeft, DPadRight };

        public string keyName;
        public KeyCode keyInput;
        public KeyCode keyInput2;
        public int keyWheelState;
        public int keyWheelState2;
        public float keyValue;

        public bool saveKey;

        public hardKey(string keyNameGIVE, KeyCode inputKeyGIVE, KeyCode inputKey2GIVE, int keyWheelStateGIVE, int keyWheelState2GIVE, bool saveKeyGIVE)
        {
            keyName = keyNameGIVE;
            keyInput = inputKeyGIVE;
            keyInput2 = inputKey2GIVE;
            keyWheelState = keyWheelStateGIVE;
            keyWheelState2 = keyWheelState2GIVE;
            keyValue = 0;
            saveKey = saveKeyGIVE;
        }

        public int CompareTo(hardKey other)
        {
            return 1;
        }
    }
}