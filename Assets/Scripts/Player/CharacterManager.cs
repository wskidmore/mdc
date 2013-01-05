using UnityEngine;
using System.Collections;
using JsonFx.Json;
using System.IO;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    public Character[] Characters;
    private int _activeIndex = -1;

    void Start()
    {
        // Load from save state
        /*
        Characters[0].LearnSpell("FireBolt");
        Characters[0].SetSpell("FireBolt", true);
        */
    }

    // Update is called once per frame
    void Update()
    {
        DetectKeyPresses();
        UpdateInitiative();

    }

    private void UpdateInitiative()
    {
        foreach(var character in Characters)
        {
            if (character.Disabled)
            {
                character.Initiative -= 1;
                if (character.Initiative < 0) character.Initiative = 0;
            }

            if (character.Initiative == 0)
            {
                character.Disabled = false;
            }

        }
    }

    private void DetectKeyPresses()
    {
        // detect char switch
        if (Input.GetButtonUp("Tab") && !GameStateManager.Paused)
        {
            ActivateCharacter(GetNextAvailableIndex());
        }

        if (Input.GetButtonUp("Search"))
        {
            SearchCursor();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (!SearchCursor())
                if (_activeIndex >= 0)
                    Characters[_activeIndex].OnPrimaryAction(transform);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            if (_activeIndex >= 0)
                Characters[_activeIndex].OnAlternateAction(transform);
        }

        // quick save
        if (Input.GetKeyUp(KeyCode.F10))
        {
            SaveLocal();
        }

        // quick reload
        if (Input.GetKeyUp(KeyCode.F12))
        {
            LoadLocal();
        }
        
    }

    public bool SearchCursor()
    {
        RaycastHit hit;
        var searched = false;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 3.0F) && hit.collider.gameObject.tag == "clickable")
        {
            hit.collider.gameObject.SendMessage("Toggle");
            searched = true;
        }

        return searched;
    }

    public int GetNextAvailableIndex()
    {
        var found = false;
        var nextChar = (_activeIndex + 1 >= Characters.Length) ? 0 : _activeIndex + 1;
        while (!found)
        {
            if (!Characters[nextChar].Disabled)
                found = true;
            else
                nextChar = (nextChar + 1 >= Characters.Length) ? 0 : nextChar + 1;
        }

        return nextChar;
    }

    public void ActivateCharacter(int position)
    {
        if (_activeIndex >= 0)
            Characters[_activeIndex].DeActivate();

        _activeIndex = position;

        Characters[_activeIndex].Activate();
    }

    public void DeActivateAll()
    {
        foreach (var character in Characters)
        {
            character.DeActivate();
        }
        _activeIndex = -1;
    }

    public Character GetCharacterByIdex(int index)
    {
        if (index < 0 || index > Characters.Length)
        {
            Debug.LogError("Character Index out of range");
        }

        return Characters[index];
    }

    public void SaveLocal()
    {
        var charWriter = new JsonWriter(new JsonFx.Serialization.DataWriterSettings(new JsonFx.Json.Resolvers.JsonResolverStrategy()));
        var charData = charWriter.Write(Characters);

        var filePath = Application.dataPath + @"/Data/save.json";

        using (var saveFile = new StreamWriter(filePath))
        {
            saveFile.Write(charData);
        }
    }
    public void LoadLocal()
    {
        var filePath = Application.dataPath + @"/Data/save.json";

        var reader = new JsonReader();

        using (var saveFile = new StreamReader(filePath))
        {
            var output = reader.Read<List<Dictionary<string, object>>>(saveFile.ReadToEnd());
            var i = 0;
            foreach (var character in Characters)
            {
                character.ApplyData(output[i]);
                ++i;
            }
        }
    }

    public bool IsElligibleForLevelUp(int level, int exp)
    {
        return LevelSpread[level+1] <= exp;
    }

    public readonly int[] LevelSpread = {0, 0, 1000, 3000, 5000, 7500, 10000};

}
