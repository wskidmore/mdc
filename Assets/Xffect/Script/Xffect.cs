using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Xffect")]
public class Xffect : MonoBehaviour {

    Dictionary<string, VertexPool> MatDic = new Dictionary<string, VertexPool>();
    List<EffectLayer> EflList = new List<EffectLayer>();

    //Editable
    public float LifeTime = -1f;
    protected float ElapsedTime = 0f;
     

    void Awake()
    {
        Initialize();
    }


	public void Initialize () {

        if (EflList.Count > 0)
        {//already inited.
            return;
        }

        foreach (Transform child in transform)
        {
            EffectLayer el = (EffectLayer)child.GetComponent(typeof(EffectLayer));
            if (el == null || el.Material == null)
                continue;
            Material mat = el.Material;
            EflList.Add(el);
            MeshFilter Meshfilter;
            MeshRenderer Meshrenderer;

            Transform oldMesh = transform.Find("mesh " + mat.name);
            if (oldMesh != null)
            {//already instaniate by object cache.just recreate vertex pool.\
                Meshfilter = (MeshFilter)oldMesh.GetComponent(typeof(MeshFilter));
                Meshrenderer = (MeshRenderer)oldMesh.GetComponent(typeof(MeshRenderer));
                Meshfilter.mesh.Clear();
                MatDic[mat.name] = new VertexPool(Meshfilter.mesh, mat);
            }
            if (!MatDic.ContainsKey(mat.name))
            {
                GameObject obj = new GameObject("mesh " + mat.name);
                obj.transform.parent = this.transform;
                obj.AddComponent("MeshFilter");
                obj.AddComponent("MeshRenderer");
                Meshfilter = (MeshFilter)obj.GetComponent(typeof(MeshFilter));
                Meshrenderer = (MeshRenderer)obj.GetComponent(typeof(MeshRenderer));
                Meshrenderer.castShadows = false;
                Meshrenderer.receiveShadows = false;
                Meshrenderer.renderer.material = mat;
                MatDic[mat.name] = new VertexPool(Meshfilter.mesh, mat);
            }
        }
        foreach (EffectLayer efl in EflList)
        {
            efl.Vertexpool = MatDic[efl.Material.name];
        }
	}

    public void SetClient(Transform client)
    {
        foreach (EffectLayer el in EflList)
        {
            el.ClientTransform = client;
        }
    }

    public void SetDirectionAxis(Vector3 axis)
    {
        foreach (EffectLayer el in EflList)
        {
            el.OriVelocityAxis = axis;
        }
    }

    public void SetEmitPosition(Vector3 pos)
    {
        foreach (EffectLayer el in EflList)
        {
            el.EmitPoint = pos;
        }
    }

    public void DeActive()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.active = false;
        }
        this.gameObject.active = false;
    }

    void Start()
    {
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
        //Make sure every child pos = 0
        foreach (Transform child in transform)
        {
            child.transform.position = Vector3.zero;
            child.transform.rotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
        }
        foreach (EffectLayer el in EflList)
        {
            el.StartCustom();
        }
    }

    public void Active()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.active = true;
        }
        this.gameObject.active = true;
        ElapsedTime = 0f;
    }

	// Fixed Update may be slower.
	void Update () {
        ElapsedTime += Time.deltaTime;
        foreach (EffectLayer el in EflList)
        {
            if (ElapsedTime > el.StartTime)
            {
                el.FixedUpdateCustom();
            }
                
        }
	}

    void LateUpdate()
    {
        foreach (KeyValuePair<string, VertexPool> pair in MatDic)
        {
            pair.Value.LateUpdate();
        }
        if (ElapsedTime > LifeTime && LifeTime >= 0)
        {
            foreach (EffectLayer el in EflList)
            {
                el.Reset();
            }
            DeActive();
            ElapsedTime = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        //foreach (KeyValuePair<string, VertexPool> pair in MatDic)
        {
            //Mesh mesh = pair.Value.Mesh;
            //Gizmos.DrawWireCube(tempre.bounds.center, tempre.bounds.size);
            //Debug.Log(mesh.bounds.size);
        }
    }
}
