using UnityEngine;

namespace SimpleWars
{
    public class Manager : Singleton<Manager>
    {
        protected Manager () { }

        protected int _randomSeed = 0;
        public int RandomSeed
        {
            get
            {
                return _randomSeed;
            }
            set
            {
                _randomSeed = value;
                Random.InitState(value);
            }
        }

        public bool modEnabled = false;

        new void Awake ()
        {
            Unbug.Log(Application.persistentDataPath);
            RandomSeed = SystemInfo.deviceUniqueIdentifier.GetHashCode();
        }

        private void Start()
        {
            SceneLoader.LoadScene("Menu", callback: () => { SceneLoader.instance.fadeDuration = 0.5f; });
        }
    }
}