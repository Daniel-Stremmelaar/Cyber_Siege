using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : Target
{
    public float penaltyTime;

    public override void Hit()
    {
        manager.timer += penaltyTime;
        Destroy(gameObject);
    }
}
