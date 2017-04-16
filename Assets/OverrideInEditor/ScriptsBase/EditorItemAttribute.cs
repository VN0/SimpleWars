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
    public class EditorItemAttribute : PropertyAttribute
    {
        private bool _showTrigger = false;
        public bool ShowTrigger
        {
            get { return _showTrigger; }
        }
        public EditorItemAttribute()
        {
        }
        public EditorItemAttribute(bool showTrigger)
        {
            _showTrigger = showTrigger;
        }
    }
}