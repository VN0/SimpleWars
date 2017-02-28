using System.Text;
using UnityEngine;

namespace BitStrap.Examples
{
	public class CircularBufferExample : MonoBehaviour
	{
		[Header( "Edit the fields and click the buttons to test them!" )]
		public int value = 10;

		private CircularBuffer<int> buffer = new CircularBuffer<int>( 4 );

		[Button]
		public void Add()
		{
			if( !Application.isPlaying )
			{
				Debug.LogWarning( "In order to see CircularBuffer working, please enter Play mode." );
				return;
			}

			buffer.Add( value++ );
			Print();
		}

		[Button]
		public void Print()
		{
			if( !Application.isPlaying )
			{
				Debug.LogWarning( "In order to see CircularBuffer working, please enter Play mode." );
				return;
			}

			StringBuilder sb = new StringBuilder();
			for( int i = 0; i < buffer.Count; i++ )
			{
				sb.Append( buffer[i] );
				sb.Append( ", " );
			}

			Debug.Log( sb );
		}
	}
}
