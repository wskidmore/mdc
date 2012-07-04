using UnityEngine;
using System.Collections;

public class RibbonTrail
{
     
    public class Element
    {
        public Vector3 Position;
        public float Width;

        public Element(Vector3 position, float width)
        {
            Position = position;
            Width = width;
        }
    }

    protected VertexPool.VertexSegment Vertexsegment;
    public int MaxElements;
    public Element[] ElementArray;

    public const int CHAIN_EMPTY = 99999;
    public int Head;
    public int Tail;

    protected Vector3 HeadPosition;
    protected float TrailLength;
    protected float ElemLength;
    public float SquaredElemLength;
    protected float UnitWidth;

    protected bool IndexDirty;
    protected Vector2 LowerLeftUV;
    protected Vector2 UVDimensions;
    protected Color Color = Color.white;
    public int ElemCount = 0;

    //0:  up to bottom
    //1:  left to right
    protected int StretchType = 0;

    protected float ElapsedTime = 0f;
    protected float Fps;

    public RibbonTrail(VertexPool.VertexSegment segment, float width, int maxelemnt, float len, Vector3 pos, int stretchType,float maxFps)
    {
        if (maxelemnt <= 2)
        {
            Debug.LogError("ribbon trail's maxelement should > 2!");
        }
        MaxElements = maxelemnt;
        Vertexsegment = segment;
        ElementArray = new Element[MaxElements];
        Head = Tail = CHAIN_EMPTY;
        SetTrailLen(len);
        UnitWidth = width;
        HeadPosition = pos;
        StretchType = stretchType;
        Element dtls = new Element(HeadPosition, UnitWidth);
        IndexDirty = false;
        Fps = 1f / maxFps;

        // Add the start position
        AddElememt(dtls);
        // Add another on the same spot, this will extend
        Element dtls2 = new Element(HeadPosition, UnitWidth);
        AddElememt(dtls2);
    }
    public void ResetElementsPos()
    {
        if (Head != CHAIN_EMPTY && Head != Tail)
        {
            int laste = Head;
            while (true) // until break
            {
                int e = laste;
                // Wrap forwards
                if (e == MaxElements)
                    e = 0;
                ElementArray[e].Position = HeadPosition;
                if (e == Tail)
                    break; // last one
                laste = e + 1;
            }
        }
        //UpdateVertices(Camera.main.transform.position);
        //UpdateIndices();
    }

    public void Reset()
    {
        ResetElementsPos();
    }

    public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
    {
        LowerLeftUV = lowerleft;
        UVDimensions = dimensions;
    }

    public void SetColor(Color color)
    {
        Color = color;
    }

    public void SetTrailLen(float len)
    {
        TrailLength = len;
        ElemLength = TrailLength / (MaxElements - 1);
        SquaredElemLength = ElemLength * ElemLength;
    }

    public void SetHeadPosition(Vector3 pos)
    {
        HeadPosition = pos;
    }


    // not very useful.
    public void Smooth()
    {
        if (ElemCount <= 3)
            return;
        Element headElem = ElementArray[Head];
        int middleElemIdx = Head + 1;
        if (middleElemIdx == MaxElements)
            middleElemIdx = 0;
        int lastElemIdx = middleElemIdx + 1;
        if (lastElemIdx == MaxElements)
            lastElemIdx = 0;
        Element middleElem = ElementArray[middleElemIdx];
        Element lastElem = ElementArray[lastElemIdx];

        Vector3 dir1 = headElem.Position - middleElem.Position;
        Vector3 dir2 = middleElem.Position - lastElem.Position;
        float angle = Vector3.Angle(dir1, dir2);

        if (angle > 60)
        {
            Vector3 target = (headElem.Position + lastElem.Position) / 2;
            Vector3 dir = target - middleElem.Position;
            Vector3 vel = Vector3.zero;
            float time = 0.1f / ((float)angle / 60);
            middleElem.Position = Vector3.SmoothDamp(middleElem.Position, middleElem.Position + dir.normalized * middleElem.Width, ref vel, time);
        }

    }

