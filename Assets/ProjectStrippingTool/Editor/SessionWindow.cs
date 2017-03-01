using UnityEngine;
using UnityEditor;

namespace UnityEditor.ProjectStripper
{
	public class SessionWindow : EditorWindow
	{
		private Vector2 _scrollPosition;

		[MenuItem ("Window/Project Stripping")]
		public static void Init ()
		{
			GetWindow<SessionWindow> ().ShowTab();
		}

		private Editor _editor;

		public void OnEnable ()
		{
			titleContent = new GUIContent ("Project Stripping");
		}

		public void OnDisable()
		{
			if (_editor) DestroyImmediate(_editor);
		}

		public void OnGUI ()
		{
			var session = Session.DefaultSession;
			
			Editor.CreateCachedEditor(session, typeof(SessionEditor), ref _editor);

			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			_editor.DrawHeader();
	
			EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
			_editor.OnInspectorGUI();
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.EndScrollView();
		}
	}
}