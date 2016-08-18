using System.Collections.Generic;
using System.IO;
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
        startButton.onClick.AddListener(delegate
        {
            start = true;
        });
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 30 * files.Length);
        int i = -15;
        for (int j = 0; j < files.Length; j++)
        {
            FileInfo file = files[j];
            GameObject btn = Instantiate(Resources.Load<GameObject>("Prefabs/Button"));
            //btn.transform.position = new Vector2(content.GetComponent<RectTransform>().rect.width / 2, i);
            btn.GetComponent<RectTransform>().localPosition = new Vector2(content.GetComponent<RectTransform>().rect.width / 2, i);
            btn.GetComponentInChildren<Text>().text = file.Name;
            btn.GetComponent<Button>().onClick.AddListener(delegate
            {
                string path = Path.Combine(appPath, "Vehicles/" + btn.GetComponentInChildren<Text>().text);
                vehicle = LoadVehicle(path);
                if (vehicle == null)
                {
                    vehicle = LoadVehicleSR(path);
                }
            });
            btn.transform.SetParent(content.transform, true);
            i -= 30;
        }
    }


    void Update ()
    {
        if (vehicle && (alpha > 0 && alpha < 1 || Input.GetKeyDown(KeyCode.Return) || start))
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

    /// <summary>
    /// Load a legacy vehicle
    /// </summary>
    /// <param name="file">File to load</param>
    /// <returns></returns>
    public GameObject LoadVehicleSR (string file)
    {
        float mass = 0;
        Destroy(GameObject.Find("Vehicle"));
        GameObject vehicleGO = new GameObject("Vehicle");
        VehicleSR v = VehicleSR.Load(file);
        if (v.version != 1)
        {
            return null;
        }

        print(assets.Count);
        foreach (VehicleSR.Part part in v.parts)
        {
            GameObject go;
            GameObject prefab = null;
            if (!assets.TryGetValue(part.type, out prefab))
            {
                prefab = new GameObject();
                Debug.LogErrorFormat("The prefab '{0}' of part '{1}' is null.", part.type, part.id);
            }

            go = Instantiate(
                prefab,
                new Vector3(part.x * 0.6f, part.y * 0.6f),
                Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * part.r)
            ) as GameObject;

            try
            {
                mass += go.GetComponent<Rigidbody2D>().mass;
            }
            catch { }
            go.name = part.id.ToString();
            go.transform.SetParent(vehicleGO.transform);
            go.transform.localScale = new Vector3(
                (part.flipX ? -1 : 1) / go.transform.parent.localScale.x,
                (part.flipY ? -1 : 1) / go.transform.parent.localScale.y, 1);
            if (part.type.ToLower().Contains("pod"))      //If this part is the pod
            {
                go.tag = "Player";
                ActivationGroups ag = go.AddComponent<ActivationGroups>();
                List<string[]> stages = new List<string[]>();
                foreach (List<VehicleSR.Activate> stage in part.pod.stages)
                {
                    List<string> _stage = new List<string>();
                    foreach (VehicleSR.Activate act in stage)
                    {
                        _stage.Add(act.id.ToString());
                    }
                    stages.Add(_stage.ToArray());
                }
                ag.steps = stages.ToArray();
                ag.ready = true;
            }
        }
        massText.text = (mass * 500).ToString("N0") + " kg";



        foreach (VehicleSR.Connection con in v.connections)        //Set parents and connections
        {
            GameObject parent = GameObject.Find(con.parentPart.ToString());
            GameObject child = GameObject.Find(con.childPart.ToString());
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
                joint.anchor = child.transform.worldToLocalMatrix.MultiplyPoint3x4(child.GetComponent<Collider2D>().bounds.ClosestPoint(parent.transform.position));
                //joint.connectedAnchor = parent.transform.worldToLocalMatrix.MultiplyPoint3x4(parent.GetComponent<Collider2D>().bounds.ClosestPoint(child.transform.position));
                //joint.autoConfigureConnectedAnchor = false;
                joint.breakForce = 4000;
                joint.breakTorque = 4000;
            }
        }
        return vehicleGO;
    }

    /// <summary>
    /// Load a vehicle
    /// </summary>
    /// <param name="file">File to load</param>
    /// <returns></returns>
    public GameObject LoadVehicle (string file)
    {
        float mass = 0;
        Destroy(GameObject.Find("Vehicle"));
        GameObject vehicleGO = new GameObject("Vehicle");
        Vehicle v;
        try
        {
            v = Vehicle.Load(file);
        }
        catch
        {
            return null;
        }

        foreach (Vehicle.Part part in v.parts)
        {
            GameObject go;
            GameObject prefab = null;
            if (!assets.TryGetValue(part.type, out prefab))
            {
                prefab = new GameObject();
                Debug.LogError("Prefab is null");
            }

            go = Instantiate(
                prefab,
                new Vector3(part.x * 0.6f, part.y * 0.6f),
                Quaternion.Euler(0f, 0f, part.r)
            ) as GameObject;

            try
            {
                mass += go.GetComponent<Rigidbody2D>().mass;
            }
            catch { }
            go.name = part.id.ToString();
            go.transform.SetParent(vehicleGO.transform);
            go.transform.localScale = new Vector3(
                part.scaleX / go.transform.parent.localScale.x * (part.flipX ? -1 : 1), 
                part.scaleY / go.transform.parent.localScale.x * (part.flipX ? -1 : 1), 1);
            if (part.type.ToLower().Contains("pod"))      //If this part is the pod
            {
                go.tag = "Player";
                ActivationGroups ag = go.AddComponent<ActivationGroups>();
                List<string[]> stages = new List<string[]>();
                foreach (List<Vehicle.Activation> stage in v.stages)
                {
                    List<string> _stage = new List<string>();
                    foreach (Vehicle.Activation act in stage)
                    {
                        _stage.Add(act.id.ToString());
                    }
                    stages.Add(_stage.ToArray());
                }
                ag.steps = stages.ToArray();
                ag.ready = true;
            }
        }
        massText.text = (mass * 500).ToString("N0") + " kg";



        foreach (Vehicle.Connection con in v.connections)        //Set parents and connections
        {
            GameObject parent = GameObject.Find(con.parent.ToString());
            GameObject child = GameObject.Find(con.child.ToString());
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
                joint.anchor = child.transform.worldToLocalMatrix.MultiplyPoint3x4(child.GetComponent<Collider2D>().bounds.ClosestPoint(parent.transform.position));
                //joint.connectedAnchor = parent.transform.worldToLocalMatrix.MultiplyPoint3x4(parent.GetComponent<Collider2D>().bounds.ClosestPoint(child.transform.position));
                //joint.autoConfigureConnectedAnchor = false;
                joint.breakForce = 4000;
                joint.breakTorque = 4000;
            }
        }
        return vehicleGO;
    }
}
