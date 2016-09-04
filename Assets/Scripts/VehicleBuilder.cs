using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Unity.Linq;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

public class VehicleBuilder : MonoBehaviour
{
    public RectTransform partPanel;
    public ModLoader modLoader;
    public Button createButton;
    public GameObject connectionPoint;
    public Material cpNormal;
    public Material cpHighlighted;

    [HideInInspector]
    public GameObject vehicle;

    string appPath;
    Dictionary<string, GameObject> assets;
    PartInfos partInfos;
    VehicleLoader loader;
    GameObject draggingObject;
    bool dragging = false;
    bool firstTime;
    Vector3 lastPos;
    Vector3 lastGridPos;
    Vector3 rawPos;
    bool turnLeft;
    bool turnRight;

    void Awake ()
    {
        if (FindObjectsOfType<VehicleBuilder>().Length > 1)
        {
            Destroy(this);
        }
        loader = FindObjectOfType<VehicleLoader>();
        partInfos = FindObjectOfType<ObjectReference>().obj as PartInfos;
        modLoader = FindObjectOfType<ModLoader>();
        createButton.onClick.AddListener(delegate
        {
            vehicle = loader.NewVehicle();
        });
        assets = modLoader.assets;
        appPath = Application.persistentDataPath;
        partPanel.sizeDelta = new Vector2(0, 52 * assets.Count);
        int i = -26;
        foreach (PartInfos.PartInfo type in partInfos.partInformations)
        {
            if (!type.hidden)
            {
                GameObject btn = Instantiate(Resources.Load<GameObject>("Prefabs/ImageButton"));
                GameObject prefab = assets[type.id];
                btn.GetComponent<RectTransform>().localPosition =
                                new Vector2(partPanel.GetComponent<RectTransform>().rect.center.x + 35, i);
                btn.transform.FindChild("Image").gameObject.GetComponent<Image>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
                btn.GetComponentInChildren<Text>().text = type.name;

                #region BeginDrag
                EventTrigger trigger = btn.GetComponent<EventTrigger>();
                EventTrigger.Entry beginEntry = new EventTrigger.Entry();
                beginEntry.eventID = EventTriggerType.BeginDrag;
                beginEntry.callback.AddListener(delegate (BaseEventData arg)
                {
                    lastPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                    lastPos = Camera.main.ScreenToWorldPoint(lastPos);
                    lastGridPos = lastPos;
                    draggingObject = Instantiate(prefab, lastPos, Quaternion.Euler(0, 0, 0)) as GameObject;
                    //draggingObject.GetComponent<Rigidbody2D>().simulated = false;
                    Color color = draggingObject.GetComponent<SpriteRenderer>().color;
                    color.a = 0.5f;
                    draggingObject.GetComponent<SpriteRenderer>().color = color;
                    rawPos = draggingObject.transform.position;
                    foreach (PartType partType in vehicle.transform.GetComponentsInChildren<PartType>())
                    {
                        PartInfos.PartInfo info = partInfos.Get(partType.type);
                        foreach (PartInfos.AttachPoint point in info.attachPoints)
                        {
                            LineRenderer lr = (Instantiate(connectionPoint,
                                partType.transform.TransformPoint(new Vector3(point.position.x, point.position.y, 0)),
                                Quaternion.Euler(0, 0, point.rotation + partType.transform.rotation.eulerAngles.z)) as GameObject).GetComponent<LineRenderer>();
                            Vector3[] positions = new Vector3[2] { new Vector3(-point.length / 2f * 0.3f, 0, 0), new Vector3(point.length / 2f * 0.3f, 0, 0) };
                            lr.SetPositions(positions);
                            lr.GetComponent<AttachPoint>().reference = partType.gameObject;
                            lr.GetComponent<BoxCollider2D>().size = new Vector2(0.3f * point.length, 0.3f);
                        }
                    }
                    foreach (PartType partType in draggingObject.DescendantsAndSelf().OfComponent<PartType>())
                    {
                        try
                        {
                            if (partType.transform.IsChildOf(vehicle.transform))
                            {
                                continue;
                            }
                        }
                        catch (System.NullReferenceException) { }
                        PartInfos.PartInfo info = partInfos.Get(partType.type);
                        foreach (PartInfos.AttachPoint point in info.attachPoints)
                        {
                            LineRenderer lr = (Instantiate(connectionPoint,
                                partType.transform.TransformPoint(new Vector3(point.position.x, point.position.y, 0)),
                                Quaternion.Euler(0, 0, point.rotation + partType.transform.rotation.eulerAngles.z)) as GameObject).GetComponent<LineRenderer>();
                            lr.gameObject.name = "ConnectionDetached";
                            Vector3[] positions = new Vector3[2] { new Vector3(-point.length / 2f * 0.3f, 0, 0), new Vector3(point.length / 2f * 0.3f, 0, 0) };
                            lr.SetPositions(positions);
                            lr.GetComponent<AttachPoint>().reference = partType.gameObject;
                            BoxCollider2D col = lr.GetComponent<BoxCollider2D>();
                            col.size = new Vector2(0.3f * point.length, 0.3f);
                            col.isTrigger = true;
                        }
                    }
                    dragging = true;
                    firstTime = true;
                });
                trigger.triggers.Add(beginEntry);
                #endregion

                #region Drag
                EventTrigger.Entry dragEntry = new EventTrigger.Entry();
                dragEntry.eventID = EventTriggerType.Drag;
                dragEntry.callback.AddListener(delegate (BaseEventData arg)
                {
                    //SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.name.StartsWith("ConnectionDetached")).Destroy();
                    if (!dragging)
                    {
                        return;
                    }
                    Vector3 currentPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                    currentPos = Camera.main.ScreenToWorldPoint(currentPos);
                    Vector3 movePos = currentPos - lastPos;
                    if (firstTime && Mathf.Abs(movePos.x) < Mathf.Abs(movePos.y))
                    {
                        firstTime = false;
                        dragging = false;
                        return;
                    }
                    if (!vehicle)
                    {
                        return;
                    }
                    rawPos += movePos;
                    lastPos = currentPos;
                    firstTime = false;
                    if (turnLeft)
                    {
                        turnLeft = false;
                        draggingObject.transform.Rotate(0, 0, 90);
                    }
                    else if (turnRight)
                    {
                        turnRight = false;
                        draggingObject.transform.Rotate(0, 0, -90);
                    }
                    draggingObject.transform.position = SmartMath.Math.RoundVectorToGrid(rawPos, 0.3f);
                    foreach (GameObject lr in SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.name.StartsWith("ConnectionDetached")))
                    {
                        lr.transform.position += draggingObject.transform.position - lastGridPos;
                    }
                    lastGridPos = draggingObject.transform.position;
                });
                trigger.triggers.Add(dragEntry);
                #endregion

                #region EndDrag
                EventTrigger.Entry endEntry = new EventTrigger.Entry();
                endEntry.eventID = EventTriggerType.EndDrag;
                endEntry.callback.AddListener(delegate (BaseEventData arg)
                {
                    dragging = false;
                    SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.name.StartsWith("Connection")).Destroy();
                });
                trigger.triggers.Add(endEntry);
                #endregion

                btn.transform.SetParent(partPanel, true);
                i -= 52;
            }
        }
    }

    void Update ()
    {
        if (Input.GetButtonDown("Turn Left"))
        {
            turnLeft = true;
        }
        if (Input.GetButtonDown("Turn Right"))
        {
            turnLeft = true;
        }
        if (Input.GetButtonUp("Turn Left"))
        {
            turnLeft = false;
        }
        if (Input.GetButtonUp("Turn Right"))
        {
            turnLeft = false;
        }
    }
}
