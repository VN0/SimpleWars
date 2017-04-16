/*****************************************************************************
 * Copyright (C) 2017 Doron Weiss  - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of unity3d license https://unity3d.com/legal/as_terms.
 * 
 * See https://abnormalcreativity.wixsite.com/home for more info
 ******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace Dweiss
{

    public class RunOnKey : MonoBehaviour
    {

        public List<RuntimePlatform> runtimePlatforms = new List<RuntimePlatform>(new[] {
            RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor, RuntimePlatform.WindowsEditor });

        public bool showInfo = true;

        [Tooltip("Max time between each letter input before the input automatically fails")]
        public float maxTimeBetweenLetters = 3f;
       
        public bool fixedUpdateExecute = false;

        [EditorItem(true)]
        [Tooltip("Run on Update()")]
        public ItemsToChange[] itemsToChangeOnUpdate = new ItemsToChange[0];
        private KeySequence[] keysItemsOnUpdate;

        private List<KeySequence> _activateOnFixed = new List<KeySequence>();

        public void TriggerUpdate(int itemIndex)
        {
            itemsToChangeOnUpdate[itemIndex].Override(showInfo, "API");
        }
        

        private void Reset()
        {
            runtimePlatforms = new List<RuntimePlatform>(new[] {
                RuntimePlatform.OSXEditor, RuntimePlatform.LinuxEditor, RuntimePlatform.WindowsEditor });

        }

        public void TestFunction(string info)
        {
            Debug.Log("Call TestFunction with: " + info);
        }
        private void Start()
        {
            Setup();
        }

        public void RefreshItems()
        {
            if (showInfo) Debug.Log(name +" - RefreshItems");
            Setup();
        }

        private KeySequence[] GetListenerList(ItemsToChange[] items)
        {
            var list = new List<KeySequence>();
            foreach (var a in items)
            {
                try
                {
                    var seq = a.Sequence;//Trigger sequence reset 
                    list.Add(new KeySequence(a, ItemsToChange.CombineChar,
                        ItemsToChange.SpecialNameStart, ItemsToChange.SpecialNameEnd, maxTimeBetweenLetters));
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error with " + a + " of Sequence " + a.Sequence + " ::: " + e);
                }
            }
            return list.ToArray();
        }

        private void Setup() {

            _activateOnFixed = new List<KeySequence>();
            keysItemsOnUpdate = GetListenerList(itemsToChangeOnUpdate);
            
        }
        private void Update()
        {
            if( keysItemsOnUpdate.Length != itemsToChangeOnUpdate.Length)
            {
                Setup();
            }

            var activate = Check(keysItemsOnUpdate);
            if (fixedUpdateExecute) _activateOnFixed = activate;
            else Activate(activate);
        }
        private void FixedUpdate()
        {
            Activate(_activateOnFixed);
        }

        private void Activate(List<KeySequence> itemsToChange)
        {
            for (int i = 0; i < itemsToChange.Count; ++i)
            {
                try
                {
                    itemsToChange[i].Activate(showInfo);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error in override " + itemsToChange[i] + ": " + e);
                    if (Application.isEditor == false)
                    {
                        Debug.LogError("Prefabs might have problems in override during play");
                    }

                }
            }
        }
        private static KeyCode[] GetAllAsciiKeyCodes()
        {
            var ret = new List<KeyCode>();
            var arr = System.Enum.GetValues(typeof(KeyCode));
            for(int i=0; i < arr.Length; ++i)
            {
                var k = ((KeyCode)arr.GetValue(i));
                if ((int)k < (int)KeyCode.Numlock)
                    ret.Add(k);
            }
            return ret.ToArray();
        }
        private static KeyCode[] AsciiKeys = GetAllAsciiKeyCodes();
        private bool IsAsciiKeysDown()
        {
            for(int i=0; i < AsciiKeys.Length; ++i)
            {
                if (Input.GetKeyDown(AsciiKeys[i])) return true;
            }
            return false;
        }

        private List<KeySequence> Check(KeySequence[] itemsToChange)
        {
            var ret = new List<KeySequence>();
            var asciiDown = IsAsciiKeysDown();
            for (int i = 0; i < itemsToChange.Length; ++i)
            {
                try
                {
                    if(itemsToChange[i].IsSequanceChanged)
                    {
                        itemsToChange[i] = new KeySequence(itemsToChange[i].Item, ItemsToChange.CombineChar,
                            ItemsToChange.SpecialNameStart, ItemsToChange.SpecialNameEnd, maxTimeBetweenLetters);
                    }
                    if(itemsToChange[i].Update(asciiDown))
                    {
                        ret.Add(itemsToChange[i]);
                    }

                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error in override " + itemsToChange[i] + ": " + e);
                    if (Application.isEditor == false)
                    {
                        Debug.LogError("Prefabs might have problems in override during play");
                    }

                }
            }
            return ret;
        }


        [System.Serializable]
        class KeySequence
        {
            private ItemsToChange _item;
            public ItemsToChange Item { get { return _item; } }
            private List<List<object>> _sequences;
            private string _oldSeq;
            private float _lastEntryTime, _maxTimeBetweenInput;
            private int _seqIndex;


            public bool IsSequanceChanged
            {
                get { return _oldSeq != _item.Sequence; }
            }

            public KeySequence(ItemsToChange item, char combineTag, char nameTagStart, char nameTagEnd, float maxTimeBetweenInput)
            {
                _item = item;
                _oldSeq = item.Sequence;
                _maxTimeBetweenInput = maxTimeBetweenInput;
                ParseKeys(combineTag, nameTagStart, nameTagEnd);
            }
            private void ParseKeys(char combineTag, char nameTagStart, char nameTagEnd)
            {
                _sequences = new List<List<object>>();
                var combinedKeys = new List<object>();
                bool keyAsWord = false, add = false;
                string keyName = "";


                for (int i = 0; i < _item.Sequence.Length; ++i)
                {
                    var letter = _item.Sequence[i];
                    if (letter == nameTagStart)
                    {
                        if (keyAsWord) throw new System.ArgumentException(_item + " >> Invalid input (word started twice) " + _item.Sequence + " at " + i
                          + " = " + letter);

                        //If ADD not active - Add combinedKeys 
                        //else set inSequence = true
                        // +< a<    <...>< 
                        if (add == false)
                        {
                            if (string.IsNullOrEmpty(keyName) == false)//case of  start
                                combinedKeys.Add(GetTypedKey(keyName, keyAsWord));
                            if (combinedKeys.Count > 0) _sequences.Add(combinedKeys);
                            combinedKeys = new List<object>();

                            //_sequences.Add(combinedKeys);
                            //combinedKeys = new List<object>();
                            keyName = "";
                        }
                        keyAsWord = true;
                    }
                    else if (letter == nameTagEnd)
                    {
                        /// +<...> a<...>
                        if (keyAsWord)
                        {
                            //Stop Sequence and adding
                            combinedKeys.Add(GetTypedKey(keyName, keyAsWord));
                            keyName = "";
                            keyAsWord = false;
                            add = false;
                        }
                        else throw new System.ArgumentException(_item + " >> Invalid input (word stoped without starting) " + _item.Sequence + " at " + i
                            + " = " + letter);
                    }
                    else if (letter == combineTag)
                    {
                        if (add) throw new System.ArgumentException(_item + " >> Invalid input (add started twice) " + _item.Sequence + " at " + i
                          + " = " + letter);
                        if (keyAsWord) throw new System.ArgumentException(_item + " >> Invalid input (add started inside word) " + _item.Sequence + " at " + i
                          + " = " + letter);

                        // a+ <...>+
                        //Add keyName to combinedKeys - finishd reading word
                        if (string.IsNullOrEmpty(keyName) == false)//case of  >+
                            combinedKeys.Add(GetTypedKey(keyName, keyAsWord));
                        keyName = "";
                        add = true;
                    }
                    else
                    {
                        //     ab              a+b         <aaab         <abcd>a
                        // [,..,"a"]["b  [,..,"a","b    [..,"aaab    [,..,"abcd"]["a
                        if (keyAsWord == false && add == false)
                        {
                            if (string.IsNullOrEmpty(keyName) == false)//case of  >+a
                                combinedKeys.Add(GetTypedKey(keyName, keyAsWord));
                            if (combinedKeys.Count > 0) _sequences.Add(combinedKeys);
                            combinedKeys = new List<object>();
                            keyName = letter.ToString();
                        }
                        else if (add)
                        {
                            if (string.IsNullOrEmpty(keyName) == false)//case of  >+a
                                combinedKeys.Add(GetTypedKey(keyName, keyAsWord));
                            keyName = letter.ToString();
                            add = false;
                        }
                        else if (keyAsWord)
                        {
                            keyName += letter.ToString();
                        }
                        else if (keyAsWord && add)
                        {
                            throw new System.ArgumentException(
                                _item + " >> Invalid input (KeyCode contains add) " + _item.Sequence + " at " + i
                          + " = " + letter);
                        }
                    }

                }
                if (keyAsWord) throw new System.ArgumentException(_item + " >> Invalid input (word never ended) " +
                    _item.Sequence + " at " + _item.Sequence.Length);
                else if (add) throw new System.ArgumentException(_item + " >> Invalid input (ended with add sign) " +
                    _item.Sequence + " at " + _item.Sequence.Length);
                else if (string.IsNullOrEmpty(keyName)  == false) {//empty string
                    combinedKeys.Add(GetTypedKey(keyName, keyAsWord));
                    _sequences.Add(combinedKeys);
                    
                } else if (combinedKeys.Count > 0) {//empty string
                    _sequences.Add(combinedKeys);
                }

            }

            private string SequencesToString()
            {
                var ret = _sequences.Count + ". ";
                for (int i = 0; i < _sequences.Count; ++i)
                {
                    ret += " (" + _sequences[i].Count  + "):" + string.Join(" , ", _sequences[i].Select(a => a.ToString()).ToArray()) + "\n";
                }
                return ret;
            }
            
            private object GetTypedKey(string name, bool keyCode)
            {
                if(keyCode)
                {
                    try
                    {
                        KeyCode attEnum = (KeyCode)System.Enum.Parse(typeof(KeyCode), name);
                        return attEnum;
                    }catch(System.Exception e)
                    {
                        var info = _item + " " +  _item.Sequence + " >> Input isn't KeyCode " + name;
                        Debug.LogError(info);
                        throw new System.ArgumentException(info,e);
                    }
                }
                else { return name; }
            }
            
            public bool Update(bool asciiKeysDown)
            {
                if (_item.disabled) return false;

                if (Time.realtimeSinceStartup - _lastEntryTime > _maxTimeBetweenInput) _seqIndex = 0;

                if (_sequences.Count != 0)
                {
                    var keys = _sequences[_seqIndex];
                    var key = keys[keys.Count - 1];
                    int hitKeys = (key.GetType() != typeof(string) ? Input.GetKeyDown((KeyCode)key) : Input.GetKeyDown((string)key)) ? 1 : 0;
                    if (hitKeys == 1)
                    {
                        for (int i = 0; i < _sequences[_seqIndex].Count - 1; ++i)
                        {
                            key = keys[i];
                            var pressed = key.GetType() != typeof(string) ?
                                Input.GetKey((KeyCode)key) :
                                Input.GetKey((string)key);

                            if (pressed) hitKeys++;
                            else break;
                        }
                    }else if (string.IsNullOrEmpty(Input.inputString) == false && asciiKeysDown)
                    {
                        _seqIndex = 0;
                    }
                    if (hitKeys == keys.Count)
                    {
                        _seqIndex++;
                        _lastEntryTime = Time.realtimeSinceStartup;
                    }

                }
                if (_seqIndex == _sequences.Count) {
                     _seqIndex = 0;
                    return true;
                    
                } return false;

            }
            public void Activate(bool showInfo)
            {
                _item.Override(showInfo, "Event -" + _item.Sequence + "-");
            }
        }
    }
}