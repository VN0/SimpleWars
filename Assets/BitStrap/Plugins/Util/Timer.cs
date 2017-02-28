using UnityEngine;

namespace BitStrap
{
	/// <summary>
	/// Timer utility class. Allows you to receive a callback after a certain
	/// amount of time has elapsed.
	/// </summary>
	[System.Serializable]
	public class Timer
	{
		/// <summary>
		/// The timer's length in seconds.
		/// </summary>
		public float length = 1.0f;

		/// <summary>
		/// Callback that gets called when "length" seconds has elapsed.
		/// </summary>
		public SafeAction onTimer = new SafeAction();

		/// <summary>
		/// The timestamp used to trigger timer events.
		/// </summary>
		public static float CurrentTime
		{
			get { return Time.realtimeSinceStartup; }
		}

		private float startTime = -1.0f;

		/// <summary>
		/// The countdown time in seconds.
		/// </summary>
		public float RemainingTime
		{
			get { return startTime < 0.0f ? 0.0f : Mathf.Clamp( startTime + length - CurrentTime, 0.0f, length ); }
		}

		/// <summary>
		/// Return a 0.0 to 1.0 number where 1.0 means the timer completed and is now stopped.
		/// </summary>
		public float Progress
		{
			get { return startTime < 0.0f ? 1.0f : Mathf.InverseLerp( 0.0f, length, CurrentTime - startTime ); }
		}

		/// <summary>
		/// Is the timer countdown running?
		/// </summary>
		public bool IsRunning
		{
			get { return startTime >= 0.0f; }
		}

		public Timer()
		{
		}

		public Timer( float length )
		{
			this.length = length;
		}

		/// <summary>
		/// You need to manually call this at your script Update() method
		/// for the timer to work properly.
		/// </summary>
		public void OnUpdate()
		{
			if( startTime < 0.0f )
			{
				// Already triggered callback.
			}
			else if( CurrentTime >= startTime + length )
			{
				startTime = -1.0f;
				onTimer.Call();
			}
		}

		/// <summary>
		/// Stop the timer and its counter.
		/// </summary>
		public void Stop()
		{
			startTime = -1.0f;
		}

		/// <summary>
		/// Start the timer and play its counter.
		/// </summary>
		public void Start()
		{
			startTime = CurrentTime;
		}
	}
}