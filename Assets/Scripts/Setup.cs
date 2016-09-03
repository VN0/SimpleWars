using UnityEngine;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

public class Setup : MonoBehaviour
{
    public AudioSource music;
    Animator mask;
    string emptyVehiclePath;

    void Awake ()
    {
        mask = GameObject.Find("SceneTransitionMask").GetComponent<Animator>();
        DontDestroyOnLoad(music);
        DontDestroyOnLoad(mask.transform.root);
        StartCoroutine(Wait(0.1f, delegate
        {
            mask.SetBool("open", true);
        }));
        emptyVehiclePath = Path.Combine(Application.persistentDataPath, "EmptyVehicle.xml");
        if (File.Exists(emptyVehiclePath))
        {
            SHA256Managed sha2 = new SHA256Managed();
            FileStream stream = File.OpenRead(emptyVehiclePath);
            sha2.ComputeHash(stream);
            string hash = sha256(stream: stream);
            print(hash);
            if (hash == "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")
            {
                print("EmptyVehicle.xml is valid.");
                return;
            }
            stream.Close();
        }
        using (FileStream stream = File.Create(emptyVehiclePath))
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Vehicle xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" name="""" version=""2"" >
  <GlobalData />
  <Parts >
    <Part type=""pod-1"" id=""1"" x=""0"" y=""0.75"" r=""0"" flipX=""false"" flipY=""false"" scaleX=""1"" scaleY=""1"" />
  </Parts >
  <Disconnecteds />
  <Connections />
  <Staging />
</Vehicle > ");
            writer.Close();
        }
    }

    public static IEnumerator Wait (float seconds, System.Action func)
    {
        yield return new WaitForSeconds(seconds);
        func();
    }

    void Update ()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            Application.Quit();
        }
    }

    static string sha256 (string str = null, Stream stream = null)
    {
        SHA256Managed crypt = new SHA256Managed();
        StringBuilder hash = new StringBuilder();
        byte[] crypto = null;

        if (str == null && stream == null)
        {
            return null;
        }
        else if (str != null)
        {
            crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.ASCII.GetByteCount(str));
        }
        else if (stream != null)
        {
            crypto = crypt.ComputeHash(stream);
        }

        foreach (byte theByte in crypto)
        {
            hash.Append(theByte.ToString("x2"));
        }
        return hash.ToString();
    }
}
