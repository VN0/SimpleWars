using UnityEngine;
using System.Collections;
using System;

public class hardKey : IComparable<hardKey> {

    public string keyName;
    public KeyCode keyInput;
    public KeyCode keyInput2;
    public int keyWheelState;
    public int keyWheelState2;
    public float keyValue;

    public hardKey(string keyNameGIVE, KeyCode inputKeyGIVE, KeyCode inputKey2GIVE, int keyWheelStateGIVE, int keyWheelState2GIVE)
    {
        keyName = keyNameGIVE;
        keyInput = inputKeyGIVE;
        keyInput2 = inputKey2GIVE;
        keyWheelState = keyWheelStateGIVE;
        keyWheelState2 = keyWheelState2GIVE;
        keyValue = 0;
    }

    public int CompareTo(hardKey other)
    {
        return 1;
    }
}
