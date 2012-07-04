using UnityEngine;
using System.Collections;

public class VertexPool {
    public class VertexSegment
    {
        public int VertStart;
        public int IndexStart;
        public int VertCount;
        public int IndexCount;
        public VertexPool Pool;

        public VertexSegment(int start, int count, int istart, int icount,VertexPool pool)
        {
            VertStart = start;
            VertCount = count;
            IndexCount = icount;
            IndexStart = istart;
            Pool = pool;
        }
    }
     
    public Vector3[] Vertices;
    public int[] Indices;
    public Vector2[] UVs;
    public Color[] Colors;

    public bool IndiceChanged;
    public bool ColorChanged;
    public bool UVChanged;
    public bool VertChanged;



    public Mesh Mesh;
    public Material Material;

    protected int VertexTotal;
    protected int VertexUsed;
    protected int IndexTotal = 0;
    protected int IndexUsed = 0;
    protected bool FirstUpdate = true;

    protected bool VertCountChanged;


    public const int BlockSize = 36;

    public float BoundsScheduleTime = 1f;
    public float ElapsedTime = 0f;

    public void RecalculateBounds()
    {
        Mesh.RecalculateBounds();
    }

    public VertexPool(Mesh mesh, Material material)
    {
        VertexTotal = VertexUsed = 0;
        VertCountChanged = false;
        Mesh = mesh;
        Material = material;
        InitArrays();
        Vertices = Mesh.vertices;
        Indices = Mesh.triangles;
        Colors = Mesh.colors;
        UVs = Mesh.uv;
        IndiceChanged = ColorChanged = UVChanged = VertChanged  = true;
    }

    public Sprite AddSprite(float width, float height,STYPE type, ORIPOINT ori,Camera cam, int uvStretch, float maxFps)
    {
        VertexSegment segment = GetVertices(4,6);
        Sprite s = new Sprite(segment, width, height, type, ori, cam,uvStretch, maxFps);
        //Debug.Log("|"+segment.VertStart + "," + segment.VertCount);
        return s;
    }

    public RibbonTrail AddRibbonTrail(float width,int maxelemnt, float len,Vector3 pos,int stretchType,float maxFps)
    {
        VertexSegment segment = GetVertices(maxelemnt * 2, (maxelemnt - 1) * 6);
        RibbonTrail trail = new RibbonTrail(segment, width, maxelemnt, len, pos, stretchType, maxFps);
        return trail;
    }

    public Material GetMaterial()
    {
        return Material;
    }

    public VertexSegment GetVertices(int vcount, int icount)
    {
        int vertNeed = 0;
        int indexNeed = 0;
        if (VertexUsed+vcount >= VertexTotal)
        {
            vertNeed = (vcount / BlockSize + 1) * BlockSize;
        }
        if (IndexUsed + icount >= IndexTotal)
        {
            indexNeed = (icount / BlockSize + 1) * BlockSize;
        }
        VertexUsed += vcount;
        IndexUsed += icount;
        if (vertNeed != 0 || indexNeed != 0)
        {
            EnlargeArrays(vertNeed, indexNeed);
            VertexTotal += vertNeed;
            IndexTotal += indexNeed;
        }
        return new VertexSegment(VertexUsed - vcount, vcount, IndexUsed - icount, icount, this);
    }
    
    protected void InitArrays()
    {
        Vertices = new Vector3[4];
        UVs = new Vector2[4];
        Colors = new Color[4];
        Indices = new int[6];
        VertexTotal = 4;
        IndexTotal = 6;
    }

    public void EnlargeArrays(int count,int icount)
    {
        Vector3[] tempVerts = Vertices;
        Vertices = new Vector3[Vertices.Length + count];
        tempVerts.CopyTo(Vertices, 0);
        
        Vector2[] tempUVs = UVs;
        UVs = new Vector2[UVs.Length + count];
        tempUVs.CopyTo(UVs, 0);

        Color[] tempColors = Colors;
        Colors = new Color[Colors.Length + count];
        tempColors.CopyTo(Colors, 0);

        int[] tempTris = Indices;
        Indices = new int[Indices.Length + icount];
        tempTris.CopyTo(Indices, 0);

        VertCountChanged = true;
        IndiceChanged = true;
        ColorChanged = true;
        UVChanged = true;
        VertChanged = true;
    }

    public void LateUpdate()
    {
        if (VertCountChanged)
        {
            Mesh.Clear();
        }
        
        // we assume the vertices is always changed.
        Mesh.vertices = Vertices;
        if (UVChanged)
        {
            Mesh.uv = UVs;
        }
           
        if (ColorChanged)
        {
            Mesh.colors = Colors;
        }

        if (IndiceChanged)
        {
            Mesh.triangles = Indices;
        }

        ElapsedTime += Time.deltaTime;
        if (ElapsedTime > BoundsScheduleTime || FirstUpdate)
        {
            RecalculateBounds();
            ElapsedTime = 0f;
        }

        //how to recognise the first update?..
        if (ElapsedTime > BoundsScheduleTime)
            FirstUpdate = false;

        VertCountChanged = false;
        IndiceChanged = false;
        ColorChanged = false;
        UVChanged = false;
        VertChanged = false;
    }
}
