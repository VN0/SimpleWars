using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


namespace HardShellStudios.InputManager
{
    ///<summary>
    ///Custom editor inspector script that actions on the hardUI script.
    ///</summary>
    [CustomEditor(typeof(hardInputUI))]
    public class hardUIEditor : Editor
    {
        hardInputUI parentScript;
        hardManager manager;

        string[] uivalues = new string[] { "Rebinding Button", "Reset Button", "Reset All Button" };

        public override void OnInspectorGUI()
        {
            parentScript = (hardInputUI)target;

            parentScript.buttonAction = EditorGUILayout.Popup("Hard UI Type", parentScript.buttonAction, uivalues);

            if (parentScript.buttonAction == 0)
            {
                // Display text
                if (parentScript.displayText == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.green;

                parentScript.displayText = (Text)EditorGUILayout.ObjectField("UI Text Object", parentScript.displayText, typeof(Text), true);
                GUI.color = Color.white;
                // End
            }


            if (parentScript.buttonAction == 0 || parentScript.buttonAction == 1)
            {
                // Display text
                if (parentScript.keyName == "")
                    GUI.color = Color.red;
                else if (nameExists(parentScript.keyName))
                    GUI.color = Color.green;
                else
                    GUI.color = Color.yellow;

                parentScript.keyName = EditorGUILayout.TextField("Key Name", parentScript.keyName);
                GUI.color = Color.white;
                // End
            }

            if (parentScript.buttonAction == 0)
            {
                parentScript.useSecondary = EditorGUILayout.Toggle("Target Secondary Key", parentScript.useSecondary);
            }

        }

        bool nameExists(string namer)
        {
            manager = FindObjectOfType<hardManager>();

            for (int i = 0; i < manager.inputs.Length; i++)
            {
                if (manager.inputs[i].keyName == namer)
                {
                    return true;
                }
            }

            return false;
        }
    }
}