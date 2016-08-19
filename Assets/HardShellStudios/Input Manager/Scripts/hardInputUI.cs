using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[AddComponentMenu("Hard Shell Studios/Input Manager/Rebind Button")]
public class hardInputUI : MonoBehaviour
{
    public Text displayText;
    public string keyName;
    public bool useSecondary;
    [HideInInspector]
    public bool beingBound = false;

    public void remapKey()
    {
        beingBound = true;
        hardInput.HardStartRebind(keyName, useSecondary, gameObject.GetComponent<hardInputUI>());
    }

    void OnGUI()
    {
        if (displayText != null)
        {
            if (!beingBound)
                displayText.text = hardInput.GetKeyName(keyName, useSecondary);
            else
                displayText.text = "PRESS A KEY";
        }
    }


}
