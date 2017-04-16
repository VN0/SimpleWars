/*****************************************************************************
 * Copyright (C) 2017 Doron Weiss  - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of unity3d license https://unity3d.com/legal/as_terms.
 * 
 * See https://abnormalcreativity.wixsite.com/home for more info
 ******************************************************************************/
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using Dweiss.Common;

namespace Dweiss
{
    [System.Serializable]
    public class ItemsToChange
    {
        [Tooltip("The char that defines starting of the special keys. Example: <LeftControl> <Space>. Can't be used in the input string")]
        public const char SpecialNameStart = '<';
        [Tooltip("The char that defines ending of the special keys. Example: <LeftControl> <Space>. Can't be used in the input string")]
        public const char SpecialNameEnd = '>';
        [Tooltip("The char that defines combination between input (like a+c = you must press on a and c at the same time). Can't be used in the input string")]
        public const char CombineChar = '+';

        //For inspector visualization
        public bool expanded;
        public int displayItemsCount;

        //Data
        public bool _advancedInput;
        public bool AdvancedInput
        {
            get { return _advancedInput; }
            set {
                if (_advancedInput != value)
                {
                    _advancedInput = value;
                    if (_advancedInput == false)
                    {
                        UpdateSequenceFromKeys();
                    } else
                    {
                        keys = new KeyCode[0];
                    }
                }
            }
        }
        public KeyCode[] keys;

        public bool disabled = false;

        public string _sequence;//For saving

        public void UpdateSequenceFromKeys()
        {
            _sequence = "";
            if(keys == null)
            {
                return;
            }
            if (keys.Length != 3)
            {
                for (int i = 0; i < keys.Length; ++i)
                {
                    if (keys[i] != KeyCode.None)
                    {
                        _sequence += (_sequence != "" ? "" + CombineChar + SpecialNameStart : "" + SpecialNameStart) + keys[i] + SpecialNameEnd;
                    }
                }
            }
            else
            {
                if (keys[0] != KeyCode.None) _sequence = "" + SpecialNameStart + keys[0] + SpecialNameEnd;
                if (keys[1] != KeyCode.None) _sequence += (_sequence != "" ? "" + CombineChar + "" + SpecialNameStart : "" + SpecialNameStart) + keys[1] + SpecialNameEnd;
                if (keys[2] != KeyCode.None) _sequence += (_sequence != "" ? "" + SpecialNameStart : "" + SpecialNameStart) + keys[2] + SpecialNameEnd;
            }
            //Debug.Log("Sequence calc " + _sequence + " - " + string.Join(",", keys.Select(a => a.ToString()).ToArray()));

        }
        public string Sequence
        {
            get
            {
                if (string.IsNullOrEmpty(_sequence))
                {
                    UpdateSequenceFromKeys();
                }
                return _sequence;
            }
            set { _sequence = value; }
        }

        private static HashSet<string> _keys;
        public static IEnumerable<string> Keys
        {
            get
            {
                if (_keys == null)
                {
                    var newKeys = new HashSet<string>();// { "backspace", "delete", "tab", "clear", "return", "pause", "escape", "space", "up", "down", "right", "left", "insert", "home", "end", "page up", "page down", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "f10", "f11", "f12", "f13", "f14", "f15", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "!", "\"", "#", "$", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "@", "[", "\\", "]", "^", "_", "`", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "numlock", "caps lock", "scroll lock", "right shift", "left shift", "right ctrl", "left ctrl", "right alt", "left alt" };
                    System.Array enumValues = System.Enum.GetValues(typeof(KeyCode));
                    for (var i = 0; i < enumValues.Length; ++i)
                    {
                        var key = enumValues.GetValue(i).ToString();
                        newKeys.Add(key);
                        if (key == "Joystick1Button2") break;
                    }
                    for (var i = 33; i < 127; ++i)
                    {
                        var key = (Convert.ToChar(i)).ToString();
                        newKeys.Add(key);
                    }
                    _keys = newKeys;

                    //Debug.Log("Key list calc");
                }
                return _keys;
            }
        }

