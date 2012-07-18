using UnityEngine;
using System.Collections;

public class DangerTrigger : MonoBehaviour
{

    private int _id;

    public void Start()
    {
        _id = this.transform.parent.GetInstanceID();
    }

    public void OnTriggerEnter(Collider o)
    {
        if (o.tag == "Player")
        {
            GameStateManager.OnEnterDangerTrigger(_id);
        }
    }
    public void OnTriggerExit(Collider o)
    {
        if (o.tag == "Player")
        {
            GameStateManager.OnLeaveDangerTrigger(_id);
        }
    }
}
