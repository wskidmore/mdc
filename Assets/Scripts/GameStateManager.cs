using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{

    public GameObject PanelGameObject;
    public static bool Paused = false;
    public static int DangerProximity = 0;
    public static SpellRegistry SpellRegistry = new SpellRegistry();

    // Use this for initialization
    void Awake()
    {
        SpellRegistry.Initialize();
    }

    void Start()
    {
        NGUITools.SetActive(PanelGameObject, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Pause"))
            Paused = !Paused;


    }
}
