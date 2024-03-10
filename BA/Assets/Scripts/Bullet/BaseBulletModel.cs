using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBulletModel : ScriptableObject
{
    public abstract void Apply(BulletState bulletState);


}
