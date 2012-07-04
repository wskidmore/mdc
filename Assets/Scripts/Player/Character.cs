using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    public string Name = "Name";

    public int Level = 1;
    public uint Exp = 1;
    public int Hp = 1;
    public int Mp = 0;
    public int MaxHp = 1;
    public int MaxMp = 0;

    public bool disabled = false;

    public int Strength = 10;
    public int Endurance = 10;
    public int Accuracy = 10;
    public int Speed = 10;
    public int Luck = 10;
    public int Intelligence = 10;
    public int Wisdom = 10;

    public Action Primary;
    public Action Alternate;

    public GameObject Portrait;
    public GameObject HpBar;
    public GameObject MpBar;

    public List<Spell> KnownSpells = new List<Spell>();
    public Action Melee = new Melee();

    void LevelUp()
    {
        Level += 1;
    }

    public void OnPrimaryAction(Transform playerTransform)
    {
        if (Primary != null)
            Primary.OnFire(playerTransform, this);
        else
            Melee.OnFire(playerTransform, this);
    }
    public void OnAlternateAction(Transform playerTransform)
    {
        if (Alternate != null)
            Alternate.OnFire(playerTransform, this);
        else
            Melee.OnFire(playerTransform, this);
    }
    public void LearnSpell(string spellName)
    {
        Spell newSpell = GameStateManager.SpellRegistry.GetSpell(spellName);
        if (newSpell.CanLearn(this))
        {
            KnownSpells.Add(newSpell);
        }
    }
    public void SetSpell(string spellName, bool asPrimary)
    {
        Spell spellToSet = KnownSpells.Find(ByName(spellName));

        if (asPrimary)
            Primary = spellToSet;
        else
            Alternate = spellToSet;
    }

    static Predicate<Spell> ByName(string name)
    {
        return delegate(Spell spell)
        {
            return spell.Name == name;
        };
    }

    public void ApplyHpDamage(int amount)
    {
        Hp -= amount;
        HpBar.SendMessage("Set", Mathf.Clamp(Hp / MaxHp, 0, 1));

        if (Hp <= 0)
        {
            // die
            disabled = true;
        }
    }
    public void ApplyMpDamage(int amount)
    {
        Mp -= amount;
        MpBar.SendMessage("Set", Mathf.Clamp(Hp / MaxHp, 0, 1));
    }
    public void Activate()
    {
        if (Portrait != null)
            Portrait.SendMessage("Activate");
    }
    public void DeActivate()
    {
        if (Portrait != null)
            Portrait.SendMessage("DeActivate");
    }

}
