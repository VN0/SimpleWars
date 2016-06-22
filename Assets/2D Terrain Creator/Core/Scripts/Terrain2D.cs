#if UNITY_EDITOR 
using UnityEditor;
#endif

using UnityEngine;
using System;
using System.Collections.Generic;

public abstract partial class Terrain2D : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();
    public List<Vector3> controlPoints = new List<Vector3>();
    public enum BezierControlPointMode { Free, Aligned, Mirrored };
    public List<BezierControlPointMode> modes = new List<BezierControlPointMode>();


    public Vector3 GetPoint(int index, float t)
    {
        return Bezier.GetPoint(controlPoints[index], controlPoints[index + 1], controlPoints[index + 2], controlPoints[index + 3], t);
    }


    public float GetCurveLength(int index)
    {
        Vector3 segmentStart = GetPoint(index, 0f);
        float length = 0;

        for (int j = 0; j <= 10; j++)
        {
            Vector3 segmentEnd = GetPoint(index, j / 10f);
            length += Vector3.Distance(segmentStart, segmentEnd);
            segmentStart = segmentEnd;
        }

        return length;
    }

    public int GetCurveSegments(int index)
    {
        return (int)(GetCurveLength(index) / segmentsLength);
    }

    public Vector3 GetCurveLowestPoint(int index)
    {
        Vector3 point = GetPoint(index, 0f);

        for (int i = 0; i < GetCurveSegments(index); i++)
        {
            Vector3 segmentEnd = GetPoint(index, i / (float)GetCurveSegments(index));

            if (segmentEnd.y < point.y) point = segmentEnd;
        }

        return point;     
    }

    public Vector3 GetLowestPoint()
    {
        Vector3 point = GetCurveLowestPoint(0).y > 0f ? Vector3.zero : GetCurveLowestPoint(0);

        for (int i = 0; i < CurveCount * 3; i += 3)
        {
            if (GetCurveLowestPoint(i).y < point.y) point = GetCurveLowestPoint(i);
        }

        return point;
    }


    public int CurveCount
    {
        get
        {
            return (controlPoints.Count - 1) / 3;
        }
    }

    public int GetCurve(int index)
    {
        return ((int)(index / 3));
    }

    public int GetCurve(float x)
    {
        for (int i = 1; i < controlPoints.Count; i += 3)
        {
            if (x >= GetPoint(i - 1, 0f).x && x <= GetPoint(i - 1, 1f).x)
            {
                return i;
            }
        }

        return -1;
    }


    public Vector3 GetControlPoint(int index)
    {
        return controlPoints[index];
    }

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Control Point Position");
        #endif

        controlPoints[index] = point;
        UpdateChunkMesh(GetCurve(index == controlPoints.Count - 1 ? index - 3 : index) * 3);
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Control Point Mode");
        #endif

        modes[(index + 1) / 3] = mode;
        EnforceMode(index);
    }

    protected void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        int middleIndex = modeIndex * 3;
        BezierControlPointMode mode = modes[modeIndex];
        
        if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Count - 1)
        {        
            return;
        }
    
        int fixedIndex, enforcedIndex;

        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
        }
        else
        {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
        }

        Vector3 middle = controlPoints[middleIndex];
        Vector3 enforcedTangent = middle - controlPoints[fixedIndex];

        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, controlPoints[enforcedIndex]);
        }

        controlPoints[enforcedIndex] = middle + enforcedTangent;

        UpdateChunkMesh(GetCurve(enforcedIndex) * 3);
    }


    public Vector3 StartOrEndPosition(float x, float z, float offset)
    {
        float zPosition = (graphics == GraphicsMode.ThreeD) ? Mathf.Clamp(z, 0f, top.width) : 0f;
        Vector3 startPoint = (new Vector3(controlPoints[0].x + 0.001f, controlPoints[0].y + offset, zPosition));
        Vector3 endPoint = (new Vector3(controlPoints[controlPoints.Count - 1].x, controlPoints[controlPoints.Count - 1].y + offset, zPosition));
        return Mathf.Abs(x - (controlPoints[0]).x) < Mathf.Abs(x - (controlPoints[controlPoints.Count - 1]).x) ? startPoint : endPoint;
    }

    public Vector3 GetPosition(float x, float z, float offset)
    {
        float zPosition = (graphics == GraphicsMode.ThreeD) ? Mathf.Clamp(z, 0f, top.width) : 0f;

        for (int i = 0; i < controlPoints.Count - 1; i += 3)
        {
            int segments = GetCurveSegments(i);

            Vector3 segmentStart = GetPoint(i, 0f);

            for (int j = 0; j <= segments; j++)
            {
                Vector3 segmentEnd = (GetPoint(i, j / (float)segments));

                if (x >= segmentStart.x && x <= segmentEnd.x)
                {
                    float xPosition = x - segmentStart.x;
                    float length = xPosition / (segmentEnd.x - segmentStart.x);
                    Vector3 direction = segmentEnd - segmentStart;

                    Vector3 newPosition = segmentStart + direction.normalized * Vector3.Distance(segmentEnd, segmentStart) * length;

                    return new Vector3(newPosition.x, newPosition.y + offset, zPosition);
                }

                segmentStart = segmentEnd;
            }
        }

        return (x >= controlPoints[0].x && x <= controlPoints[controlPoints.Count - 1].x) ? new Vector3(x, offset, zPosition) : StartOrEndPosition(x, zPosition, offset);
    }
}

