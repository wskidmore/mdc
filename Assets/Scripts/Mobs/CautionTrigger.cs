using UnityEngine;
using System.Collections;

public class CautionTrigger : MonoBehaviour
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
            GameStateManager.OnEnterCautionTrigger(_id);
        }
    }
    public void OnTriggerExit(Collider o)
    {
        if (o.tag == "Player")
        {
            GameStateManager.OnLeaveCautionTrigger(_id);
        }
    }
}
