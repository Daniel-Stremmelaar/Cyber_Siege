using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHead : Target
{
    public FadingText adjustText;
    public float headshotTime;
    public Target mainBody;

    public override void Hit()
    {
        gameObject.GetComponent<MeshRenderer>().material = red;
        manager.timer -= headshotTime;
        mainBody.Hit();
        adjustText.timeChange.text = "-" + headshotTime.ToString();
        adjustText.PopIn();
        adjustText.FadeOut();
    }
}
