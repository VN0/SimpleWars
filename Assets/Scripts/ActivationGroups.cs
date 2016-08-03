using UnityEngine;

public class ActivationGroups : MonoBehaviour
{
    public string[][] steps;
    public bool ready = false;
    int currentStage = 0;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && ready && currentStage<steps.Length)
        {
            foreach(string part in steps[currentStage])
            {
                try
                {
                    GameObject.Find(part).GetComponent<PartFunction>().enabled = true;
                } catch (System.NullReferenceException){ }
            }
            currentStage += 1;
        }
    }
}
