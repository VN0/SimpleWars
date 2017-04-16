using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using Lean.Localization;
using MarkLight;
using UnityEngine;
using DG.DeAudio;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace SimpleWars
{
    public class Manager : Singleton<Manager>
    {
        protected Manager () { }

        public static Dictionary<string, GameObject> parts;

        protected int _randomSeed;
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
        public float sceneFadeDuration = 1;
        public GameObject audioManager;
        public AudioClip[] ambient;
        public DeAudioSource source;
        public bool fadeAmbient = true;

        System.Random rand;
        bool waiting = false;
        int currentIndex;

        public static FileInfo[] GetSaves ()
        {
            return GetSaves(Path.Combine(Application.persistentDataPath, "Vehicles"));
        }

        public static FileInfo[] GetSaves (string path)
        {
            var dirinfo = Directory.CreateDirectory(path);
            return dirinfo.GetFiles();
        }

        public DeAudioSource PlayAmbient (int? index = null)
        {
            currentIndex = index ?? Mathf.Clamp(currentIndex - 1, rand.Next(0, ambient.Length), currentIndex + 1);
            var audio = new DeAudioClipData(DeAudioGroupId.Ambient, true);
            audio.clip = ambient[currentIndex];
            var audioSource = DeAudioManager.Play(audio);
            return audioSource;
        }

        protected override void Initialize ()
        {
            RenderSettings.fog = false;
            rand = new System.Random();
            Unbug.Log(Application.persistentDataPath);
            RandomSeed = SystemInfo.deviceUniqueIdentifier.GetHashCode();
            Instantiate(audioManager);
            LeanLocalization.CurrentLanguage = Application.systemLanguage.ToString();
            string iso = LanguageHelper.Get2LetterISOCodeFromSystemLanguage();
            ResourceDictionary.SetConfiguration(iso);
            ResourceDictionary.Language = iso;
            ResourceDictionary.NotifyObservers(true);

            var v = new Vehicle();
            v.globalData.Add("foo", "Bar");
            var p = new Vehicle.Part("pod-0", 1, new Vector2(16, -14), 45, true, false, new Vector2(1, 1.6f), v.globalData);
            v.parts.Add(p);
            p = new Vehicle.Part("engine-3", 5, new Vector2(18, -20), 270, false, false, Vector2.one);
            v.parts.Add(p);
            v.connections.Add(5, 1);
            var act = new Vehicle.Activation(5);
            var stg = new List<Vehicle.Activation>();
            stg.Add(act);
            v.stages.Add(stg);

            var path = "test.json";
            print(v.ExportToString());
            v.Save(Application.persistentDataPath, path);
            Vehicle v_ = Vehicle.Load(Application.persistentDataPath, path);
            print(v_.ToString());
        }

        void Start ()
        {
            source = PlayAmbient();
            SceneLoader.LoadScene("Menu", callback: () => { SceneLoader.instance.fadeDuration = sceneFadeDuration; });
        }

        void Update ()
        {
            if (!waiting && fadeAmbient)
            {
                waiting = true;
                StartCoroutine(Loop());
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                int index = SceneManager.GetActiveScene().buildIndex - 1;
                if (index <= 0)
                {
                    return;
                }
                try
                {
                    SceneLoader.LoadScene(index);
                }
                catch
                {
                    throw;
                }
            }
        }

        IEnumerator Loop ()
        {
            yield return new WaitForSecondsRealtime(rand.Next(20, 60));
            var _source = PlayAmbient();
            _source.volume = 0;
            source.FadeOut(10);
            yield return new WaitForSecondsRealtime(3);
            _source.Seek(source.time);
            _source.FadeIn(10);
            source = _source;
            waiting = false;
        }
    }
}