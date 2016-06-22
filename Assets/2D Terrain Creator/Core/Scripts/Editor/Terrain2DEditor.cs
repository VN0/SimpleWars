using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Terrain2D))]
public class Terrain2DEditor : Editor
{
    public Terrain2D terrain;

    public Transform handleTransform;
    public Quaternion handleRotation;


    public virtual void OnSceneGUI()
    {
        terrain = target as Terrain2D;

        terrain.transform.eulerAngles = Vector3.zero;

        foreach (Terrain2D.Chunk chunk in terrain.chunks)
        {
            if (chunk.chunkObject.transform.FindChild("Mesh").GetComponent<MeshFilter>().sharedMesh == null)
            {
                terrain.UpdateAllChunkMeshes();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        terrain = target as Terrain2D;
    }


    public virtual void SettingsInspector()
    {
        EditorGUILayout.BeginVertical("box");

        GUI.color = new Color(1f, 1f, 0.5f, 1f);
        GUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Settings");   
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.color = Color.white;

        EditorGUI.BeginChangeCheck();

        float segmentsLength = EditorGUILayout.FloatField("Segments Length", terrain.segmentsLength);

        if (EditorGUI.EndChangeCheck())
        {
            terrain.SetSegmentsLength(segmentsLength);
        }

        EditorGUI.BeginChangeCheck();

        Terrain2D.GraphicsMode graphics = (Terrain2D.GraphicsMode)EditorGUILayout.EnumPopup("Graphics", terrain.graphics);    

        if (EditorGUI.EndChangeCheck())
        {
            terrain.SetGraphicsMode(graphics);
        }

        EditorGUI.BeginChangeCheck();
    
        bool capEnabled = EditorGUILayout.Toggle("Cap", terrain.cap.enabled);

        if (EditorGUI.EndChangeCheck())
        {
            terrain.EnableCap(capEnabled);
        }

        EditorGUI.BeginChangeCheck();

        bool flatBottomEnabled = EditorGUILayout.Toggle("Flat Bottom", terrain.front.flatBotttom);

        if (EditorGUI.EndChangeCheck())
        {
            terrain.SetFlatBottom(flatBottomEnabled);
        }

        EditorGUI.BeginChangeCheck();

        float meshHeight = EditorGUILayout.FloatField("Height", terrain.front.height);

        if (EditorGUI.EndChangeCheck())
        {
            terrain.SetHeight(meshHeight);
        }

        if (terrain.cap.enabled)
        {
            EditorGUI.BeginChangeCheck();

            float capMeshHeight = EditorGUILayout.FloatField("Cap Height", terrain.cap.height);

            if (EditorGUI.EndChangeCheck())
            {
                terrain.SetCapHeight(capMeshHeight);
            }
        }

        if (terrain.graphics == Terrain2D.GraphicsMode.ThreeD)
        {
            EditorGUI.BeginChangeCheck();

            float meshWidth = EditorGUILayout.FloatField("Width", terrain.top.width);

            if (EditorGUI.EndChangeCheck())
            {
                terrain.SetWidth(meshWidth);
            }
        }

        EditorGUI.BeginChangeCheck();

        Material frontMaterial = (Material)EditorGUILayout.ObjectField("Front Material", terrain.front.material, typeof(Material), true) as Material;

        if (EditorGUI.EndChangeCheck())
        {
            terrain.SetFrontMaterial(frontMaterial);
        }

        if (terrain.cap.enabled)
        {
            EditorGUI.BeginChangeCheck();

            Material capMaterial = (Material)EditorGUILayout.ObjectField("Cap Material", terrain.cap.material, typeof(Material), true) as Material;

            if (EditorGUI.EndChangeCheck())
            {
                terrain.SetCapMaterial(capMaterial);
            }
        }

        if (terrain.graphics == Terrain2D.GraphicsMode.ThreeD)
        {
            EditorGUI.BeginChangeCheck();

            Material topMaterial = (Material)EditorGUILayout.ObjectField("Top Material", terrain.top.material, typeof(Material), true) as Material;

            if (EditorGUI.EndChangeCheck())
            {
                terrain.SetTopMaterial(topMaterial);
            }
        }    

        EditorGUILayout.EndVertical();
    }

    public virtual void DrawGenerationInspector()
    {
        EditorGUILayout.BeginVertical("box");

        GUI.color = new Color(1f, 1f, 0.5f, 1f);
        GUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Generation");
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.color = Color.white;
    }

    public virtual void DrawPrefabsInspector()
    {
        EditorGUILayout.BeginVertical("box");

        GUI.color = new Color(1f, 1f, 0.5f, 1f);
        GUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Prefabs");
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.color = Color.white;
    }
}
