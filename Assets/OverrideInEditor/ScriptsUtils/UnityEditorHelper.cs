/*****************************************************************************
 * Copyright (C) 2017 Doron Weiss  - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of unity3d license https://unity3d.com/legal/as_terms.
 * 
 * See https://abnormalcreativity.wixsite.com/home for more info
 ******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityEditorHelper
{
    public static void SetScriptAwakeOrder<T>(short num)
    {
        string scriptName = typeof(T).Name;
        SetScriptAwakeOrder(scriptName, num);
    }
    public static void SetScriptAwakeOrder(string scriptName, short num)
    {
#if UNITY_EDITOR
            foreach (var monoScript in UnityEditor.MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (monoScript.name == scriptName)
                {
                    var exeOrder = UnityEditor.MonoImporter.GetExecutionOrder(monoScript);
                    if (exeOrder != num)
                    {
                        //Debug.Log("Change script " + monoScript.name + " old " + exeOrder + " new " + num);
                        UnityEditor.MonoImporter.SetExecutionOrder(monoScript, num);
                    }
                    break;
                }
            }
#endif
    }
}
