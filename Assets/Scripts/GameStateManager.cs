using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{

    public GameObject PanelGameObject;
    public GameObject DangerStatus;
    public static bool Paused = false;
    public static SpellRegistry SpellRegistry = new SpellRegistry();
    private static readonly Dictionary<string, int> DangerProx = new Dictionary<string, int>();
    private static int _maxDanger = 0;
    private static DangerProxStatus _dangerProxStatus;


    // Use this for initialization
    void Awake()
    {
        SpellRegistry.Initialize();
    }

    void Start()
    {
        NGUITools.SetActive(PanelGameObject, false);
        if (_dangerProxStatus == null)
            _dangerProxStatus = DangerStatus.GetComponent<DangerProxStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Pause"))
            Paused = !Paused;


    }

    public static void OnEnterCautionTrigger(int id)
    {
        DangerProx[id.ToString(CultureInfo.InvariantCulture)] = 1;

        if (1 <= _maxDanger)
            return;

        _maxDanger = 1;
        _dangerProxStatus.Set(1);
    }

    public static void OnLeaveCautionTrigger(int id)
    {
        DangerProx.Remove(id.ToString(CultureInfo.InvariantCulture));
        if (DangerProx.Count > 1)
            return;

        _maxDanger = 0;
        _dangerProxStatus.Set(0);
    }

    public static void OnEnterDangerTrigger(int id)
    {
        DangerProx[id.ToString(CultureInfo.InvariantCulture)] = 2;

        if (2 <= _maxDanger)
            return;

        _maxDanger = 2;
        _dangerProxStatus.Set(2);
    }

    public static void OnLeaveDangerTrigger(int id)
    {
        DangerProx.Remove(id.ToString(CultureInfo.InvariantCulture));

        if (DangerProx.Count == 0)
        {
            _maxDanger = 0;
            _dangerProxStatus.Set(0);
        }

        if (DangerProx.ContainsValue(2))
            return;

        _maxDanger = 1;
        _dangerProxStatus.Set(1);
    }

}
