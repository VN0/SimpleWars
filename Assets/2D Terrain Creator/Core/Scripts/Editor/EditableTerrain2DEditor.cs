using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EditableTerrain2D))]
public class EditableTerrain2DEditor : Terrain2DEditor
{
    private EditableTerrain2D editableTerrain;


    public override void OnSceneGUI()
    {
        base.OnSceneGUI();

        editableTerrain = target as EditableTerrain2D;      

        handleTransform = terrain.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? terrain.transform.rotation : Quaternion.identity;

        if (terrain.controlPoints.Count > 3)
        {
            Vector3 p0 = ShowControlPoint(0);

            for (int i = 1; i < terrain.controlPoints.Count; i += 3)
            {
                Vector3 p1 = ShowControlPoint(i);
                Vector3 p2 = ShowControlPoint(i + 1);
                Vector3 p3 = ShowControlPoint(i + 2);

                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                int segments = editableTerrain.GetCurveSegments(i - 1);

                Handles.color = Color.white;
                Vector3 segmentStart = terrain.transform.TransformPoint(terrain.GetPoint(i - 1, 0f));

                for (int j = 0; j <= segments; j++)
                {
                    Vector3 segmentEnd = terrain.transform.TransformPoint(terrain.GetPoint(i - 1, j / (float)segments));
                    Handles.DrawLine(segmentStart, segmentEnd);
                    segmentStart = segmentEnd;
                }

                p0 = p3;
            }
        }

        foreach (Terrain2D.Prefab prefab in editableTerrain.prefabs)
        {
            for (int i = 0; i < prefab.clones.Count; i++)
            {
                if (prefab.clones.Count == 0) return;

                ShowPrefabClonePoint(prefab, i);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        editableTerrain = target as EditableTerrain2D;

        SettingsInspector(); 

        DrawGenerationInspector();
        DrawPrefabsInspector();
        
        DrawSelectedControlPointInspector();
        DrawSelectedPrefabCloneInspector();
    }


    private Vector3 ShowControlPoint(int index)
    {
        Vector3 point = terrain.transform.TransformPoint(new Vector3(terrain.GetControlPoint(index).x, terrain.GetControlPoint(index).y, 0f));
        float size = HandleUtility.GetHandleSize(point);

        float modeIndex = (index + 1) / 3;
        float middleIndex = modeIndex * 3;

        Handles.color = index == middleIndex ? Color.blue : Color.white;

        if (Handles.Button(point, handleRotation, size * 0.04f, size * 0.06f, Handles.DotCap))
        {
            Undo.RecordObject(editableTerrain, "Select Control Point");

            editableTerrain.selectedControlPoint = index;

            editableTerrain.selectedPrefab.type = -1;
            editableTerrain.selectedPrefab.clone = -1;

            if (index - 1 > 0)
            {
                editableTerrain.SetleftControlPointPosition(index);
            }

            if (index + 1 < terrain.controlPoints.Count)
            {
                editableTerrain.SetRightControlPointPosition(index);
            }            
        }

        if (editableTerrain.selectedControlPoint == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(new Vector3(point.x, point.y, point.z), handleRotation);

            if (EditorGUI.EndChangeCheck())
            {
                editableTerrain.MoveControlPoint(index, point);
            }       
        }

        return point;
    }

    private void ShowPrefabClonePoint(Terrain2D.Prefab prefab, int index)
    {
        Vector3 point = (prefab.clones[index].cloneObjet.transform.position);
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = Color.red;

        if (Handles.Button(point, handleRotation, size * 0.04f, size * 0.06f, Handles.DotCap))
        {
            Undo.RecordObject(editableTerrain, "Select Prefab Clone");

            editableTerrain.selectedPrefab.type = editableTerrain.prefabs.IndexOf(prefab);
            editableTerrain.selectedPrefab.clone = index;

            editableTerrain.selectedControlPoint = -1;
        }

        if (editableTerrain.selectedPrefab.clone == index && editableTerrain.selectedPrefab.type == editableTerrain.prefabs.IndexOf(prefab) && editableTerrain.selectedPrefab.clone >= 0)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);

            if (EditorGUI.EndChangeCheck())
            {
                editableTerrain.MovePrefabClone(prefab, index, terrain.transform.InverseTransformPoint(point), prefab.clones[index].offset);
                EditorUtility.SetDirty(editableTerrain);
            }
        }
    }
   

