using UnityEditor;

namespace BitStrap.Examples
{
	/// <summary>
	/// Open this window by navigating in Unity Editor to "Window/BitStrap Examples/Extensions/AllEditorStyles".
	/// </summary>
	public class AllEditorStylesExample : EditorWindow
	{
		[MenuItem( "Window/BitStrap Examples/Helpers/AllEditorStyles" )]
		public static void CreateWindow()
		{
			GetWindow<AllEditorStylesExample>().Show();
		}

		private void OnGUI()
		{
			EditorHelper.DrawAllStyles();
		}
	}
}
