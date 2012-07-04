using UnityEngine;
using System.Collections;

public class EffectNode{

    //constructor
    protected int Type;
    public    int Index;
    public Transform ClientTrans;
    public bool SyncClient;
    public EffectLayer Owner;

    //internal using
    protected Vector3 CurDirection;
    protected Vector3 LastWorldPos = Vector3.zero;
    protected Vector3 CurWorldPos;
    protected float   ElapsedTime;
    public Sprite Sprite;
    public RibbonTrail Ribbon;
    
    //affect by affector
    public Vector3 Position;
    public Vector2 LowerLeftUV;
    public Vector2 UVDimensions;
    public Vector3 Velocity;
    public float Acceleration;
    public Vector2 Scale;
    public float RotateAngle;
    public Color Color;

    //reset
    protected ArrayList AffectorList;
    protected Vector3 OriDirection;
    protected float LifeTime;
    protected int OriRotateAngle;
    protected float OriScaleX;
    protected float OriScaleY;

    public EffectNode(int index, Transform clienttrans, bool sync, EffectLayer owner)
    {
        Index = index;
        ClientTrans = clienttrans;
        SyncClient = sync;
        Owner = owner;
        LowerLeftUV = Vector2.zero;
        UVDimensions = Vector2.one;
        Scale = Vector2.one;
        RotateAngle = 0;
        Color = Color.white;
    }

    public void SetAffectorList(ArrayList afts)
    {
        AffectorList = afts;
    }


    public void Init(Vector3 oriDir, float speed,float life, int oriRot,float oriScaleX,float oriScaleY, Color oriColor, Vector2 oriLowerUv, Vector2 oriUVDimension)
    {

        //OriDirection = ClientTrans.TransformDirection(oriDir);
        OriDirection = oriDir;
        LifeTime = life;
        OriRotateAngle = oriRot;
        OriScaleX = oriScaleX;
        OriScaleY = oriScaleY;
        Color = oriColor;
        ElapsedTime = 0f;
        Velocity = OriDirection*speed;
        Acceleration = 0f;
        LowerLeftUV  = oriLowerUv;
        UVDimensions = oriUVDimension;
        if (Type == 1)
        {
            Sprite.SetUVCoord(LowerLeftUV, UVDimensions);
            Sprite.SetColor(oriColor);
        }

        else if (Type == 2)
        {
            Ribbon.SetUVCoord(LowerLeftUV, UVDimensions);
            Ribbon.SetColor(oriColor);
            Ribbon.SetHeadPosition(ClientTrans.position + Position + OriDirection.normalized*Owner.TailDistance);
            Ribbon.ResetElementsPos();
        }

        //set  sprite direction
        if (Type == 1)
        {
            Sprite.SetRotationTo(OriDirection);
        }
    }
    public float GetElapsedTime()
    {
        return ElapsedTime;
    }

    public float GetLifeTime()
    {
        return LifeTime;
    }

    public void SetLocalPosition(Vector3 pos)
    {
        Position = pos;
    }
    public Vector3 GetLocalPosition()
    {
        return Position;
    }

    //sprite
    public void SetType(float width, float height, STYPE type, ORIPOINT orip, int uvStretch, float maxFps)
    {
        Type = 1;
        Sprite = Owner.GetVertexPool().AddSprite(width, height, type, orip, Camera.main, uvStretch, maxFps);
        
    }
    //ribbon trail
    public void SetType(float width, int maxelemnt, float len,Vector3 pos,int stretchType,float maxFps)
    {
        Type = 2;
        Ribbon = Owner.GetVertexPool().AddRibbonTrail(width, maxelemnt, len, pos, stretchType, maxFps);
        
    }

    public void Reset()
    {
        Position = Vector3.up * 9999;
        Velocity = Vector3.zero;
        Acceleration = 0f;
        ElapsedTime = 0f;

        LastWorldPos = CurWorldPos = Vector3.zero;

        foreach (Affector aft in AffectorList)
        {
            aft.Reset();
        }

        if (Type == 1)
        {
            Sprite.SetRotation(OriRotateAngle);
            Sprite.SetPosition(Position);
            Sprite.SetColor(Color.clear);
            Sprite.Update(true);
            //TODO:should reset in ScaleAffector.
            Scale = Vector2.one;
        }
        else if (Type == 2)
        {
            //set head to the tail position
            Ribbon.SetHeadPosition(ClientTrans.position + OriDirection.normalized * Owner.TailDistance);
            Ribbon.Reset();
            Ribbon.SetColor(Color.clear);
            Ribbon.UpdateVertices(Vector3.zero);
        }
    }

    public void Remove()
    {
        Owner.RemoveActiveNode(this);
    }


    public void UpdateSprite()
    {
        ////set direction
        if (Owner.AlongVelocity)
        {
            Vector3 curDir = Vector3.zero;
            if (LastWorldPos != Vector3.zero)
            {
                curDir = CurWorldPos - LastWorldPos;
            }
            else
            {//LastWorldPos = zero means first update.
                return;
            }
            if (curDir != Vector3.zero)
            {
                CurDirection = curDir;
                Sprite.SetRotationTo(CurDirection);
            }
        }
        Sprite.SetScale(Scale.x*OriScaleX, Scale.y*OriScaleY);
        if (Owner.ColorAffectorEnable)
            Sprite.SetColor(Color);
        if (Owner.UVAffectorEnable)
            Sprite.SetUVCoord(LowerLeftUV, UVDimensions);

        Sprite.SetRotation((float)OriRotateAngle + RotateAngle);
        Sprite.SetPosition(CurWorldPos);
        Sprite.Update(false);
    }

    public void UpdateRibbonTrail()
    {
        Ribbon.SetHeadPosition(CurWorldPos);
        if (Owner.UVAffectorEnable)
            Ribbon.SetUVCoord(LowerLeftUV, UVDimensions);


        Ribbon.SetColor(Color);
        Ribbon.Update();
    }

    public void Update()
    {
        ElapsedTime += Time.deltaTime;

        
        foreach(Affector aft in AffectorList)
        {
            aft.Update();
        }


        Position += Velocity * Time.deltaTime;

        if (Mathf.Abs(Acceleration) > 1e-04)
        {
            Velocity += Velocity.normalized * Acceleration * Time.deltaTime;
        }

        if (SyncClient)
        {
            //Should be optimizing? use "+" when needed.
            //NOTICE: scale and rotation is synced, please make sure client scale and rotation is you wanted.
            CurWorldPos = ClientTrans.TransformPoint(Position);
            //CurWorldPos = Position + ClientTrans.position;
        }
        else
        {
            CurWorldPos = Position;
        }
        if (Type == 1)
            UpdateSprite();
        else if (Type == 2)
            UpdateRibbonTrail();

        LastWorldPos = CurWorldPos;

        if (ElapsedTime > LifeTime && LifeTime > 0)
        {
            Reset();
            Remove();
        }
    }
}
