using UnityEngine;
using System.Collections;


public enum STYPE
{
    BILLBOARD,
    BILLBOARD_SELF,
    XZ,
}

public enum ORIPOINT
{
    CENTER,
    LEFT_UP,
    LEFT_BOTTOM,
    RIGHT_BOTTOM,
    RIGHT_UP,
    BOTTOM_CENTER,
    TOP_CENTER,
    LEFT_CENTER,
    RIGHT_CENTER
}

public enum EMITTYPE
{
    POINT,
    BOX,
    SPHERE,
    CIRCLE,
    LINE
}

public enum RSTYPE
{
    NONE,
    SIMPLE,
    CURVE
}

public enum COLOR_GRADUAL_TYPE
{
    CLAMP,
    LOOP,
    REVERSE
}

public class EffectLayer : MonoBehaviour {
   
    public VertexPool Vertexpool;

    protected Camera MainCamera;
    //Main Config
    public Transform ClientTransform;
    public bool SyncClient;
    public Material Material;
    public int RenderType;
    public float StartTime = 0f;
    public float MaxFps = 60f;

    //Sprite Config
    public int SpriteType;
    public int OriPoint;
    public float SpriteWidth = 1;
    public float SpriteHeight = 1;
    public int SpriteUVStretch = 0;

    //Rotation Config
    public int OriRotationMin;
    public int OriRotationMax;
    public bool RotAffectorEnable = false;
    public RSTYPE RotateType = RSTYPE.NONE;
    public float DeltaRot;
    public AnimationCurve RotateCurve;

    //Scale Config
    public float OriScaleXMin = 1f;
    public float OriScaleXMax = 1f;
    public float OriScaleYMin = 1f;
    public float OriScaleYMax = 1f;
    public bool ScaleAffectorEnable = false;
    public RSTYPE ScaleType = RSTYPE.NONE;
    public float DeltaScaleX = 0f;
    public float DeltaScaleY = 0f;
    public AnimationCurve ScaleXCurve;
    public AnimationCurve ScaleYCurve;

    //Color Config
    public bool ColorAffectorEnable = false;
    public int ColorAffectType = 0;
    public float ColorGradualTimeLength = 1f;
    public COLOR_GRADUAL_TYPE ColorGradualType = COLOR_GRADUAL_TYPE.CLAMP;
    public Color Color1 = Color.white;
    public Color Color2;
    public Color Color3;
    public Color Color4;

    //RibbonTrail Config
    public float RibbonWidth = 0.5f;
    public int MaxRibbonElements = 6;
    public float RibbonLen = 1f;
    public float TailDistance = 0f;
    public int StretchType = 0;

    //Emitter Config
    public int     EmitType;
    public Vector3 BoxSize;
    public Vector3 EmitPoint;
    public float   Radius;
    public Vector3 CircleDir;
    public float LineLengthLeft = -1f;
    public float LineLengthRight = 1f;
    public int MaxENodes = 1;
    public bool IsNodeLifeLoop = true;
    public float NodeLifeMin = 1;
    public float NodeLifeMax = 1;
    public bool IsEmitByDistance = false;
    public float DiffDistance = 0.1f;
    
    public float ChanceToEmit = 100f;
    public float EmitDuration = 10f;
    public int EmitRate = 20;
    public int EmitLoop = 1;
    public float EmitDelay = 0f;

    //Original Direction Config
    public bool IsRandomDir;
    public Vector3 OriVelocityAxis;
    public int     AngleAroundAxis;
    public float   OriSpeed;
    public bool AlongVelocity = false;

    //LinearForce Affector
    public bool LinearForceAffectorEnable = false;
    public Vector3 LinearForce;
    public float LinearMagnitude = 1f;

    //Jet Affector
    public bool JetAffectorEnable = false;
    public float JetMin;
    public float JetMax;

    //Vortex Affector
    public bool VortexAffectorEnable = false;
    public bool UseVortexCurve = false;
    public float VortexMag = 0.1f;
    public AnimationCurve VortexCurve;
    public Vector3 VortexDirection;

    //Attraction Affector
    public bool AttractionAffectorEnable = false;
    public bool UseAttractCurve = false;
    public float AttractMag = 0.1f;
    public AnimationCurve AttractionCurve;
    public Vector3 AttractionPosition;

