using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellRegistry
{
    public Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();

    public void RegisterSpell(string id, Spell spell)
    {
        Debug.Log("adding..."+id);
        Spells.Add(id, spell);
    }
    public Spell GetSpell(string id)
    {
        Debug.Log("id..." + id);
        return Spells[id];
    }
    public void Initialize()
    {
        RegisterSpell("FireBolt", new FireBolt());
    }
}
