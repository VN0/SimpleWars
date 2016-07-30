using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public string scene;

    void Start () {
        GetComponent<Button>().onClick.AddListener(delegate
        {
            SceneManager.LoadScene(scene);
        });
    }
}
