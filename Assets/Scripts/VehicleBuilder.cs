using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Serialization;

public class VehicleBuilder : MonoBehaviour
{
    public RectTransform partPanel;
    public ModLoader modLoader;
    string appPath;
    Dictionary<string, GameObject> assets;
    PartInfos partInfos;

    void Awake ()
    {
        if (FindObjectsOfType<VehicleBuilder>().Length > 1)
        {
            Destroy(this);
        }
        partInfos = FindObjectOfType<SOLinker>().obj as PartInfos;
        modLoader = FindObjectOfType<ModLoader>();
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
                btn.GetComponent<Button>().onClick.AddListener(delegate
                {
                });
                btn.transform.SetParent(partPanel, true);
                i -= 52;
            }
        }
    }

    void Update ()
    {

    }
}
