using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class VehicleLoader : MonoBehaviour
{
    public GameObject content;
    public GameObject mask;
    public ModLoader modLoader;
    public Button startButton;
    public Text massText;

    DirectoryInfo dirinfo;
    FileInfo[] files;
    GameObject vehicle;
    string appPath;
    AssetBundle mod;
    Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
    bool modEnabled;
    float alpha = 0;
    bool start = false;

    void Awake ()
    {
        modLoader = FindObjectOfType<ModLoader>();
        assets = modLoader.assets;
        appPath = Application.persistentDataPath;
        print(appPath);
        Directory.CreateDirectory(Path.Combine(appPath, "Vehicles"));
        DirectoryInfo dirinfo = new DirectoryInfo(Path.Combine(appPath, "Vehicles"));
        files = dirinfo.GetFiles();
        Time.timeScale = 0f;
        int i = 320;
        startButton.onClick.AddListener(delegate
        {
            start = true;
        });
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
            }
        }
    }


    void Update ()
    {
        if (vehicle && alpha > 0 && alpha < 1 || Input.GetKeyDown(KeyCode.Return) || start)
        {
            start = false;
            alpha += Time.unscaledDeltaTime * 2;
            mask.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
        else if (alpha >= 1)
        {
            DontDestroyOnLoad(vehicle);

            Collider2D[] cols = FindObjectsOfType(typeof(Collider2D)) as Collider2D[];
            float[] bounds = new float[cols.Length];
            int i = 0;
            foreach (Collider2D col in cols)
            {
                bounds[i] = (col.bounds.min.y);
                i++;
            }
            float min = Mathf.Min(bounds);
            vehicle.transform.position = new Vector3(0f, -min, 0f);

            SceneManager.LoadScene("Earth");
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    void LoadSave (string file)
    {
        float mass = 0;
        Destroy(GameObject.Find("Vehicle"));
        vehicle = new GameObject("Vehicle");
        XmlDocument doc = new XmlDocument();
        doc.Load(Path.Combine(Path.Combine(appPath, "Vehicles"), file));
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
            if (!assets.TryGetValue(type, out prefab))
            {
                prefab = new GameObject();
                Debug.LogError("Prefab is null");
            }

            go = Instantiate(
                prefab,
                new Vector3(
                    float.Parse(attr.GetNamedItem("x").InnerText) * 0.6f,
                    float.Parse(attr.GetNamedItem("y").InnerText) * 0.6f
                ),
                Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * float.Parse(attr.GetNamedItem("angle").InnerText))
            ) as GameObject;

            try
            {
                mass += go.GetComponent<Rigidbody2D>().mass;
            }
            catch { }
            go.name = id;
            go.transform.SetParent(vehicle.transform);
            if (id == "1")      //If this part is the pod
            {
                go.transform.SetParent(vehicle.transform);
                go.tag = "Player";
                ActivationGroups ag = go.AddComponent<ActivationGroups>();
                XmlNodeList acts = part.SelectNodes("./Pod/Staging/Step");
                if (acts.Count == 0)
                {
                    acts = part.SelectNodes("/Ship/Staging/Step");
                }
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
        massText.text = (mass * 500).ToString("N0") + " kg";

        foreach (XmlNode con in connections)        //Set parents and connections
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
                joint.autoConfigureConnectedAnchor = true;
                //joint.anchor = Vector2.zero;// child.transform.localPosition;
                //joint.connectedAnchor = Vector2.zero;// parent.transform.localPosition;
                joint.breakForce = 5000;
                joint.breakTorque = 5000;
            }
        }
    }
}
