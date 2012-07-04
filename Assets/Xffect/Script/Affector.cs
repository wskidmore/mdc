using UnityEngine;
using System.Collections;

public class Affector
{
    protected EffectNode Node;
    public Affector(EffectNode node)
    {
        Node = node;
    }
    public virtual void Update()
    {

    }

    public virtual void Reset()
    {
    }
}
public class RotateAffector : Affector
{
    protected AnimationCurve RotateCurve;
    protected RSTYPE Type;
    protected float Delta = 0f;

    public RotateAffector(AnimationCurve curve,EffectNode node) : base(node)
    {
        Type = RSTYPE.CURVE;
        RotateCurve = curve;
    }

    public RotateAffector(float delta, EffectNode node): base(node)
    {
        Type = RSTYPE.SIMPLE;
        Delta = delta;
    }


    public override void Update()
    {
        float time = Node.GetElapsedTime();
        if (Type == RSTYPE.CURVE)
            Node.RotateAngle = (int)RotateCurve.Evaluate(time);
        else if (Type == RSTYPE.SIMPLE)
        {
            float angle = Node.RotateAngle + Delta * Time.deltaTime;
            Node.RotateAngle = angle;
        }
    }
}

public class UVAffector : Affector
{
    protected UVAnimation Frames;
    protected float ElapsedTime;
    protected float UVTime;

    public UVAffector(UVAnimation frame, float time, EffectNode node): base(node)
    {
        Frames = frame;
        UVTime = time;
    }

    public override void Reset()
    {
        ElapsedTime = 0;
        Frames.curFrame = 0;
    }
    public override void Update()
    {
        ElapsedTime += Time.deltaTime;
        float framerate;
        if (UVTime <= 0f)
        {
            framerate = Node.GetLifeTime() / Frames.frames.Length;
        }
        else
        {
            framerate = UVTime / Frames.frames.Length;
        }
        if (ElapsedTime >= framerate)
        {
            Vector2 uv = Vector2.zero;
            Vector2 dm = Vector2.zero;
            Frames.GetNextFrame(ref uv, ref dm);
            Node.LowerLeftUV = uv;
            Node.UVDimensions = dm;
            ElapsedTime -= framerate;
        }
    }
}

public class ScaleAffector : Affector
{
    protected AnimationCurve ScaleXCurve;
    protected AnimationCurve ScaleYCurve;

    protected RSTYPE Type;
    protected float DeltaX = 0f;
    protected float DeltaY = 0f;

    public ScaleAffector(AnimationCurve curveX, AnimationCurve curveY, EffectNode node)
        : base(node)
    {
        Type = RSTYPE.CURVE;
        ScaleXCurve = curveX;
        ScaleYCurve = curveY;
    }

    public ScaleAffector(float x, float y, EffectNode node)
        : base(node)
    {
        Type = RSTYPE.SIMPLE;
        DeltaX = x;
        DeltaY = y;
    }

    public override void Update()
    {
        float time = Node.GetElapsedTime();
        if (Type == RSTYPE.CURVE)
        {
            if (ScaleXCurve != null)
                Node.Scale.x = ScaleXCurve.Evaluate(time);
            if (ScaleYCurve != null)
                Node.Scale.y = ScaleYCurve.Evaluate(time);
        }
        else if (Type == RSTYPE.SIMPLE)
        {
            float scalex = Node.Scale.x + DeltaX * Time.deltaTime;
            float scaley = Node.Scale.y + DeltaY * Time.deltaTime;
            if (scalex * Node.Scale.x > 0)
                Node.Scale.x = scalex;
            if (scaley * Node.Scale.y > 0)
                Node.Scale.y = scaley;
        }
    }
}

public class ColorAffector : Affector
{
    protected Color[] ColorArr;
    protected float GradualLen;
    protected COLOR_GRADUAL_TYPE Type;
    protected float ElapsedTime = 0f;
    protected bool IsNodeLife = false;
    public ColorAffector(Color[] colorArr, float gradualLen, COLOR_GRADUAL_TYPE type, EffectNode node)
        : base(node)
    {
        ColorArr = colorArr;
        Type = type;
        GradualLen = gradualLen;
        if (GradualLen < 0)
            IsNodeLife = true;
    }

    public override void Reset()
    {
        ElapsedTime = 0;
    }

