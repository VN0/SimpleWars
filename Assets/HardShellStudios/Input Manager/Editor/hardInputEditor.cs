using UnityEngine;
using UnityEditor;

namespace HardShellStudios.InputManager
{

    ///<summary>
    ///Custom editor inspector script that actions on the InputController prefab.
    ///</summary>
    [InitializeOnLoad]
    [CustomEditor(typeof(hardManager))]
    public class hardInputEditor : Editor
    {

        public hardManager myscript;
        string currentVersion = "Current Version - 2.61 | Created by Haydn Comley";

        // Btw you can change this if you really want, all it will do is change the colour scheme and images shown... It wont actually allow you to download the early updates if you havent purchased
        // it on the asset store. :)
        bool earlyUpdate = true;

        string inputName = "";
        KeyCode keyPrime;
        KeyCode keySec;
        hardKey.controllerMap joyPrime;
        hardKey.controllerMap joySec;
        bool saveable = true;
        bool showAll = false;

        string[] axisOptions;

        string[] axisOptionsBASE = new string[] {   "Mouse or Button Press", // 0
                                            "Mouse Axis/Wheel Up", "Mouse Axis/Wheel Down", "Mouse Axis/Position X", "Mouse Axis/Position Y"};

        string[] axisOptionsFULL = new string[] {   "Mouse or Button Press", // 0
                                            "Mouse Axis/Wheel Up", "Mouse Axis/Wheel Down", "Mouse Axis/Position X", "Mouse Axis/Position Y", // 1-4
                                            "Controller Button Press", // 5

                                            "Controller Axis/Right Stick/X Axis", "Controller Axis/Right Stick/Y Axis",       // 6-7
                                            "Controller Axis/Left Stick/X Axis", "Controller Axis/Left Stick/Y Axis"};    // 8-9

        string[] controllerTypes = new string[] { "Playstation 4", "Playstation 3", "Xbox One", "Xbox 360" };

        int axisSelected = 0;
        int axisSelected2 = 0;
        bool[] opened = new bool[0];

        static hardInputEditor()
        {
            EditorApplication.hierarchyWindowChanged += doOnLoad;
        }

        public static void doOnLoad()
        {
            hardInputEditor edit = (hardInputEditor)ScriptableObject.CreateInstance("hardInputEditor");
            edit.checkController();
        }

