/*****************************************************************************
 * Copyright (C) 2017 Doron Weiss  - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of unity3d license https://unity3d.com/legal/as_terms.
 * 
 * See https://abnormalcreativity.wixsite.com/home for more info
 ******************************************************************************/
using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

public static class PropertyDrawerUtils
{

    public static object[] GetTForSerializedProperty<T>(this PropertyDrawer pd, SerializedProperty property)
         where T : class
    {
        return GetActualObjectForSerializedProperty<T>(pd.fieldInfo, property);
    }
    public static object[] GetActualObjectForSerializedProperty<T>(
        FieldInfo fieldInfo, SerializedProperty property) where T : class
    {
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
        if (obj == null) { return null; }
        int index = -1;
        T[] actualObject = null;
        if (obj.GetType().IsArray)
        {
            index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
            actualObject = (T[])obj;// ((T[])obj).Length > index ? ((T[])obj)[index] : null;
        }
        else
        {
            actualObject = new T[] { obj as T };
            index = 0;
        }
        return new object[] { actualObject, index};
    }

    public static T GetFieldByName<T>(object obj, string fieldName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
    {
        FieldInfo fieldInfo = obj.GetType().GetField(fieldName, bindingFlags);

        if (fieldInfo == null)
            return default(T);

        return (T)fieldInfo.GetValue(obj);
    }

    public static T GetFieldByName<T>(this SerializedProperty obj)
    {
        return GetFieldByName<T>(obj.serializedObject.targetObject, obj.propertyPath);
    }

   
    public static T SerializedPropertyToObject<T>(SerializedProperty property)
    {
        return GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property), true); //The "true" means we will also check all base classes
    }

    public static Component GetSerializedPropertyRootComponent(SerializedProperty property)
    {
        return (Component)(property.serializedObject.targetObject);
    }

    public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = false)
    {
        object ret = obj;
        foreach (string part in path.Split('.'))
        {
            ret = GetFieldOrPropertyValue<object>(part, ret, includeAllBases);
            //Debug.Log(path + ": " + obj + " -> " + ret);
        }
        return (T)ret;
    }

    public static T GetFieldOrPropertyValue<T>(string fieldName, object obj, bool includeAllBases = false, BindingFlags bindings =
        BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Default
        | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
    {
        var objT = obj.GetType();
        FieldInfo field = objT.GetField(fieldName, bindings);
        if (field != null) return (T)field.GetValue(obj);

        PropertyInfo property = objT.GetProperty(fieldName, bindings);
        if (property != null) return (T)property.GetValue(obj, null);

        if (includeAllBases)
        {

            foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
            {
                field = type.GetField(fieldName, bindings);
                if (field != null) return (T)field.GetValue(obj);

                property = type.GetProperty(fieldName, bindings);
                if (property != null) return (T)property.GetValue(obj, null);
            }
        }

        return default(T);
    }

    public static void SetFieldOrPropertyValue<T>(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
    {
        FieldInfo field = obj.GetType().GetField(fieldName, bindings);
        if (field != null)
        {
            field.SetValue(obj, value);
            return;
        }

        PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
        if (property != null)
        {
            property.SetValue(obj, value, null);
            return;
        }

        if (includeAllBases)
        {
            foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
            {
                field = type.GetField(fieldName, bindings);
                if (field != null)
                {
                    field.SetValue(obj, value);
                    return;
                }

                property = type.GetProperty(fieldName, bindings);
                if (property != null)
                {
                    property.SetValue(obj, value, null);
                    return;
                }
            }
        }
    }

    public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
    {
        List<Type> allTypes = new List<Type>();
        if (type == null) return allTypes;
        if (includeSelf) allTypes.Add(type);

        if (type.BaseType == typeof(object))
        {
            allTypes.AddRange(type.GetInterfaces());
        }
        else
        {
            allTypes.AddRange(
                    Enumerable
                    .Repeat(type.BaseType, 1)
                    .Concat(type.GetInterfaces())
                    .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                    .Distinct());
        }

        return allTypes;
    }
}
