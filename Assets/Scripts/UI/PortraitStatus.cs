using UnityEngine;
using System.Collections;

public class PortraitStatus : MonoBehaviour
{
    public UISprite target;
    public GameObject PanelGameObject;
    public int CharIndex = 0;
    private string activeName = "active_char";
    private string unActiveName = "empty_char";
    private InfoTab infoTab;

    // Use this for initialization
    void Start()
    {
        if (target == null) target = GetComponentInChildren<UISprite>();
        infoTab = PanelGameObject.GetComponent<InfoTab>();
    }

    void Activate()
    {
        if (target == null) return;
        target.spriteName = activeName;
        target.MakePixelPerfect();
    }
    void DeActivate()
    {
        if (target == null) return;
        target.spriteName = unActiveName;
        target.MakePixelPerfect();
    }

    void OnClick()
    {
        infoTab.ApplyChar(CharIndex);
        NGUITools.SetActive(PanelGameObject, true);
    }
}
