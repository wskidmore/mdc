using UnityEngine;
using System.Collections;

public class DangerProxStatus : MonoBehaviour
{
    public UISprite Target;

    private const string Safe = "prox_green";
    private const string Caution = "prox_yellow";
    private const string Danger = "prox_red";

    // Use this for initialization
    void Start()
    {
        if (Target == null) Target = GetComponentInChildren<UISprite>();
    }
    private void OnSafe()
    {
        if (Target == null) return;
        Target.spriteName = Safe;
    }
    private void OnCaution()
    {
        if (Target == null) return;
        Target.spriteName = Caution;
    }
    private void OnDanger()
    {
        if (Target == null) return;
        Target.spriteName = Danger;
    }

    public void Set(int dangerLevel)
    {
        switch (dangerLevel)
        {
            case 0:
                OnSafe();
                break;
            case 1:
                OnCaution();
                break;
            case 2:
                OnDanger();
                break;
        }
    }

}
