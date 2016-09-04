using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Linq;


public class VehicleLoader : MonoBehaviour
{
    public GameObject content;
    public ModLoader modLoader;
    public Button startButton;
    public Text massText;
    Animator mask;
    VehicleBuilder builder;
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
        if (FindObjectsOfType<VehicleBuilder>().Length > 1)
        {
            Destroy(this);
        }
        mask = GameObject.Find("SceneTransitionMask").GetComponent<Animator>();
        modLoader = FindObjectOfType<ModLoader>();
        builder = FindObjectOfType<VehicleBuilder>();
        assets = modLoader.assets;
        appPath = Application.persistentDataPath;
        DirectoryInfo dirinfo = Directory.CreateDirectory(Path.Combine(appPath, "Vehicles"));
#if UNITY_ANDROID
        FileInfo[] _files = dirinfo.GetFiles();
        FileInfo[] _filesSR;
        try
        {
            DirectoryInfo dirinfoSR = new DirectoryInfo(Path.Combine(appPath, "../../com.jundroo.simplerockets/files/ships"));
            _filesSR = dirinfoSR.GetFiles();
            files = new FileInfo[_filesSR.Length + _files.Length];
            _files.CopyTo(files, 0);
            _filesSR.CopyTo(files, _files.Length);
        }
        catch
        {
            files = _files;
        }
#elif UNITY_STANDALONE_WIN
        FileInfo[] _files = dirinfo.GetFiles();
        FileInfo[] _filesSR;
        try
        {
            DirectoryInfo dirinfoSR = new DirectoryInfo(Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)
                , "Jundroo/SimpleRockets/ships"));
            _filesSR = dirinfoSR.GetFiles();
            files = new FileInfo[_filesSR.Length + _files.Length];
            _files.CopyTo(files, 0);
            _filesSR.CopyTo(files, _files.Length);
        }
        catch
        {
            files = _files;
        }
#else
        files = dirinfo.GetFiles();
#endif
        Time.timeScale = 0f;
        startButton.onClick.AddListener(delegate
        {
            start = true;
        });
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 52 * files.Length);
        int i = -26;
        for (int j = 0; j < files.Length; j++)
        {
            FileInfo file = files[j];
            GameObject btn = Instantiate(Resources.Load<GameObject>("Prefabs/Button"));
            string path = file.FullName;
            btn.GetComponent<RectTransform>().localPosition =
                            new Vector2(content.GetComponent<RectTransform>().rect.center.x + 35, i);
            btn.GetComponentInChildren<Text>().text = file.Name;
            btn.GetComponent<Button>().onClick.AddListener(delegate
            {
                vehicle = LoadVehicle(path);
                if (vehicle == null)
                {
                    vehicle = LoadVehicleSR(path);
                }
                builder.vehicle = vehicle;
            });
            btn.transform.SetParent(content.transform, true);
            i -= 52;
        }
        StartCoroutine(Wait(0.1f, delegate
        {
            mask.SetBool("open", true);
        }));
    }

    public static IEnumerator Wait (float seconds, System.Action func)
    {
        yield return new WaitForSecondsRealtime(seconds);
        func();
    }


    void Update ()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            mask.SetBool("open", false);
            StartCoroutine(SceneLoader.LoadSceneAnim(mask, delegate
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("Menu");
            }));
        }
        if (vehicle && (Input.GetKeyDown(KeyCode.Return) || start))
        {
            mask.SetBool("open", false);
            StartCoroutine(SceneLoader.LoadSceneAnim(mask, delegate
            {
                DontDestroyOnLoad(vehicle);
                SceneManager.LoadScene("Earth");

                Collider2D[] cols = FindObjectsOfType(typeof(Collider2D)) as Collider2D[];
                float[] bounds = new float[cols.Length];
                int i = 0;
                foreach (Collider2D col in cols)
                {
                    bounds[i] = (col.bounds.min.y);
                    i++;
                }
                float min = Mathf.Min(bounds);
                vehicle.transform.position = new Vector3(0, -min + 0.5f, 0);

                Time.timeScale = 1f;
            }));
        }
    }


    public GameObject NewVehicle ()
    {
        Destroy(GameObject.Find("Vehicle"));
        vehicle = LoadVehicle(Path.Combine(Application.persistentDataPath, "EmptyVehicle.xml"));
        return vehicle;
    }


    public GameObject GetVehicle ()
    {
        return vehicle;
    }


    /// <summary>
    /// Load a SR vehicle
    /// </summary>
    /// <param name="file">File to load</param>
    /// <returns></returns>
    public GameObject LoadVehicleSR (string file)
    {
        VehicleSR v = VehicleSR.Load(file);
        if (v.version != 1)
        {
            return null;
        }
        float mass = 0;
        Destroy(GameObject.Find("Vehicle"));
        GameObject vehicleGO = new GameObject("Vehicle");

        foreach (VehicleSR.Part part in v.parts)
        {
            GameObject go;
            GameObject prefab = null;
            if (!assets.TryGetValue(part.type, out prefab))
            {
                prefab = new GameObject();
                prefab.AddComponent<Rigidbody2D>();
                prefab.AddComponent<BoxCollider2D>();
                prefab.AddComponent<FixedJoint2D>();
                Debug.LogErrorFormat("The prefab '{0}' of part '{1}' is null.", part.type, part.id);
            }

            go = Instantiate(
                prefab,
                new Vector3(part.x * 0.6f, part.y * 0.6f),
                Quaternion.Euler(0, 0, Mathf.Rad2Deg * part.r)
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
                AnchoredJoint2D joint = child.GetComponent<AnchoredJoint2D>();
                joint.connectedBody = parent.GetComponent<Rigidbody2D>();
                joint.connectedAnchor = child.transform.localPosition;
                joint.enabled = true;
            }
            else
            {
                AnchoredJoint2D joint = child.GetComponent<AnchoredJoint2D>();
                joint.connectedBody = parent.GetComponent<Rigidbody2D>();
                joint.enableCollision = true;
                joint.anchor = child.transform.worldToLocalMatrix.MultiplyPoint3x4(
                    child.GetComponent<Collider2D>().bounds.ClosestPoint(parent.transform.position));
                joint.enabled = true;
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
        Vehicle v;
        try
        {
            v = Vehicle.Load(file);
        }
        catch
        {
            return null;
        }
        float mass = 0;
        Destroy(GameObject.Find("Vehicle"));
        GameObject vehicleGO = new GameObject("Vehicle");

        foreach (Vehicle.Part part in v.parts)
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
                Quaternion.Euler(0, 0, part.r)
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
                AnchoredJoint2D joint = child.GetComponent<AnchoredJoint2D>();
                joint.connectedBody = parent.GetComponent<Rigidbody2D>();
                joint.connectedAnchor = child.transform.localPosition;
                joint.enabled = true;
            }
            else
            {
                AnchoredJoint2D joint = child.GetComponent<AnchoredJoint2D>();
                joint.connectedBody = parent.GetComponent<Rigidbody2D>();
                joint.enableCollision = true;
                joint.anchor = child.transform.worldToLocalMatrix.MultiplyPoint3x4(
                    child.GetComponent<Collider2D>().bounds.ClosestPoint(parent.transform.position));
                joint.enabled = true;
            }
        }
        return vehicleGO;
    }
}
