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
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ReflectionUtils
{


    public const BindingFlags PublicBindingFlags =
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;


    public const BindingFlags PublicAndStaticBindingFlags =
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.InvokeMethod |
        BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.OptionalParamBinding;


    public const BindingFlags PrivateBindingFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;


    private static Type[] _primitivesTypes = new[] {
            typeof(string),
            typeof(bool),
            typeof(Vector3),
            typeof(int),
            typeof(float),
            //typeof(UnityEngine.Object),
            //typeof(char),
           // typeof(byte),
          //  typeof(sbyte),
           // typeof(ushort),
          //  typeof(short),
          //  typeof(uint),
          //  typeof(ulong),
          //  typeof(long),
            //typeof(double),
            //typeof(decimal),
        };
    private static bool IsPrimitive(Type t)
    {
        // TODO: put any type here that you consider as primitive as I didn't
        // quite understand what your definition of primitive type is
        return _primitivesTypes.Contains(t) || t.IsEnum;
        //|| t == typeof(UnityEngine.Object) || t.GetBaseClassesAndInterfaces().Contains(typeof(UnityEngine.Object));
    }


    public static object GetDefaultParam(System.Type t)
    {
        //Debug.Log("Get default " + t + " is enum " + t.IsEnum + " from types " + string.Join(",", t.GetBaseClassesAndInterfaces().Select(a=>a.Name).ToArray()));
        if (t.IsEnum)
        {
            System.Array enumValues = System.Enum.GetValues(t);
            if (enumValues.Length > 0)
            {
                return enumValues.GetValue(0);
            }
            return null;
        }
        if (t == typeof(string)) return string.Empty;
        //else if (t == typeof(char)) return default(char);
        else if (t == typeof(bool)) return default(bool);
        //else if (t == typeof(byte)) return default(byte);
        //else if (t == typeof(sbyte)) return default(sbyte);
        //else if (t == typeof(ushort)) return default(ushort);
        // else if (t == typeof(short)) return default(short);
        // else if (t == typeof(uint)) return default(uint);
        else if (t == typeof(int)) return default(int);
        //   else if (t == typeof(ulong)) return default(ulong);
        //  else if (t == typeof(long)) return default(long);
        else if (t == typeof(float)) return default(float);
        // else if (t == typeof(double)) return default(double);
        // else if (t == typeof(decimal)) return default(decimal);
        else if (t == typeof(Vector3)) return Vector3.zero;
        //else if (t == typeof(UnityEngine.Object) || t.GetBaseClassesAndInterfaces().Contains(typeof(UnityEngine.Object))) return null;

        throw new System.ArgumentException("Type not supported " + t);
    }

    public static IEnumerable<PropertyInfo> VisitProperty(Type t, BindingFlags bf, bool checkWritable = true)
    {
        var result = new List<PropertyInfo>();
        //    InternalVisitProperty(t, result);
        //    return result;
        //}

        //private static void InternalVisitProperty(Type t, IList<PropertyInfo> result)
        //{
        foreach (var property in t.GetProperties(bf))
        {

            if (IsPrimitive(property.PropertyType) && (checkWritable == false || property.CanWrite))
            {
                result.Add(property);
                // ok += property.Name + " - " + property.PropertyType + "\n";
            }
            else
            {
                //notOk += property.Name + " - " + property.PropertyType + "\n";
            }
            //InternalVisitProperty(property.PropertyType, visitedTypes, result);
        }
        return result;
        // Debug.Log(t.Name + " valid : " + ok);
        // Debug.Log(t.Name + " NOT : " + notOk);

    }
    public static IEnumerable<FieldInfo> VisitField(Type t, BindingFlags bf)
    {
        var result = new List<FieldInfo>();
        var visited = new HashSet<Type>();

        InternalVisitField(t, visited, result, bf);
        return result;
    }

    private static void InternalVisitField(Type t, HashSet<Type> visitedTypes, IList<FieldInfo> result, BindingFlags bf)
    {
        if (visitedTypes.Contains(t))
        {
            return;
        }
        //string ok = "", notOk = "";
        if (!IsPrimitive(t))
        {
            visitedTypes.Add(t);
            foreach (var property in t.GetFields(bf))
            {

                if (IsPrimitive(property.FieldType))
                {
                    result.Add(property);
                   // ok += property.Name + " - " + property.FieldType + "\n";
                }
                else
                {
                 //   notOk += property.Name + " - " + property.FieldType + "\n";
                }
                //InternalVisitProperty(property.PropertyType, visitedTypes, result);
            }
        }
        //Debug.Log(t.Name + " valid : " + ok);
        //Debug.Log(t.Name + " NOT : " + notOk);

    }

    public class MemberClass
    {
        public string Name;
        public Type ParamType;
        public MemberTypes MemberT;
        public override string ToString()
        {
            return string.Format("[MC {0}-{1} ({2}) ]", MemberT, Name, ParamType);
        }
        public MemberClass(string n, Type t, MemberTypes mt) { Name = n; ParamType = t; MemberT = mt; }
        public MemberClass(MemberClass mc) { Name = mc.Name; ParamType = mc.ParamType; MemberT = mc.MemberT; }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other.GetType() != GetType()) return false;
            var o = (MemberClass)other;
            return o.Name == Name && o.ParamType == ParamType && o.MemberT == MemberT;
        }

        public override int GetHashCode()
        {
            if(string.IsNullOrEmpty(Name) || ParamType == null) return base.GetHashCode();
            return Name.GetHashCode() + ParamType.GetHashCode() + MemberT.GetHashCode();
        }
    }
    public class Compare :  IEqualityComparer<MemberClass>
    {
        public bool Equals(MemberClass x, MemberClass y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(MemberClass obj)
        {
            return obj.GetHashCode();
        }
    }

    public static void AddRange<T>(this HashSet<T> that, IEnumerable<T> l)
    {
        foreach (var t in l) that.Add(t);
    }
    public static MemberClass[] GetPrimitiveMembers(Type type, BindingFlags bf = PublicBindingFlags)
    {
        var props = new HashSet<MemberClass>( new Compare());
        do
        {
            var result = VisitProperty(type, bf);
            props.AddRange(result.Select(p => new MemberClass(p.Name, p.PropertyType, p.MemberType)));
            var resultF = VisitField(type, bf);
            props.AddRange(resultF.Select(p => new MemberClass(p.Name, p.FieldType, p.MemberType)));
            var funcs = GetPrimitiveMethods(type, bf);
            props.AddRange(funcs.Select(p =>
            {
                var prms = p.GetParameters();
                return new MemberClass(p.Name, prms.Length == 0 ? null : prms[0].ParameterType, p.MemberType);
            }));


        } while ((type = type.BaseType) != null);

        //Debug.Log("Var type " + type + ": " + string.Join(",", props.Select(a => a.ToString()).ToArray()));
        return props.ToArray();
    }

    public static MemberClass GetPrimitiveMember(Type t, string name, BindingFlags bf = PublicBindingFlags)
    {
        var f = t.GetField(name, bf);
        if (f != null) {
            return new MemberClass(f.Name, f.FieldType, f.MemberType) ;
        }

        var p = t.GetProperty(name, bf);
        if (p != null)
        {
            return new MemberClass(p.Name, p.PropertyType, p.MemberType);
        }


        var m = GetPrimitiveMethods(t, bf).First(a => a.Name == name);
        if (m != null)
        {
            var prms = m.GetParameters();
            return new MemberClass(m.Name, prms.Length == 0 ? null : prms[0].ParameterType, m.MemberType);
        }

        return null;
        //Debug.Log("Var type " + type + ": " + string.Join(",", props.Select(a => a.ToString()).ToArray()));
    }

    public static PropertyInfo[] GetPrimitiveProperties(Type type, BindingFlags bf = PublicBindingFlags)
    {
        var props = new List<PropertyInfo>();
        do
        {
            var result = VisitProperty(type, bf);
            props.AddRange(result);

        } while ((type = type.BaseType) != null);

        //Debug.Log("Var type " + type + ": " + string.Join(",", props.Select(a => a.ToString()).ToArray()));
        return props.ToArray();
    }
    public static void SetField<T>(T script, string propName, object value, BindingFlags bf = PublicBindingFlags)
    {
        var t = script.GetType();
        var prop = t.GetField(propName, bf);

        prop.SetValue(script, value);
    }
    public static object GetField<T>(T script, string propName, BindingFlags bf = PublicBindingFlags)
    {
        var t = script.GetType();
        var prop = t.GetField(propName, bf);
        return prop.GetValue(script);
    }
    public static void SetProperty<T>(T script, string propName, object value, BindingFlags bf = PublicBindingFlags)
    {
        var t = script.GetType();
        var prop = t.GetProperty(propName, bf);

        prop.SetValue(script, value, null);
    }
    public static object GetProperty<T>(T script, string propName, BindingFlags bf = PublicBindingFlags)
    {
        var t = script.GetType();
        var prop = t.GetProperty(propName, bf);
        //Debug.LogFormat("{0} - {1} = {2} ", t, propName, prop );
        return prop.GetValue(script, null);
    }

    public static object SetFunc<T>(T script, string memberName, object value, BindingFlags bf = PublicBindingFlags)
    {
        //Debug.Log("Script " + script + " T " + typeof(T) + " mmbr " + memberName + " v " + value);
        var t = script.GetType();
        var func = t.GetMethod(memberName, bf);
        
        return func.Invoke(script, value == null ? null : new object[] { value });
    }
   
    //////////////////////////////
    public static IEnumerable<MethodInfo> VisitMethods(Type t, BindingFlags bf = PublicBindingFlags, bool addSpecailName = false)
    {
        // var visitedTypes = new HashSet<Type>();
        var result = new List<MethodInfo>();

        //visitedTypes.Add(t);
        foreach (var method in t.GetMethods(bf))
        {
            ParameterInfo[] myArr = method.GetParameters();
            // if (method.ReturnType != typeof(void)) continue;
            if ((method.IsSpecialName == false || addSpecailName) &&
                        (myArr.Length == 0 ||
                    (myArr.Length == 1 && IsPrimitive(myArr[0].ParameterType)))
                )

            {
                result.Add(method);
            }
            //InternalVisitProperty(property.PropertyType, visitedTypes, result);
        }

        return result;
    }

    public static MethodInfo[] GetPrimitiveMethods(Type type, BindingFlags bf = PublicBindingFlags)
    {
        var result = VisitMethods(type, bf);
        var ret = result.ToArray();
        return ret;
    }
    //////////////////////////////
    /// <summary>
    /// /
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static PropertyInfo[] GetProperties<T>(BindingFlags bf = PublicBindingFlags)
    {

        var type = typeof(T);
        var propertyInfos = type.GetProperties(bf);

        return propertyInfos;
    }

    public static MethodInfo[] GetFunctions<T>(BindingFlags bf = PublicBindingFlags)
    {
        var type = typeof(T);
        var propertyInfos = type.GetMethods(bf);

        return propertyInfos;
    }

    /////////////////////////////

    public static System.Type GetType(string TypeName)
    {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // If the TypeName is a full name, then we can try loading the defining assembly directly
        if (TypeName.Contains("."))
        {

            // Get the name of the assembly (Assumption is that we are using 
            // fully-qualified type names)
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;

            // Ask that assembly to return the proper Type
            type = assembly.GetType(TypeName);
            if (type != null)
                return type;

        }

        // If we still haven't found the proper type, we can enumerate all of the 
        // loaded assemblies and see if any of them define the type
        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {

            // Load the referenced assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                // See if that assembly defines the named type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }

        // The type just couldn't be found...
        return null;

    }

}

