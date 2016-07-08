using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPlayer : MonoBehaviour {

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return) && SceneManager.GetActiveScene().name != "Builder")
        {
            SceneManager.LoadScene("Builder");
            /*foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
            {
                Destroy(o);
            }*/
            Destroy(GameObject.Find("Rocket"));
        }
    }
}