        public void checkController()
        {
            //myscript = (hardManager)target;

            if (myscript != null)
            {

                if (myscript.useController)
                {
                    try
                    {
                        Input.GetAxis("DPADVER");
                        Input.GetAxis("DPADHOR");
                        Input.GetAxis("STICKLHOR");
                        Input.GetAxis("STICKRHOR");
                        Input.GetAxis("STICKLVER");
                        Input.GetAxis("STICKRVER");
                    }
                    catch
                    {
                        myscript.useController = false;
                        Debug.LogError("Unity Inputs not properly configured for use with controllers.");
                        Debug.LogError("Please open the 'Readme.txt' for guidence. Video tutorial included.");
                    }
                }
            }
            else
            {
                //myscript.useController = false;
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;

        }

        [MenuItem("Hard Shell Studios/Unity's Input Manager")]
        private static void NewMenuOption()
        {
            EditorApplication.ExecuteMenuItem("Edit/Project Settings/Input");
        }

        float ColorConv(float value)
        {
            return value / 255;
        }

        public override void OnInspectorGUI()
        {
            //Begin
            myscript = (hardManager)target;

            bool anySaved = true;
            if (myscript.useController) { axisOptions = axisOptionsFULL; } else { axisOptions = axisOptionsBASE; }
            Texture headerImg = (Texture)Resources.Load("header-default");
            var centered = GUI.skin.GetStyle("Label");
            centered.alignment = TextAnchor.UpperCenter;
            GUILayout.Label(headerImg, centered);

            if (!showAll)
            {
                if (GUILayout.Button("Hide All Keys"))
                {
                    showAll = true;
                }
            }

            for (int i = 0; i < myscript.inputs.Length; i++)
            {

                string currName = myscript.inputs[i].keyName;
                KeyCode currPrim = myscript.inputs[i].primaryKeycode;
                KeyCode currSec = myscript.inputs[i].secondaryKeycode;
                hardKey.controllerMap currJoyPrim = myscript.inputs[i].controllerOne;
                hardKey.controllerMap currJoySec = myscript.inputs[i].controllerTwo;
                int axisType = myscript.inputs[i].axisType;
                int axisType2 = myscript.inputs[i].axisType2;
                bool saveKey = myscript.inputs[i].saveKey;
                bool[] hold = opened;
                opened = new bool[opened.Length + 1];
                for (int i2 = 0; i2 < hold.Length; i2++)
                {
                    opened[i2] = hold[i2];
                }


                // Alternating Color Scheme
                Color[] colors = new Color[] { new Color32(170, 36, 143, 255), new Color32(97, 97, 97, 255) };
                GUIStyle style = new GUIStyle();
                style.normal.background = MakeTex(600, 1, colors[i % 2]);
                EditorGUILayout.BeginVertical(style);

                //GUIStyle styleFoldout = new GUIStyle();

                if (showAll)
                {
                    opened[i] = false;
                }

                opened[i] = EditorGUILayout.Foldout(opened[i], currName);

                if (opened[i])
                {

                    currName = EditorGUILayout.TextField("Name", currName);
                    if (myscript.inputs[i].keyName != currName)
                        myscript.inputs[i].keyName = currName;

                    saveKey = EditorGUILayout.Toggle("Save In-Game Rebinds", saveKey);
                    if (myscript.inputs[i].saveKey != saveKey)
                        myscript.inputs[i].saveKey = saveKey;

                    axisType = EditorGUILayout.Popup("Primary Key Type", axisType, axisOptions);
                    if (myscript.inputs[i].axisType != axisType)
                        myscript.inputs[i].axisType = axisType;


                    if (axisType == 0)
                    {

                        currPrim = (KeyCode)EditorGUILayout.EnumPopup("Primary Key", currPrim);
                        if (myscript.inputs[i].primaryKeycode != currPrim)
                            myscript.inputs[i].primaryKeycode = currPrim;
                    }
                    else if (axisType == 5)
                    {
                        currJoyPrim = (hardKey.controllerMap)EditorGUILayout.EnumPopup("Primary Button", currJoyPrim);
                        if (myscript.inputs[i].controllerOne != currJoyPrim)
                            myscript.inputs[i].controllerOne = currJoyPrim;
                    }

                    axisType2 = EditorGUILayout.Popup("Secondary Key Type", axisType2, axisOptions);
                    if (myscript.inputs[i].axisType2 != axisType2)
                        myscript.inputs[i].axisType2 = axisType2;

                    if (axisType2 == 0)
                    {
                        currSec = (KeyCode)EditorGUILayout.EnumPopup("Secondary Input", currSec);
                        if (myscript.inputs[i].secondaryKeycode != currSec)
                            myscript.inputs[i].secondaryKeycode = currSec;
                    }
                    else if (axisType2 == 5 || axisType2 >= 10 && axisType2 <= 13)
                    {
                        currJoySec = (hardKey.controllerMap)EditorGUILayout.EnumPopup("Secondary Button", currJoySec);
                        if (myscript.inputs[i].controllerTwo != currJoySec)
                            myscript.inputs[i].controllerTwo = currJoySec;
                    }

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Delete Key"))
                        deleteSelected(i);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Separator();
            }

            showAll = false;



            // Alternating Color Scheme
            GUIStyle colour = new GUIStyle();
            colour.normal.background = MakeTex(600, 1, new Color32(170, 36, 143, 255));

            if (!anySaved)
            {
                GUI.color = Color.green;
                if (GUILayout.Button("Make all keys saveable ingame"))
                {
                    for (int i = 0; i < myscript.inputs.Length; i++)
                    {
                        myscript.inputs[i].saveKey = true;
                    }
                }
                GUI.color = Color.white;
            }



            EditorGUILayout.BeginVertical(colour);
            // Layout myscript.inputs For key creation
            EditorGUILayout.LabelField("Create Control");
            inputName = EditorGUILayout.TextField("Key Name", inputName);
            saveable = EditorGUILayout.Toggle("Save In-Game Rebinds", saveable);
            axisSelected = EditorGUILayout.Popup("Primary Key Type", axisSelected, axisOptions);
            if (axisSelected == 0)
            {
                keyPrime = (KeyCode)EditorGUILayout.EnumPopup("Primary Key", keyPrime);
            }
            else if (axisSelected == 5)
            {
                joyPrime = (hardKey.controllerMap)EditorGUILayout.EnumPopup("Primary Button", joyPrime);
            }

            axisSelected2 = EditorGUILayout.Popup("Secondary Key Type", axisSelected2, axisOptions);

            if (axisSelected2 == 0)
            {
                keySec = (KeyCode)EditorGUILayout.EnumPopup("Secondary Key", keySec);
            }
            else if (axisSelected2 == 5)
            {
                joySec = (hardKey.controllerMap)EditorGUILayout.EnumPopup("Secondary Button", joySec);
            }


            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            //Create Input From Options
            if (GUILayout.Button("Add Input"))
                addInput();

            //Remove Last Input
            if (GUILayout.Button("Remove Last Input") && myscript.inputs.Length > 0)
                removeInput();

            EditorGUILayout.Separator();

            bool change;
            change = EditorGUILayout.Toggle("Allow controller", myscript.useController);
            if (myscript.useController != change)
            {
                myscript.useController = change;

            }

            if (myscript.useController)
            {
                try
                {
                    Input.GetAxis("DPADVER");
                    Input.GetAxis("DPADHOR");
                    Input.GetAxis("STICKLHOR");
                    Input.GetAxis("STICKRHOR");
                    Input.GetAxis("STICKLVER");
                    Input.GetAxis("STICKRVER");
                }
                catch
                {
                    myscript.useController = false;
                    Debug.LogWarning("Unity Inputs not properly configured for use with controllers.");
                    Debug.LogWarning("Please open the 'Readme.txt' for guidence. Video tutorial included.");
                }
            }

            if (myscript.useController)
            {

                myscript.controllerType = EditorGUILayout.Popup("Controller Name Stlye", myscript.controllerType, controllerTypes);
                myscript.saveControllerType = EditorGUILayout.Toggle("Save controller name style?", myscript.saveControllerType);
            }
            EditorGUILayout.Separator();

            if (GUILayout.Button("Copy Inputs"))
                UnityEditorInternal.ComponentUtility.CopyComponent(myscript);

            if (GUILayout.Button("Paste Inputs"))
                UnityEditorInternal.ComponentUtility.PasteComponentValues(myscript);

            if (GUILayout.Button("Reset bound keys"))
            {
                resetPlayerPrefs();
            }

            //if (GUILayout.Button("Apply Changes"))
            //    applyChanges();

            //if (GUILayout.Button("Load Existing"))
            //    applyChanges();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(myscript);
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.LabelField(currentVersion);
            EditorGUILayout.LabelField("Current Platform: " + SystemInfo.operatingSystem);
        }

        void resetPlayerPrefs()
        {
            for (int i = 0; i < myscript.inputs.Length; i++)
            {
                if (PlayerPrefs.HasKey("settings_bindings_" + myscript.inputs[i].keyName))
                    PlayerPrefs.DeleteKey("settings_bindings_" + myscript.inputs[i].keyName);

                if (PlayerPrefs.HasKey("settings_bindings_sec_" + myscript.inputs[i].keyName))
                    PlayerPrefs.DeleteKey("settings_bindings_sec_" + myscript.inputs[i].keyName);
            }

            if (PlayerPrefs.HasKey("settings_bindings_controller_type"))
                PlayerPrefs.DeleteKey("settings_bindings_controller_type");

            Debug.Log("All bindings reset to default values. PlayerPrefs have been removed for each key.");
        }

        void addInput()
        {
            hardManager.givenInputs[] savedInputs = new hardManager.givenInputs[] { };
            savedInputs = myscript.inputs;
            myscript.inputs = new hardManager.givenInputs[myscript.inputs.Length + 1];

            for (int i = 0; i < savedInputs.Length; i++)
            {
                myscript.inputs[i].keyName = savedInputs[i].keyName;
                myscript.inputs[i].primaryKeycode = savedInputs[i].primaryKeycode;
                myscript.inputs[i].secondaryKeycode = savedInputs[i].secondaryKeycode;
                myscript.inputs[i].axisType = savedInputs[i].axisType;
                myscript.inputs[i].axisType2 = savedInputs[i].axisType2;
                myscript.inputs[i].saveKey = savedInputs[i].saveKey;
                myscript.inputs[i].controllerOne = savedInputs[i].controllerOne;
                myscript.inputs[i].controllerTwo = savedInputs[i].controllerTwo;
            }

            myscript.inputs[myscript.inputs.Length - 1].keyName = inputName;
            myscript.inputs[myscript.inputs.Length - 1].axisType = axisSelected;
            myscript.inputs[myscript.inputs.Length - 1].axisType2 = axisSelected2;
            myscript.inputs[myscript.inputs.Length - 1].primaryKeycode = keyPrime;
            myscript.inputs[myscript.inputs.Length - 1].secondaryKeycode = keySec;
            myscript.inputs[myscript.inputs.Length - 1].controllerOne = joyPrime;
            myscript.inputs[myscript.inputs.Length - 1].controllerTwo = joySec;
            myscript.inputs[myscript.inputs.Length - 1].saveKey = saveable;
            myscript.inputs[myscript.inputs.Length - 1].controllerOne = joyPrime;
            myscript.inputs[myscript.inputs.Length - 1].controllerTwo = joySec;

            //Reset The Selection
            inputName = "";
            axisSelected = 0;
            axisSelected2 = 0;
            keyPrime = KeyCode.None;
            keySec = KeyCode.None;
            joyPrime = hardKey.controllerMap.None;
            joySec = hardKey.controllerMap.None;
            saveable = true;

        }

        void deleteSelected(int getname)
        {
            hardManager.givenInputs[] savedInputs = new hardManager.givenInputs[] { };
            savedInputs = myscript.inputs;
            myscript.inputs = new hardManager.givenInputs[myscript.inputs.Length - 1];
            int saved = 0;

            for (int i = 0; i < myscript.inputs.Length; i++)
            {
                if (saved != getname)
                {
                    myscript.inputs[i].keyName = savedInputs[saved].keyName;
                    myscript.inputs[i].primaryKeycode = savedInputs[saved].primaryKeycode;
                    myscript.inputs[i].secondaryKeycode = savedInputs[saved].secondaryKeycode;
                    myscript.inputs[i].axisType = savedInputs[saved].axisType;
                    myscript.inputs[i].axisType2 = savedInputs[saved].axisType2;
                    myscript.inputs[i].saveKey = savedInputs[saved].saveKey;
                    myscript.inputs[i].controllerOne = savedInputs[i].controllerOne;
                    myscript.inputs[i].controllerTwo = savedInputs[i].controllerTwo;
                }
                else
                {
                    saved++;
                    myscript.inputs[i].keyName = savedInputs[saved].keyName;
                    myscript.inputs[i].primaryKeycode = savedInputs[saved].primaryKeycode;
                    myscript.inputs[i].secondaryKeycode = savedInputs[saved].secondaryKeycode;
                    myscript.inputs[i].axisType = savedInputs[saved].axisType;
                    myscript.inputs[i].axisType2 = savedInputs[saved].axisType2;
                    myscript.inputs[i].saveKey = savedInputs[saved].saveKey;
                    myscript.inputs[i].controllerOne = savedInputs[saved].controllerOne;
                    myscript.inputs[i].controllerTwo = savedInputs[saved].controllerTwo;
                }
                saved++;
            }

            //Reset The Selection
            inputName = "";
            axisSelected = 0;
            keyPrime = KeyCode.None;
            keySec = KeyCode.None;
        }

        void removeInput()
        {
            hardManager.givenInputs[] savedInputs = new hardManager.givenInputs[] { };
            savedInputs = myscript.inputs;
            myscript.inputs = new hardManager.givenInputs[myscript.inputs.Length - 1];

            if (savedInputs.Length - 1 > 0)
            {

                for (int i = 0; i < savedInputs.Length - 1; i++)
                {
                    myscript.inputs[i].keyName = savedInputs[i].keyName;
                    myscript.inputs[i].primaryKeycode = savedInputs[i].primaryKeycode;
                    myscript.inputs[i].secondaryKeycode = savedInputs[i].secondaryKeycode;
                    myscript.inputs[i].axisType = savedInputs[i].axisType;
                    myscript.inputs[i].axisType2 = savedInputs[i].axisType2;
                    myscript.inputs[i].saveKey = savedInputs[i].saveKey;
                    myscript.inputs[i].controllerOne = savedInputs[i].controllerOne;
                    myscript.inputs[i].controllerTwo = savedInputs[i].controllerTwo;
                }


                //Reset The Selection
                inputName = "";
                axisSelected = 0;
                keyPrime = KeyCode.None;
                keySec = KeyCode.None;
            }
        }

    }
}
