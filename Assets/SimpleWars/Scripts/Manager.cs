using UnityEngine;
using EazyTools.SoundManager;
using Lean.Localization;

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
        public AudioClip bgm;

        new void Awake ()
        {
            Unbug.Log(Application.persistentDataPath);
            RandomSeed = SystemInfo.deviceUniqueIdentifier.GetHashCode();
            SoundManager.PlayMusic(bgm, 0.2f, true, true, 0, 0);
            LeanLocalization.CurrentLanguage = Application.systemLanguage.ToString();
        }

        private void Start()
        {
            SceneLoader.LoadScene("Menu", callback: () => { SceneLoader.instance.fadeDuration = 0.5f; });
        }
    }
}