    //UV Config
    public bool UVAffectorEnable = false;
    public int UVType = 0;
    public Vector2 OriLowerLeftUV = Vector2.zero;
    public Vector2 OriUVDimensions = Vector2.one;
    public int Cols = 1;
    public int Rows = 1;
    public int LoopCircles = -1;
    public float UVTime = 30;
    public string EanPath = "none";
    public int EanIndex = 0;

    public bool RandomOriScale = false;
    public bool RandomOriRot = false;


    protected Emitter emitter;


    public  EffectNode[] AvailableENodes;
    public EffectNode[] ActiveENodes;
    public int AvailableNodeCount;

    public Vector3 LastClientPos;

    protected ArrayList InitAffectors(EffectNode node)
    {
        ArrayList AffectorList = new ArrayList();

        if (UVAffectorEnable)
        {
            UVAnimation uvAnim = new UVAnimation();
            Texture t = Vertexpool.GetMaterial().GetTexture("_MainTex");

            if (UVType == 2)
            {
                uvAnim.BuildFromFile(EanPath, EanIndex, UVTime, t);
                OriLowerLeftUV = uvAnim.frames[0];
                OriUVDimensions = uvAnim.UVDimensions[0];
            }
            else if (UVType == 1)
            {
                float perWidth = t.width / Cols;
                float perHeight = t.height / Rows;
                Vector2 cellSize = new Vector2(perWidth / t.width, perHeight / t.height);
                Vector2 start = new Vector2(0f, 1f);
                uvAnim.BuildUVAnim(start, cellSize, Cols, Rows, Cols * Rows);
                OriLowerLeftUV = start;
                OriUVDimensions = cellSize;
                OriUVDimensions.y = -OriUVDimensions.y;
            }

            if (uvAnim.frames.Length == 1)
            {
                OriLowerLeftUV = uvAnim.frames[0];
                OriUVDimensions = uvAnim.UVDimensions[0];
            }
            else
            {
                uvAnim.loopCycles = LoopCircles;
                Affector aft = new UVAffector(uvAnim, UVTime,node);
                AffectorList.Add(aft);
            }
        }

        if (RotAffectorEnable && RotateType != RSTYPE.NONE)
        {
            Affector aft;
            if (RotateType == RSTYPE.CURVE)
                aft = new RotateAffector(RotateCurve,node);
            else
                aft = new RotateAffector(DeltaRot,node);
            AffectorList.Add(aft);
        }
        if (ScaleAffectorEnable && ScaleType != RSTYPE.NONE)
        {
            Affector aft;
            if (ScaleType == RSTYPE.CURVE)
                aft = new ScaleAffector(ScaleXCurve,ScaleYCurve,node);
            else
                aft = new ScaleAffector(DeltaScaleX, DeltaScaleY,node);
            AffectorList.Add(aft);
        }
        if (ColorAffectorEnable && ColorAffectType != 0)
        {
            ColorAffector aft;
            if (ColorAffectType == 2)
            {
                Color[] carr = new Color[4];
                carr[0] = Color1; carr[1] = Color2; carr[2] = Color3; carr[3] = Color4;
                aft = new ColorAffector(carr,ColorGradualTimeLength,ColorGradualType,node);
            }
            else
            {
                Color[] carr = new Color[2];
                carr[0] = Color1; carr[1] = Color2;
                aft = new ColorAffector(carr,ColorGradualTimeLength,ColorGradualType,node);
            }
            AffectorList.Add(aft);
        }
        if (LinearForceAffectorEnable)
        {
            Affector aft = new LinearForceAffector(LinearForce.normalized * LinearMagnitude,node);
            AffectorList.Add(aft);
        }
        if (JetAffectorEnable)
        {
            Affector aft = new JetAffector(JetMin,JetMax,node);
            AffectorList.Add(aft);
        }
        if (VortexAffectorEnable)
        {
            Affector aft;
            if (UseVortexCurve)
                aft = new VortexAffector(VortexCurve, VortexDirection,node);
            else
                aft = new VortexAffector(VortexMag, VortexDirection,node);
            AffectorList.Add(aft);
        }
        if (AttractionAffectorEnable)
        {
            Affector aft ;
            if (UseVortexCurve)
                aft = new AttractionForceAffector(AttractionCurve, AttractionPosition,node);
            else
                aft = new AttractionForceAffector(AttractMag, AttractionPosition,node);
            AffectorList.Add(aft);
        }

        return AffectorList;
    }

