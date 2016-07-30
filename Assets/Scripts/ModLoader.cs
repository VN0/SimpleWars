using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ModLoader : MonoBehaviour
{
    public string modPath;
    public bool modEnabled = false;
    public Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
    AssetBundle mod;
    bool loaded = false;

    public void Awake()
    {
        if(FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Start()
    {
        if (!loaded)
        {
            string[] args = System.Environment.GetCommandLineArgs();
            args =
                (from arg in args
                    where System.IO.File.Exists(arg)
                    select arg).ToArray();

            if (args.Length > 1)
            {
                modPath = args[1];
                modEnabled = true;
            }

            GameObject[] vanillaParts = Resources.LoadAll<GameObject>("Prefabs/Parts");
            foreach (GameObject asset in vanillaParts)
            {
                assets.Add(asset.name, asset);
            }

            if (modEnabled == true) {
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
            print(assets.Count);
            print(assets.Keys);
            loaded = true;
        }
        
    }
    public AssetBundle Get()
    {
        return mod;
    }
}