        public GameObject go;

        public string _component;
        public string Component
        {
            get { return _component; }
            set
            {
                _component = value;
            }
        }

        public string _lastComp;
        public System.Type _lastCompType;
        public System.Type ComponentType
        {
            get
            {
                if (go == null || string.IsNullOrEmpty(_component)) return null;
                if(_lastComp != _component || _lastCompType == null)
                {

                    //Debug.Log("Reflection ComponentType");
                    _lastCompType = ReflectionUtils.GetType(_component);
                    _lastComp = _component;
                }
                return _lastCompType;
            }
        }


        public GameObject _lastGo;
        public string[] _allComponents;
        public string[] AllComponents
        {
            get
            {
                if (go == null) return new string[] { };
                else
                {
                    if (_lastGo != go)
                    {
                        //Debug.Log("Reflection AllComps");
                        var ret = go.GetComponents<Component>().Select(c => c.GetType().FullName).ToList();
                        ret.Add(go.GetType().FullName);
                        _allComponents = ret.ToArray();
                        _lastGo = go;
                    }
                    return _allComponents;
                }
            }
        }

        private ReflectionUtils.MemberClass GetVarInfo()
        {
            if (go == null || string.IsNullOrEmpty(_component) || string.IsNullOrEmpty(_variable) ) return null;
            else
            {
                //Debug.Log("Reflection Primitive ");
                var prop = ReflectionUtils.GetPrimitiveMembers(ComponentType).FirstOrDefault(p => p.Name == _variable);

                //ret = ReflectionUtils.GetPrimitiveMembers(ComponentType).Select(p => p.Name).ToArray();
                //var prop = ReflectionUtils.GetPrimitiveMembers(ComponentType).First(p => p.Name == _variable);
                return prop == null ? null : prop;
            }
        }
        private ReflectionUtils.MemberClass _variableType;
        public System.Type VariableType
        {
            get
            {
                if (go == null || string.IsNullOrEmpty(_component)) _variableType = null;
                else if (_variableType == null)
                {
                    //Debug.Log("Reflection VarType");
                    var info = GetVarInfo();
                    _variableType = info == null ? null : info;
                }
                return _variableType == null ? null : _variableType.ParamType;
            }
        }
        public ReflectionUtils.MemberClass VariableClassInfo
        {
            get
            {
                if (go == null || string.IsNullOrEmpty(_component)) _variableType = null;
                else if (_variableType == null)
                {
                    //Debug.Log("Reflection VarType");
                    var info = GetVarInfo();
                    _variableType = info == null ? null : info;
                }
                return _variableType;
            }
        }
        public string _variable;//Publis so unity will save it on change
        public string Variable
        {
            get { return _variable; }
            set
            {
                if (_variable != value)
                {
                    _realMemberInfo = null;
                    _realComp = null;
                    //var old = _variable;
                    _variable = value;
                    //Reset var type
                    _variableType = null;
                    param = null;
                    var temp = VariableType;
                    _input = null;
                }
            }
        }

        public Type _lastVarCalcComponentType;
        public string[] _allVars;
        public string[] AllVariables
        {
            get
            {
                if (go == null || string.IsNullOrEmpty(_component)) return new string[] { };
                else if (_lastVarCalcComponentType != ComponentType || _lastVarCalcComponentType == null)
                {
                    //Debug.Log("Reflection AllVars");
                    _allVars = ReflectionUtils.GetPrimitiveMembers(ComponentType).Select(p => p.Name).ToArray();
                    _lastVarCalcComponentType = ComponentType;
                }

                return _allVars;
            }
        }

        public enum OperationType
        {
            Equal,
            Add,
            Substract,
            Mul,
            Div
        }