    public override void Update()
    {
        ElapsedTime += Time.deltaTime;
        if (IsNodeLife)
        {
            GradualLen = Node.GetLifeTime();
        }

        if (GradualLen <= 0f)//node life loop
            return;
       
        if (ElapsedTime > GradualLen)
        {
            if (Type == COLOR_GRADUAL_TYPE.CLAMP)
                return;
            else if (Type == COLOR_GRADUAL_TYPE.LOOP)
            {
                ElapsedTime = 0f;
                return;
            }
            else
            {
                Color[] TempArr = new Color[ColorArr.Length];
                ColorArr.CopyTo(TempArr,0);
                for (int i = 0; i < TempArr.Length / 2; i++)
                {
                    ColorArr[TempArr.Length - i - 1] = TempArr[i];
                    ColorArr[i] = TempArr[TempArr.Length - i - 1];
                }
                ElapsedTime = 0f;
                return;
            }
        }
        int curIndex = (int)((ColorArr.Length - 1) * (ElapsedTime / GradualLen));
        if (curIndex == ColorArr.Length - 1)
            curIndex--;
        int targetIndex = curIndex + 1;
        float segmentTime = GradualLen / (ColorArr.Length - 1);
        float t = (ElapsedTime - segmentTime * curIndex) / segmentTime;
        Node.Color = Color.Lerp(ColorArr[curIndex], ColorArr[targetIndex], t);
    }
}

public class LinearForceAffector : Affector
{
    protected Vector3 Force;
    public LinearForceAffector(Vector3 force, EffectNode node)
        : base(node)
    {
        Force = force;
    }

    public override void Update()
    {
        Node.Velocity += Force * Time.deltaTime;
    }
}
public class JetAffector : Affector
{
    protected float MinAcceleration;
    protected float MaxAcceleration;
    public JetAffector(float min, float max, EffectNode node)
        : base(node)
    {
        MinAcceleration = min;
        MaxAcceleration = max;
    }

    public override void Update()
    {
        if (Mathf.Abs(Node.Acceleration) < 1e-06)
        {
            float acc = Random.Range(MinAcceleration, MaxAcceleration);
            Node.Acceleration = acc;
        }
    }
}

public class VortexAffector : Affector
{
    AnimationCurve VortexCurve;
    protected Vector3 Direction;
    bool UseCurve;
    float Magnitude;

    public VortexAffector(AnimationCurve vortexCurve, Vector3 dir, EffectNode node)
        : base(node)
    {
        VortexCurve = vortexCurve;
        Direction = dir;
        UseCurve = true;
    }

    public VortexAffector(float mag, Vector3 dir, EffectNode node)
        : base(node)
    {
        Magnitude = mag;
        Direction = dir;
        UseCurve = false;
    }

    public override void Update()
    {
        Vector3 diff = Node.GetLocalPosition() - Node.Owner.EmitPoint;

        float distance = diff.magnitude;

        if (distance == 0f)
            return;

        float segParam = Vector3.Dot(Direction, diff);
        diff -= segParam * Direction;

        Vector3 deltaV = Vector3.zero;
        if (diff == Vector3.zero)
        {
            deltaV = diff;
        }
        else
        {
            deltaV = Vector3.Cross(Direction, diff).normalized;
        }
        float time = Node.GetElapsedTime();
        float magnitude;
        if (UseCurve)
            magnitude = VortexCurve.Evaluate(time);
        else
            magnitude = Magnitude;
        deltaV *= magnitude * Time.deltaTime;
        Node.Position += deltaV;
    }
}

public class AttractionForceAffector : Affector
{
    AnimationCurve AttractionCurve;
    protected Vector3 Position;
    float Magnitude = 0f;
    bool UseCurve = false;

    public AttractionForceAffector(AnimationCurve curve, Vector3 pos, EffectNode node)
        : base(node)
    {
        AttractionCurve = curve;
        Position = pos;
        UseCurve = true;
    }

    public AttractionForceAffector(float magnitude, Vector3 pos, EffectNode node)
        : base(node)
    {
        Magnitude = magnitude;
        Position = pos;
        UseCurve = false;
    }

    public override void Update()
    {
        Vector3 attraction;
        if (Node.SyncClient)
        {
            attraction = Position - Node.GetLocalPosition();
        }
        else
        {
            attraction = Node.ClientTrans.position + Position - Node.GetLocalPosition();
        }
        float time = Node.GetElapsedTime();
        float magnitude;
        if (UseCurve)
            magnitude = AttractionCurve.Evaluate(time);
        else
            magnitude = Magnitude;
        float deltaV = magnitude;
        Node.Velocity += attraction.normalized * deltaV * Time.deltaTime;
    }
}