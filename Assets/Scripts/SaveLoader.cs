using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SaveLoader : MonoBehaviour
{
    public GameObject content;
    public GameObject mask;
    public ModLoader modLoader;

    DirectoryInfo dirinfo;
    FileInfo[] files;
    GameObject rocket;
    string appPath;
    AssetBundle mod;
    Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
    bool modEnabled;
    float alpha = 0;
    float lastTime;
    float currentTime;

    void Start()
    {
        lastTime = Time.realtimeSinceStartup;
        appPath = Application.persistentDataPath;
        print(appPath);
        Directory.CreateDirectory(appPath + "/Ships");
        DirectoryInfo dirinfo = new DirectoryInfo(appPath + "/Ships");
        files = dirinfo.GetFiles();
        Time.timeScale = 0f;
        int i = 320;
        for (int j = 0; j < files.Length; j++)
        {
            FileInfo file = files[j];
            GameObject btn = Instantiate(Resources.Load<GameObject>("Prefabs/Button"));
            //btn.transform.position = new Vector2(content.GetComponent<RectTransform>().rect.width / 2, i);
            btn.GetComponent<RectTransform>().localPosition = new Vector2(content.GetComponent<RectTransform>().rect.width / 2, i);
            btn.GetComponentInChildren<Text>().text = file.Name;
            btn.GetComponent<Button>().onClick.AddListener(delegate
            {
                LoadSave(btn.GetComponentInChildren<Text>().text);
            });
            btn.transform.SetParent(content.transform, true);
            i -= 30;
            if (j == files.Length - 1)
            {
                content.GetComponent<RectTransform>().sizeDelta = btn.GetComponent<RectTransform>().position;
                content.GetComponent<RectTransform>().localPosition = new Vector3(content.GetComponent<RectTransform>().localPosition.x, -btn.GetComponent<RectTransform>().position.y, 0);
                print(btn.GetComponent<RectTransform>().position);
            }
        }
        modLoader.Load();
        //modEnabled = modLoader.modEnabled;
        assets = modLoader.assets;
        /*GameObject[] vanillaParts = Resources.LoadAll<GameObject>("Prefabs/Parts");
        foreach (GameObject asset in vanillaParts)
        {
            assets.Add(asset.name, asset);
        }
        if (System.Environment.GetCommandLineArgs().Length > 1 || modEnabled)
        {
            mod = modLoader.Load();
            GameObject[] assets_ = mod.LoadAllAssets<GameObject>();
            foreach (GameObject asset in assets_)
            {
                assets[asset.name] = asset;
            }
            modEnabled = true;
        }*/

        
    }


    void Update()
    {
        currentTime = Time.realtimeSinceStartup;
        if (alpha > 0 && alpha < 1 || Input.GetKeyDown(KeyCode.Return))
        {
            alpha += (currentTime - lastTime) * 2;
            mask.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
        else if (alpha >= 1)
        {
            try
            {
                DontDestroyOnLoad(rocket);
                Collider2D[] cols = FindObjectsOfType(typeof(Collider2D)) as Collider2D[];
                float[] bounds = new float[cols.Length];
                int i = 0;
                foreach (Collider2D col in cols)
                {
                    bounds[i] = (col.bounds.min.y);
                    i++;
                }
                float min = Mathf.Min(bounds);
                rocket.transform.position = new Vector3(0f, -min, 0f);
            }
            catch { }
            SceneManager.LoadScene(1);
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        lastTime = currentTime;
    }

    void LoadSave(string file)
    {

        Destroy(GameObject.Find("Rocket"));
        rocket = new GameObject();
        rocket.name = "Rocket";
        XmlDocument doc = new XmlDocument();
        doc.Load(appPath + "/Ships/" + file);
        XmlNode root = doc.DocumentElement;
        
        XmlNodeList parts = root.SelectNodes("./Parts/Part");
        XmlNodeList connections = root.SelectNodes("./Connections/Connection");

        foreach (XmlNode part in parts)
        {
            XmlAttributeCollection attr = part.Attributes;
            string type = attr.GetNamedItem("partType").InnerText;
            string id = attr.GetNamedItem("id").InnerText;
            GameObject go;
            GameObject prefab = null;
            try
            {
                prefab = assets[type];
            }
            catch {}
            if (!prefab)
            {
                prefab = new GameObject();
            }

            go = Instantiate(
                prefab,
                new Vector3(
                    float.Parse(attr.GetNamedItem("x").InnerText) / 5 * 3,
                    float.Parse(attr.GetNamedItem("y").InnerText) / 5 * 3
                ),
                Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * float.Parse(attr.GetNamedItem("angle").InnerText))
            ) as GameObject;

            go.name = id;
            go.transform.SetParent(rocket.transform);
            if (id == "1")
            {
                go.transform.SetParent(rocket.transform);
                go.tag = "Player";
                ActivationGroups ag = go.AddComponent<ActivationGroups>();
                XmlNodeList acts = part.SelectNodes("./Pod/Staging/Step");
                List<string[]> acts_ = new List<string[]>();
                foreach (XmlNode act in acts)
                {
                    XmlNodeList steps = act.SelectNodes("./Activate");
                    List<string> steps_ = new List<string>();
                    foreach (XmlNode step in steps)
                    {
                        steps_.Add(step.Attributes.GetNamedItem("Id").InnerText);
                    }
                    acts_.Add(steps_.ToArray());
                }
                ag.steps = acts_.ToArray();
                ag.ready = true;
            }
        }

        foreach (XmlNode con in connections)
        {
            XmlAttributeCollection attr = con.Attributes;
            GameObject parent = GameObject.Find(attr.GetNamedItem("parentPart").InnerText);
            GameObject child = GameObject.Find(attr.GetNamedItem("childPart").InnerText);
            child.transform.SetParent(parent.transform);
            if (child.CompareTag("Wheel"))
            {
                WheelJoint2D joint = child.AddComponent<WheelJoint2D>();
                joint.connectedBody = parent.GetComponent<Rigidbody2D>();
                joint.enableCollision = false;
                joint.connectedAnchor = child.transform.localPosition;
                JointSuspension2D sus = new JointSuspension2D();
                sus.dampingRatio = 0.9f;
                sus.frequency = 100;
                sus.angle = 0;
                joint.suspension = sus;
                joint.breakForce = 5000;
            }
            else
            {
                FixedJoint2D joint = child.AddComponent<FixedJoint2D>();
                joint.connectedBody = parent.GetComponent<Rigidbody2D>();
                joint.enableCollision = true;
                joint.connectedAnchor = child.transform.localPosition;
                joint.breakForce = 4000;
                joint.breakTorque = 5000;
            }
        }
    }
}
