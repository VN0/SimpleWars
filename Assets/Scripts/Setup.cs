using UnityEngine;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
#if UNITY_STANDALONE_WIN_
using Microsoft.Win32;
using System.Linq;
using System.Diagnostics;
#endif

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

#if UNITY_STANDALONE_WIN && UNITY_EDITOR_

        print(Directory.GetParent(Application.dataPath).FullName);
        string exe = Path.Combine(Directory.GetParent(Application.dataPath).FullName,
            (from file in Directory.GetFiles(Directory.GetParent(Application.dataPath).FullName)
             where file.EndsWith(".exe")
             select file
        ).ToArray()[0]);
        print(exe);
        var p = RegistryKeyPermissionCheck.ReadWriteSubTree;
        RegistryKey swProtocol = Registry.ClassesRoot.CreateSubKey("simplewars", p);
        RegistryKey swExtension = Registry.ClassesRoot.CreateSubKey("SimpleWars.Mod", p);
        if (swExtension.CreateSubKey("shell", p).CreateSubKey("open", p).CreateSubKey("command", p)
            .GetValue("") as string != string.Format("\"{0}\" \"%1\"", exe))
        {
            print("Setup Registry");
            swProtocol.SetValue("", "URL:SimpleWars Protocol");
            swProtocol.SetValue("URL Protocol", "");
            swProtocol.CreateSubKey("DefaultIcon", p).SetValue("", exe);
            swProtocol.CreateSubKey("shell", p).CreateSubKey("open", p).CreateSubKey("command", p)
                .SetValue("", string.Format("\"{0}\" \"%1\"", exe));
            Registry.ClassesRoot.CreateSubKey(".swmod", p).SetValue("", "SimpleWars.Mod");
            swExtension.SetValue("", "SimpleWars Mod");
            swExtension.CreateSubKey("DefaultIcon", p).SetValue("", string.Format("\"{0}\" , 1", exe));
            swExtension.CreateSubKey("shell", p).CreateSubKey("open", p).CreateSubKey("command", p)
                .SetValue("", string.Format("\"{0}\" \"%1\"", exe));
        }
#endif

        emptyVehiclePath = Path.Combine(Application.persistentDataPath, "EmptyVehicle.xml");
        if (File.Exists(emptyVehiclePath))
        {
            FileStream stream = File.OpenRead(emptyVehiclePath);
            string hash = GetMD5(stream: stream);
            print(hash);
            if (hash == "f00974d668b360246e19dc4bed8b03d3")
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
        if (hardInput.GetKeyUp("Quit"))
        {
            Application.Quit();
        }
    }

    static string GetMD5 (string str = null, Stream stream = null)
    {
        //SHA256Managed crypt = new SHA256Managed();
        MD5 crypt = MD5.Create();
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
