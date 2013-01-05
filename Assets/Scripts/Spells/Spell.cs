using UnityEngine;
using System.Collections;

public class Spell : Action
{

    override public bool OnFire(Transform playerTransform, Character character)
    {
        return true;
    }

    virtual public bool CanLearn(Character character)
    {
        return true;
    }

    virtual public bool CanCast(Character character)
    {
        return true;
    }

}
