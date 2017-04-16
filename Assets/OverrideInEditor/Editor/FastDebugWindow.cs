using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

public class FastDebugWindow : EditorWindow
{

    class ClassMemeberWithInfo : ReflectionUtils.MemberClass
    {
        public object data;
        public string paramTypeDesc;
        public enum OperationType
        {
            Equal,
            Add,
            Substract,
            Mul,
            Div
        }
        public string[] OperationTypes = new string[] { "=", "+","-","*","/"};
        public int operationTypeIndex = 0; //For display

        public string OperationSign() {
            if (ParamType == typeof(Vector3)
                    || ParamType == typeof(float)
                    || ParamType == typeof(int))
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
        public OperationType operation;

        private object Operator(object a, object b)
        {
            if (ParamType != null)
            {
                if (ParamType == typeof(int))
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
                else if (ParamType == typeof(float))
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
                else if (ParamType == typeof(Vector3))
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
                else if (ParamType == typeof(string)
                    || ParamType == typeof(bool) || ParamType.IsEnum)
                {
                    return b;
                }
            }
            throw new System.InvalidOperationException("Type of objects are unknown or not supported " + ParamType + " operator " + operation);

        }

        public void Activate(UnityEngine.Object obj)
        {

            Undo.RecordObject(obj, "Change object");
            if (MemberT == System.Reflection.MemberTypes.Property)
            {
                var current = ReflectionUtils.GetProperty(obj, Name, ReflectionUtils.PrivateBindingFlags);
                var ret = Operator(current, data);
                ReflectionUtils.SetProperty(obj, Name, ret, ReflectionUtils.PrivateBindingFlags);
            }
            else if (MemberT == System.Reflection.MemberTypes.Method)
            {
                Debug.Log("Reflection: " + ReflectionUtils.SetFunc(obj, Name, data, ReflectionUtils.PrivateBindingFlags));
            }
            else if (MemberT == System.Reflection.MemberTypes.Field)
            {
                var current = ReflectionUtils.GetField(obj, Name, ReflectionUtils.PrivateBindingFlags);
                var ret = Operator(current, data);
                ReflectionUtils.SetField(obj, Name, ret, ReflectionUtils.PrivateBindingFlags);
            }
            else
            {
                Debug.LogError("Override Type not supported " + obj + " by " + this);
            }
        }

        public ClassMemeberWithInfo(ReflectionUtils.MemberClass mmbr) : base(mmbr)
        {
            if (mmbr.ParamType != null)
                data = ReflectionUtils.GetDefaultParam(mmbr.ParamType);

            paramTypeDesc = "?";
            if (mmbr.ParamType != null)
            {
                var dotIndx = mmbr.ParamType.ToString().LastIndexOf(".");
                paramTypeDesc = mmbr.ParamType.ToString().Substring(dotIndx > -1 ? dotIndx + 1 : 0);
            }
            else
            {
                paramTypeDesc = "Void";
            }
        }
    }

    bool showProperty = true;
    bool useSelection = true;
    bool showLog = true;

    UnityEngine.Object script;
    GameObject gameObject;


    int selectedIndex;
    string[] components = new string[0];

    Vector2 scrollPos;
    ClassMemeberWithInfo[] memebers = new ClassMemeberWithInfo[0];

    [MenuItem("Tools/Fast Debug")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (FastDebugWindow)EditorWindow.GetWindow(typeof(FastDebugWindow));
        window.titleContent = new GUIContent("Fast Debug");
        window.Show();

    }

    private void OnDisable()
    {
        Selection.selectionChanged -= SelectionChanged;
    }

    private void OnEnable()
    {
        Selection.selectionChanged += SelectionChanged;
    }

    private List<UnityEngine.Object> _selectedGo = new List<UnityEngine.Object>();
    private void SelectionChanged()
    {
        if (useSelection)
        {
            _selectedGo = GetRelevantObjects();
            Repaint();
        }
        //if (useSelection && Selection.activeObject != null &&
        //       Selection.activeObject != gameObject) {
        //}
        
    }


    // Because the window is returning to visibility, Repaint() is usually redundant.
    // However, if we make changes in Visual Studio and then return to Unity, we do get
    // an OnFocus without a subsequent repaint.
    void OnFocus()
    {
        Repaint();
    }

