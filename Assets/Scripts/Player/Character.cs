using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    public string Name = "Name";
    public int Level = 1;
    public int Exp = 0;
    public int Hp = 1;
    public int Mp = 0;
    public int MaxHp = 1;
    public int MaxMp = 0;
    public int Strength = 10;
    public int Endurance = 10;
    public int Accuracy = 10;
    public int Speed = 10;
    public int Intelligence = 10;
    public int Wisdom = 10;

    [JsonFx.Json.JsonIgnoreAttribute]
    public int Initiative = 0;

    [JsonFx.Json.JsonIgnoreAttribute]
    public Action Primary;

    public Action Alternate;

    [JsonFx.Json.JsonIgnoreAttribute]
    public bool Disabled = false;

    [JsonFx.Json.JsonIgnoreAttribute]
    public GameObject Portrait;
    [JsonFx.Json.JsonIgnoreAttribute]
    public GameObject HpBar;
    [JsonFx.Json.JsonIgnoreAttribute]
    public GameObject MpBar;

    private readonly Action _melee = new Melee();

    public List<Spell> KnownSpells = new List<Spell>();


    void LevelUp()
    {
        Level += 1;
    }

    void Start()
    {
        Primary = _melee;
        Alternate = _melee;
    }

    public void OnPrimaryAction(Transform playerTransform)
    {
        Primary.OnFire(playerTransform, this);
    }

    public void OnAlternateAction(Transform playerTransform)
    {
        Alternate.OnFire(playerTransform, this);
    }

    public void LearnSpell(string spellName)
    {
        var newSpell = GameStateManager.SpellRegistry.GetSpell(spellName);
        if (!newSpell.CanLearn(this)) return;

        KnownSpells.Add(newSpell);
    }

    public void SetSpell(string spellName)
    {
        var spellToSet = KnownSpells.Find(ByName(spellName));
        Debug.Log(spellToSet.Name);
        Alternate = spellToSet;
    }

    static Predicate<Spell> ByName(string name)
    {
        return spell => spell.Name == name;
    }

    public void ApplyHpDamage(int amount)
    {
        Hp -= amount;

        if (Hp <= 0)
        {
            // die
            Disabled = true;
        }

        UpdateBars();
    }

    public void ApplyMpDamage(int amount)
    {
        Mp -= amount;

        UpdateBars();
    }

    private void UpdateBars()
    {
        var hpPercent = MaxHp > 0 ? Hp / (float)MaxHp : 0;
        var mpPercent = MaxMp > 0 ? Mp / (float)MaxMp : 0;

        HpBar.SendMessage("Set", hpPercent);
        MpBar.SendMessage("Set", mpPercent);
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

    public void ApplyData(Dictionary<string, object> data)
    {
        Name = (string)data["Name"];
        Level = (int)data["Level"];
        Exp = (int)data["Exp"];
        Hp = (int)data["Hp"];
        MaxHp = (int)data["MaxHp"];
        Mp = (int)data["Mp"];
        MaxMp = (int)data["MaxMp"];
        Strength = (int)data["Strength"];
        Endurance = (int)data["Endurance"];
        Accuracy = (int)data["Accuracy"];
        Speed = (int)data["Speed"];
        Intelligence = (int)data["Intelligence"];
        Wisdom = (int)data["Wisdom"];


        // needs to be next to last (before alternate setting)
        KnownSpells.Clear();

        var spells = data["KnownSpells"].GetType().Name == "String[]"
            ? (string[])data["KnownSpells"]
            : new string[0];
        foreach (var spell in spells)
        {
            LearnSpell(spell);
        }

        // alternate is last as spells have to be learned first
        var alternate = (Dictionary<string, object>)data["Alternate"];
        var alternateAction = (string)alternate["Name"];
        if (alternateAction == "Melee")
        {
            Alternate = _melee;
        }
        else
        {
            SetSpell(alternateAction);
        }

        UpdateBars();
    }


}
