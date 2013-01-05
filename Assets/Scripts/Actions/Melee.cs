using UnityEngine;
using System.Collections;

public class Melee : Action
{
    public override string Name {
        get {
            return "Melee";
        }
        set {
            base.Name = value;
        }
    }

    override public bool OnFire(Transform playerTransform, Character character)
    {
        return true;
    }
}
