using UnityEngine;

public class ActivationGroups : MonoBehaviour
{
    public string[][] steps;
    public bool ready = false;
    int currentStage = 0;


    public void Update()
    {
        if(ready && Input.GetKeyDown(KeyCode.Space))
        {
            foreach(string part in steps[currentStage])
            {
                try
                {
                    GameObject.Find(part).GetComponent<PartFunction>().enabled = true;
                } catch { }
            }
            currentStage += 1;
        }
    }
}
