using UnityEngine;

namespace BitStrap.Examples
{
	public static class InjectionCode
	{
		// A call to this method will be injected to "MethodInjectionExample.MethodToBeProcessed()".
		public static void InjectedMethod( string[] args, object[] values )
		{
			Debug.Log( "THIS IS AN INJECTED CODE!" );
			Debug.Log( "PROCESSED METHOD PARAM NAMES: " + args.ToStringFull() );
			Debug.Log( "PROCESSED METHOD PARAM VALUES: " + values.ToStringFull() );
		}
	}

	public class MethodInjectionExample : MonoBehaviour
	{
		// Defines an Attribute to mark a method to be processed by SubstituteMethodTest.
		[System.AttributeUsage( System.AttributeTargets.Method )]
		public class SubstituteThisAttribute : System.Attribute { }

		// Calls the processed method.
		[Button]
		public void CallProcessedMethod()
		{
			Debug.Log( "Calling processed method." );
			Debug.Log( "Do not forget to enable Assembly Processing in Edit/Preferences/BitStrap!" );
			MethodToBeProcessed( 17, "text" );
		}

		[SubstituteThis]
		public void MethodToBeProcessed( int integerArg, string stringArg )
		{
			Debug.Log( "THIS IS THE DEFAULT METHOD BODY" );
		}
	}
}
