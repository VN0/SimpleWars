using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.ProjectStripper
{

	[CustomEditor (typeof(Session))]
	public class SessionEditor : Editor
	{

		private SerializedProperty m_BasePath;
		private SerializedProperty m_Steps;

		private class Styles
		{
			public GUIStyle wrappedLabel;
			public GUIStyle richTextLabel;

			public Styles ()
			{
				wrappedLabel = new GUIStyle (GUI.skin.label);
				wrappedLabel.wordWrap = true;
				richTextLabel = new GUIStyle (GUI.skin.label);
				richTextLabel.richText = true;
			}
		}

		private Styles m_Styles;

		private string m_BackupAreaSize;
		private int m_BackupAreaNumSteps;

		private ReorderableList m_StepList;

		private Dictionary<Object, long> m_TopLevelDirectorySizes;
		private GUIContent m_AssetsFolderSize;

		private long GetDirectorySize (System.IO.DirectoryInfo di)
		{
			if (!di.Exists)
				return 0;

			long result = 0;
			foreach (var fi in di.GetFiles())
				result += fi.Length;
			foreach (var cdi in di.GetDirectories())
				result += GetDirectorySize (cdi);
			return result;
		}

		private void RefreshBackupAreaSize ()
		{
			m_BackupAreaSize = string.Format ("Current stripped files: <b>{0}</b>", EditorUtility.FormatBytes (GetDirectorySize (new System.IO.DirectoryInfo (m_BasePath.stringValue))));
			m_BackupAreaNumSteps = m_Steps.arraySize;
		}

		private void RefreshTopLevelDirectorySizes ()
		{	
			m_TopLevelDirectorySizes = new Dictionary<Object, long> ();
			var assets = new System.IO.DirectoryInfo ("Assets");
			foreach (var child in assets.GetDirectories()) {
				m_TopLevelDirectorySizes.Add (AssetDatabase.LoadMainAssetAtPath ("Assets/" + child.Name), GetDirectorySize (child));
			}
		}

		private void RefreshAssetsFolderSize ()
		{
			m_AssetsFolderSize = new GUIContent (string.Format ("Total Assets Folder size: <b>{0}</b>", EditorUtility.FormatBytes (GetDirectorySize (new System.IO.DirectoryInfo ("Assets")))));
		}

		public void OnEnable ()
		{
			m_BasePath = serializedObject.FindProperty ("_backupAreaRoot");
			m_Steps = serializedObject.FindProperty ("_steps");

			m_StepList = new ReorderableList (serializedObject, m_Steps);
			m_StepList.displayAdd = false;
			m_StepList.displayRemove = false;
			m_StepList.draggable = false;
			m_StepList.drawHeaderCallback = DrawStepListHeader;
			m_StepList.drawElementCallback = DrawStepListElement;

			RefreshBackupAreaSize ();
			RefreshTopLevelDirectorySizes ();
			RefreshAssetsFolderSize ();
		}

		private void DrawStepListHeader (Rect rc)
		{
			EditorGUI.LabelField (rc, "Stripping steps performed");
		}

		private void DrawStepListElement (Rect rc, int index, bool isActive, bool isFocused)
		{
			var step = m_Steps.GetArrayElementAtIndex (index);
			var rootPath = step.FindPropertyRelative ("rootPath");
			var operation = (StrippingOperationType)step.FindPropertyRelative ("operation").enumValueIndex;

			string formatString;

			switch (operation) {
			case StrippingOperationType.StripAll:
				formatString = "Strip {0}";
				break;
			case StrippingOperationType.StripAudio:
				formatString = "Strip audio clips from {0}";
				break;
			case StrippingOperationType.StripMaterials:
				formatString = "Strip materials from {0}";
				break;
			case StrippingOperationType.StripTextures:
				formatString = "Strip textures from {0}";
				break;
			case StrippingOperationType.StripModels:
				formatString = "Strip models from {0}";
				break;
			case StrippingOperationType.StripArtNotInSceneView:
				formatString = "Strip hidden art from {0}";
				break;
			case StrippingOperationType.StripArtNotInLoadedScenes:
				formatString = "Strip unused art from {0}";
				break;
			default:
				formatString = "Do unknown operation to {0}";
				break;
			}

			GUI.Label (rc, string.Format (formatString, rootPath.stringValue));
		}

		void DoStrippingAreaGUI ()
		{
			GUILayout.BeginVertical ("box");
			GUILayout.Label ("Stripped files", EditorStyles.boldLabel);
			GUILayout.Label ("When you strip files from the project, they are not deleted, but moved to a temporary location, and will be automatically cleaned up by your OS at a later time.", m_Styles.wrappedLabel);
			GUILayout.BeginHorizontal ();
			GUILayout.Label (m_BackupAreaSize, m_Styles.richTextLabel, GUILayout.ExpandWidth (true));
			if (GUILayout.Button ("Refresh"))
				RefreshBackupAreaSize ();
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("Browse stripped files")) {
				EditorUtility.RevealInFinder (m_BasePath.stringValue);
			}
			if (GUILayout.Button ("Delete stripped files")) {
				if (EditorUtility.DisplayDialog ("Are you sure?", "If you delete the stripped files, you will not be able to reverse any of the stripping operations performed up to this point.", "No, don't delete", "Yes, I'm sure"))
					return;
				if (System.IO.Directory.Exists (m_BasePath.stringValue))
					FileUtil.DeleteFileOrDirectory (m_BasePath.stringValue);
				m_Steps.ClearArray ();
			}
			GUILayout.EndVertical ();
		}

		void DoProjectSizeInfo ()
		{
			GUILayout.Label ("Largest top-level folders", EditorStyles.boldLabel);
			foreach (var kvp in m_TopLevelDirectorySizes.OrderByDescending (v => v.Value).Take (10)) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (kvp.Key.name, EditorStyles.label))
					EditorGUIUtility.PingObject (kvp.Key);
				GUILayout.Label (EditorUtility.FormatBytes (kvp.Value), GUILayout.Width (70));
				GUILayout.EndHorizontal ();
			}
			GUILayout.Space (10);
			GUILayout.Label (m_AssetsFolderSize, m_Styles.richTextLabel);
		}

		void DoStrippingSteps ()
		{
			m_StepList.DoLayoutList ();
			GUILayout.BeginHorizontal ();
			bool hasSelectedFile = (m_StepList.index >= 0 && m_StepList.index < m_Steps.arraySize);
			EditorGUI.BeginDisabledGroup (!hasSelectedFile);
			var fileList = hasSelectedFile ? m_Steps.GetArrayElementAtIndex (m_StepList.index).FindPropertyRelative ("files") : null;
			bool showFileList = hasSelectedFile && fileList.isExpanded;
			showFileList = EditorGUILayout.Foldout (showFileList, "Affected files");
			if (hasSelectedFile)
				fileList.isExpanded = showFileList;
			EditorGUI.EndDisabledGroup ();
			EditorGUI.BeginDisabledGroup (m_Steps.arraySize == 0);
			if (GUILayout.Button (hasSelectedFile ? "Undo selected operation" : "Undo last operation")) {
				if (hasSelectedFile && m_StepList.index < m_Steps.arraySize - 1 && !EditorUtility.DisplayDialog ("Undo stripping", "This will undo the selected step as well as all the steps after it.", "OK, do it", "Cancel"))
					return;
				int stepsToUndo = hasSelectedFile ? m_Steps.arraySize - m_StepList.index : 1;
				try {
					AssetDatabase.StartAssetEditing ();
					for (int i = 0; i < stepsToUndo; ++i)
						Session.DefaultSession.UndoLastOperation ();
				}
				finally {
					AssetDatabase.StopAssetEditing ();
					AssetDatabase.Refresh ();
				}
			}
			EditorGUI.EndDisabledGroup ();
			GUILayout.EndHorizontal ();
			if (showFileList) {
				GUILayout.BeginHorizontal ();
				GUILayout.Space (10);
				GUILayout.BeginVertical ();
				if (fileList.arraySize == 0) {
					GUILayout.Label ("No files affected");
				}
				else {
					fileList = fileList.GetArrayElementAtIndex (0);
					do {
						GUILayout.Label (fileList.stringValue);
					}
					while (fileList.Next (false));
				}
				GUILayout.EndVertical ();
				GUILayout.EndHorizontal ();
			}
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			if (m_Styles == null)
				m_Styles = new Styles ();

			if (m_BackupAreaNumSteps != m_Steps.arraySize) {
				RefreshTopLevelDirectorySizes ();
				RefreshBackupAreaSize ();
				RefreshAssetsFolderSize ();
			}

			DoStrippingSteps ();

			GUILayout.Space (20);

			DoProjectSizeInfo ();

			GUILayout.Space (20);

			DoStrippingAreaGUI ();

			serializedObject.ApplyModifiedProperties ();
		}
	}
}