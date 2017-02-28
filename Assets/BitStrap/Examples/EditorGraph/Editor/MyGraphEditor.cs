using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace BitStrap.Examples
{
	/// <summary>
	/// Open the graph editor window by navigating in Unity Editor to "Window/BitStrap Examples/EditorGraph".
	/// </summary>
	public class MyGraphController : EditorGraphController
	{
		public MyGraph target;

		public override void OnNodeRemoved( EditorGraphNode node )
		{
			if( target == null )
				return;

			target.nodes.Remove( node.Data as MyGraphNode );
			EditorUtility.SetDirty( target );
		}

		public override void OnNodeChanged( EditorGraphNode node )
		{
			if( target == null )
				return;

			EditorUtility.SetDirty( target );
		}

		public override void OnCopiedNodes( object[] data )
		{
			if( target == null )
				return;

			Undo.RecordObject( target, "EditorGraph.CopyNodes" );
			int offset = 64;

			foreach( object d in data )
			{
				var node = d as MyGraphNode;
				if( node == null )
					continue;

				node.x += offset;
				node.y += offset;

				target.nodes.Add( node );
			}

			EditorUtility.SetDirty( target );
			UpdateGraph();
		}

		protected override void OnCreateGraph()
		{
			if( target == null )
				return;

			MapNodeType<MyGraphNode, MyGraphNodeController>();

			foreach( var node in target.nodes )
				AddNode( node );
		}

		protected override void OnToolbarGUI()
		{
			if( GUILayout.Button( "Add Node", EditorStyles.toolbarButton ) )
			{
				Undo.RecordObject( target, "EditorGraph.AddNode" );

				target.nodes.Add( new MyGraphNode() );
				EditorUtility.SetDirty( target );
				UpdateGraph();
			}

			GUILayout.FlexibleSpace();

			EditorGUI.BeginChangeCheck();
			target = EditorGUILayout.ObjectField( target, typeof( MyGraph ), false ) as MyGraph;
			if( EditorGUI.EndChangeCheck() )
			{
				UpdateGraph();
				CenterGraph();
			}
		}
	}

	/// <summary>
	/// Open the graph editor window by navigating in Unity Editor to "Window/BitStrap Examples/EditorGraph".
	/// </summary>
	public class MyGraphEditor : EditorWindow
	{
		public MyGraphController controller = new MyGraphController();

		[MenuItem("Window/BitStrap Examples/EditorGraph")]
		public static void ShowWindow()
		{
			var window = GetWindow<MyGraphEditor>("MyGraphEditor");
			window.Show();
		}

		private void OnGUI()
		{
			if (controller.target != null)
				Undo.RecordObject(controller.target, "EditGraph");

			controller.OnGUI(this);
		}

		/// <summary>
		/// Add the possibility to open the asset by just double clicking a my graph scriptable object
		/// </summary>
		/// <returns>True if the clicked item was of the type <see cref="MyGraph"/></returns>
		[OnOpenAsset(10)]
		public static bool OnOpenGraphAsset(int instanceID, int line)
		{
			UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);
			System.Type type = obj.GetType();
			if (type == typeof(MyGraph))
			{
				var window = GetWindow<MyGraphEditor>("MyGraphEditor");
				window.controller.target = obj as MyGraph;
				window.Show();
				return true;
			}
			return false;
		}
	}
}
