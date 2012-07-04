using UnityEngine;
using System.Collections;

public class CloseTabs : MonoBehaviour
{

    public GameObject PanelGameObject;

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
        NGUITools.SetActive(PanelGameObject, false);
    }

}
