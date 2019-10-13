using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : Target
{
    public FadingText adjustText;
    public float penaltyTime;

    public override void Death(float timeChange)
    {
        base.Death(timeChange);
    }
}
