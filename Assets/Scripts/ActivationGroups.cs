using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ActivationGroups : MonoBehaviour
{
    public string[][] steps;
    public bool ready = false;
    int currentStage = 0;
    bool firstTime = true;

    public void Awake ()
    {
        SceneManager.sceneLoaded += delegate (Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Earth" && firstTime == true)
            {
                firstTime = false;
                GameObject.Find("ActivateButton").GetComponent<Button>().onClick.AddListener(ActivateNextStage);
            }
        };
    }

    public void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space) && ready && currentStage < steps.Length)
        {
            ActivateNextStage();
        }
    }

    public void ActivateNextStage ()
    {
        foreach (string part in steps[currentStage])
        {
            try
            {
                GameObject.Find(part).GetComponent<PartFunction>().enabled = true;
            }
            catch (System.NullReferenceException) { }
        }
        currentStage += 1;
    }
}
