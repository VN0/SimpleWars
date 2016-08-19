Coded by Haydn Comley for Hard Shell Studios. Anyone can use free of charge for whatever you want.
Online video tutorial can be found at: https://www.youtube.com/watch?v=A2ncsHosqa0 

To get setup simply drag in the "InputManager" prefab into every scene that requires the custom inputs.
use the inspector to setup default inputs with the custom bindings.

You can simply convert unity inputs to hard shell inputs like the following.

Input.GetButton("Forward")		=	 hardInput.GetKey("Forward")
Input.GetButtonDown("Jump")		=	 hardInput.GetKeyDown("Jump")
Input.GetAxis("Horizontal")		=	 hardInput.GetAxis("Forward", "Backward", 1)
Input.GetAxis("Horizontal")		=	 hardInput.GetAxis("Forward", 1)


//	INFORMATION REGUARDING CONTROLLER INPUTS.
	Due to Untiy's, and my own limitations to get controller inputs to work you must first add some items into the default Unity Input Manager.
	I comprehensive guide can be found here https://www.youtube.com/watch where I show and explain how to get things setup.

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