    public override void DrawGenerationInspector()
    {
        if (PrefabUtility.GetPrefabType(terrain) == PrefabType.Prefab) return;

        base.DrawGenerationInspector();

        if (editableTerrain.selectedControlPoint / 3f == editableTerrain.GetCurve(editableTerrain.selectedControlPoint))
        {
            if (GUILayout.Button("Add Curve"))
            {
                editableTerrain.AddCurve(editableTerrain.selectedControlPoint);
            }

            if (editableTerrain.CurveCount > 1 && editableTerrain.selectedControlPoint != terrain.controlPoints.Count - 1)
            {
                if (GUILayout.Button("Delete Curve"))
                {
                    editableTerrain.DeleteCurve(editableTerrain.selectedControlPoint);
                }
            }

            if (editableTerrain.selectedControlPoint != 0 && editableTerrain.selectedControlPoint != terrain.controlPoints.Count - 1)
            {
                if (GUILayout.Button("Delete Point"))
                {
                    editableTerrain.DeletePoint(editableTerrain.selectedControlPoint);
                }
            }
        }

        if (GUILayout.Button("Reset")) 
        {
            Undo.RecordObject(this, "Selected Control Point");

            editableTerrain.selectedControlPoint = -1;

            Undo.RecordObject(this, "Selected Prefab Clone");

            editableTerrain.selectedPrefab.clone = -1;
            editableTerrain.selectedPrefab.type = -1;

            editableTerrain.Reset();
        }

        EditorGUILayout.EndVertical();
    }

    public override void DrawPrefabsInspector()
    {
        base.DrawPrefabsInspector();

        for (int i = 0; i < editableTerrain.prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            editableTerrain.prefabs[i].lastPrefabToClone = editableTerrain.prefabs[i].prefabToClone;
            GameObject prefabToClone = EditorGUILayout.ObjectField("Prefab " + editableTerrain.prefabs.IndexOf(editableTerrain.prefabs[i]).ToString(), editableTerrain.prefabs[i].prefabToClone, typeof(GameObject), true) as GameObject;      

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(editableTerrain, "Record Prefab Clone");

                editableTerrain.prefabs[i].prefabToClone = prefabToClone;

                if (editableTerrain.prefabs[i].prefabToClone != null)
                {
                    if (editableTerrain.prefabs[i].prefabToClone != editableTerrain.prefabs[i].lastPrefabToClone)
                    {
                        editableTerrain.ReplacePrefabClones(editableTerrain.prefabs[i]);
                        editableTerrain.prefabs[i].lastPrefabToClone = editableTerrain.prefabs[i].prefabToClone;
                    }
                }
            }

            if (PrefabUtility.GetPrefabType(terrain) != PrefabType.Prefab)
            {
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    int middleIndex = ((editableTerrain.selectedControlPoint + 1) / 3) * 3;
                    Vector3 startPosition = editableTerrain.controlPoints[middleIndex];
                    editableTerrain.AddPrefabClone(editableTerrain.prefabs[i], startPosition, 0f);
                }
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                editableTerrain.selectedPrefab.type = -1;
                editableTerrain.selectedPrefab.clone = -1;

                editableTerrain.DeletePrefabClones(editableTerrain.prefabs[i]);

                Undo.RecordObject(editableTerrain, "Record Prefab");               
                editableTerrain.prefabs.RemoveAt(i);                
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("New Type of Prefab"))
        {
            editableTerrain.prefabs.Add(new Terrain2D.Prefab());
        }

