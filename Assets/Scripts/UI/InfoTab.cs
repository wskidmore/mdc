using UnityEngine;
using System.Collections;

public class InfoTab : MonoBehaviour
{
    public UILabel PlayerNameLabel;
    public GameObject Chars;

    private int activeCharIndex = -1;

    private CharacterManager manager;

    // Use this for initialization
    void Start()
    {
        manager = Chars.GetComponent<CharacterManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ApplyChar(int charIndex)
    {
        Character character = manager.GetCharacterByIdex(charIndex);
        activeCharIndex = charIndex;

        /* Apply all variables */
        PlayerNameLabel.text = character.Name;
    }
}
