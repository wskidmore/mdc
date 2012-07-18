using UnityEngine;
using System.Collections;

public class ShowTab : MonoBehaviour
{

    public GameObject PanelObject;
    public string TargetTab;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnClick()
    {
        InfoTab tabs = PanelObject.GetComponent<InfoTab>();
        tabs.ShowTab(TargetTab, -1);
    }
}
