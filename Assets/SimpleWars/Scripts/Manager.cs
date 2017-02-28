using System.Collections.Generic;
using System.Linq;
using System.IO;
using EazyTools.SoundManager;
using Lean.Localization;
using MarkLight;
using UnityEngine;

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
        public AudioClip bgm;

        public static FileInfo[] GetSaves ()
        {
            return GetSaves(Path.Combine(Application.persistentDataPath, "Vehicles"));
        }

        public static FileInfo[] GetSaves (string path)
        {
            var dirinfo = Directory.CreateDirectory(path);
            return dirinfo.GetFiles();
        }

        new void Awake ()
        {
            Unbug.Log(Application.persistentDataPath);
            RandomSeed = SystemInfo.deviceUniqueIdentifier.GetHashCode();
            SoundManager.PlayMusic(bgm, 0.2f, true, true, 0, 0);
            LeanLocalization.CurrentLanguage = Application.systemLanguage.ToString();
            ResourceDictionary.SetConfiguration(LanguageHelper.Get2LetterISOCodeFromSystemLanguage());
            ResourceDictionary.NotifyObservers();

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
            SceneLoader.LoadScene("Menu", callback: () => { SceneLoader.instance.fadeDuration = 0.5f; });
        }
    }
}