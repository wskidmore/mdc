using UnityEngine;

public class HpStatus : MonoBehaviour
{
    public UISprite target;

    private UIFilledSprite filler;
    private string healthy = "g_bar";
    private string caution = "y_bar";
    private string danger = "r_bar";

    // Use this for initialization
    void Start()
    {
        if (target == null) target = GetComponentInChildren<UISprite>();
        filler = GetComponent<UIFilledSprite>();
    }

    void OnHealthy()
    {
        if (target == null) return;
        target.spriteName = healthy;
        target.MakePixelPerfect();
    }
    void OnCaution()
    {
        if (target == null) return;
        target.spriteName = caution;
        target.MakePixelPerfect();
    }
    void OnDanger()
    {
        if (target == null) return;
        target.spriteName = danger;
        target.MakePixelPerfect();
    }
    void Set(float amount)
    {
        // set state
        if (amount >= .65)
            OnHealthy();
        else
            if (amount > .25 && amount < 0.65)
                OnCaution();
            else
                OnDanger();

        // set fill
        filler.fillAmount = amount;
    }
}