public abstract partial class Terrain2D : MonoBehaviour
{
    [Serializable]
    public class Chunk
    {
        public GameObject chunkObject;
    }

    public List<Chunk> chunks = new List<Chunk>();


    protected void AddChunk(int index)
    {
        Chunk newChunk = new Chunk();     

        GameObject chunkObject = new GameObject();
        chunkObject.name = "Chunk";
        chunkObject.transform.rotation = transform.rotation;
        chunkObject.transform.parent = gameObject.transform;
        chunkObject.transform.localPosition = Vector3.zero;

        GameObject meshObject = new GameObject();
        meshObject.name = "Mesh";
        meshObject.transform.parent = chunkObject.transform;
        meshObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        meshObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        meshObject.AddComponent<MeshFilter>();
        meshObject.AddComponent<MeshRenderer>();

        GameObject prefabsObject = new GameObject();
        prefabsObject.name = "Prefabs";
        prefabsObject.transform.parent = chunkObject.transform;
        prefabsObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        newChunk.chunkObject = chunkObject;
        chunks.Add(newChunk);

        #if UNITY_EDITOR
        if (undo) Undo.RegisterCreatedObjectUndo(chunkObject, "Created Chunk");
        #endif      
    }

    protected void DeleteChunk(int index)
    {
        if (undo)
        {
            #if UNITY_EDITOR
            Undo.DestroyObjectImmediate(chunks[(int)(index / 3)].chunkObject);
            #endif
        }
        else
        {
            DestroyImmediate(chunks[(int)(index / 3)].chunkObject);
        }

        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Chunks");
        #endif

        chunks.RemoveAt((int)(index / 3));
    }

    protected void DeleteAllChunks()
    {
        foreach (Chunk chunk in chunks)
        {
            if (undo)
            {
                #if UNITY_EDITOR
                Undo.DestroyObjectImmediate(chunk.chunkObject);
                #endif
            }
            else
            {
                DestroyImmediate(chunk.chunkObject);
            }
        }

        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Chunks");
        #endif

        chunks.Clear();
    }
}

public abstract partial class Terrain2D : MonoBehaviour
{
    public float segmentsLength = 0.25f;

    public enum GraphicsMode { TwoD, ThreeD }
    public GraphicsMode graphics = GraphicsMode.TwoD;
    public GraphicsMode lastGraphics = GraphicsMode.TwoD;

    public Front front = new Front();
    public Cap cap = new Cap();
    public Top top = new Top();

    [Serializable]
    public class Front
    {
        public bool flatBotttom;
        public float height = 1f;
        public Material material;
    }

    [Serializable]
    public class Cap
    {
        public bool enabled = false;
        public float height = 0.1f;
        public Material material;
    }

    [Serializable]
    public class Top
    {
        public float width = 1f;
        public Material material;
    }


    public void SetSegmentsLength(float lenght)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Segments Lenght");
        #endif

