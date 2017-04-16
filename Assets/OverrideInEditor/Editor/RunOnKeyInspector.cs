/*****************************************************************************
 * Copyright (C) 2017 Doron Weiss  - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of unity3d license https://unity3d.com/legal/as_terms.
 * 
 * See https://abnormalcreativity.wixsite.com/home for more info
 ******************************************************************************/
using UnityEngine;
using UnityEditor;

namespace Dweiss
{
    [CustomEditor(typeof(RunOnKey))]
    public class RunOnKeyInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = ((RunOnKey)target);
            if (GUILayout.Button("Refresh"))
            {
                script.RefreshItems();
            }
            DrawDefaultInspector();
        }

    }
}