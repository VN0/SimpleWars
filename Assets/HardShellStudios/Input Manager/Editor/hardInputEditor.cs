using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(hardManager))]
public class hardInputEditor : Editor
{

    hardManager myscript;
    string currentVersion = "Current Version - 1.4 | Created by Haydn Comley - www.HardShellStudios.com";

    string inputName = "";
    KeyCode keyPrime;
    KeyCode keySec;

    string[] axisOptions = new string[] { "Mouse & Button Press", "Mouse Axis/Wheel Up", "Mouse Axis/Wheel Down", "Mouse Axis/Position X", "Mouse Axis/Position Y" };
    int axisSelected = 0;
    int axisSelected2 = 0;
    bool[] opened = new bool[0];

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

    public override void OnInspectorGUI()
    {
        //Begin

        myscript = (hardManager)target;
        bool anyOpen = false;
        for (int i = 0; i < myscript.inputs.Length; i++)
        {

            string currName = myscript.inputs[i].keyName;
            KeyCode currPrim = myscript.inputs[i].primaryKeycode;
            KeyCode currSec = myscript.inputs[i].secondaryKeycode;
            int axisType = myscript.inputs[i].axisType;
            int axisType2 = myscript.inputs[i].axisType2;
            bool[] hold = opened;
            opened = new bool[opened.Length + 1];
            for (int i2 = 0; i2 < hold.Length; i2++)
            {
                opened[i2] = hold[i2];
            }


            // Alternating Color Scheme
            Color[] colors = new Color[] { new Color(0.4f, 0.66f, 0.34f, 1f), new Color(0.35f, 0.55f, 0.65f, 1f) };
            GUIStyle style = new GUIStyle();
            style.normal.background = MakeTex(600, 1, colors[i % 2]);
            EditorGUILayout.BeginVertical(style);

            //GUIStyle styleFoldout = new GUIStyle();

            opened[i] = EditorGUILayout.Foldout(opened[i], currName);

            if (opened[i])
            {

                currName = EditorGUILayout.TextField("Name", currName);
                if (myscript.inputs[i].keyName != currName)
                    myscript.inputs[i].keyName = currName;

                axisType = EditorGUILayout.Popup("Primary Key Type", axisType, axisOptions);
                if (myscript.inputs[i].axisType != axisType)
                    myscript.inputs[i].axisType = axisType;

                if (axisType == 0)
                {

                    currPrim = (KeyCode)EditorGUILayout.EnumPopup("Primary Key", currPrim);
                    if (myscript.inputs[i].primaryKeycode != currPrim)
                        myscript.inputs[i].primaryKeycode = currPrim;
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

                EditorGUILayout.Separator();

                if (GUILayout.Button("Delete Key"))
                    deleteSelected(i);
                anyOpen = true;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
        }

        // Alternating Color Scheme
        GUIStyle colour = new GUIStyle();
        colour.normal.background = MakeTex(600, 1, new Color(0.65f, 0.35f, 0.35f, 1));

        if (anyOpen)
        {
            if (GUILayout.Button("Hide All Keys"))
            {
                for (int i = 0; i < opened.Length; i++)
                {
                    opened[i] = false;
                }
            }
        }
        else
        {
            if (GUILayout.Button("Show All Keys"))
            {
                for (int i = 0; i < opened.Length; i++)
                {
                    opened[i] = true;
                }
            }
        }

        EditorGUILayout.BeginVertical(colour);
        // Layout myscript.inputs For key creation
        EditorGUILayout.LabelField("Create Control");
        inputName = EditorGUILayout.TextField("Key Name", inputName);
        axisSelected = EditorGUILayout.Popup("Primary Key Type", axisSelected, axisOptions);
        if (axisSelected == 0)
        {
            keyPrime = (KeyCode)EditorGUILayout.EnumPopup("Primary Key", keyPrime);
        }
        axisSelected2 = EditorGUILayout.Popup("Secondary Key Type", axisSelected2, axisOptions);
        if (axisSelected2 == 0)
        {
            keySec = (KeyCode)EditorGUILayout.EnumPopup("Secondary Key", keySec);
        }


        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        //Create Input From Options
        if (GUILayout.Button("Add Input"))
            addInput();

        //Remove Last Input
        if (GUILayout.Button("Remove Input") && myscript.inputs.Length > 0)
            removeInput();

        EditorGUILayout.Separator();

        if (GUILayout.Button("Copy Inputs"))
            UnityEditorInternal.ComponentUtility.CopyComponent(myscript);

        if (GUILayout.Button("Paste Inputs"))
            UnityEditorInternal.ComponentUtility.PasteComponentValues(myscript);

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
        }

        myscript.inputs[myscript.inputs.Length - 1].keyName = inputName;
        myscript.inputs[myscript.inputs.Length - 1].axisType = axisSelected;
        myscript.inputs[myscript.inputs.Length - 1].primaryKeycode = keyPrime;
        myscript.inputs[myscript.inputs.Length - 1].secondaryKeycode = keySec;

        //Reset The Selection
        inputName = "";
        axisSelected = 0;
        keyPrime = KeyCode.None;
        keySec = KeyCode.None;

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
            }
            else
            {
                saved++;
                myscript.inputs[i].keyName = savedInputs[saved].keyName;
                myscript.inputs[i].primaryKeycode = savedInputs[saved].primaryKeycode;
                myscript.inputs[i].secondaryKeycode = savedInputs[saved].secondaryKeycode;
                myscript.inputs[i].axisType = savedInputs[saved].axisType;
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
            }


            //Reset The Selection
            inputName = "";
            axisSelected = 0;
            keyPrime = KeyCode.None;
            keySec = KeyCode.None;
        }
    }

}