        segmentsLength = Mathf.Clamp(lenght, 0.1f, 3f);
        UpdateAllChunkMeshes();
    }

    public void SetGraphicsMode(GraphicsMode graphics)
    {
        if (graphics != lastGraphics)
        {
            #if UNITY_EDITOR
            if (undo) Undo.RecordObject(this, "Graphics Mode");
            #endif        

            this.graphics = graphics;

            UpdateAllChunkColliderHolders();
            AdjustAllPrefabClonesToGraphicsMode();
            UpdateAllChunkMeshes();
        }

        lastGraphics = this.graphics;
    }

    public void EnableCap(bool enabled)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Cap");
        #endif

        cap.enabled = enabled;
        UpdateAllChunkMeshes();

        UpdateHeight();
    }

    public void SetFlatBottom(bool enabled)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Flat Bottom");
        #endif
        
        front.flatBotttom = enabled;
        UpdateAllChunkMeshes();

        UpdateHeight();
    }

    public void SetHeight(float height)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Height");
        #endif

        front.height = Mathf.Clamp(height, front.flatBotttom ? Mathf.Abs(GetLowestPoint().y - (cap.enabled ? cap.height : 0f)) : 0f, Mathf.Infinity);
        UpdateAllChunkMeshes();
    }

    public void SetCapHeight(float height)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Cap Height");
        #endif

        cap.height = Mathf.Clamp(height, 0f, front.height);
        UpdateAllChunkMeshes();

        UpdateHeight();
    }

    public void SetWidth(float width)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Width");
        #endif

        SetFurthestPrefabCloneXPosition();

        top.width = Mathf.Clamp(width, furthestPrefabCloneXPosition != 0f ? furthestPrefabCloneXPosition : 0.0001f, Mathf.Infinity);

        UpdateAllChunkMeshes();
    }

    public void SetFrontMaterial(Material material)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Front Material");
        #endif

        front.material = material;
        UpdateAllChunkMeshes();
    }

    public void SetCapMaterial(Material material)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Cap Material");
        #endif

        cap.material = material;
        UpdateAllChunkMeshes();
    }

    public void SetTopMaterial(Material material)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Top Material");
        #endif

        top.material = material;
        UpdateAllChunkMeshes();
    }


    public void UpdateHeight()
    {
        if (front.flatBotttom)
        {
            if (GetLowestPoint().y - (cap.enabled ? cap.height : 0f) < -front.height)
            {
                SetHeight(controlPoints[0].y - (GetLowestPoint().y - (cap.enabled ? cap.height : 0f)));
            }
        }
    }

    public void UpdateHeight(int index)
    {
        if (GetCurveLowestPoint(GetCurve(index) * 3).y - (cap.enabled ? cap.height : 0f) < -front.height)
        {
            SetHeight(controlPoints[0].y - (GetCurveLowestPoint(GetCurve(index) * 3).y - (cap.enabled ? cap.height : 0f)));
        }    
    }


    public void UpdateChunkMesh(int index)
    {
        List<Mesh> meshes = new List<Mesh>();
        List<Material> materials = new List<Material>();       

        meshes.Add(FrontMesh(index));
        materials.Add(front.material);

        if (cap.enabled)
        {
            meshes.Add(CapFrontMesh(index));
            materials.Add(cap.material);
        }

        if (graphics == GraphicsMode.ThreeD)
        {
            List<Mesh> meshColliders = new List<Mesh>();

            meshes.Add(TopMesh(index));
            materials.Add(top.material);
            meshColliders.Add(TopMesh(index));     
      
            meshes.Add(BottomMesh(index));
            materials.Add(front.material);
            meshColliders.Add(BottomMesh(index));

            meshes.Add(BackMesh(index));
            materials.Add(front.material);    

            if ((int)((index + 1) / 3) == 0)
            {
                meshes.Add(LeftMesh(true));
                materials.Add(front.material);
                meshColliders.Add(LeftMesh(false));
            }

            if ((int)((index + 1) / 3) == chunks.Count - 1)
            {
                meshes.Add(RightMesh(true));
                materials.Add(front.material);
                meshColliders.Add(RightMesh(false));
            }

            if (cap.enabled)
            {
                meshes.Add(CapBackMesh(index));
                materials.Add(cap.material);

                if ((int)((index + 1) / 3) == 0)
                {
                    meshes.Add(CapLeftMesh());
                    materials.Add(cap.material);
                }

                if ((int)((index + 1) / 3) == chunks.Count - 1)
                {
                    meshes.Add(CapRightMesh());
                    materials.Add(cap.material);
                }         
            }

            Collider3D(index, meshColliders);
        }
        else 
        {
            Collider2D(index);
        }     

        CombineMeshes(index, meshes, materials);       
    }

    public void UpdateAllChunkMeshes()
    {
        for (int i = 0; i < controlPoints.Count - 1; i += 3)
        {
            UpdateChunkMesh(i);
        }
    }


    private Mesh TopMesh(int index)
    {
        int squareCount = 0;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int segments = GetCurveSegments(index);

        for (int j = 0; j < segments; j++)
        {
            Vector3 downLeft = new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y, 0f);
            Vector3 upLeft = new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y, top.width);
            Vector3 upRight = new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y, top.width);
            Vector3 downRight = new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y, 0f);

            vertices.Add(downLeft);
            vertices.Add(upLeft);
            vertices.Add(upRight);
            vertices.Add(downRight);

            triangles.Add((squareCount * 4));
            triangles.Add((squareCount * 4) + 1);
            triangles.Add((squareCount * 4) + 3);
            triangles.Add((squareCount * 4) + 1);
            triangles.Add((squareCount * 4) + 2);
            triangles.Add((squareCount * 4) + 3);

            uvs.Add(new Vector2(downLeft.x, downLeft.z));
            uvs.Add(new Vector2(upLeft.x, upLeft.z));
            uvs.Add(new Vector2(upRight.x, upRight.z));
            uvs.Add(new Vector2(downRight.x, downRight.z));

            squareCount++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }

    private Mesh BottomMesh(int index)
    {
        int squareCount = 0;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        if (front.flatBotttom)
        {
            Vector3 downLeft = new Vector3(GetPoint(index, 0f).x, - front.height, 0f);
            Vector3 upLeft = new Vector3(GetPoint(index, 0f).x, - front.height, top.width);
            Vector3 upRight = new Vector3(GetPoint(index, 1f).x, - front.height, top.width);
            Vector3 downRight = new Vector3(GetPoint(index, 1f).x,- front.height, 0f);

            vertices.Add(upLeft);
            vertices.Add(downLeft);
            vertices.Add(downRight);
            vertices.Add(upRight);

            triangles.Add((squareCount * 4));
            triangles.Add((squareCount * 4) + 1);
            triangles.Add((squareCount * 4) + 3);
            triangles.Add((squareCount * 4) + 1);
            triangles.Add((squareCount * 4) + 2);
            triangles.Add((squareCount * 4) + 3);

            uvs.Add(new Vector2(upLeft.x, upLeft.z));
            uvs.Add(new Vector2(downLeft.x, downLeft.z));
            uvs.Add(new Vector2(downRight.x, downRight.z));
            uvs.Add(new Vector2(upRight.x, upRight.z));
        }
        else
        {
            int segments = GetCurveSegments(index);

            for (int j = 0; j < segments; j++)
            {
                Vector3 downLeft = new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y - front.height, 0f);
                Vector3 upLeft = new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y - front.height, top.width);
                Vector3 upRight = new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y - front.height, top.width);
                Vector3 downRight = new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y - front.height, 0f);

                vertices.Add(upLeft);
                vertices.Add(downLeft);
                vertices.Add(downRight);
                vertices.Add(upRight);

                triangles.Add((squareCount * 4));
                triangles.Add((squareCount * 4) + 1);
                triangles.Add((squareCount * 4) + 3);
                triangles.Add((squareCount * 4) + 1);
                triangles.Add((squareCount * 4) + 2);
                triangles.Add((squareCount * 4) + 3);

                uvs.Add(new Vector2(upLeft.x, upLeft.z));
                uvs.Add(new Vector2(downLeft.x, downLeft.z));
                uvs.Add(new Vector2(downRight.x, downRight.z));
                uvs.Add(new Vector2(upRight.x, upRight.z));

                squareCount++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }

    private Mesh FrontMesh(int index)
    {
        int squareCount = 0;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float capHeight = cap.enabled ? cap.height : 0f;

        int segments = GetCurveSegments(index);

        for (int j = 0; j < segments; j++)
        {
            float downLeftY = front.flatBotttom ? -front.height : GetPoint(index, j / (float)segments).y - front.height;
            float downRightY = front.flatBotttom ? -front.height : GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y - front.height;

            Vector3 downLeft = new Vector3(GetPoint(index, j / (float)segments).x, downLeftY, 0f);
            Vector3 upLeft = new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y - capHeight, 0f);
            Vector3 upRight = new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y - capHeight, 0f);
            Vector3 downRight = new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, downRightY, 0f);

            vertices.Add(downLeft);
            vertices.Add(upLeft);
            vertices.Add(upRight);
            vertices.Add(downRight);

            triangles.Add((squareCount * 4));
            triangles.Add((squareCount * 4) + 1);
            triangles.Add((squareCount * 4) + 3);
            triangles.Add((squareCount * 4) + 1);
            triangles.Add((squareCount * 4) + 2);
            triangles.Add((squareCount * 4) + 3);

            uvs.Add(new Vector2(downLeft.x, downLeft.y));
            uvs.Add(new Vector2(upLeft.x, upLeft.y));
            uvs.Add(new Vector2(upRight.x, upRight.y));
            uvs.Add(new Vector2(downRight.x, downRight.y));

            squareCount++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }

    private Mesh BackMesh(int index)
    {
        int squareCount = 0;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float capHeight = cap.enabled ? cap.height : 0f;

        int segments = GetCurveSegments(index);

        for (int j = 0; j < segments; j++)
        {
            float downLeftY = front.flatBotttom ? -front.height : GetPoint(index, j / (float)segments).y - front.height;
            float downRightY = front.flatBotttom ? -front.height : GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y - front.height;

            Vector3 downLeft = new Vector3(GetPoint(index, j / (float)segments).x, downLeftY, top.width);
            Vector3 upLeft = new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y - capHeight, top.width);
            Vector3 upRight = new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y - capHeight, top.width);
            Vector3 downRight = new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, downRightY, top.width);

            vertices.Add(upLeft);
            vertices.Add(downLeft);          
            vertices.Add(downRight);
            vertices.Add(upRight);

            triangles.Add((squareCount * 4));
            triangles.Add((squareCount * 4) + 1);
            triangles.Add((squareCount * 4) + 3);
            triangles.Add((squareCount * 4) + 1);
            triangles.Add((squareCount * 4) + 2);
            triangles.Add((squareCount * 4) + 3);

            uvs.Add(new Vector2(upLeft.x, upLeft.y));
            uvs.Add(new Vector2(downLeft.x, downLeft.y));        
            uvs.Add(new Vector2(downRight.x, downRight.y));
            uvs.Add(new Vector2(upRight.x, upRight.y));

            squareCount++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }

    private Mesh LeftMesh(bool useCap)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float capHeight = useCap ? (cap.enabled ? cap.height : 0f) : 0f;
        float downY = front.flatBotttom ? -front.height : controlPoints[0].y - front.height;

        Vector3 downLeft = new Vector3(controlPoints[0].x, downY, controlPoints[0].z + top.width);
        Vector3 upLeft = new Vector3(controlPoints[0].x, controlPoints[0].y - capHeight, controlPoints[0].z + top.width);
        Vector3 upRight = new Vector3(controlPoints[0].x, controlPoints[0].y - capHeight, controlPoints[0].z);
        Vector3 downRight = new Vector3(controlPoints[0].x, downY, controlPoints[0].z);

        vertices.Add(downLeft);
        vertices.Add(upLeft);
        vertices.Add(upRight);
        vertices.Add(downRight);

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(3);

        uvs.Add(new Vector2(downLeft.z, downLeft.y));
        uvs.Add(new Vector2(upLeft.z, upLeft.y));
        uvs.Add(new Vector2(upRight.z, upRight.y));
        uvs.Add(new Vector2(downRight.z, downRight.y));

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }

    private Mesh RightMesh(bool useCap)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float capHeight = useCap ? (cap.enabled ? cap.height : 0f) : 0f;
        float downY = front.flatBotttom ? -front.height : controlPoints[controlPoints.Count - 1].y - front.height;

        Vector3 downRight = new Vector3(controlPoints[controlPoints.Count - 1].x, downY, controlPoints[controlPoints.Count - 1].z + top.width);
        Vector3 upRight = new Vector3(controlPoints[controlPoints.Count - 1].x, controlPoints[controlPoints.Count - 1].y - capHeight, controlPoints[controlPoints.Count - 1].z + top.width);
        Vector3 upLeft = new Vector3(controlPoints[controlPoints.Count - 1].x, controlPoints[controlPoints.Count - 1].y - capHeight, controlPoints[controlPoints.Count - 1].z);
        Vector3 downLeft = new Vector3(controlPoints[controlPoints.Count - 1].x, downY, controlPoints[controlPoints.Count - 1].z);

        vertices.Add(downLeft);
        vertices.Add(upLeft);
        vertices.Add(upRight);
        vertices.Add(downRight);

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(3);

        uvs.Add(new Vector2(downLeft.z, downLeft.y));
        uvs.Add(new Vector2(upLeft.z, upLeft.y));
        uvs.Add(new Vector2(upRight.z, upRight.y));
        uvs.Add(new Vector2(downRight.z, downRight.y));

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }


    private Mesh CapFrontMesh(int index)
    {
        int squareCount = 0;

        List<Vector3> capVertices = new List<Vector3>();
        List<int> capTriangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int segments = GetCurveSegments(index);

        for (int j = 0; j < segments; j++)
        {
            Vector3 downLeft = (new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y - cap.height, 0f));
            Vector3 upLeft = (new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y, 0f));
            Vector3 upRight = (new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y, 0f));
            Vector3 downRight = (new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y - cap.height, 0f));

            capVertices.Add(downLeft);
            capVertices.Add(upLeft);
            capVertices.Add(upRight);
            capVertices.Add(downRight);

            capTriangles.Add((squareCount * 4));
            capTriangles.Add((squareCount * 4) + 1);
            capTriangles.Add((squareCount * 4) + 3);
            capTriangles.Add((squareCount * 4) + 1);
            capTriangles.Add((squareCount * 4) + 2);
            capTriangles.Add((squareCount * 4) + 3);

            uvs.Add(new Vector2(downLeft.x, downLeft.y));
            uvs.Add(new Vector2(upLeft.x, upLeft.y));
            uvs.Add(new Vector2(upRight.x, upRight.y));
            uvs.Add(new Vector2(downRight.x, downRight.y));

            squareCount++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = capVertices.ToArray();
        mesh.triangles = capTriangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }

    private Mesh CapBackMesh(int index)
    {
        int squareCount = 0;

        List<Vector3> capVertices = new List<Vector3>();
        List<int> capTriangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int segments = GetCurveSegments(index);

        for (int j = 0; j < segments; j++)
        {
            Vector3 downLeft = (new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y - cap.height, top.width));
            Vector3 upLeft = (new Vector3(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y, top.width));
            Vector3 upRight = (new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y, top.width));
            Vector3 downRight = (new Vector3(GetPoint(index, (j / (float)segments) + (1f / (float)segments)).x, GetPoint(index, (j / (float)segments) + (1f / (float)segments)).y - cap.height, top.width));

            capVertices.Add(upLeft);
            capVertices.Add(downLeft);          
            capVertices.Add(downRight);
            capVertices.Add(upRight);

            capTriangles.Add((squareCount * 4));
            capTriangles.Add((squareCount * 4) + 1);
            capTriangles.Add((squareCount * 4) + 3);
            capTriangles.Add((squareCount * 4) + 1);
            capTriangles.Add((squareCount * 4) + 2);
            capTriangles.Add((squareCount * 4) + 3);

            uvs.Add(new Vector2(downLeft.x, downLeft.y));
            uvs.Add(new Vector2(upLeft.x, upLeft.y));
            uvs.Add(new Vector2(upRight.x, upRight.y));
            uvs.Add(new Vector2(downRight.x, downRight.y));

            squareCount++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = capVertices.ToArray();
        mesh.triangles = capTriangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }

    private Mesh CapLeftMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3 downLeft = new Vector3(controlPoints[0].x, controlPoints[0].y - cap.height, controlPoints[0].z + top.width);
        Vector3 upLeft = new Vector3(controlPoints[0].x, controlPoints[0].y, controlPoints[0].z + top.width);
        Vector3 upRight = new Vector3(controlPoints[0].x, controlPoints[0].y, controlPoints[0].z);
        Vector3 downRight = new Vector3(controlPoints[0].x, controlPoints[0].y - cap.height, controlPoints[0].z);

        vertices.Add(downLeft);
        vertices.Add(upLeft);
        vertices.Add(upRight);
        vertices.Add(downRight);

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(3);

        uvs.Add(new Vector2(downLeft.z, downLeft.y));
        uvs.Add(new Vector2(upLeft.z, upLeft.y));
        uvs.Add(new Vector2(upRight.z, upRight.y));
        uvs.Add(new Vector2(downRight.z, downRight.y));

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }

    private Mesh CapRightMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3 downRight = new Vector3(controlPoints[controlPoints.Count - 1].x, controlPoints[controlPoints.Count - 1].y - cap.height, controlPoints[controlPoints.Count - 1].z + top.width);
        Vector3 upRight = new Vector3(controlPoints[controlPoints.Count - 1].x, controlPoints[controlPoints.Count - 1].y, controlPoints[controlPoints.Count - 1].z + top.width);
        Vector3 upLeft = new Vector3(controlPoints[controlPoints.Count - 1].x, controlPoints[controlPoints.Count - 1].y, controlPoints[controlPoints.Count - 1].z);
        Vector3 downLeft = new Vector3(controlPoints[controlPoints.Count - 1].x, controlPoints[controlPoints.Count - 1].y - cap.height, controlPoints[controlPoints.Count - 1].z);

        vertices.Add(downLeft);
        vertices.Add(upLeft);
        vertices.Add(upRight);
        vertices.Add(downRight);

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(3);

        uvs.Add(new Vector2(downLeft.z, downLeft.y));
        uvs.Add(new Vector2(upLeft.z, upLeft.y));
        uvs.Add(new Vector2(upRight.z, upRight.y));
        uvs.Add(new Vector2(downRight.z, downRight.y));

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }
    

    private void CombineMeshes(int index, List<Mesh> meshes, List<Material> materials)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(chunks[(int)((index + 1) / 3)].chunkObject.transform.FindChild("Mesh").GetComponent<MeshFilter>(), "Mesh");
        if (undo) Undo.RecordObject(chunks[(int)((index + 1) / 3)].chunkObject.transform.FindChild("Mesh").GetComponent<MeshRenderer>(), "Mesh");
        #endif

        CombineInstance[] combine = new CombineInstance[meshes.Count];

        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        }

        chunks[(int)((index + 1) / 3)].chunkObject.transform.FindChild("Mesh").GetComponent<MeshFilter>().sharedMesh = new Mesh();
        chunks[(int)((index + 1) / 3)].chunkObject.transform.FindChild("Mesh").GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, false);

        Material[] replaceMaterials = new Material[materials.Count];

        for (int a = 0; a < materials.Count; a++)
        {
            replaceMaterials[a] = materials[a];
        }

        chunks[(int)((index + 1) / 3)].chunkObject.transform.FindChild("Mesh").GetComponent<MeshRenderer>().sharedMaterials = replaceMaterials;  
    }


    protected void UpdateChunkColliderHolder(int index)
    {
        Chunk chunk = chunks[(int)index / 3];

        if (chunk.chunkObject.transform.FindChild("Collider"))
        {
            if (undo)
            {
                #if UNITY_EDITOR
                Undo.DestroyObjectImmediate(chunk.chunkObject.transform.FindChild("Collider").gameObject);
                #else
                DestroyObject(chunk.chunkObject.transform.FindChild("Collider").gameObject);
                #endif
            }
            else
            {
                DestroyImmediate(chunk.chunkObject.transform.FindChild("Collider").gameObject);
            }
        }

        if (graphics == GraphicsMode.ThreeD)
        {
            if (!chunk.chunkObject.transform.FindChild("Collider"))
            {
                GameObject collider = new GameObject();
                collider.name = "Collider";
                collider.transform.parent = chunk.chunkObject.transform;
                collider.transform.localPosition = new Vector3(0f, 0f, 0f);
                collider.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                collider.AddComponent<MeshCollider>();

                #if UNITY_EDITOR
                if (undo) Undo.RegisterCreatedObjectUndo(collider, "Collider Holder");
                #endif
            }
        }
        else
        {
            if (!chunk.chunkObject.transform.FindChild("Collider"))
            {
                GameObject collider = new GameObject();
                collider.name = "Collider";
                collider.transform.parent = chunk.chunkObject.transform;
                collider.transform.localPosition = new Vector3(0f, 0f, 0f);
                collider.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                collider.AddComponent<EdgeCollider2D>();

                #if UNITY_EDITOR
                if (undo) Undo.RegisterCreatedObjectUndo(collider, "Collider Holder");
                #endif
            }
        }
    }

    protected void UpdateAllChunkColliderHolders()
    {
        for (int i = 0; i < controlPoints.Count - 1; i += 3)
        {
            UpdateChunkColliderHolder(i);
        }
    }


    private void Collider2D(int index)
    {
        List<Vector2> newPoints = new List<Vector2>();

        newPoints.Add(new Vector2(GetPoint(index, 0f).x, GetPoint(index, 0f).y));

        int segments = GetCurveSegments(index);

        for (int j = 1; j < segments + 1; j++)
        {
            newPoints.Add(new Vector2(GetPoint(index, j / (float)segments).x, GetPoint(index, j / (float)segments).y));     
        }

        if (front.flatBotttom)
        {
            newPoints.Add(new Vector2(GetPoint(index, 1f).x,- front.height));
            newPoints.Add(new Vector2(GetPoint(index, 0f).x,- front.height));
        }
        else
        {
            for (int a = segments + 1; a >= 0; a--)
            {
                newPoints.Add(new Vector2(GetPoint(index, a / (float)segments).x, GetPoint(index, a / (float)segments).y - front.height));
            }
        }

        newPoints.Add(new Vector2(GetPoint(index, 0f).x, GetPoint(index, 0f).y));

        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(chunks[(int)((index + 1) / 3)].chunkObject.transform.FindChild("Collider").GetComponent<EdgeCollider2D>(), "Edge Collider");
        #endif

        chunks[(int)((index + 1) / 3)].chunkObject.transform.FindChild("Collider").GetComponent<EdgeCollider2D>().points = newPoints.ToArray();
    }

    private void Collider3D(int index, List<Mesh> meshColliders)
    {
        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(chunks[(int)((index + 1) / 3)].chunkObject.transform.FindChild("Collider").GetComponent<MeshCollider>(), "Mesh Collider");
        #endif

        CombineInstance[] combine = new CombineInstance[meshColliders.Count];
        
        for (int i = 0; i < meshColliders.Count; i++)
        {
            combine[i].mesh = meshColliders[i];
            combine[i].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        }

        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combine, false);

        chunks[(int)((index + 1) / 3)].chunkObject.transform.FindChild("Collider").GetComponent<MeshCollider>().sharedMesh = newMesh;
    }
}

