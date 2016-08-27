using UnityEngine;
using UnityEngine.SceneManagement;

public class PartType : MonoBehaviour
{
    public string type;

    void Awake ()
    {
        SceneManager.sceneUnloaded += delegate (Scene scene)
        {
            Destroy(this);
        };
    }
}
