using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Manager : MonoBehaviour
{
    public GameObject menu;
	
	// Update is called once per frame
	void Update ()
    {
        if (hardInput.GetKeyDown("Menu"))
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
}