        public OperationType operation;
        internal object oldValue;
        public string param;//Allow saving in editor
        private object _input = null;
        public object Input
        {
            get { if (_input == null) _input = ParsedInput; return _input; }
            set
            {
                var old = _input;
                _input = value;
                var vt = VariableType;
                if (vt == null)
                    _input = null;
                else if (_input == null || _input.GetType() != vt)
                {
                    _input = ReflectionUtils.GetDefaultParam(vt);
                }
                if ((_input == null && old != null) ||
                     (old == null && _input != null)
                     || (old != null && _input.Equals(old) == false))
                {
                    param = _input == null ? null : JsonUtility.ToJson(new ForJson(_input));
                }
            }
        }
        private object ParsedInput
        {
            get
            {
                
                var ret = VariableType == null || string.IsNullOrEmpty(param) ? null :
                    (JsonUtility.FromJson<ForJson>(param)).GetData(VariableType);
                return ret;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}->{1}->{2} | T: ({3}) | Value: {4} => {5}{6} {7}]",
                go == null ? "<MISSING>" : go.name, _component, _variable, VariableType, _input, string.IsNullOrEmpty(param) ? "Void => " : "", ParsedInput, operation);
        }
        public string ToString(bool shortStr)
        {
            if (shortStr)
            {
                var index = string.IsNullOrEmpty(_component) ? -1 : _component.LastIndexOf(".") + 1;
                return string.Format("{0}->{1}->{2}",
                    go == null ? "<MISSING>" : go.name, index < 0 ? _component : _component.Substring(index), _variable);
            }
            else
            {
                return string.Format("[{0}->{1}->{2} | T: ({3}) | Value: {4} => {5} => {6} {7}]",
                    go == null ? "<MISSING>" : go.name, _component,
                    _variable, VariableType, _input, string.IsNullOrEmpty(param) ? "Void" : param, ParsedInput, operation);
            }
        }

        public string OperationSign()
        {
            if (VariableType == typeof(Vector3)
                    || VariableType == typeof(float)
                    || VariableType == typeof(int))
            {

                switch (operation)
                {
                    case OperationType.Equal: return "=";
                    case OperationType.Add: return "+";
                    case OperationType.Substract: return "-";
                    case OperationType.Mul: return "*";
                    case OperationType.Div: return "/";
                }
            }
            return "=";
        }
        public string OverrideInfo(bool revert, string desc = null)
        {
            if (desc == null) desc = revert ? "Revert" : "Override";
            return string.Format("[{0}: {1}{2}]", desc, Info(),
                revert ? (oldValue == null ? " => <NULL>" : " => " + oldValue.ToString()) : "");
        }
        public string Info()
        {
            return string.Format("{0}->{1}->{2} => {3} ({4}) {5}",
                go == null ? "<MISSING>" : go.name, _component, _variable, ParsedInput, VariableType, operation);
        }

        public bool UndoOverride(bool info)
        {
            if (disabled || go == null)
            {
                if (info) Debug.LogError("Disabled: " + ToString(true));
                return true;
            }

            if (_realComp == null)
                _realComp = ComponentType == typeof(GameObject) ? go : (object)go.GetComponent(ComponentType);

            if (_realMemberInfo == null) _realMemberInfo = GetVarInfo();

            if (info) Debug.Log(OverrideInfo(true));

            if (_realComp == null || _realMemberInfo == null)
            {
                if (info) Debug.Log("Object/Script doesn't exists to revert: " + ToString(true));
                return true;
            }
            if (_realMemberInfo.MemberT == System.Reflection.MemberTypes.Property)
            {
                ReflectionUtils.SetProperty(_realComp, _variable, oldValue);
                return true;
            }

            else if (_realMemberInfo.MemberT == System.Reflection.MemberTypes.Field)
            {
                ReflectionUtils.SetField(_realComp, _variable, oldValue);
                return true;
            }
            else if (_realMemberInfo.MemberT == System.Reflection.MemberTypes.Method)
            { return true; }//Do nothing 
            else
            {
                Debug.LogError("Override Type not supported " + _realMemberInfo + " by " + this);
            }
            return false;
        }


        private object Operator(object a, object b)
        {
            if (VariableType != null)
            {
                if (VariableType == typeof(int))
                {
                    switch (operation)
                    {
                        case OperationType.Equal: return (int)b;
                        case OperationType.Add: return (int)a + (int)b;
                        case OperationType.Substract: return (int)a - (int)b;
                        case OperationType.Mul: return (int)a * (int)b;
                        case OperationType.Div: return (int)a / (int)b;
                            //Default will throw exception
                    }
                }
                else if (VariableType == typeof(float))
                {
                    switch (operation)
                    {
                        case OperationType.Equal: return (float)b;
                        case OperationType.Add: return (float)a + (float)b;
                        case OperationType.Substract: return (float)a - (float)b;
                        case OperationType.Mul: return (float)a * (float)b;
                        case OperationType.Div: return (float)a / (float)b;
                            //Default will throw exception
                    }
                }
                else if (VariableType == typeof(Vector3))
                {
                    Vector3 v1 = (Vector3)a, v2 = (Vector3)b;
                    switch (operation)
                    {
                        case OperationType.Equal: return v2;
                        case OperationType.Add: return v1 + v2;
                        case OperationType.Substract: return v1 - v2;
                        case OperationType.Mul: return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
                        case OperationType.Div: return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
                            //Default will throw exception
                    }
                }
                else if (VariableType == typeof(string)
                    || VariableType == typeof(bool) || VariableType.IsEnum)
                {
                    return b;
                }
            }
            throw new System.InvalidOperationException("Type of objects are unknown or not supported " + VariableType + " operator " + operation);

        }
        private bool IsValid()
        {
            return go != null && string.IsNullOrEmpty(Component) == false && string.IsNullOrEmpty(Variable) == false;
        }
        private object _realComp = null;
        public ReflectionUtils.MemberClass _realMemberInfo = null;
        public ReflectionUtils.MemberClass MemeberInfo { get { return _realMemberInfo; } }
        public void Override(bool info, string desc = null)
        {
            if (disabled)
            {
                if (info) Debug.LogError("Disabled: " + ToString(true));
                return;
            }

            if(IsValid() == false)
            {
                Debug.LogError(OverrideInfo(false, "Failed - configuration invalid"));
                return;
            }

            if (_realComp == null)
                _realComp = ComponentType == typeof(GameObject) ? go : (object)go.GetComponent(ComponentType);

            if (_realMemberInfo == null) _realMemberInfo = GetVarInfo();
            

            if (_realComp == null || _realMemberInfo == null || _variable == null)
            {
                Debug.LogWarning(OverrideInfo(false, "Failed"));
                return;
            }

            if (_realMemberInfo.MemberT == System.Reflection.MemberTypes.Property)
            {
                var current = ReflectionUtils.GetProperty(_realComp, _variable);
                if (oldValue == null) oldValue = current;

                var res = Operator(current, ParsedInput);
                if (info) Debug.Log(OverrideInfo(false, desc) + " " + current + "," + ParsedInput + " => " + res);
                ReflectionUtils.SetProperty(_realComp, _variable, res);

            }
            else if (_realMemberInfo.MemberT == System.Reflection.MemberTypes.Method)
            {
                var res = ReflectionUtils.SetFunc(_realComp, _variable, ParsedInput);
                if (info) Debug.LogWarning(OverrideInfo(false, desc) + " === " + res);
            }
            else if (_realMemberInfo.MemberT == System.Reflection.MemberTypes.Field)
            {
                var current = ReflectionUtils.GetField(_realComp, _variable);
                if (oldValue == null) oldValue = current;
                var res = Operator(current, ParsedInput);

                if (info) Debug.Log(OverrideInfo(false, desc ) + " " + current + "," + ParsedInput + " => " + res);
                ReflectionUtils.SetField(_realComp, _variable, res);
            }
            else
            {
                Debug.LogError("Override Type not supported " + _realMemberInfo + " by " + this);
            }
        }
    }
}