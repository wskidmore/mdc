using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour
{
    public Character[] Characters;
    private int activeIndex = -1;

    void Start()
    {
        // Load from save state
        Characters[0].LearnSpell("FireBolt");
        Characters[0].SetSpell("FireBolt", true);
    }

    void OnClick()
    {
        RaycastHit hit;
        // check for clickables (doors, switches, etc) at mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 3.0F))
        {
            if (hit.collider.gameObject.tag == "clickable")
                hit.collider.gameObject.SendMessage("Toggle");
        }
        else
        {
            if (activeIndex >= 0)
                Characters[activeIndex].OnPrimaryAction(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // detect char switch
        if (Input.GetButtonUp("Tab") && !GameStateManager.Paused)
        {
            ActivateCharacter(GetNextAvailableIndex());
        }

        if (Input.GetButtonUp("Search"))
        {
            RaycastHit hit;
            // check for clickables (doors, switches etc) in front of player
            if (Physics.Raycast(transform.position, transform.forward, out hit, 3.0F))
                if (hit.collider.gameObject.tag == "clickable")
                    hit.collider.gameObject.SendMessage("Toggle");
        }

        if (Input.GetButtonUp("Fire2"))
        {
            // todo: change this to a mouselook maybe? how to handle monster ID

            //Characters[activeIndex].OnAlternateAction(transform);
        }
    }

    public int GetNextAvailableIndex()
    {
        bool found = false;
        int nextChar = (activeIndex + 1 >= Characters.Length) ? 0 : activeIndex + 1;
        while (!found)
        {
            if (!Characters[nextChar].disabled)
                found = true;
            else
                nextChar = (nextChar + 1 >= Characters.Length) ? 0 : nextChar + 1;
        }

        return nextChar;
    }

    public void ActivateCharacter(int position)
    {
        if (activeIndex >= 0)
            Characters[activeIndex].DeActivate();

        activeIndex = position;

        Characters[activeIndex].Activate();
    }

    public void DeActivateAll()
    {
        foreach (var character in Characters)
        {
            character.DeActivate();
        }
        activeIndex = -1;
    }

    public Character GetCharacterByIdex(int index)
    {
        if (index < 0 || index > Characters.Length)
        {
            Debug.LogError("Character Index out of range");
        }

        return Characters[index];
    }

}