    public void Update()
    {
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime < Fps)
        {
            return;
        }
        else
        {
            ElapsedTime -= Fps;
        }
        //Smooth();
        bool done = false;
        while (!done)
        {
            Element headElem = ElementArray[Head];
            int nextElemIdx = Head + 1;
            // wrap
            if (nextElemIdx == MaxElements)
                nextElemIdx = 0;
            Element nextElem = ElementArray[nextElemIdx];

            // Vary the head elem, but bake new version if that exceeds element len
            Vector3 newPos = HeadPosition;

            Vector3 diff = newPos - nextElem.Position;
            float sqlen = diff.sqrMagnitude;
            if (sqlen >= SquaredElemLength)
            {
                // Move existing head to mElemLength
                Vector3 scaledDiff = diff * (ElemLength / diff.magnitude);
                headElem.Position = nextElem.Position + scaledDiff;
                // Add a new element to be the new head
                Element dtls = new Element(newPos, UnitWidth);
                //dtls.Position = newPos; dtls.Width = UnitWidth;
                AddElememt(dtls);
                // alter diff to represent new head size
                diff = newPos - headElem.Position;
                // check whether another step is needed or not
                if (diff.sqrMagnitude <= SquaredElemLength)
                    done = true;
            }
            else
            {
                // Extend existing head
                headElem.Position = newPos;
                done = true;
            }

            // Is this segment full?
            if ((Tail + 1) % MaxElements == Head)
            {
                // If so, shrink tail gradually to match head extension
                Element tailElem = ElementArray[Tail];
                int preTailIdx;
                if (Tail == 0)
                    preTailIdx = MaxElements - 1;
                else
                    preTailIdx = Tail - 1;
                Element preTailElem = ElementArray[preTailIdx];

                // Measure tail diff from pretail to tail
                Vector3 taildiff = tailElem.Position - preTailElem.Position;
                float taillen = taildiff.magnitude;
                if (taillen > 1e-06)
                {
                    float tailsize = ElemLength - diff.magnitude;
                    taildiff *= tailsize / taillen;
                    tailElem.Position = preTailElem.Position + taildiff;
                }
            }
        } //end while
        Vector3 eyePos = Camera.main.transform.position;
        UpdateVertices(eyePos);
        UpdateIndices();
    }

    public void UpdateIndices()
    {
        if (!IndexDirty)
            return;
        VertexPool pool = Vertexsegment.Pool;
        // Skip 0 or 1 element segment counts
        if (Head != CHAIN_EMPTY && Head != Tail)
        {
            // Start from head + 1 since it's only useful in pairs
            int laste = Head;
            int ecount = 0;
            while (true) // until break
            {
                int e = laste + 1;
                // Wrap forwards
                if (e == MaxElements)
                    e = 0;
                // indexes of this element are (e * 2) and (e * 2) + 1
                // indexes of the last element are the same, -2
                if (e * 2 >= 65536)
                    Debug.LogError("Too many elements!");

                int baseIdx = Vertexsegment.VertStart + e * 2;
                int lastBaseIdx = Vertexsegment.VertStart + laste * 2;
                //int vidx = (Vertexsegment.Start + ecount * 2) / 4;
                int iidx = Vertexsegment.IndexStart + ecount * 6;


                pool.Indices[iidx + 0] = lastBaseIdx;
                pool.Indices[iidx + 1] = lastBaseIdx + 1;
                pool.Indices[iidx + 2] = baseIdx;
                pool.Indices[iidx + 3] = lastBaseIdx + 1;
                pool.Indices[iidx + 4] = baseIdx + 1;
                pool.Indices[iidx + 5] = baseIdx;


                if (e == Tail)
                    break; // last one

                laste = e;
                ecount++;
            }

            pool.IndiceChanged = true;
        }

        IndexDirty = false;
    }

    public void UpdateVertices(Vector3 eyePos)
    {
        Vector3 chainTangent;
        float uvSegment = 0f;
        float uvLen = 0f;

        float trailLen = ElemLength * (MaxElements - 2);

        //Element headElem = ElementArray[Head];
        //int nextElemIdx = Head + 1;
        //if (nextElemIdx == MaxElements)
        //    nextElemIdx = 0;
        //Element nextElem = ElementArray[nextElemIdx];
        //float headLen = (headElem.Position - nextElem.Position).magnitude;
        //float trailLen = ElemLength * (ElemCount - 2) + headLen;

        if (Head != CHAIN_EMPTY && Head != Tail)
        {
            int laste = Head;
            for (int e = Head; ; ++e) // until break
            {
                // Wrap forwards
                if (e == MaxElements)
                    e = 0;

                Element elem = ElementArray[e];
                if (e * 2 >= 65536)
                    Debug.LogError("Too many elements!");
                int baseIdx = Vertexsegment.VertStart + e * 2;

                // Get index of next item
                int nexte = e + 1;
                if (nexte == MaxElements)
                    nexte = 0;

                if (e == Head)
                {
                    // No laste, use next item
                    chainTangent = ElementArray[nexte].Position - elem.Position;
                }
                else if (e == Tail)
                {
                    // No nexte, use only last item
                    chainTangent = elem.Position - ElementArray[laste].Position;
                }
                else
                {
                    // A mid position, use tangent across both prev and next
                    chainTangent = ElementArray[nexte].Position - ElementArray[laste].Position;

                }

                Vector3 vP1ToEye = eyePos - elem.Position;
                Vector3 vPerpendicular = Vector3.Cross(chainTangent, vP1ToEye);
                vPerpendicular.Normalize();
                vPerpendicular *= (elem.Width * 0.5f);

                Vector3 pos0 = elem.Position - vPerpendicular;
                Vector3 pos1 = elem.Position + vPerpendicular;


                VertexPool pool = Vertexsegment.Pool;

                if (StretchType == 0)
                    uvSegment = (uvLen / trailLen) * Mathf.Abs(UVDimensions.y);
                else
                    uvSegment = (uvLen / trailLen) * Mathf.Abs(UVDimensions.x);
                Vector2 uvCoord = Vector2.zero;
                // pos0
                pool.Vertices[baseIdx] = pos0;
                pool.Colors[baseIdx] = Color;
                if (StretchType == 0)
                {
                    uvCoord.x = LowerLeftUV.x + UVDimensions.x;
                    uvCoord.y = LowerLeftUV.y - uvSegment;
                }
                else
                {
                    uvCoord.x = LowerLeftUV.x + uvSegment;
                    uvCoord.y = LowerLeftUV.y;
                }

                pool.UVs[baseIdx] = uvCoord;
                //pos1
                pool.Vertices[baseIdx + 1] = pos1;
                pool.Colors[baseIdx + 1] = Color;
                if (StretchType == 0)
                {
                    uvCoord.x = LowerLeftUV.x;
                    uvCoord.y = LowerLeftUV.y - uvSegment;
                }
                else
                {
                    uvCoord.x = LowerLeftUV.x + uvSegment;
                    uvCoord.y = LowerLeftUV.y - Mathf.Abs(UVDimensions.y);
                }
                pool.UVs[baseIdx + 1] = uvCoord;

                if (e == Tail)
                    break; // last one
                laste = e;
                uvLen += (ElementArray[nexte].Position - elem.Position).magnitude;

            } // element
            Vertexsegment.Pool.UVChanged = true;
            Vertexsegment.Pool.VertChanged = true;
            Vertexsegment.Pool.ColorChanged = true;
        } // segment valid?
    }
    public void AddElememt(Element dtls)
    {
        if (Head == CHAIN_EMPTY)
        {
            Tail = MaxElements - 1;
            Head = Tail;
            IndexDirty = true;
            ElemCount++;
        }
        else
        {
            if (Head == 0)
            {
                Head = MaxElements - 1;
            }
            else
            {
                --Head;
            }
            // Run out of elements?
            if (Head == Tail)
            {
                // Move tail backwards too, losing the end of the segment and re-using
                // it in the head
                if (Tail == 0)
                    Tail = MaxElements - 1;
                else
                    --Tail;
            }
            else
            {
                ElemCount++;
            }
        }
        ElementArray[Head] = dtls;
        IndexDirty = true;
    }
}