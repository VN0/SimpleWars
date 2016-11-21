using UnityEngine;

namespace SimpleWars
{
    public class Manager : Singleton<Manager>
    {
        protected Manager () { }

        public bool modEnabled = false;

        new void Awake ()
        {
            print(Application.persistentDataPath);
        }

        void Start ()
        {
            StartCoroutine(Coroutines.ExecuteAfter(2, delegate
            {
                SceneLoader.LoadScene("Menu", callback: delegate
                {
                    SceneLoader.instance.fadeDuration = 0.5f;
                });
            }));
        }
    }
}