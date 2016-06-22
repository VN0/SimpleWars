using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomTerrain2D))]
public class RandomTerrain2DEditor : Terrain2DEditor
{
    private RandomTerrain2D randomTerrain;


    public override void OnSceneGUI()
    {
        base.OnSceneGUI(); 
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        randomTerrain = target as RandomTerrain2D;

        randomTerrain.GenerateRandomPrefabsClones();

        SettingsInspector();
        DrawGenerationInspector();
        DrawPrefabsInspector();
    }


    public override void DrawGenerationInspector()
    {
        base.DrawGenerationInspector();

        int previewCurves = EditorGUILayout.IntField("Preview Curves", randomTerrain.previewCurves);
        randomTerrain.SetPreviewCurves(previewCurves);
        float minLenght = EditorGUILayout.FloatField("Min Length", randomTerrain.minLength);
        randomTerrain.SetMinLenght(minLenght);
        float maxLenght = EditorGUILayout.FloatField("Max Length", randomTerrain.maxLength);
        randomTerrain.SetMaxLenght(maxLenght);
        float minHeight = EditorGUILayout.FloatField("Min Height", randomTerrain.minHeight);
        randomTerrain.SetMinHeight(minHeight);
        float maxHeight = EditorGUILayout.FloatField("Max Height", randomTerrain.maxHeight);
        randomTerrain.SetMaxHeight(maxHeight);

        if (PrefabUtility.GetPrefabType(terrain) != PrefabType.Prefab)
        {
            if (GUILayout.Button("Reset"))
            {
                foreach (RandomTerrain2D.RandomPrefab prefab in randomTerrain.prefabs)
                {
                    if (prefab.clones.Count > 0)
                    {
                        prefab.clones.Clear();
                    }
                }

                randomTerrain.Reset();
            }
        }

        EditorGUILayout.EndVertical();
    }

    public override void DrawPrefabsInspector()
    {
        base.DrawPrefabsInspector();

        for (int i = 0; i < randomTerrain.prefabs.Count; i++)
        {
            RandomTerrain2D.RandomPrefab prefab = randomTerrain.prefabs[i];

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(9);

            GUILayout.BeginVertical();
            
            prefab.foldout = EditorGUILayout.Foldout(prefab.foldout, "Prefab " + randomTerrain.prefabs.IndexOf(prefab).ToString());

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(12);

            GUILayout.BeginVertical();

            if (prefab.foldout)
            {
                prefab.prefabToClone = EditorGUILayout.ObjectField("Prefab", prefab.prefabToClone, typeof(GameObject), true) as GameObject;

                float offset = EditorGUILayout.FloatField("Offset", Mathf.Clamp(prefab.offset, 0, Mathf.Infinity));
                randomTerrain.SetPrefabOffset(prefab, offset);

                if (terrain.graphics == Terrain2D.GraphicsMode.ThreeD)
                {
                    float zPosition = EditorGUILayout.FloatField("Z Position", prefab.zPosition);
                    randomTerrain.SetPrefabZPosition(prefab,zPosition);
                }

                float minRepeatDistance = EditorGUILayout.FloatField("Min Repeat Distance", prefab.minRepeatDistance);
                randomTerrain.SetPrefabMinRepeatDistance(prefab, minRepeatDistance);
                float maxRepeatDistance = EditorGUILayout.FloatField("Max Repeat Distance", prefab.maxRepeatDistance);
                randomTerrain.SetPrefabMaxRepeatDistance(prefab, maxRepeatDistance);

                bool groupEnabled = EditorGUILayout.Toggle("Group", prefab.groupEnabled);
                randomTerrain.EnablePrefabGroup(prefab, groupEnabled);

                if (prefab.groupEnabled)
                {
                    int minGroupSize = EditorGUILayout.IntField("Min Group Size", prefab.minGroupSize);
                    randomTerrain.SetPrefabMinGroupSize(prefab, minGroupSize);
                    int maxGroupSize = EditorGUILayout.IntField("Max Group Size", (int)Mathf.Clamp(prefab.maxGroupSize, prefab.minGroupSize, Mathf.Infinity));
                    randomTerrain.SetPrefabMaxGroupSize(prefab, maxGroupSize);               
                    float groupSpacing = EditorGUILayout.FloatField("Clones Distance", prefab.groupSpacing);
                    randomTerrain.SetPrefabGroupSpacing(prefab, groupSpacing);
                }

                if (GUILayout.Button("Delete"))
                {
                    randomTerrain.DeletePrefabClones(prefab);
                    randomTerrain.prefabs.Remove(prefab);             
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }      

        if (GUILayout.Button("New Type of Prefab"))
        {
            randomTerrain.prefabs.Add(new RandomTerrain2D.RandomPrefab());
        }

        EditorGUILayout.EndVertical();
    }
}
