using UnityEngine;
using System.Collections;

public class MpStatus : MonoBehaviour {
    private UIFilledSprite filler;

    // Use this for initialization
    void Start()
    {
        filler = GetComponent<UIFilledSprite>();
    }

    void Set(float amount)
    {
        filler.fillAmount = amount;
    }

}
