using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : Target
{
    public FadingText adjustText;
    public float penaltyTime;

    public override void Hit()
    {
        manager.timer += penaltyTime;
        adjustText.timeChange.text = "+" + penaltyTime.ToString("F4");
        adjustText.PopIn();
        adjustText.FadeOut();
        Destroy(gameObject);
    }
}
