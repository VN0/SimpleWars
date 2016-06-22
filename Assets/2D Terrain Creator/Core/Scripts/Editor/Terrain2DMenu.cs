using UnityEditor;
using UnityEngine;

public class Terrain2DMenu
{
    [MenuItem("Component/2D Terrain Creator/Editable Terrain 2D")]

    private static void EditableTerrain2D()
    {
        GameObject temp = AssetDatabase.LoadAssetAtPath("Assets/2D Terrain Creator/Core/Prefabs/Editable Terrain 2D.prefab", typeof(GameObject)) as GameObject;
        GameObject prefab = GameObject.Instantiate(temp) as GameObject;
        prefab.name = "Eitable Terrain 2D";
    }

    [MenuItem("Component/2D Terrain Creator/Random Terrain 2D")]

    private static void RandomTerrain2D()
    {
        GameObject temp = AssetDatabase.LoadAssetAtPath("Assets/2D Terrain Creator/Core/Prefabs/Random Terrain 2D.prefab", typeof(GameObject)) as GameObject;
        GameObject prefab = GameObject.Instantiate(temp) as GameObject;
        prefab.name = "Random Terrain 2D";
    }
}
