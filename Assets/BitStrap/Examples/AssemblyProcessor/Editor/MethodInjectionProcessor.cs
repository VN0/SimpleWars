using Mono.Cecil;
using Mono.Cecil.Cil;

namespace BitStrap.Examples
{
	public class MethodInjectionProcessor : AssemblyProcessor
	{
		// Defines which methods will be processed by this processor.
		protected override System.Type MethodFilterAttribute { get { return typeof( MethodInjectionExample.SubstituteThisAttribute ); } }

		// Process/change the method's C# IL code using the ILProcessor.
		protected override void OnProcessMethod()
		{
			MethodDefinition.Body.Instructions.Clear();

			// string[] args;
			MethodDefinition.Body.Variables.Add( CreateLocalVar<string[]>() );
			// object[] values;
			MethodDefinition.Body.Variables.Add( CreateLocalVar<object[]>() );

			// args = new string[params.Count];
			ILProcessor.Emit( OpCodes.Ldc_I4, MethodDefinition.Parameters.Count );
			TypeReference stringTypeReference = GetTypeReference<string>();
			ILProcessor.Emit( OpCodes.Newarr, stringTypeReference );
			ILProcessor.Emit( OpCodes.Stloc_0 );

			for( int i = 0; i < MethodDefinition.Parameters.Count; i++ )
			{
				// args[i] = "params[i].Name";
				ILProcessor.Emit( OpCodes.Ldloc_0 );
				ILProcessor.Emit( OpCodes.Ldc_I4, i );
				ILProcessor.Emit( OpCodes.Ldstr, MethodDefinition.Parameters[i].Name );
				ILProcessor.Emit( OpCodes.Stelem_Ref );
			}

			// values = new object[params.Count];
			ILProcessor.Emit( OpCodes.Ldc_I4, MethodDefinition.Parameters.Count );
			TypeReference objectTypeReference = GetTypeReference<object>();
			ILProcessor.Emit( OpCodes.Newarr, objectTypeReference );
			ILProcessor.Emit( OpCodes.Stloc_1 );

			for( int i = 0; i < MethodDefinition.Parameters.Count; i++ )
			{
				// values[i] = "params[i].Value";
				ILProcessor.Emit( OpCodes.Ldloc_1 );
				ILProcessor.Emit( OpCodes.Ldc_I4, i );
				ILProcessor.Emit( OpCodes.Ldarg, MethodDefinition.IsStatic ? i : i + 1 );

				var parameterType = MethodDefinition.Parameters[i].ParameterType;
				if( parameterType.IsValueType )
					ILProcessor.Emit( OpCodes.Box, parameterType );

				ILProcessor.Emit( OpCodes.Stelem_Ref );
			}

			// Test.TestCode.AnotherMethod( args, values );
			ILProcessor.Emit( OpCodes.Ldloc_0 );
			ILProcessor.Emit( OpCodes.Ldloc_1 );
			CallMethod( () => InjectionCode.InjectedMethod( null, null ) );

			ILProcessor.Emit( OpCodes.Ret );
		}
	}
}