public abstract partial class Terrain2D : MonoBehaviour
{
    [Serializable]
    public class Prefab
    {
        public GameObject prefabToClone;
        public GameObject lastPrefabToClone;
        public List<Clone> clones = new List<Clone>();

        [Serializable]
        public class Clone
        {
            public GameObject cloneObjet;
            public float offset;
            public float savedZPosition;
        }
    }

    protected float furthestPrefabCloneXPosition;


    protected virtual void SetFurthestPrefabCloneXPosition()
    {

    }

    protected virtual void AdjustAllPrefabClonesToGraphicsMode()
    {

    }
    

    protected void AdjustPrefabClone(Prefab.Clone clone)
    {
        float zPosition = Mathf.Clamp(clone.cloneObjet.transform.position.z, transform.transform.position.z, transform.position.z + top.width);

        for (int i = 0; i < controlPoints.Count - 1; i += 3)
        {
            int segments = GetCurveSegments(i);

            Vector3 segmentStart = transform.TransformPoint(GetPoint(i, 0f));

            for (int j = 0; j <= segments; j++)
            {
                Vector3 segmentEnd = transform.TransformPoint(GetPoint(i, j / (float)segments));

                if (clone.cloneObjet.transform.position.x >= segmentStart.x && clone.cloneObjet.transform.position.x <= segmentEnd.x)
                {
                    float xPosition = clone.cloneObjet.transform.position.x - segmentStart.x;
                    float length = xPosition / (segmentEnd.x - segmentStart.x);
                    Vector3 direction = segmentEnd - segmentStart;

                    Vector3 newPosition = segmentStart + direction.normalized * Vector3.Distance(segmentEnd, segmentStart) * length;
                    Debug.Log(newPosition.x);

                    if (newPosition == clone.cloneObjet.transform.position) return;

                    #if UNITY_EDITOR
                    if (undo) Undo.RecordObject(clone.cloneObjet.transform, "Prefab Clone Position");
                    #endif

                    clone.cloneObjet.transform.position = new Vector3(newPosition.x, newPosition.y + clone.offset, zPosition);
                }

                segmentStart = segmentEnd;
            }
        }

        if (clone.cloneObjet.transform.position.x < transform.TransformPoint(controlPoints[0]).x || clone.cloneObjet.transform.position.x > transform.TransformPoint(controlPoints[controlPoints.Count - 1]).x)
        {
            clone.cloneObjet.transform.position = StartOrEndPosition(clone.cloneObjet.transform.position.x, clone.cloneObjet.transform.position.z, clone.offset);
        }     
    }