    protected void Init()
    {
        AvailableENodes = new EffectNode[MaxENodes];
        ActiveENodes = new EffectNode[MaxENodes];
        for (int i = 0; i < MaxENodes; i++)
        {
            EffectNode n = new EffectNode(i, ClientTransform, SyncClient, this);
            ArrayList afts = InitAffectors(n);
            n.SetAffectorList(afts);
            if (RenderType == 0)
                n.SetType(SpriteWidth, SpriteHeight, (STYPE)SpriteType, (ORIPOINT)OriPoint, SpriteUVStretch, MaxFps);
            else
                n.SetType(RibbonWidth, MaxRibbonElements, RibbonLen, ClientTransform.position, StretchType, MaxFps);
            AvailableENodes[i] = n;
        }
        AvailableNodeCount = MaxENodes;
        emitter = new Emitter(this);
    }


    public VertexPool GetVertexPool()
    {
        return Vertexpool;
    }
    public void RemoveActiveNode(EffectNode node)
    {
        if (AvailableNodeCount == MaxENodes)
            Debug.LogError("out index!");
        if (ActiveENodes[node.Index] == null) //already removed
            return;
        ActiveENodes[node.Index] = null;
        AvailableENodes[node.Index] = node;
        AvailableNodeCount++;
    }


    public void AddActiveNode(EffectNode node)
    {
        if (AvailableNodeCount == 0)
            Debug.LogError("out index!");
        if (AvailableENodes[node.Index] == null) //already added
            return;
        ActiveENodes[node.Index] = node;
        AvailableENodes[node.Index] = null;
        AvailableNodeCount--;
    }


    protected void AddNodes(int num)
    {
        int added = 0;
        for (int i = 0; i < MaxENodes; i++)
        {
            if (added == num)
                break;
            EffectNode node = AvailableENodes[i];
            if (node != null)
            {
                AddActiveNode(node);
                added++;

                emitter.SetEmitPosition(node);
                float nodeLife = 0;
                if (IsNodeLifeLoop)
                    nodeLife = -1;
                else
                    nodeLife = Random.Range(NodeLifeMin, NodeLifeMax);
                Vector3 oriDir = emitter.GetEmitRotation(node);
                node.Init(oriDir.normalized, OriSpeed, nodeLife, Random.Range(OriRotationMin, OriRotationMax),
                    Random.Range(OriScaleXMin, OriScaleXMax), Random.Range(OriScaleYMin, OriScaleYMax), Color1, OriLowerLeftUV, OriUVDimensions);
            }
            else
                continue;
        }
    }
    public void Reset()
    {
        for (int i = 0; i < MaxENodes; i++)
        {
            if (ActiveENodes == null)
                return;
            EffectNode node = ActiveENodes[i];
            if (node != null)
            {
                node.Reset();
                RemoveActiveNode(node);
            }
        }
        emitter.Reset();
    }

    public void FixedUpdateCustom()
    {
        int needToAdd = emitter.GetNodes();
        AddNodes(needToAdd);
        for (int i = 0; i < MaxENodes; i++)
        {
            EffectNode node = ActiveENodes[i];
            if (node == null)
                continue;
            node.Update();
        }
    }

    public void StartCustom()
    {
        if (MainCamera == null)
            MainCamera = Camera.main;
        Init();
        LastClientPos = ClientTransform.position;
    }

    void OnDrawGizmosSelected()
    {
        //Visual Debug
        //if (ClientTransform != null)
        //    Gizmos.DrawWireCube(ClientTransform.position+EmitPoint, BoxSize);
    }


    //API for one node use only.
    public RibbonTrail GetRibbonTrail()
    {
        if (ActiveENodes == null | ActiveENodes.Length != 1 || MaxENodes != 1 || RenderType != 1)
            return null;
        return ActiveENodes[0].Ribbon;
    }

}
