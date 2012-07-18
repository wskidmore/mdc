using UnityEngine;
using System.Collections;

public class InfoTab : MonoBehaviour
{
    public UILabel PlayerNameLabel;
    public GameObject Chars;

    public GameObject StatsTab;
    public GameObject SkillsTab;
    public GameObject InventoryTab;

    private Character activeChar;
    private string activeTab = "Stats";

    private CharacterManager manager;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void Setup()
    {
        manager = Chars.GetComponent<CharacterManager>();

        NGUITools.SetActive(StatsTab, true);
        NGUITools.SetActive(SkillsTab, false);
        NGUITools.SetActive(InventoryTab, false);
    }

    public void ShowTab(string tabName, int charIndex)
    {
        if (manager == null)
            Setup();

        if (charIndex >= 0)
        {
            activeChar = manager.GetCharacterByIdex(charIndex);
        }


        if (tabName == "same")
            tabName = activeTab;
        else
            activeTab = tabName;

        switch (tabName)
        {
            case "Stats":
                ShowStats(charIndex);
                NGUITools.SetActive(StatsTab, true);
                NGUITools.SetActive(SkillsTab, false);
                NGUITools.SetActive(InventoryTab, false);
                break;
            case "Skills":
                ShowSkills(charIndex);
                NGUITools.SetActive(StatsTab, false);
                NGUITools.SetActive(SkillsTab, true);
                NGUITools.SetActive(InventoryTab, false);
                break;
            case "Inventory":
                ShowInventory(charIndex);
                NGUITools.SetActive(StatsTab, false);
                NGUITools.SetActive(SkillsTab, false);
                NGUITools.SetActive(InventoryTab, true);
                break;
        }
    }

    void ShowStats(int charIndex)
    {
        /* Apply all variables */
        PlayerNameLabel.text = activeChar.Name;
    }
    void ShowSkills(int charIndex)
    {

    }
    void ShowInventory(int charIndex)
    {

    }
}
