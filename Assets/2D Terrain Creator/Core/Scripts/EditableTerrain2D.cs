#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System;
using System.Collections.Generic;

public partial class EditableTerrain2D : Terrain2D
{
    #if UNITY_EDITOR
    public int selectedControlPoint;
    #endif

    public Vector3 leftControlPointPosition = new Vector3(-1f,0f,0f);
    public Vector3 rightControlPointPosition = new Vector3(1f, 0f, 0f);


    public void AddCurve(int index)
    {
        Vector3 middlePoint = GetControlPoint(index);

        #if UNITY_EDITOR
        Undo.RecordObject(this, "Control Points and Modes");
        #endif

        controlPoints.Insert(index + 1, middlePoint);
        middlePoint.x += 1;
        controlPoints[index + 1] = middlePoint;

        controlPoints.Insert(index + 2, middlePoint);
        middlePoint.x += 1;
        controlPoints[index + 2] = middlePoint;

        controlPoints.Insert(index + 3, middlePoint);
        middlePoint.x += 1;
        controlPoints[index + 3] = middlePoint;

        for (int i = 0; i < controlPoints.Count - 3; i++)
        {
            if (i > index)
            {
                Vector3 newPoint = controlPoints[3 + i];
                newPoint.x += 3;
                controlPoints[3 + i] = newPoint;
            }
        }     

        if (index != 0) modes[(index + 1) / 3] = BezierControlPointMode.Free;
        modes.Insert((index + 1) / 3, BezierControlPointMode.Free);

        SetRightControlPointPosition(index);

        AddChunk(index);

        if (index - 3 >= 0)
        {
            UpdateChunkMesh(index - 3);
        }

        for (int a = 0; a < controlPoints.Count - 1; a += 3)
        {
            if (a >= index)
            {
                UpdateChunkColliderHolder(a);
                UpdateChunkMesh(a);
            }
        }

        foreach (Prefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                if (transform.InverseTransformPoint(clone.cloneObjet.transform.position).x > controlPoints[index].x)
                {
                    #if UNITY_EDITOR
                    Undo.RecordObject(clone.cloneObjet.transform, "Prefab Clone Transform");
                    #endif

                    clone.cloneObjet.transform.position = new Vector3(clone.cloneObjet.transform.position.x + 3, clone.cloneObjet.transform.position.y, clone.cloneObjet.transform.position.z);
                    AdjustPrefabCloneToChunk(clone);
                }
            }
        }   
    }

    public void DeleteCurve(int index)
    {
        DeleteAllPrefabClonesInCurve(index);

        float distance = controlPoints[index + 3].x - controlPoints[index].x;

        #if UNITY_EDITOR
        Undo.RecordObject(this, "Control Points and Modes");
        #endif

        controlPoints.RemoveAt(index + 1);
        controlPoints.RemoveAt(index + 1);
        controlPoints.RemoveAt(index + 1);

        for (int i = 0; i < controlPoints.Count; i++)
        {
            if (i > index)
            {
                Vector3 newPoint = controlPoints[i];
                newPoint.x -= 3;
                controlPoints[i] = newPoint;
            }
        }     

        modes[((index + 1) / 3) + 1] = BezierControlPointMode.Free;
        modes.RemoveAt((index + 1) / 3);

        DeleteChunk(index);

        if (index - 3 >= 0)
        {
            UpdateChunkMesh(index - 3);
        }

        for (int a = 0; a < controlPoints.Count - 1; a += 3)
        {
            if (a >= index)
            {
                UpdateChunkColliderHolder(a);
                UpdateChunkMesh(a);
            }
        }

        foreach (Prefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                if (transform.InverseTransformPoint(clone.cloneObjet.transform.position).x > controlPoints[index].x)
                {
                    #if UNITY_EDITOR
                    Undo.RecordObject(clone.cloneObjet.transform, "Prefab Clone Transform");
                    #endif

                    clone.cloneObjet.transform.position = new Vector3(clone.cloneObjet.transform.position.x - distance, clone.cloneObjet.transform.position.y, clone.cloneObjet.transform.position.z);
                    AdjustPrefabClone(clone);
                }
            }
        }   
    }

    public void DeletePoint(int index)
    {
        int modeIndex = (index + 1) / 3;
        int middleIndex = modeIndex * 3;

        foreach (Prefab.Clone clone in GetAllPrefabClones(middleIndex))
        {
            #if UNITY_EDITOR

            Undo.SetTransformParent(clone.cloneObjet.transform, transform, "Prefab Clone Parent (OUT)");   
            Undo.RecordObject(this, "Prefab Clone");

            #else
            clone.cloneObjet.transform.parent = transform;
            #endif
        }

        #if UNITY_EDITOR
        Undo.RecordObject(this, "Cotrol Points and Modes");
        #endif

        controlPoints.RemoveAt(index - 1);
        controlPoints.RemoveAt(index - 1);
        controlPoints.RemoveAt(index - 1);       

        modes[((index + 1) / 3) + 1] = BezierControlPointMode.Free;
        modes.RemoveAt((index + 1) / 3);

        DeleteChunk(index);
        DeleteChunk(index - 3);
        AddChunk(index);

        for (int a = 0; a < controlPoints.Count - 1; a += 3)
        {
            if (a >= index - 3)
            {
                UpdateChunkColliderHolder(a);
                UpdateChunkMesh(a);
            }
        }

        foreach (Prefab.Clone clone in GetAllPrefabClones(index - 3))
        {
            #if UNITY_EDITOR
            Undo.SetTransformParent(clone.cloneObjet.transform, chunks[GetCurve(index - 3)].chunkObject.transform.FindChild("Prefabs"), "Prefab Clone Parent (IN)");
            #else
            clone.cloneObjet.transform.parent = chunks[GetCurve(index - 3)].chunkObject.transform.FindChild("Prefabs");
            #endif

            AdjustPrefabCloneToChunk(clone);
            AdjustPrefabClone(clone);
        }
    }


    public void SetleftControlPointPosition(int index)
    {
        Vector3 directionLeft = (controlPoints[index - 1] - controlPoints[index]).normalized;
        float distanceLeft = Vector3.Distance(controlPoints[index], controlPoints[index - 1]);
        leftControlPointPosition = directionLeft * distanceLeft;
    }

    public void SetRightControlPointPosition(int index)
    {
        Vector3 directionRight = (controlPoints[index + 1] - controlPoints[index]).normalized;
        float distanceRight = Vector3.Distance(controlPoints[index], controlPoints[index + 1]);
        rightControlPointPosition = directionRight * distanceRight;
    }


    public void MoveControlPoint(int index, Vector3 point)
    {
        #if UNITY_EDITOR
        Undo.RecordObject(this, "Control Point Position");
        #endif

        int modeIndex = (index + 1) / 3;
        int middleIndex = modeIndex * 3;

        if (index == 0)
        {
            List<Vector3> lastControlPoints = new List<Vector3>();
            List<Vector3> lastPrefabs = new List<Vector3>();

            foreach (Vector3 controlPoint in controlPoints)
            {
                lastControlPoints.Add(transform.TransformPoint(controlPoint));
            }

            foreach (Prefab prefab in prefabs)
            {
                foreach (Prefab.Clone clone in prefab.clones)
                {
                    lastPrefabs.Add(clone.cloneObjet.transform.position);
                }
            }

            #if UNITY_EDITOR

            foreach (Prefab.Clone clone in GetAllPrefabClonesInCurve(index))
            {          
                Undo.RecordObject(clone.cloneObjet.transform, "Prefab Clone Transform");
            }
            
            Undo.RecordObject(transform, "Transform");

            #endif

            transform.position = new Vector3(transform.TransformPoint(controlPoints[0]).x, transform.TransformPoint(controlPoints[0]).y, transform.position.z);
            controlPoints[0] = Vector3.zero;

            for (int i = 1; i < controlPoints.Count; i++)
            {
                controlPoints[i] = transform.InverseTransformPoint(lastControlPoints[i]);
            }

            foreach (Prefab prefab in prefabs)
            {
                foreach (Prefab.Clone clone in prefab.clones)
                {
                    clone.cloneObjet.transform.position = lastPrefabs[prefab.clones.IndexOf(clone)];
                }
            }
        }

        if (index == controlPoints.Count - 1)
        {
            point = new Vector3(Mathf.Clamp(point.x, RightFurthestPrefabClone() + 0.01f, Mathf.Infinity), point.y, transform.TransformPoint(Vector3.zero).z);
        }
        else if (index == 0)
        {
            point = new Vector3(Mathf.Clamp(point.x, -Mathf.Infinity, LeftFurthestPrefabClone() - 0.01f), point.y, transform.TransformPoint(Vector3.zero).z);
            UpdateAllChunkMeshes();
        }

        if (index == middleIndex)
        {
            float leftDistance = index - 1 > 0 ? ((controlPoints[index].x - controlPoints[index - 1].x) > (controlPoints[index - 2].x - controlPoints[index - 3].x)) ? controlPoints[index].x - controlPoints[index - 1].x : controlPoints[index - 2].x - controlPoints[index - 3].x : 0f;
            float leftLimitPoint = index - 1 > 0 ? transform.TransformPoint(controlPoints[index - 3]).x + leftDistance : -Mathf.Infinity;
            float rightDistance = index + 1 < controlPoints.Count - 1 ? ((controlPoints[index + 1].x - controlPoints[index].x) > (controlPoints[index + 3].x - controlPoints[index + 2].x)) ? controlPoints[index + 1].x - controlPoints[index].x : controlPoints[index + 3].x - controlPoints[index + 2].x : 0f;
            float rightLimitPoint = index + 1 < controlPoints.Count - 1 ? transform.TransformPoint(controlPoints[index + 3]).x - rightDistance : Mathf.Infinity;

            Vector3 middlePoint = new Vector3(Mathf.Clamp(point.x, leftLimitPoint, rightLimitPoint), point.y, transform.TransformPoint(Vector3.zero).z);

            SetControlPoint(index, transform.InverseTransformPoint(middlePoint));
            EnforceMode(index);

            #if UNITY_EDITOR
            Undo.RecordObject(this, "Left Control Point Position");
            #endif

            if (index - 1 > 0) SetControlPoint(index - 1, controlPoints[index] + leftControlPointPosition);

            #if UNITY_EDITOR
            Undo.RecordObject(this, "Right Control Point Position");
            #endif

            if (index + 1 < controlPoints.Count - 1) SetControlPoint(index + 1, controlPoints[index] + rightControlPointPosition);

            if (index == middleIndex)
            {
                if (index == 0)
                {
                    UpdateHeight();
                }
                else
                {
                    if (index != controlPoints.Count - 1)
                    {
                        UpdateHeight(GetCurve(index) * 3);
                    }

                    if (index - 1 >= 0)
                    {
                        UpdateHeight(GetCurve(index - 1) * 3);
                    }
                }
            }
            else
            {
                UpdateHeight(GetCurve(index) * 3);
            }

            AdjustAllPrefabClonesInCurve(index, true);
            if (index != 0) AdjustAllPrefabClonesInCurve(index - 3, true);
        }
        else
        {
            Vector3 middlePoint = transform.TransformPoint(controlPoints[(int)middleIndex]);
            Vector3 newPoint = Vector3.zero;

            if (index - 1 != 0 && index + 1 != controlPoints.Count - 1)
            {
                if (index == (int)middleIndex + 1)
                {
                    float distance = controlPoints[index - 1].x - controlPoints[index - 4].x;
                    float rightLimitPoint = transform.TransformPoint(controlPoints[(((index - 1 + 1) / 3) * 3) - 1]).x <= transform.TransformPoint(controlPoints[index - 4]).x ? middlePoint.x + distance : transform.TransformPoint(controlPoints[(int)middleIndex + 3]).x;
                    newPoint = new Vector3(Mathf.Clamp(point.x, middlePoint.x, rightLimitPoint), point.y, point.z);
                }

                if (index == (int)middleIndex - 1)
                {
                    float distance = controlPoints[index + 4].x - controlPoints[index + 1].x;
                    float leftLimitPoint = transform.TransformPoint(controlPoints[(int)middleIndex + 1]).x >= transform.TransformPoint(controlPoints[index + 4]).x ? middlePoint.x - distance : transform.TransformPoint(controlPoints[(int)middleIndex - 3]).x;
                    newPoint = new Vector3(Mathf.Clamp(point.x, leftLimitPoint, middlePoint.x), point.y, point.z);
                }
            }
            else
            {
                float leftLimitPoint = transform.TransformPoint(controlPoints[((index - 1 + 1) / 3) * 3]).x;
                float rightLimitPoint = transform.TransformPoint(controlPoints[((index + 1 + 1) / 3) * 3]).x;

                newPoint = new Vector3(Mathf.Clamp(point.x, leftLimitPoint, rightLimitPoint), point.y, point.z);
            }

            SetControlPoint(index, transform.InverseTransformPoint(new Vector3(newPoint.x, newPoint.y, transform.TransformPoint(Vector3.zero).z)));
            EnforceMode(index);

            UpdateHeight(GetCurve(index) * 3);

            AdjustAllPrefabClonesInCurve(index, false);

            BezierControlPointMode mode = modes[modeIndex];

            if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Count - 1)
            {
                return;
            }

            int enforcedIndex;

            if (index <= middleIndex)
            {
                enforcedIndex = middleIndex + 1;
            }
            else
            {
                enforcedIndex = middleIndex - 1;
            }

            UpdateHeight(GetCurve(enforcedIndex) * 3);

            AdjustAllPrefabClonesInCurve(enforcedIndex, false);    
        }       
    }    
}

