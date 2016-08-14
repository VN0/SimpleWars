using UnityEngine;
using System.Collections;

public class Player_Look : MonoBehaviour
{

    public bool inverted;
    public float speedX;
    public float speedY;
    float xrot;
    float yrot;
	
	// Update is called once per frame
	void Update () 
	{
        transform.parent.eulerAngles += (new Vector3(0, hardInput.GetAxis("MouseX", "MouseX", 1), 0) * speedX);
        xrot = transform.eulerAngles.y;

        if (inverted)
            yrot = Mathf.Clamp(yrot + hardInput.GetAxis("MouseY", "MouseY", 1) * speedY, -80, 60);
        else
            yrot = Mathf.Clamp(yrot + -hardInput.GetAxis("MouseY", "MouseY", 1) * speedY, -80, 60);


        transform.rotation = Quaternion.Euler(yrot, xrot, 0);
    }
}
