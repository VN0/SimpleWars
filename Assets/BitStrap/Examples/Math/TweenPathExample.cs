using UnityEngine;

namespace BitStrap.Examples
{
	public class TweenPathExample : MonoBehaviour
	{
		[Header( "Click play and press 'Enter' to make the box lerp the path." )]
		[Header( "Also, try editing 'TweenPath/StepX's trasnform and tangent." )]
		public TweenPath tweenPath;

		public KeyCode moveToNextStep = KeyCode.Return;
		public KeyCode resetToFirstStep = KeyCode.Escape;

		private int currentIndex = 0;
		private bool canAdvance = true;

		private void Awake()
		{
			tweenPath.onFinish.Register( OnFinishTween );
		}

		private void Update()
		{
			if( canAdvance && Input.GetKeyDown( moveToNextStep ) )
			{
				if( currentIndex < tweenPath.transform.childCount - 1 )
					MovePosition();
				else
					ResetPosition();
			}

			if( Input.GetKeyDown( resetToFirstStep ) )
				ResetPosition();
		}

		private void MovePosition()
		{
			tweenPath.PlayForward( currentIndex );
			currentIndex++;
			canAdvance = false;
		}

		private void ResetPosition()
		{
			currentIndex = 0;
			tweenPath.SampleAt( currentIndex, 0.0f );
			canAdvance = true;
		}

		private void OnFinishTween()
		{
			canAdvance = true;
		}
	}
}
