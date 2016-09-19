Coded by Haydn Comley for Hard Shell Studios. Anyone can use free of charge for whatever you want.

Main tutorial:			https://www.youtube.com/watch?v=A2ncsHosqa0 
Controller tutorial:	https://www.youtube.com/watch?v=uvNXGMyjldg
New Features:			https://www.youtube.com/watch?v=sIG1v7H7Bew

// Quick Start //

To get setup simply drag in the "InputManager" prefab into every scene that requires the custom inputs.
use the inspector to setup default inputs with the custom bindings.

You can simply convert unity inputs to hard shell inputs like the following.

Input.GetButton("Forward")		=	 hardInput.GetKey("Forward")
Input.GetButtonDown("Jump")		=	 hardInput.GetKeyDown("Jump")
Input.GetAxis("Horizontal")		=	 hardInput.GetAxis("Forward", "Backward", 1)
Input.GetAxis("Horizontal")		=	 hardInput.GetAxis("Forward", 1)



// Functions Help //

hardInput.	

	GetKey(string)						Returns true when a key is being held down.					E.g. When someone holds the W key down to walk forward.
	GetKeyDown(string)					Returns true when a key is pressed once.					E.g. When the spacebar is pressed to jump.
	GetAxis(string, gravity)			Returns float of the specified axis.						E.g. Used for a preselected axis on the prefab. Like MouseX, MouseY, Controller Stick etc.		
	GetAxis(string, string, gravity)	Returns float of the specified axis between two keys.		E.g. Used for an axis between a forward and backwards key.	

	GetKeyName(string, bool)			Returns string of the key currnetly bound to an input.		E.g. Would return "W" in the example scene if you asked about the "Forward" key.
	GetKeyCode(string, bool)			Returns KeyCode of ths specified key.						E.g. Is helpful for seeing what KeyCode is bound to a certain key. 
	ForceRebind(string, bool, keycode)	Force rebinds the key specified.							E.g. Useful for rebinding keys when if say a player selects a binding profile in your game.

	MouseLock(bool)						Will either lock or unlock the mouse.						E.g. Used for locking the mouse to the center of the screen, or unlocking for menu use.
	MouseVisible(bool)					Will either make the mouse visible or hidden.				E.g. Hide mouse when moving player but then show for menu use.

	SetControllerType(controllerType)	Sets the desired naming scheme for controller inputs.		E.g. Use in game to let players choose whether to see controller inputs in the stlye of playstation or xbox.
	GetControllerType()					Returns controllerType of current scheme.					E.g. Use to see what controller scheme is currently being used.
	GetControllerTypeIndex()			Returns int of the current controller type.					E.g. Use to get the controller type index, can be used for cycling through all controller inputs like in the example scene.
	  
	ResetAllBindings()					Simply just resets all bindings to default values.			E.g. Good for use in your game attached to a button that resets input to default.
	ResetBinding(string)				Resets the current binding for the specific input.			E.g. Use for resetting a specific key to default values.



//	INFORMATION REGUARDING CONTROLLER INPUTS //

	Due to Untiy's, and my own limitations to get controller inputs to work you must first add some items into the default Unity Input Manager.
	I comprehensive guide can be found here https://www.youtube.com/watch?v=uvNXGMyjldg where I show and explain how to get things setup.

	If you dont want to or cant watch the video then here is a list of items that NEED to be added to the unity manager.
	
	Step 1) Simply change the number at the top where it says "Size" to 24.
	Step 2) Add the following inputs in any order but with the settings shown.
		

	// D-Pad Buttons

		Name:			DPADHOR
		Gravity:		1000
		Dead:			0.001
		Sensitivity:	1000
		Type:			Joystick Axis
		Axis:			7th
		Joy Num:		Joystick 1

		Name:			DPADVER
		Gravity:		1000
		Dead:			0.001
		Sensitivity:	1000
		Type:			Joystick Axis
		Axis:			8th
		Joy Num:		Joystick 1

	// Analouge Sticks

		Name:			STICKLHOR
		Gravity:		1
		Dead:			0.2
		Sensitivity:	0.5
		Type:			Joystick Axis
		Axis:			3rd
		Joy Num:		Joystick 1

		Name:			STICKLVER
		Gravity:		1
		Dead:			0.2
		Sensitivity:	0.5
		Invert:			True (Personal preference but I reccomend it)
		Type:			Joystick Axis
		Axis:			6th
		Joy Num:		Joystick 1

		Name:			STICKRHOR
		Gravity:		1
		Dead:			0.2
		Sensitivity:	0.5
		Type:			Joystick Axis
		Axis:			X axis
		Joy Num:		Joystick 1

		Name:			STICKRVER
		Gravity:		1
		Dead:			0.2
		Sensitivity:	0.5
		Invert:			True (Personal preference but I reccomend it)
		Type:			Joystick Axis
		Axis:			Y axis
		Joy Num:		Joystick 1

		If done correctly it should look something like this: http://puu.sh/qyENE/17aff089da.png
		Lastly goto your "InputManager" object ingame and check the box called "Allow Controller".
		Note aswell that the option for "Controller Name Style" just changes the names of keys shown in game when rebound, not in the editor.