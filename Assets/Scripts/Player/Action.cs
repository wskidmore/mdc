using UnityEngine;
using System.Collections;

public class Action
{
    public virtual string Name { get { return "Action"; } }


    /**
     * Returns true if Fire happened, false if not (ie missed, spell fizzle)
     */

    public virtual bool OnFire(Transform playerTransform, Character character)
    {
        return true;
    }


}