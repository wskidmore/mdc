using UnityEngine;
using System.Collections;

class Straight : MonoBehaviour
{
    public float Speed = 4F;
    public bool Moving = false;
    public bool Finished = false;
    public Vector3 Target;

    private float _incr = 0;

    void Update()
    {
        if (Finished)
            Destroy(gameObject, .01F);

        if (Moving)
        {
            if (_incr <= 1)
                _incr += Speed/100;
            else
                Finished = true;

            transform.position = Vector3.Lerp(transform.position, Target, _incr);
        }
    }
}
