using UnityEngine;
using System.Collections;

public class FireBolt : Spell
{
    [JsonFx.Json.JsonIgnoreAttribute]
    public int Cost = 1;
    [JsonFx.Json.JsonIgnoreAttribute]
    public float Speed = 2F;
    [JsonFx.Json.JsonIgnoreAttribute]
    public GameObject Prefab = Resources.Load("FireBolt", typeof (GameObject)) as GameObject;
    private GameObject _instance;

    public override string Name {
        get {
            return "FireBolt";
        }
        set {
            base.Name = value;
        }
    }

    public override bool OnFire(Transform playerTransform, Character character)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // TODO: always mouse pos?
        RaycastHit hit;
        Vector3 targetPos = playerTransform.position + (playerTransform.forward * 100);

        // test for collision
        if (Physics.Raycast(ray, out hit, 100F))
        {
            targetPos = hit.point;
            if (hit.collider.gameObject.tag == "mob")
            {
                var info = new SpellInfo { Damage = GetDamage(), Element = "Fire" };
                hit.collider.gameObject.SendMessage("OnSpellCast", info);
            }
        }

        // create particles
        _instance = Object.Instantiate(Prefab, playerTransform.position, playerTransform.rotation) as GameObject;
        if (_instance != null)
        {
            _instance.audio.Play();
            var instanceStraightScript = _instance.GetComponent<Straight>();
            instanceStraightScript.Speed = Speed;
            instanceStraightScript.Target = targetPos;
            instanceStraightScript.Moving = true;
        }


        return true;
    }

    private float GetDamage()
    {
        return 1.0F;
    }


}
