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


namespace Dweiss
{
    public static class MonoBehaviourEditorExtension
    {
        public static void SetScriptAwakeOrder(this MonoBehaviour that, short num)
        {
            //Debug.LogFormat("Script Order {0} ({1})", that.GetType(), num);
            UnityEditorHelper.SetScriptAwakeOrder(that.GetType().Name, num);
        }
    }
}