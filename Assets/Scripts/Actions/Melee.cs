using UnityEngine;
using System.Collections;

public class Melee : Action
{
    override public bool OnFire(Transform playerTransform, Character character)
    {
        return true;
    }
}