    private List<UnityEngine.Object> GetRelevantObjects()
    {
        var list = new List<UnityEngine.Object>();
        if (script == null) return list;
        for (int i = 0; i < Selection.gameObjects.Length; ++i)
        {
            var obj = script.GetType() == typeof(GameObject) ?
                (UnityEngine.Object)Selection.gameObjects[i].gameObject :
                Selection.gameObjects[i].GetComponent(script.GetType());
            if (obj != null) list.Add(obj);
        }
        return list;

    }
    private void ShowMember(ClassMemeberWithInfo mmbr, UnityEngine.Object script)
    {
        Type type = mmbr.ParamType;



        if (type != null)
        {

            if (mmbr.MemberT != MemberTypes.Method && showProperty == false) return;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Activate"))
            {
                if (useSelection)
                {
                    GetRelevantObjects().ForEach(s => { try { mmbr.Activate(s); } catch (Exception e) { Debug.LogWarning("Probelm with debug call on " + s + ": " + e); } });
                }
                else
                {
                    mmbr.Activate(script);
                }
                if(showLog) Debug.Log("FastDebugWindow - Execute");
            }

            var inputLbl = mmbr.Name + (mmbr.MemberT == MemberTypes.Method ? "() " : " ");// +  " " + " (" + mmbr.paramTypeDesc + ")";
            EditorGUILayout.LabelField(inputLbl);
            if (type == typeof(int) || type == typeof(Vector3) || type == typeof(float))
            {
                mmbr.operationTypeIndex = EditorGUILayout.Popup(mmbr.operationTypeIndex, mmbr.OperationTypes);
                mmbr.operation = (ClassMemeberWithInfo.OperationType)mmbr.operationTypeIndex;
            }
            if (type == typeof(int))
            {
                mmbr.data = EditorGUILayout.IntField((int)(mmbr.data));
            }

            else if (type == typeof(Vector3))
            {
                mmbr.data = EditorGUILayout.Vector3Field("",(Vector3)(mmbr.data));
            }
            else if (type == typeof(bool))
            {
                mmbr.data = EditorGUILayout.Toggle((bool)(mmbr.data));
            }
            else if (type == typeof(float))
            {
                mmbr.data = EditorGUILayout.FloatField((float)(mmbr.data));
            }
            else if (type == typeof(string))
            {
                mmbr.data = EditorGUILayout.TextField((string)(mmbr.data));
            }
            else if (type.IsEnum)
            {
                mmbr.data = EditorGUILayout.EnumPopup((Enum)mmbr.data);
            }
            //else if (type == typeof(UnityEngine.Object) 
            //    || type.GetBaseClassesAndInterfaces().Contains(typeof(UnityEngine.Object)))
            //{
            //    var chosen = EditorGUI.ObjectField(pos, inputLbl, (UnityEngine.Object)(mmbr.data), type, true);
            //    mmbr.data = chosen;
            //    //Debug.Log("Changed " + type + " input to " + chosen + " ? " + obj);
            //}
            else
            {
                Debug.LogError("Type not supported " + type + ". Contact developer please with this project ");
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void UpdateComponents()
    {
        Undo.RecordObject(this, "new compoenents");
        if (gameObject == null) components = new string[0];
        else
        {
            var newList = ((GameObject)gameObject)
                   .GetComponents<Component>()
                   .Select(n => n.GetType().FullName).ToList();

            newList.Add(typeof(GameObject).FullName);
            components = newList.ToArray();
        }
        //selectedGo = GetRelevantObjects();
    }
    private void UpdateMemebers()
    {
        Undo.RecordObject(this, "new memebers");
        memebers = script == null ? new ClassMemeberWithInfo[0] :
                            ReflectionUtils.GetPrimitiveMembers(script.GetType(), ReflectionUtils.PrivateBindingFlags)
                            .Select(a => new ClassMemeberWithInfo(a)).ToArray();
        _selectedGo = GetRelevantObjects();
    }

    private void UpdateScripts(UnityEngine.Object comp)
    {
        if(comp == null)
        {
            //Debug.Log("Here0");
            Undo.RecordObject(this, "new gameobject");gameObject = null;

            Undo.RecordObject(this, "new script"); script = null;

            Undo.RecordObject(this, "new selectedIndex");
            selectedIndex = 0;
            UpdateComponents();
            UpdateMemebers();
            return;
        }


        if (comp.GetType() == typeof(GameObject))
        {
            //Debug.LogFormat("Here1 New:{0} GO:{1} SCRIPT:{2}", comp, gameObject, script);
            if (comp != gameObject || script == null || 
                (script.GetType() == typeof(GameObject) && comp != script))
            {
                Undo.RecordObject(this, "new gameObject");
                gameObject = (GameObject)comp;
                UpdateComponents();

                var found = false;
                
                if (script != null)
                {
                    for (int i = 0; i < components.Length; ++i)
                    {
                        if (components[i] == script.GetType().FullName)
                        {
                            Undo.RecordObject(this, "new selectedIndex");
                            selectedIndex = i;
                            Undo.RecordObject(this, "new script");
                            script = selectedIndex == components.Length - 1 ? 
                                (UnityEngine.Object)gameObject :
                                gameObject.GetComponents<Component>()
                                .First(c => c.GetType().FullName == components[selectedIndex]);
                            found = true;
                            break;
                        }
                    }
                }
                if (found == false)
                {
                    Undo.RecordObject(this, "new script");
                    script = gameObject;
                    Undo.RecordObject(this, "new selectedIndex");
                    selectedIndex = components.Length - 1;
                }

                UpdateMemebers();

            }
        }
        else if (comp != script)
        {
            var oldGo = gameObject;

            Undo.RecordObject(this, "new GameObject");
            gameObject = ((Component)comp).gameObject;
            if(oldGo != gameObject) UpdateComponents();
            // selectedIndex = 0;

            Undo.RecordObject(this, "new script");
            script = null;
            for (int i = 0; i < components.Length; ++i)
            {
                if (components[i] == comp.GetType().FullName)
                {

                    Undo.RecordObject(this, "new script");
                    script = comp;
                    Undo.RecordObject(this, "new selectedIndex");
                    selectedIndex = i;
                    break;
                }
            }
            //Debug.LogFormat("comp != script: {0} - ({1}) - {2]", script==null ? "NULL" : script.ToString(), 
            //    selectedIndex, string.Join(",", components));

            UpdateMemebers();

        }
    }
    private static int NameLabelLength = 120, MultiNameMaxLength = 6;
    private bool played = false;
    void OnGUI()
    {
        if (Application.isPlaying != played) UpdateScripts(null);
        played = Application.isPlaying;
        //GUILayout.Label("Fast Debug", EditorStyles.boldLabel);

        //useSelection = EditorGUILayout.ToggleLeft("On Selection", useSelection);
        //showProperty = EditorGUILayout.ToggleLeft("Show Property", showProperty);
        showLog = EditorGUILayout.ToggleLeft("Show Log", showLog);

        EditorGUILayout.Space();

        var selectionNotExists = useSelection && gameObject == null;
        if (_selectedGo.Count == 0 && useSelection) { gameObject = null; UpdateScripts(null); }
        //if (selectionNotExists == false)
        {
            var names = _selectedGo.Count == 0 ? "NULL" : string.Join(", ", _selectedGo.Select(a => a.name).ToArray());
            if (_selectedGo.Count > 0 && names.Length > NameLabelLength) names = string.Join(", ", 
                _selectedGo.Select(a => {
                    var length = Math.Min(a.name.Length, MultiNameMaxLength);
                    return a.name.Substring(0, length) + (length < a.name.Length? "*" : "");
                    }).ToArray()
                );
            if (names.Length > NameLabelLength) names = names.Substring(0, NameLabelLength) + "...";
            if (useSelection == false) names = (gameObject == null ? "null" : gameObject.name.ToString());
            //GUILayout.Label(" #" + _selectedGo.Count + " - " + (gameObject == null ? "" : gameObject.name.ToString()));
            GUILayout.Label((useSelection ? "(#" + _selectedGo.Count + "): " : "") + names);
        }

        EditorGUILayout.BeginHorizontal();
        if (selectionNotExists)
        {
            GUILayout.Label("Choose GameObject");
        }
        if (useSelection)
        {
            if (Selection.activeObject != null &&
                (Selection.activeObject.GetType() == typeof(GameObject) ||
                typeof(Component).IsAssignableFrom(Selection.activeObject.GetType())))
            {
                UpdateScripts(Selection.activeObject);
            }
            if (selectionNotExists == false)
            {
                var newIndex = EditorGUILayout.Popup(selectedIndex, components);
                //Debug.LogFormat("index/selected {0}/{1}", selectedIndex, newIndex);

                if (newIndex != selectedIndex)
                {

                    Undo.RecordObject(this, "new selectedIndex");
                    selectedIndex = newIndex;

                    Undo.RecordObject(this, "new script");
                    script = selectedIndex == components.Length - 1 ? (UnityEngine.Object)gameObject :
                        gameObject.GetComponents<Component>().First(c => c.GetType().FullName == components[selectedIndex]);

                    UpdateMemebers();
                }
            }
            // if (Selection.transforms.Length > 0)

        }
        else
        {
            //newObj = EditorGUILayout.ObjectField(newObj, typeof(UnityEngine.Object), true);
            var newScript = EditorGUILayout.ObjectField(script, typeof(Component), true);

            UpdateScripts(newScript);

            
        }
        EditorGUILayout.EndHorizontal();

       
        
        EditorGUILayout.Space();

        //Debug.Log(selectedIndex +". GO " + gameObject + " script " + oldScript + " -> " + script + " Selected (" + Selection.activeObject + ")");


        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        try
        {
            foreach (var m in memebers)
            {
                ShowMember(m, script);
            }
        } finally
        {
        }
        EditorGUILayout.EndScrollView();

    }
}