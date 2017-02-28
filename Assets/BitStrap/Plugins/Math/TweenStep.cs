using UnityEngine;

namespace BitStrap
{
	public class TweenStep : MonoBehaviour
	{
		public float duration = 1.0f;
		public AnimationCurve curve = AnimationCurve.Linear( 0.0f, 0.0f, 1.0f, 1.0f );

		public Vector3 tangent = Vector3.right;
	}
}
