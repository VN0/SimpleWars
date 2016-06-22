//Basic example for using CS-Script in Unity by http://www.dotmos.org
//More info can be found here: http://www.csscript.net/help/script_hosting_guideline_.html

using UnityEngine;
using System.Collections;
using CSScriptLibrary;

public class BasicCSScriptExample : MonoBehaviour {

	// Use this for initialization
	void Start () {

		//Load script and compile. 
		//.txt file ending is used instead of .cs so unity will not pre-compile the script. If you store the scripts at an external location, you can use .cs
		System.Reflection.Assembly _assembly = CSScript.Load(Application.dataPath + "/CS-Script/01 BasicExample/Scripts/BasicExampleScript.txt", null, true);

		//List all public classes inside the script
		Debug.Log("------ Public classes:");
		foreach(System.Type t in _assembly.GetExportedTypes())
			Debug.Log(t);

		Debug.Log("------ ALL classes:");
		//List ALL classes inside the script
		foreach(System.Type t in _assembly.GetTypes())
			Debug.Log(t);
		Debug.Log("----------------------");

		//Add the Unity Component from our script file: MyMonoBehaviour
		//We know that it's the first class, but you should make sure and check. You could do a simple string check as a start
		this.gameObject.AddComponent(_assembly.GetExportedTypes()[0]);



		//Call a function in SomePublicClass in BasicExampleScript
		//Create an AsmHelper for easier handling. You only need one per Assembly. Multiple classes can be instantiated with it.
		AsmHelper helper = new AsmHelper(_assembly);

		//Call static function Calc(...) and print return value
		Debug.Log( "Sum:" + (int)helper.Invoke("SomePublicClass.Sum", 1, 4) );


		//Create an instance of SomePublicClass using reflection
		IMyScript _somePublicClassInstance = (IMyScript)helper.CreateObject("SomePublicClass");
		//Call function Test() on our new instance
		_somePublicClassInstance.Test();

	}
}

//Interface for SomePublicClass in BasicExampleScript.txt
public interface IMyScript
{
	void Test();
}
