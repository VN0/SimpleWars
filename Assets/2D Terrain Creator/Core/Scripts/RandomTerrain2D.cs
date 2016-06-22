using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class RandomTerrain2D : Terrain2D
{
    public int previewCurves = 1;
    public float minLength = 2;
    public float maxLength = 3;
    public float minHeight = 0;
    public float maxHeight = 2;


    public void SetPreviewCurves(int previewCurves)
    {
        this.previewCurves = (int)Mathf.Clamp(previewCurves, 1, Mathf.Infinity);
    }

    public void SetMinLenght(float minLenght)
    {
        this.minLength = Mathf.Clamp(minLenght, 0f, Mathf.Infinity);
    }

    public void SetMaxLenght(float maxLenght)
    {
        this.maxLength = Mathf.Clamp(maxLenght, minLength, Mathf.Infinity);
    }

    public void SetMinHeight(float minHeight)
    {
        this.minHeight = minHeight;
    }

    public void SetMaxHeight(float maxHeight)
    {
        this.maxHeight = Mathf.Clamp(maxHeight, minHeight, Mathf.Infinity);
    }


    private void AddCurve(int index)
    {
        Vector3 middlePoint = GetControlPoint(index);
        float x = Random.Range(minLength, maxLength);

        controlPoints.Insert(index + 1, middlePoint);
        middlePoint.x += Random.Range(x / 4, x / 2);
        middlePoint.y = Random.Range(minHeight, maxHeight);
        controlPoints[index + 1] = middlePoint;

        controlPoints.Insert(index + 2, middlePoint);
        middlePoint.x += x - Random.Range(x / 4, x / 2);
        middlePoint.y = Random.Range(minHeight, maxHeight);
        controlPoints[index + 2] = middlePoint;

        controlPoints.Insert(index + 3, middlePoint);
        middlePoint.x += x;
        middlePoint.y = Random.Range(minHeight, maxHeight);
        controlPoints[index + 3] = middlePoint;

        modes[(index + 1) / 3] = BezierControlPointMode.Mirrored;
        modes.Insert((index + 1) / 3, BezierControlPointMode.Mirrored);      

        AddChunk(index);

        UpdateChunkColliderHolder(index);

        if (index - 3 >= 0)
        {
            UpdateChunkMesh(index - 3);
        }

        EnforceMode(index);       

        UpdateHeight(index);

        foreach (RandomPrefab prefab in prefabs)
        {
            prefab.startClone = prefab.lastClone;
        }
    }   
}

public partial class RandomTerrain2D : Terrain2D
{
    [System.Serializable]
    public class RandomPrefab : Prefab
    {
        public bool foldout;

        public float offset;
        public float zPosition;

        public float minRepeatDistance = 1;
        public float maxRepeatDistance = 1;

        public bool groupEnabled;
        public int maxGroupSize = 2;
        public int minGroupSize = 2;
        public float groupSpacing = 1;

        public int lastGroupSize;
        public int lastClone;
        public int startClone;
        public float lastCloneXPosition;
    }

    public List<RandomPrefab> prefabs = new List<RandomPrefab>();


    public void SetPrefabOffset(RandomPrefab prefab, float offset)
    {
        prefab.offset = Mathf.Clamp(offset, 0f, Mathf.Infinity);
    }

    public void SetPrefabZPosition(RandomPrefab prefab, float zPosition)
    {
        prefab.zPosition = Mathf.Clamp(zPosition, 0f, top.width);
    }

    public void SetPrefabMinRepeatDistance(RandomPrefab prefab, float minRepeatDistance)
    {
        prefab.minRepeatDistance = Mathf.Clamp(minRepeatDistance, 0f, Mathf.Infinity);
    }

    public void SetPrefabMaxRepeatDistance(RandomPrefab prefab, float maxRepeatDistance)
    {
        prefab.maxRepeatDistance = Mathf.Clamp(maxRepeatDistance, prefab.minRepeatDistance, Mathf.Infinity);
    }

    public void EnablePrefabGroup(RandomPrefab prefab, bool enabled)
    {
        prefab.groupEnabled = enabled;

        if (!enabled)
        {
            prefab.minGroupSize = 1;
            prefab.maxGroupSize = 1;
        }
    }

    public void SetPrefabMinGroupSize(RandomPrefab prefab, int minGroupSize)
    {
        prefab.minGroupSize = (int)Mathf.Clamp(minGroupSize, 1, Mathf.Infinity);
    }

    public void SetPrefabMaxGroupSize(RandomPrefab prefab, int maxGroupSize)
    {
        prefab.maxGroupSize = (int)Mathf.Clamp(maxGroupSize, prefab.minGroupSize, Mathf.Infinity);
    }

    public void SetPrefabGroupSpacing(RandomPrefab prefab, float groupSpacing)
    {
        prefab.groupSpacing = Mathf.Clamp(groupSpacing, 0f, Mathf.Infinity);
    }
    

