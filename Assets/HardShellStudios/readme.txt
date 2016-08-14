Coded by Haydn Comley for Hard Shell Studios. Anyone can use free of charge for whatever you want.
Online video tutorial can be found at: https://www.youtube.com/watch?v=A2ncsHosqa0 

To get setup simply drag in the "InputManager" prefab into every scene that requires the custom inputs.
use the inspector to setup default inputs with the custom bindings.

You can simply convert unity inputs to hard shell inputs like the following.

Input.GetButton("Forward")		=	 hardInput.GetKey("Forward")
Input.GetButtonDown("Jump")		=	 hardInput.GetKeyDown("Jump")
Input.GetAxis("Horizontal")		=	 hardInput.GetAxis("Forward", "Backward", 1)