    protected void AdjustPrefabCloneToChunk(Prefab.Clone clone)
    {
        if (undo)
        {
            #if UNITY_EDITOR
            Undo.SetTransformParent(clone.cloneObjet.transform, chunks[(GetCurve((transform.InverseTransformPoint(clone.cloneObjet.transform.position).x)) - 1) / 3].chunkObject.transform.FindChild("Prefabs").transform, "Prefab Clone Transform");
            #endif
        }
        else
        {
            clone.cloneObjet.transform.parent = chunks[(GetCurve((transform.InverseTransformPoint(clone.cloneObjet.transform.position).x)) - 1) / 3].chunkObject.transform.FindChild("Prefabs").transform;
        }
    }

    protected void AdjustPrefabCloneToGraphicsMode(Prefab.Clone clone)
    {
        float newZPosition = (graphics == GraphicsMode.ThreeD) ? clone.savedZPosition : transform.position.z;

        clone.savedZPosition = clone.cloneObjet.transform.position.z;

        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(clone.cloneObjet.transform, "Prefab Clone Transform");
        #endif

        clone.cloneObjet.transform.position = new Vector3(clone.cloneObjet.transform.position.x, clone.cloneObjet.transform.position.y, newZPosition);
    }
}

public abstract partial class Terrain2D : MonoBehaviour
{
    public bool undo;