    protected override void SetFurthestPrefabCloneXPosition()
    {
        foreach (RandomPrefab prefab in prefabs)
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

    protected override void AdjustAllPrefabClonesToGraphicsMode()
    {
        base.AdjustAllPrefabClonesToGraphicsMode();

        foreach (RandomPrefab prefab in prefabs)
        {
            foreach (Prefab.Clone clone in prefab.clones)
            {
                clone.savedZPosition = prefab.zPosition;
                AdjustPrefabCloneToGraphicsMode(clone);
            }
        }
    }


    private void AddPrefabClone(RandomPrefab prefab, Vector3 position)
    {
        if (prefab.prefabToClone != null)
        {
            Prefab.Clone newClone = new Prefab.Clone();

            newClone.cloneObjet = Instantiate(prefab.prefabToClone, transform.TransformPoint(position), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
            newClone.cloneObjet.transform.Rotate(Vector3.forward, transform.rotation.eulerAngles.z);
            newClone.offset = prefab.offset;
            newClone.savedZPosition = (graphics == GraphicsMode.TwoD) ? prefab.zPosition : 0f;

            AdjustPrefabCloneToChunk(newClone);
            prefab.clones.Add(newClone);
        }
    }

    public void GenerateRandomPrefabsClones()
    {
        foreach (RandomPrefab prefab in prefabs)
        {
            if (prefab.lastCloneXPosition > controlPoints[controlPoints.Count - 1].x) continue;

            int groupSize = prefab.lastGroupSize != 0 ? prefab.lastGroupSize : Random.Range(prefab.minGroupSize, prefab.maxGroupSize + 1);

            for (int i = prefab.startClone; i < groupSize; i++)
            {
                float zPosition = (graphics == GraphicsMode.ThreeD) ? prefab.zPosition : 0f;
                Vector3 newPosition = GetPosition(prefab.lastCloneXPosition, zPosition, prefab.offset);

                if (prefab.lastCloneXPosition <= controlPoints[controlPoints.Count - 1].x)
                {
                    AddPrefabClone(prefab, newPosition);

                    if (i < groupSize - 1)
                    {
                        prefab.lastCloneXPosition = prefab.lastCloneXPosition + prefab.groupSpacing;

                        if (prefab.lastCloneXPosition > controlPoints[controlPoints.Count - 1].x)
                        {
                            if ((i + 1) >= (groupSize))
                            {
                                prefab.lastClone = 0;
                            }
                            else
                            {
                                prefab.lastClone = i + 1;
                            }

                            prefab.lastGroupSize = groupSize;
                        }
                        else
                        {
                            prefab.lastGroupSize = 0;
                        }
                    }
                    else
                    {
                        prefab.lastCloneXPosition = (prefab.lastCloneXPosition + Random.Range(prefab.minRepeatDistance, prefab.maxRepeatDistance));

                        prefab.lastClone = 0;
                        prefab.lastGroupSize = 0;
                    }                  
                }

                prefab.startClone = 0;
            }
        }
    }


    public void DeletePrefabClones(RandomPrefab prefab)
    {
        foreach (Prefab.Clone clone in prefab.clones)
        {
            DestroyImmediate(clone.cloneObjet);
        }

        prefab.clones.Clear();
    }

    public void DeleteAllPrefabClones()
    {
        foreach (RandomPrefab prefab in prefabs)
        {
            DeletePrefabClones(prefab);
        }
    }
}

public partial class RandomTerrain2D : Terrain2D
{
    private bool coroutineEnded = false;


    public override void Reset()
    {
        DeleteAllPrefabClones();

        foreach (RandomPrefab prefab in prefabs)
        {
            prefab.lastClone = 0;
            prefab.lastCloneXPosition = Vector3.Distance(Vector3.zero, ((transform.rotation * Vector3.right).normalized * Random.Range(prefab.minRepeatDistance, prefab.maxRepeatDistance)));
        }

        base.Reset();

        AddCurve(0);

        for (int i = 1; i < previewCurves; i++)
        {
            AddCurve((i * 3));
        }
    }

    protected override void Start()
    {
        StartCoroutine(GeneratePreviousCurves());

        base.Start();
    }

    private IEnumerator GeneratePreviousCurves()
    {
        Vector3 viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[controlPoints.Count - 1]));

        bool loop = false;

        while (viewPosition.x < -0.5f)
        {
            AddCurve(controlPoints.Count - 1);

            chunks[(int)(controlPoints.Count - 2) / 3].chunkObject.transform.FindChild("Mesh").gameObject.SetActive(false);
            chunks[(int)(controlPoints.Count - 2) / 3].chunkObject.transform.FindChild("Prefabs").gameObject.SetActive(false);
            chunks[(int)(controlPoints.Count - 2) / 3].chunkObject.transform.FindChild("Collider").gameObject.SetActive(false);

            viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[controlPoints.Count - 1]));

            loop = true;
            yield return null;
        }

        if (loop)
        {
            AddCurve(controlPoints.Count - 1);
            renderedCurves.Add((int)(controlPoints.Count - 2) / 3);
        }

        coroutineEnded = true;
    }

    protected override void Update()
    {
        if (!coroutineEnded) return;

        GenerateRandomPrefabsClones();

        if (GameObject.FindWithTag("MainCamera") != null)
        {
            Vector3 viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[controlPoints.Count - 1]));

            if (viewPosition.x < 1.5f)
            {
                if (renderedCurves.Count > 0 && renderedCurves[renderedCurves.Count - 1] == chunks.Count - 1)
                {
                    AddCurve(controlPoints.Count - 1);
                    renderedCurves.Add(renderedCurves[renderedCurves.Count - 1] + 1);
                }
            }
        }

        base.Update();       
    }    
}