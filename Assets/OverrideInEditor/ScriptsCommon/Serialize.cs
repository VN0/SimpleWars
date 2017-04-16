/*****************************************************************************
 * Copyright (C) 2017 Doron Weiss  - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of unity3d license https://unity3d.com/legal/as_terms.
 * 
 * See https://abnormalcreativity.wixsite.com/home for more info
 ******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Dweiss
{
    namespace Common
    {
        [System.Serializable]
        public class ForJson
        {
            //[SerializeField]
            public byte[] data;
            public object GetData(Type t)
            {
                //Debug.Log("ForJson parse");
                return (data == null || data.Length == 0) ? null :
                    Convert.ChangeType(Serialize.ExtByteArrayToObject(data), t);
            }
            public ForJson(object d) {
                data = d == null ? new byte[0] : Serialize.ExtObjectToByteArray(d);
                //Debug.Log("Json save");
            }
        }

        public static class Serialize
        {
            [Serializable]
            class SerilizeWrapper
            {
                public Type t;
                public object data;

                public override string ToString()
                {
                    return string.Format("[SWrapper {0} = {1}? ({2})]", t, data == null? "NULL" : data.GetType().ToString(), data);
                }

                public SerilizeWrapper(Type t, object data)
                {
                    this.t = t;
                    this.data = data;
                }
            }



            public static byte[] ExtObjectToByteArray(object obj)
            {
                if (obj.GetType() == typeof(Vector3))
                {
                    obj = WrapSerialize(obj);
                }
                return ObjectToByteArray(obj);
            }


            public static object ExtByteArrayToObject(byte[] arrBytes)
            {
                var ret = ByteArrayToObject(arrBytes);
                if (ret.GetType() == typeof(SerilizeWrapper))
                {
                    ret = DeWrapSerialize((SerilizeWrapper)ret);
                }
                return ret;
            }

            static object ByteArrayToObject(byte[] arrBytes)
            {
                if (arrBytes == null || arrBytes.Length == 0) return null;

                MemoryStream memStream = new MemoryStream();
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj = (object)binForm.Deserialize(memStream);

                return obj;
            }
            static byte[] ObjectToByteArray(object obj)
            {
                if (obj == null)
                    return null;
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj);
                    return ms.ToArray();
                }
            }



            static SerilizeWrapper WrapSerialize(object obj)
            {
                if (obj == null) throw new ArgumentNullException("NULL Type not supported for seriliation");
                if (obj.GetType() == typeof(Vector3))
                {
                    var v = (Vector3)obj;
                    var data = new float[3] { v.x, v.y, v.z };

                    return new SerilizeWrapper(obj.GetType(), data);
                }
                throw new InvalidOperationException("Type not supported for seriliation " + obj.GetType());
            }

            static object DeWrapSerialize(SerilizeWrapper wrapper)
            {
                if (wrapper == null) throw new ArgumentNullException("NULL Type not supported for seriliation");
                if (wrapper.t == typeof(Vector3))
                {
                    var arr = (float[])wrapper.data;
                    return new Vector3(arr[0], arr[1], arr[2]);
                }
                throw new InvalidOperationException("Type not supported for seriliation " + wrapper.t);
            }
        }
    }
}