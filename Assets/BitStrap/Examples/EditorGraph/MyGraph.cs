using System.Collections.Generic;
using UnityEngine;

namespace BitStrap.Examples
{
	/// <summary>
	/// Open the graph editor window by navigating in Unity Editor to "Window/BitStrap Examples/EditorGraph".
	/// </summary>
	public class MyGraph : ScriptableObject, ISerializationCallbackReceiver
	{
		public List<MyGraphNode> nodes = new List<MyGraphNode>();

		[SerializeField]
		[HideInInspector]
		private string serialized;

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			nodes = EditorGraphSerializer.Deserialize<List<MyGraphNode>>( serialized );
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			serialized = EditorGraphSerializer.Serialize( nodes );
		}
	}
}