        EditorGUILayout.EndVertical();
    }


    private void DrawSelectedControlPointInspector()
    {
        if (PrefabUtility.GetPrefabType(terrain) == PrefabType.Prefab) return;

        if (editableTerrain.selectedControlPoint < 0) return;

        EditorGUILayout.BeginVertical("box");

        GUI.color = new Color(1f, 1f, 0.5f, 1f);
        GUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(0);
        GUILayout.Label("Selected Control Point");
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.color = Color.white;

        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", terrain.transform.TransformPoint(terrain.GetControlPoint(editableTerrain.selectedControlPoint)));

        if (EditorGUI.EndChangeCheck())
        {
            editableTerrain.MoveControlPoint(editableTerrain.selectedControlPoint, point);
        }

        if (editableTerrain.selectedControlPoint > 1 && editableTerrain.selectedControlPoint < terrain.controlPoints.Count - 2)
        {
            EditorGUI.BeginChangeCheck();
            Terrain2D.BezierControlPointMode mode = (Terrain2D.BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", terrain.GetControlPointMode(editableTerrain.selectedControlPoint));

            if (EditorGUI.EndChangeCheck())
            {
                terrain.SetControlPointMode(editableTerrain.selectedControlPoint, mode);

                int modeIndex = (editableTerrain.selectedControlPoint + 1) / 3;
                int middleIndex = modeIndex * 3;

                if (editableTerrain.modes[modeIndex] == Terrain2D.BezierControlPointMode.Aligned || editableTerrain.modes[modeIndex] == Terrain2D.BezierControlPointMode.Mirrored)
                {
                    if (editableTerrain.selectedControlPoint <= middleIndex)
                    {
                        editableTerrain.MoveControlPoint(middleIndex + 1, editableTerrain.controlPoints[middleIndex + 1]);
                    }
                    else
                    {
                        editableTerrain.MoveControlPoint(middleIndex - 1, editableTerrain.controlPoints[middleIndex - 1]);
                    }
                }              
            }
        }
        else
        {
            EditorGUILayout.EnumPopup("Mode", Terrain2D.BezierControlPointMode.Free);       
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawSelectedPrefabCloneInspector()
    {
        if (PrefabUtility.GetPrefabType(terrain) == PrefabType.Prefab) return;

        if (editableTerrain.selectedPrefab.clone < 0) return;

        EditorGUILayout.BeginVertical("box");
        
        GUI.color = new Color(1f, 1f, 0.5f, 1f);
        GUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(0);
        GUILayout.Label("Selected Prefab Clone");
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.color = Color.white;

        EditorGUI.BeginChangeCheck();

        Terrain2D.Prefab.Clone clone = editableTerrain.prefabs[editableTerrain.selectedPrefab.type].clones[editableTerrain.selectedPrefab.clone];
        float xPosition = clone.cloneObjet.transform.position.x;
        float yPosition = clone.cloneObjet.transform.position.y;
        float zPosition = clone.cloneObjet.transform.position.z;

        Vector3 position = EditorGUILayout.Vector3Field("Position", terrain.transform.InverseTransformPoint(new Vector3(xPosition, yPosition, zPosition)));
        float offset = EditorGUILayout.FloatField("Offset", clone.offset);    

        if (EditorGUI.EndChangeCheck())
        {
            editableTerrain.MovePrefabClone(editableTerrain.prefabs[editableTerrain.selectedPrefab.type], editableTerrain.selectedPrefab.clone, position, offset);
        }

        if (GUILayout.Button("Clone"))
        {
            editableTerrain.AddPrefabClone(editableTerrain.prefabs[editableTerrain.selectedPrefab.type], editableTerrain.transform.InverseTransformPoint(clone.cloneObjet.transform.position), clone.offset);
        }

        if (GUILayout.Button("Remove"))
        {
            editableTerrain.DeletePrefabClone(editableTerrain.prefabs[editableTerrain.selectedPrefab.type], editableTerrain.prefabs[editableTerrain.selectedPrefab.type].clones[editableTerrain.selectedPrefab.clone]);

            editableTerrain.selectedPrefab.type = -1;
            editableTerrain.selectedPrefab.clone = -1;
        }

        EditorGUILayout.EndVertical();
    }
}