    protected List<int> renderedCurves = new List<int>();
    protected int lastRenderedCurve = 0;


    public virtual void Reset()
    {
        DeleteAllChunks();

        #if UNITY_EDITOR
        if (undo) Undo.RecordObject(this, "Reset");
        #endif

        points.Clear();
        controlPoints.Clear();
        modes.Clear();

        controlPoints.Add(new Vector3(0, 0, 0));
        modes.Add(BezierControlPointMode.Free);
    }    

    protected virtual void Start()
    {
        if (GameObject.FindWithTag("MainCamera") != null)
        {
            for (int i = 1; i < controlPoints.Count - 1; i += 3)
            {
                Vector3 viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[i]));

                if (viewPosition.x > -0.5f && viewPosition.x < 1.5f)
                {
                    renderedCurves.Add((int)(i - 1) / 3);
                }

                if (viewPosition.x < -0.5f || viewPosition.x > 1.5f)
                {
                    chunks[(int)(i - 1) / 3].chunkObject.transform.FindChild("Mesh").gameObject.SetActive(false);
                    chunks[(int)(i - 1) / 3].chunkObject.transform.FindChild("Prefabs").gameObject.SetActive(false);
                    chunks[(int)(i - 1) / 3].chunkObject.transform.FindChild("Collider").gameObject.SetActive(false);

                    lastRenderedCurve = (int)(i - 1) / 3;
                }
            }
        }
    }

    protected virtual void Update()
    {
        if (GameObject.FindWithTag("MainCamera") != null)
        {
            if (renderedCurves.Count > 0)
            {
                Vector3 viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[(renderedCurves[0] * 3) + 3]));

                if (viewPosition.x < -0.5f)
                {
                    chunks[renderedCurves[0]].chunkObject.transform.FindChild("Mesh").gameObject.SetActive(false);
                    chunks[renderedCurves[0]].chunkObject.transform.FindChild("Prefabs").gameObject.SetActive(false);
                    chunks[renderedCurves[0]].chunkObject.transform.FindChild("Collider").gameObject.SetActive(false);

                    lastRenderedCurve = renderedCurves[0];
                    renderedCurves.RemoveAt(0);               
                }          
            }

            if (renderedCurves.Count > 0)
            {
                if (renderedCurves[0] - 1 >= 0)
                {
                    Vector3 viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[((renderedCurves[0] - 1) * 3) + 3]));

                    if (viewPosition.x > -0.5f)
                    {
                        if (chunks[(renderedCurves[0] - 1)].chunkObject.transform.FindChild("Mesh").gameObject.activeSelf == false)
                        {
                            chunks[renderedCurves[0] - 1].chunkObject.transform.FindChild("Mesh").gameObject.SetActive(true);
                            chunks[renderedCurves[0] - 1].chunkObject.transform.FindChild("Prefabs").gameObject.SetActive(true);
                            chunks[renderedCurves[0] - 1].chunkObject.transform.FindChild("Collider").gameObject.SetActive(true);

                            renderedCurves.Insert(0, renderedCurves[0] - 1);              
                        }
                    }
                }
            }
            else
            {
                Vector3 viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[3]));

                if (viewPosition.x > -0.5f)
                {
                    if (chunks[0].chunkObject.transform.FindChild("Mesh").gameObject.activeSelf == false)
                    {
                        chunks[0].chunkObject.transform.FindChild("Mesh").gameObject.SetActive(true);
                        chunks[0].chunkObject.transform.FindChild("Prefabs").gameObject.SetActive(true);
                        chunks[0].chunkObject.transform.FindChild("Collider").gameObject.SetActive(true);

                        renderedCurves.Insert(0, 0);                       
                    }
                }            
            }

            if (renderedCurves.Count > 0)
            {
                Vector3 viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[(renderedCurves[renderedCurves.Count - 1] * 3)]));

                if (viewPosition.x > 1.5f)
                {
                    if (chunks[renderedCurves[renderedCurves.Count - 1]].chunkObject.transform.FindChild("Mesh").gameObject.activeSelf == true)
                    {
                        chunks[renderedCurves[renderedCurves.Count - 1]].chunkObject.transform.FindChild("Mesh").gameObject.SetActive(false);
                        chunks[renderedCurves[renderedCurves.Count - 1]].chunkObject.transform.FindChild("Prefabs").gameObject.SetActive(false);
                        chunks[renderedCurves[renderedCurves.Count - 1]].chunkObject.transform.FindChild("Collider").gameObject.SetActive(false);

                        lastRenderedCurve = renderedCurves[renderedCurves.Count - 1];
                        renderedCurves.RemoveAt(renderedCurves.Count - 1);
                    }
                }
            }

            if (renderedCurves.Count > 0)
            {
                Vector3 viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[((renderedCurves[renderedCurves.Count - 1] + 1) * 3)]));

                if (viewPosition.x < 1.5f)
                {
                    if (renderedCurves[renderedCurves.Count - 1] + 1 < chunks.Count)
                    {
                        if (chunks[(renderedCurves[renderedCurves.Count - 1]) + 1].chunkObject.transform.FindChild("Mesh").gameObject.activeSelf == (false))
                        {
                            chunks[(renderedCurves[renderedCurves.Count - 1]) + 1].chunkObject.transform.FindChild("Mesh").gameObject.SetActive(true);
                            chunks[(renderedCurves[renderedCurves.Count - 1]) + 1].chunkObject.transform.FindChild("Prefabs").gameObject.SetActive(true);
                            chunks[(renderedCurves[renderedCurves.Count - 1]) + 1].chunkObject.transform.FindChild("Collider").gameObject.SetActive(true);

                            renderedCurves.Add(renderedCurves[renderedCurves.Count - 1] + 1);
                        }
                    }
                }
            }
            else if (renderedCurves.Count == 0 && lastRenderedCurve > 0)
            {
                Vector3 viewPosition = Camera.main.WorldToViewportPoint(transform.TransformPoint(controlPoints[controlPoints.Count - 1]));

                if (viewPosition.x > -0.5f)
                {
                    if (chunks[chunks.Count - 1].chunkObject.transform.FindChild("Mesh").gameObject.activeSelf == (false))
                    {
                        chunks[chunks.Count - 1].chunkObject.transform.FindChild("Mesh").gameObject.SetActive(true);
                        chunks[chunks.Count - 1].chunkObject.transform.FindChild("Prefabs").gameObject.SetActive(true);
                        chunks[chunks.Count - 1].chunkObject.transform.FindChild("Collider").gameObject.SetActive(true);

                        renderedCurves.Add(chunks.Count - 1);
                    }
                }
            }
        }
    }
}

