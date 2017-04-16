/*****************************************************************************
 * Copyright (C) 2017 Doron Weiss  - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of unity3d license https://unity3d.com/legal/as_terms.
 * 
 * See https://abnormalcreativity.wixsite.com/home for more info
 ******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

namespace Dweiss
{

    [CustomPropertyDrawer(typeof(EditorItemAttribute))]
    public class EditorItemDrawer : PropertyDrawer
    {

        private static string _keysInfo;
        private static string KeysInfo
        {
            get {
                if (_keysInfo == null) _keysInfo = "Input.KeyCode and Free text without the signs '+', '<', '>' (Use keycodes <Plus> <Greater> <Less> to attach them - some won't work inside the editor). Wrap Keycode with <> wrap to use keycode. Use + to require simultanious holding. Example: \"<Space>+1\" (without the perentasis). Found more aboud KeyCode here: from https://docs.unity3d.com/ScriptReference/KeyCode.html \n\nWarning!! Some Control and Alt combination keys don't work in editor because unity catch them!! As well as some F# (F3,F4) and some other signs\n\n" + string.Join(",", ItemsToChange.Keys.ToArray());
                return _keysInfo; }
        }

        private static string[] _names = new string[] { "go", "component", "variable", "param" };

        private int[] _slctIndex = new int[] { -1, -1, -1, -1 };

        private int ShowProperty(string current, string[] allNames, int index, Rect pos)
        {
            //Debug.Log("current " + allNames.Length + " : " + index + "/" + _slctIndex.Length + " ?? " + _slctIndex[index]);
            if (current != null ||
                allNames.Length < _slctIndex[index] ||
                allNames.Length > _slctIndex[index] || _slctIndex[index] == -1 ||
                _slctIndex[index] >= allNames.Length ||
                allNames[_slctIndex[index]] != current)
            {

                int i;
                for (i = 0; i < allNames.Length; ++i)
                {
                    if (allNames[i] == current)
                    {
                        _slctIndex[index] = i;
                        break;
                    }
                }
                if (allNames.Length == 0 || i > allNames.Length || _slctIndex[index] >= allNames.Length) _slctIndex[index] = -1;
            }
            var nIndex = EditorGUI.Popup(pos, _slctIndex[index], allNames);
            return nIndex;

        }
       
        private void ResetSelectedPos(int startPos)
        {
            for (int i = startPos; i < _slctIndex.Length; ++i)
            {
                _slctIndex[i] = -1;
            }
        }
      
        private int _currentIndex;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            // First get the attribute since it contains the show trigger;
            var eia = attribute as EditorItemAttribute;
            var showTrigger = eia.ShowTrigger;

            var objAndIndex = this.GetTForSerializedProperty<ItemsToChange>(property);
            var objArr = (ItemsToChange[])objAndIndex[0]; ;
            _currentIndex = (int)objAndIndex[1];
            if (objArr.Length <= _currentIndex) return;
            var obj = objArr[_currentIndex];

            obj.displayItemsCount = 0;

            int pIndx = 0, count = 1;
            

            ++obj.displayItemsCount;
            var info = string.Format("{0}. {1}", _currentIndex, obj.go == null? "Element" : obj.ToString(true));
            //var info = string.Format("Element {0} (Active)", _currentIndex);
            position.height = heightBetween;
            obj.expanded = EditorGUI.Foldout(position, obj.expanded, info);
            //EditorGUI.LabelField(position, "Enabled", EditorStyles.boldLabel);


            if (obj.expanded)
            {
                var rects = new List<Rect>();


                for (int i = 0; i < _names.Length; ++i)
                {
                    var ret = new Rect(position);
                    ret.height = heightBetween;
                    ret.y += heightBetween * count++;
                    rects.Add(ret);
                }

                EditorGUI.indentLevel++;

                position.y += heightBetween; ++obj.displayItemsCount;
                obj.disabled = !EditorGUI.Toggle(position, "Enabled", !obj.disabled);

                if (showTrigger)
                {
                    position.y += heightBetween; ++obj.displayItemsCount;
                    obj.AdvancedInput = EditorGUI.Toggle(position, "Advanced", obj.AdvancedInput);


                    EditorGUI.indentLevel++;
                    if (obj.AdvancedInput)
                    {
                        position.y += heightBetween; ++obj.displayItemsCount;
                        obj.Sequence = EditorGUI.TextField(position, new GUIContent("Keys", KeysInfo), obj.Sequence);
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        position.y += heightBetween; ++obj.displayItemsCount;
                        var arr = new List<Rect>() { new Rect(position) };
                        int numOfObj = 4;
                        var p = arr[0];
                        var txtWidth = p.width = 40;
                        
                        
                        for (int i = 0; i < numOfObj-1; ++i) {
                            p = new Rect(position);
                            //p.width = position.width / (numOfObj*.75f);
                            //p.x +=  -15 + (p.width * .7f) * i;
                            p.width = (position.width - txtWidth) /( (numOfObj-1 ) * .8f);
                            p.x += txtWidth + (p.width ) * i * .7f;
                            arr.Add(p);
                        }

                        var size = numOfObj - 1;
                        EditorGUI.LabelField(arr[0], "Keys");

                       

                        if (obj.keys == null || obj.keys.Length != size)
                        {
                            obj.keys = new KeyCode[size];
                            obj.keys[0] = KeyCode.LeftShift;
                            obj.keys[1] = KeyCode.A;
                            obj.UpdateSequenceFromKeys();
                        }
                        for (int i = 0; i < size-1; ++i) {
                            obj.keys[i] = (KeyCode)EditorGUI.EnumPopup(arr[i + 1], obj.keys[i]);
                        }

                        p = new Rect(position);
                        var diff = 80f;
                        p.x = arr[2].x - 12f;
                        p.width = diff;
                        EditorGUI.LabelField(p, "+");

                        p = new Rect(position);
                        p.x = arr[3].x - 12f;
                        p.width = diff;
                        EditorGUI.LabelField(p, ",");
                        var inputChanged = EditorGUI.EndChangeCheck();
                        if(inputChanged)
                        {
                            obj.UpdateSequenceFromKeys();
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.BeginChangeCheck();
                position.y += heightBetween; ++obj.displayItemsCount;
                EditorGUI.PropertyField(position, property.FindPropertyRelative(_names[pIndx++]));
                var changed = EditorGUI.EndChangeCheck();
                if (changed)
                {
                    ResetSelectedPos(0);

                    property.serializedObject.ApplyModifiedProperties();
                    obj.Input = null;
                    obj.Variable = null;
                    obj.Component = null;
                    property.serializedObject.Update();

                }
                if (obj.go != null)
                {
                    var oldCompIndex = _slctIndex[pIndx];

                    var allComp = obj.AllComponents;

                    position.y += heightBetween; ++obj.displayItemsCount;
                    var compIndex = ShowProperty(obj.Component, allComp, pIndx, position);
                    //_showItemsCount++;
                    allComp = obj.AllComponents;
                    if (compIndex != _slctIndex[pIndx] || string.IsNullOrEmpty(obj.Component))
                    {
                        _slctIndex[pIndx] = compIndex;
                        if (compIndex < 0 || compIndex > allComp.Length || allComp.Length == 0)
                        {
                            obj.Component = null;
                            ResetSelectedPos(pIndx + 1);
                        }
                        else
                        {
                            obj.Component = allComp[compIndex];
                            if (string.IsNullOrEmpty(obj.Variable))
                            {
                                var vrs = obj.AllVariables;
                                _slctIndex[pIndx] = 0;
                                if (vrs.Length > 0) obj.Variable = vrs[0];
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(obj.Component) == false)
                    {
                        ++pIndx;
                        var allVars = obj.AllVariables;

                        position.y += heightBetween; ++obj.displayItemsCount;
                        var nIndex = ShowProperty(obj.Variable, allVars, pIndx, position);
                        //_showItemsCount++;

                        allVars = obj.AllVariables;
                        if (nIndex != _slctIndex[pIndx] || oldCompIndex != compIndex
                                || compIndex > allVars.Length || allVars.Length == 0)
                        {
                            _slctIndex[pIndx] = nIndex;
                            if (nIndex < 0 || nIndex > allVars.Length)
                            {
                                obj.Variable = null;
                            }
                            else
                            {
                                obj.Variable = allVars[nIndex];
                            }
                        }

                        ++pIndx;

                        var varT = "?";
                        Type type = null;
                        if (obj.VariableType != null)
                        {
                            type = obj.VariableType;
                            if (type != null)
                            {
                                var dotIndx = type.ToString().LastIndexOf(".");
                                varT = type.ToString().Substring(dotIndx > -1 ? dotIndx + 1 : 0);
                            } else
                            {
                                varT = "Void";
                            }
                        }

                        if (type != null)
                        {
                           // _showItemsCount++;
                            if (obj.Input == null || obj.Input.GetType() != type)
                            {
                                obj.Input = ReflectionUtils.GetDefaultParam(type);
                            }

                            if (showTrigger && obj!=null && obj.VariableClassInfo != null &&  obj.VariableClassInfo.MemberT != MemberTypes.Method)
                            {
                                if (type == typeof(int) || type == typeof(float) || type == typeof(Vector3))
                                {
                                    position.y += heightBetween; ++obj.displayItemsCount;
                                    obj.operation = (ItemsToChange.OperationType)EditorGUI.EnumPopup(position, obj.operation);
                                }else
                                {
                                    obj.operation = ItemsToChange.OperationType.Equal;
                                }
                            }

                                position.y += heightBetween; ++obj.displayItemsCount;

                            var inputLbl = "Input" + 
                                (obj.MemeberInfo == null ? "" : obj.MemeberInfo.MemberT == MemberTypes.Method ? "()" : "") + " <" + varT + ">";

                            if (type == typeof(int))
                            {
                                obj.Input = EditorGUI.IntField(position, inputLbl, (int)(obj.Input));
                            }

                            else if (type == typeof(Vector3))
                            {
                                obj.Input = EditorGUI.Vector3Field(position, inputLbl, (Vector3)(obj.Input));
                            }
                            else if (type == typeof(bool))
                            {
                                obj.Input = EditorGUI.Toggle(position, inputLbl, (bool)(obj.Input));
                            }
                            else if (type == typeof(float))
                            {
                                obj.Input = EditorGUI.FloatField(position, inputLbl, (float)(obj.Input));
                            }
                            else if (type == typeof(string))
                            {
                                obj.Input = EditorGUI.TextField(position, inputLbl, (string)(obj.Input));
                            }
                            else if (type.IsEnum)
                            {
                                obj.Input = EditorGUI.EnumPopup(position, (Enum) obj.Input );
                            }
                            else 
                            {
                                Debug.LogError("Type not supported " + type + ". Contact developer please with this project ");
                            }
                        }
                    }

                }

                property.serializedObject.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }
            //Debug.Log("Show items count " + _showItemsCount);
        }

        private float heightBetween = 16;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var objAndIndex = this.GetTForSerializedProperty<ItemsToChange>(property);
            var objArr = (ItemsToChange[])objAndIndex[0]; ;
            _currentIndex = (int)objAndIndex[1];
            if (objArr.Length <= _currentIndex) return 0;
            var obj = objArr[_currentIndex];

            return obj == null ? 0 :
                    obj.displayItemsCount * 1.2f * heightBetween;

        }
    }
}