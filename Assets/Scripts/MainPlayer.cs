using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPlayer : MonoBehaviour {

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return) && SceneManager.GetActiveScene().buildIndex == 1)
        {
            SceneManager.LoadScene(0);
            /*foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
            {
                Destroy(o);
            }*/
            Destroy(GameObject.Find("Rocket"));
        }
    }
}