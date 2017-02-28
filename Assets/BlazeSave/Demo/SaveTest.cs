using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveTest : MonoBehaviour
{

    public InputField inputF;
    public Text result;

    public void Save ()
    {
        BlazeSave.SaveData("demo.bin", inputF.text);
    }

    public void Load ()
    {
        string data = BlazeSave.LoadData<string>("demo.bin");
        if (data == null)
        {
            result.text = "DATA DOESN'T EXIST";
        }
        else
        {
            result.text = data;
        }
    }

}
