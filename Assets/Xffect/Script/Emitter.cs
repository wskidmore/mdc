using UnityEngine;
using System.Collections;
 
public class Emitter {
    public EffectLayer Layer;

    float EmitterElapsedTime = 0f;
    float EmitDelayTime = 0f;
    bool IsFirstEmit = true;
    float EmitLoop = 0f;
    Vector3 LastClientPos = Vector3.zero;
    
    public Emitter(EffectLayer owner)
    {
        Layer = owner;
        EmitLoop = Layer.EmitLoop;
        LastClientPos = Layer.ClientTransform.position;
    }
    public void Reset()
    {
        EmitterElapsedTime = 0f;
        EmitDelayTime = 0f;
        IsFirstEmit = true;
        EmitLoop = Layer.EmitLoop;
    }

    protected int EmitByDistance()
    {
        Vector3 diff = Layer.ClientTransform.position - LastClientPos;
        if (diff.magnitude >= Layer.DiffDistance)
        {
            LastClientPos = Layer.ClientTransform.position;
            return 1;
        }
        else
        {
            return 0;
        }
    }

    protected int EmitByRate()
    {
        //check chance to this loop;
        int random = Random.Range(0, 100);
        if (random >= 0 && random > Layer.ChanceToEmit)
        {
            return 0;
        }

        //check delay
        EmitDelayTime += Time.deltaTime;
        if (EmitDelayTime < Layer.EmitDelay && !IsFirstEmit)
        {
            return 0;
        }
        EmitterElapsedTime += Time.deltaTime;
        //time expired
        if (EmitterElapsedTime >= Layer.EmitDuration)
        {
            if (EmitLoop > 0)
                EmitLoop--;
            EmitterElapsedTime = 0;
            EmitDelayTime = 0;
            IsFirstEmit = false;
        }
        if (EmitLoop == 0)
        {
            return 0;
        }
        //no available nodes
        if (Layer.AvailableNodeCount == 0)
            return 0;

        //decided how many nodes to emit
        int numToEmit = (int)(EmitterElapsedTime * Layer.EmitRate) - (Layer.ActiveENodes.Length - Layer.AvailableNodeCount);
        int needToEmit = 0;
        if (numToEmit > Layer.AvailableNodeCount)
            needToEmit = Layer.AvailableNodeCount;
        else
            needToEmit = numToEmit;
        if (needToEmit <= 0)
            return 0;
        return needToEmit;
    }

    public Vector3 GetEmitRotation(EffectNode node)
    {
        Vector3 ret = Vector3.zero;
        //Set Direction:
        //NOTICE:SPHERE AND CIRCLE Emitter, default dir is spread from the center. not influenced with the IsRandomDir and VelocityAxis.
        if (Layer.EmitType == (int)EMITTYPE.SPHERE)
        {
            if (!Layer.SyncClient)
            {
                ret = node.Position - (Layer.ClientTransform.position + Layer.EmitPoint);
            }
            else
            {
                ret = node.Position - Layer.EmitPoint;
            }
        }
        else if (Layer.EmitType == (int)EMITTYPE.CIRCLE)
        {
            Vector3 dir;
            if (!Layer.SyncClient)
                dir = node.Position - (Layer.ClientTransform.position + Layer.EmitPoint);
            else
                dir = node.Position - Layer.EmitPoint;
            Vector3 target = Vector3.RotateTowards(dir, Layer.CircleDir, (90 - Layer.AngleAroundAxis) * Mathf.Deg2Rad, 1);
            Quaternion rot = Quaternion.FromToRotation(dir, target);
            ret = rot * dir;
        }
        else if (Layer.IsRandomDir)
        {
            //first, rotate y around z 30 degrees
            Quaternion rotY = Quaternion.Euler(0, 0, Layer.AngleAroundAxis);
            //second, rotate around y 360 random dir;
            Quaternion rotAround = Quaternion.Euler(0, Random.Range(0, 360), 0);
            //last, rotate the dir to OriVelocityAxis
            Quaternion rotTo = Quaternion.FromToRotation(Vector3.up, Layer.OriVelocityAxis);
            ret = rotTo * rotAround * rotY * Vector3.up;
        }
        else
        {
            ret = Layer.OriVelocityAxis;
        }
        return ret;
    }
    public void SetEmitPosition(EffectNode node)
    {
        Vector3 retPos  = Vector3.zero;
        if (Layer.EmitType == (int)EMITTYPE.BOX)
        {
            Vector3 center = Layer.EmitPoint;
            float x = Random.Range(center.x - Layer.BoxSize.x / 2, center.x + Layer.BoxSize.x / 2);
            float y = Random.Range(center.y - Layer.BoxSize.y / 2, center.y + Layer.BoxSize.y / 2);
            float z = Random.Range(center.z - Layer.BoxSize.z / 2, center.z + Layer.BoxSize.z / 2);
            retPos.x = x; retPos.y = y; retPos.z = z;
            if (!Layer.SyncClient)
                retPos = Layer.ClientTransform.position + retPos;
        }
        else if (Layer.EmitType == (int)EMITTYPE.POINT)
        {
            retPos = Layer.EmitPoint;
            if (!Layer.SyncClient)
            {
                retPos = Layer.ClientTransform.position + Layer.EmitPoint;
            }
        }
        else if (Layer.EmitType == (int)EMITTYPE.SPHERE)
        {
            retPos = Layer.EmitPoint;
            if (!Layer.SyncClient)
            {//同步的话在NodeUpdate里会更新位置
                retPos = Layer.ClientTransform.position + Layer.EmitPoint;
            }
            Vector3 r = Vector3.up * Layer.Radius;
            Quaternion rot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            retPos = rot * r + retPos;
        }
        //NOTICE LINE Direction currently is based on ClientTransform's forward
        else if (Layer.EmitType == (int)EMITTYPE.LINE)
        {
            Vector3 left = Layer.EmitPoint + Layer.ClientTransform.localRotation * Vector3.forward * Layer.LineLengthLeft;
            Vector3 right = Layer.EmitPoint + Layer.ClientTransform.localRotation * Vector3.forward * Layer.LineLengthRight;
            Vector3 dir = right - left;
            float p = (float)(node.Index + 1) / Layer.MaxENodes;
            float length = dir.magnitude * p;
            retPos = left + dir.normalized * length;
            if (!Layer.SyncClient)
                retPos = Layer.ClientTransform.TransformPoint(retPos);
        }
        else if (Layer.EmitType == (int)EMITTYPE.CIRCLE)
        {
            float p = (float)(node.Index + 1) / Layer.MaxENodes;
            float rangle;
            rangle = 360 * p;
            Quaternion rotY = Quaternion.Euler(0, rangle, 0);
            Vector3 v = rotY * (Vector3.right * Layer.Radius);
            Quaternion rotTo = Quaternion.FromToRotation(Vector3.up, Layer.CircleDir);
            retPos = rotTo * v;
            if (!Layer.SyncClient)
                retPos = Layer.ClientTransform.position + retPos + Layer.EmitPoint;
            else
                retPos = retPos + Layer.EmitPoint;
        }
        node.SetLocalPosition(retPos);
    }
    public int GetNodes()
    {
        if (Layer.IsEmitByDistance)
            return EmitByDistance();
        else
            return EmitByRate();
    }
}
