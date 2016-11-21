using UnityEngine;
using UnityEngine.UI;

namespace SimpleWars
{
    [RequireComponent(typeof(Button))]
    public class ChangeSceneEvent : MonoBehaviour
    {
        public string sceneName;

        void Awake ()
        {
            GetComponent<Button>().onClick.AddListener(delegate
            {
                SceneLoader.LoadScene(sceneName);
            });
        }
    }
}