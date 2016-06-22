//Referencing example for using CS-Script in Unity by http://www.dotmos.org
//More info can be found here: http://www.csscript.net/help/script_hosting_guideline_.html
using UnityEngine;
using System.Collections;
using CSScriptLibrary;

public class AdvancedCSScriptExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Wa have two script files now. ClassA.txt and ClassB.txt. ClassA has a reference to ClassB and ClassB has a reference to ClassA. This is a problem as we can only load one script at a time.
		//To fix this, we have to use special instruction in ClassA.txt. (See ClassA.txt for more info)

		//Tip: If you are lazy like me, you can tell CSScript to look for scripts in a specific folder. Now you don't have to type in the full path anymore.
		//Have a look at "Assembly and Script Probing scenarios" located at http://www.csscript.net/help/script_hosting_guideline_.html for more (important!) info on this
		CSScript.GlobalSettings.AddSearchDir( System.IO.Path.GetFullPath(Application.dataPath + "/CS-Script/02 AdvancedExample/Scripts") );
		CSScript.AssemblyResolvingEnabled = true; 

		//Load ClassA script and compile. ClassB will automatically be compiled by CS-Script, as ClassA references it (See ClassA.cs for more info)
		System.Reflection.Assembly _assembly = CSScript.Load("ClassA.txt");

		//Create instance of ClassA
		AsmHelper _helper = new AsmHelper(_assembly);
		_helper.CreateObject("ClassA");
	}

}