public partial class EditableTerrain2D : Terrain2D
{
    #if UNITY_EDITOR

    [Serializable]
    public class SelectedPrefab
    {
        public int type = -1;
        public int clone = -1;
    }

    public SelectedPrefab selectedPrefab = new SelectedPrefab();

    #endif

    public List<Prefab> prefabs = new List<Prefab>();


    protected override void SetFurthestPrefabCloneXPosition()
    {
        foreach (Prefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                List<GameObject> closestClones = new List<GameObject>();

                for (int i = 0; i < prefab.clones.Count; i++)
                {
                    if (clone.cloneObjet.transform.position.z < prefab.clones[i].cloneObjet.transform.position.z)
                    {
                        closestClones.Add(clone.cloneObjet);
                    }
                }

                if (closestClones.Count == 0)
                {
                    furthestPrefabCloneXPosition = clone.cloneObjet.transform.position.z;
                }
            }
        }

        base.SetFurthestPrefabCloneXPosition();
    }

    private float RightFurthestPrefabClone()
    {
        foreach (Prefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                List<GameObject> closestClones = new List<GameObject>();

                for (int i = 0; i < prefab.clones.Count; i++)
                {
                    if (clone.cloneObjet.transform.position.x < prefab.clones[i].cloneObjet.transform.position.x)
                    {
                        closestClones.Add(clone.cloneObjet);
                    }
                }

                if (closestClones.Count == 0)
                {
                    return clone.cloneObjet.transform.position.x;
                }
            }
        }

        return transform.TransformPoint(controlPoints[0]).x;
    }

    private float LeftFurthestPrefabClone()
    {
        foreach (Prefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                List<GameObject> closestClones = new List<GameObject>();

                for (int i = 0; i < prefab.clones.Count; i++)
                {
                    if (clone.cloneObjet.transform.position.x > prefab.clones[i].cloneObjet.transform.position.x)
                    {
                        closestClones.Add(clone.cloneObjet);
                    }
                }

                if (closestClones.Count == 0)
                {
                    return clone.cloneObjet.transform.position.x;
                }
            }
        }

        return Mathf.Infinity;
    }


    public void AddPrefabClone(Prefab prefab, Vector3 position, float offset)
    {
        if (prefab.prefabToClone != null)
        {
            #if UNITY_EDITOR
            Undo.RecordObject(this, "Prefab Clones");
            #endif

            Prefab.Clone newClone = new Prefab.Clone();

            newClone.cloneObjet = Instantiate(prefab.prefabToClone, transform.TransformPoint(position), Quaternion.identity) as GameObject;
            newClone.savedZPosition = transform.TransformPoint(Vector3.zero).z;
            newClone.offset = offset;

            prefab.clones.Add(newClone);

            #if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(newClone.cloneObjet, "Create Prefab Clone");
            #endif

            AdjustPrefabCloneToChunk(newClone);
        }
    }

    public void MovePrefabClone(Prefab prefab, int index, Vector3 position, float offset)
    {
        #if UNITY_EDITOR

        Undo.RecordObject(this, "Prefab Clones");
        Undo.RecordObject(prefab.clones[index].cloneObjet.transform, "1");

        #endif

        prefab.clones[index].offset = offset;
        prefab.clones[index].cloneObjet.transform.position = transform.TransformPoint(GetPosition(position.x, position.z, prefab.clones[index].offset));
        AdjustPrefabCloneToChunk(prefab.clones[index]);
    }

    public void DeletePrefabClone(Prefab prefab, Prefab.Clone clone)
    {
        #if UNITY_EDITOR

        Undo.DestroyObjectImmediate(clone.cloneObjet);
        Undo.RecordObject(this, "Prefab Clones");

        #else
        DestroyObject(clone.cloneObjet);
        #endif

        prefab.clones.Remove(clone);
    }


    public List<Prefab.Clone> GetAllPrefabClonesInCurve(int index)
    {
        List<Prefab.Clone> prefabClones = new List<Prefab.Clone>();

        int modeIndex = (index + 1) / 3;
        int middleIndex = modeIndex * 3;
        int myIndex = (index >= middleIndex) ? middleIndex : middleIndex - 3;

        foreach (Prefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                if (transform.InverseTransformPoint(clone.cloneObjet.transform.position).x >= controlPoints[myIndex].x && transform.InverseTransformPoint(clone.cloneObjet.transform.position).x <= controlPoints[myIndex + 3].x)
                {
                    prefabClones.Add(clone);
                }
            }
        }

        return prefabClones;
    }

    public List<Prefab.Clone> GetAllPrefabClones(int index)
    {
        List<Prefab.Clone> prefabClones = new List<Prefab.Clone>();

        foreach (Prefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                prefabClones.Add(clone);              
            }
        }

        return prefabClones;
    }


    protected override void AdjustAllPrefabClonesToGraphicsMode()
    {
        foreach (Prefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                AdjustPrefabCloneToGraphicsMode(clone);
            }
        }
    }

    private void AdjustAllPrefabClonesInCurve(int index, bool toChunk)
    {
        foreach (Prefab.Clone clone in GetAllPrefabClonesInCurve(index))
        {
            #if UNITY_EDITOR
            Undo.RecordObject(clone.cloneObjet.transform, "Prefab Clone Transform");
            #endif

            AdjustPrefabClone(clone);
            if (toChunk) AdjustPrefabCloneToChunk(clone);        
        }
    }


    public class StorePrefabClone
    {
        public Prefab.Clone clone;
        public Prefab prefab;
    }

    public void DeleteAllPrefabClonesInCurve(int index)
    {
        List<StorePrefabClone> deletePrefabsClones = new List<StorePrefabClone>();

        foreach (Prefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                if (transform.InverseTransformPoint(clone.cloneObjet.transform.position).x >= controlPoints[index].x && transform.InverseTransformPoint(clone.cloneObjet.transform.position).x <= controlPoints[index + 3].x)
                {
                    StorePrefabClone deleteClone = new StorePrefabClone();
                    deleteClone.prefab = prefab;
                    deleteClone.clone = clone;
                    deletePrefabsClones.Add(deleteClone);
                }
            }
        }

        foreach (StorePrefabClone deleteClone in deletePrefabsClones)
        {
            DeletePrefabClone(deleteClone.prefab, deleteClone.clone);
        }
    }

    public void DeletePrefabClones(Prefab prefab)
    {
        List<StorePrefabClone> deletePrefabsClones = new List<StorePrefabClone>();

        foreach (Prefab.Clone clone in prefab.clones)
        {
            StorePrefabClone deleteClone = new StorePrefabClone();
            deleteClone.prefab = prefab;
            deleteClone.clone = clone;
            deletePrefabsClones.Add(deleteClone);
        }

        foreach (StorePrefabClone deleteClone in deletePrefabsClones)
        {
            DeletePrefabClone(deleteClone.prefab, deleteClone.clone);
        }
    }
    

    public void DeleteAllPrefabClones()
    {
        foreach (Prefab prefab in prefabs)
        {
            DeletePrefabClones(prefab);
        }
    }

    public void ReplacePrefabClones(Prefab prefab)
    {
        Prefab savedPrefab = prefab;
        List<Vector3> savedClonesPosition = new List<Vector3>();
        List<float> savedClonesOffset = new List<float>();

        foreach (Prefab.Clone clone in prefab.clones)
        {
            savedClonesPosition.Add(transform.InverseTransformPoint(clone.cloneObjet.transform.position));
            savedClonesOffset.Add(clone.offset);
        }

        DeletePrefabClones(prefab);

        for (int i = 0; i < savedClonesPosition.Count; i++)
        {
            AddPrefabClone(savedPrefab, savedClonesPosition[i], savedClonesOffset[i]);
        }
    }
}

public partial class EditableTerrain2D : Terrain2D
{
    public override void Reset()
    {
        #if UNITY_EDITOR

        Undo.RecordObject(this, "Reset");

        undo = true;
        selectedControlPoint = -1;
        selectedPrefab.clone = -1;
        selectedPrefab.type = -1;

        #endif

        DeleteAllPrefabClones();

        base.Reset();

        AddCurve(0);
    }
}
