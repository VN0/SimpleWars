using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Hard Shell Studios/Examples/UI Manager")]
public class UI_Manager : MonoBehaviour
{
    public GameObject menu;
    int num = 0;

    void Start()
    {
        num = hardInput.GetControllerTypeIndex();
    }


	// Update is called once per frame
	void Update ()
    {
        if (hardInput.GetKeyDown("Menu"))
        {
            if (menu.activeSelf)
            {
                menu.SetActive(false);
                hardInput.MouseLock(true);
                hardInput.MouseVisble(false);
            }
            else
            {
                menu.SetActive(true);
                hardInput.MouseLock(false);
                hardInput.MouseVisble(true);
            }
        }

        if (hardInput.GetKeyDown("CycleModes"))
        {
            num++;
            if (num == 4)
                num = 0; 

            if(num == 0)
                hardInput.SetControllerType(hardInput.controllerType.PS3);
            else if (num == 1)
                hardInput.SetControllerType(hardInput.controllerType.PS4);
            else if (num == 2)
                hardInput.SetControllerType(hardInput.controllerType.XBOX1);
            else if (num == 3)
                hardInput.SetControllerType(hardInput.controllerType.XBOX360);

        }


    }
}
