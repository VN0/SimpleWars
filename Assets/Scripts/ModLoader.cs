using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class ModLoader : MonoBehaviour
{
    public string modPath;
    public bool modEnabled = false;
    public Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
    AssetBundle mod;
    bool loaded = false;
    CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

    void Awake ()
    {
        //Physics2D.gravity = Vector2.zero;
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        if (!Application.genuine)
        {
            var file = File.Open(
                Path.Combine(Application.persistentDataPath, "ErrorLog.txt"), FileMode.OpenOrCreate);
            new BinaryWriter(file, System.Text.Encoding.UTF8).Write(
                "The application was modified after build. Please download the official version of SimpleWars." +
                "\n此应用在编译后被修改过。请下载简单战争的官方版本。\n");
            file.Close();
            Application.Quit();
        }
        Debug.LogFormat("Screen Size: {0} x {1}", Screen.width, Screen.height);
        if (Screen.height < 550)
        {
            scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            FindObjectOfType<CanvasScaler>().uiScaleMode = scaleMode;
        }
        SceneManager.sceneLoaded += delegate
        {
            FindObjectOfType<CanvasScaler>().uiScaleMode = scaleMode;
        };
    }

    void Start ()
    {
        if (!loaded)
        {
            try
            {
                string[] args = System.Environment.GetCommandLineArgs();
                print("Arguments: > " + string.Join(" ", args));
                args =
                    (from arg in args
                     where File.Exists(arg)
                     select arg).ToArray();

                if (args.Length > (Application.isMobilePlatform ? 0 : 1))
                {
                    modPath = args[1];
                    modEnabled = true;
                }
            }
            catch (System.ArgumentNullException)
            {
                print("Argument is null");
            }


            GameObject[] vanillaParts = Resources.LoadAll<GameObject>("Prefabs/Parts");
            foreach (GameObject asset in vanillaParts)
            {
                assets.Add(asset.name, asset);
            }

            if (modEnabled == true)
            {
                print("Mod enabled");
                mod = AssetBundle.LoadFromFile(modPath);
                if (System.Environment.GetCommandLineArgs().Length > 1 || modEnabled)
                {
                    GameObject[] assets_ = mod.LoadAllAssets<GameObject>();
                    foreach (GameObject asset in assets_)
                    {
                        assets[asset.name] = asset;
                    }
                }
            }
            loaded = true;
        }
    }

    void Update ()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    public AssetBundle Get ()
    {
        return mod;
    }
}
