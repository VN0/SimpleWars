using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    Vector3 rawPos;

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

                EventTrigger trigger = btn.GetComponent<EventTrigger>();
                EventTrigger.Entry beginEntry = new EventTrigger.Entry();
                beginEntry.eventID = EventTriggerType.BeginDrag;
                beginEntry.callback.AddListener(delegate (BaseEventData arg)
                {
                    lastPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                    lastPos = Camera.main.ScreenToWorldPoint(lastPos);
                    draggingObject = Instantiate(prefab, lastPos, Quaternion.Euler(0, 0, 0)) as GameObject;
                    rawPos = draggingObject.transform.position;
                    foreach (PartType partType in vehicle.transform.GetComponentsInChildren<PartType>())
                    {
                        PartInfos.PartInfo info = partInfos.Get(partType.type);
                        foreach (PartInfos.AttachPoint point in info.attachPoints)
                        {
                            LineRenderer lr = (Instantiate(connectionPoint,
                                partType.transform.TransformPoint(new Vector3(point.position.x, point.position.y, 0)),
                                Quaternion.Euler(0, 0, point.rotation + partType.transform.rotation.eulerAngles.z)) as GameObject).GetComponent<LineRenderer>();
                            Vector3[] positions = new Vector3[2] { new Vector3(-point.length / 2 * 0.3f, 0, 0), new Vector3(point.length / 2 * 0.3f, 0, 0) };
                            lr.SetPositions(positions);
                            //Gizmos.DrawCube(Vector3.zero, new Vector3(0.2f, 0.2f, 1).Rotate(point.rotation));
                            //Gizmos.DrawSphere(type.transform.TransformPoint(point.position), 0.15f);
                        }
                    }
                    dragging = true;
                    firstTime = true;
                });
                trigger.triggers.Add(beginEntry);

                EventTrigger.Entry dragEntry = new EventTrigger.Entry();
                dragEntry.eventID = EventTriggerType.Drag;
                dragEntry.callback.AddListener(delegate (BaseEventData arg)
                {
                    if (!dragging)
                    {
                        return;
                    }
                    Vector3 currentPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                    currentPos = Camera.main.ScreenToWorldPoint(currentPos);
                    Vector3 movePos = currentPos - lastPos;
                    print(movePos.normalized.x > movePos.normalized.y);
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
                    draggingObject.transform.position = SmartMath.Basic.RoundVectorToGrid(rawPos, 0.3f);
                    lastPos = currentPos;
                    firstTime = false;
                });
                trigger.triggers.Add(dragEntry);

                EventTrigger.Entry endEntry = new EventTrigger.Entry();
                endEntry.eventID = EventTriggerType.EndDrag;
                endEntry.callback.AddListener(delegate (BaseEventData arg)
                {
                    dragging = false;
                    SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.name.StartsWith("ConnectionPoint"));
                });
                trigger.triggers.Add(endEntry);

                btn.transform.SetParent(partPanel, true);
                i -= 52;
            }
        }
    }

    void OnDraawGizmos ()
    {
        if (!vehicle)
        {
            return;
        }
        foreach (PartType type in vehicle.transform.GetComponentsInChildren<PartType>())
        {
            PartInfos.PartInfo info = partInfos.Get(type.type);
            foreach (PartInfos.AttachPoint point in info.attachPoints)
            {
                Gizmos.DrawCube(type.transform.TransformPoint(new Vector3(point.position.x, point.position.y, 0)),
                    new Vector3(point.rotation + type.transform.rotation.eulerAngles.z == 0 ?
                    point.length * 0.3f : 0.3f, point.rotation + type.transform.rotation.eulerAngles.z == 0 ? 0.3f : point.length * 0.3f, 1));
                //Gizmos.DrawCube(Vector3.zero, new Vector3(0.2f, 0.2f, 1).Rotate(point.rotation));
                //Gizmos.DrawSphere(type.transform.TransformPoint(point.position), 0.15f);
            }
        }
    }
}
