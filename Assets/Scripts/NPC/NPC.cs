using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    public float Hp = 3;
    public bool Lootable = false;
    public bool Dead = false;
    public AudioClip hitAudio;
    public AudioClip dieAudio;
    public AudioClip attackAudio;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Hp <= 0 && !Dead)
        {
            Dead = true;
            gameObject.audio.clip = dieAudio;
            gameObject.audio.Play();
            transform.gameObject.animation.Play("die");
            Destroy(gameObject, transform.gameObject.animation["die"].length);
        }

    }

    void OnSpellCast(SpellInfo spellInfo)
    {
        Debug.Log("in OnSpellCast for NPC");
        Hp -= spellInfo.Damage;
        if (Hp > 0)
        {
            transform.gameObject.animation.Play("gethit");
            if (Hp % 2 == 0)
            {
                gameObject.audio.clip = hitAudio;
                gameObject.audio.Play();
            }
        }

    }

}
