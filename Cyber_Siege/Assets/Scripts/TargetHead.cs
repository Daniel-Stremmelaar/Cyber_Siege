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
        manager.timer -= headshotTime;
        mainBody.Hit();
        adjustText.timeChange.text = "-" + headshotTime.ToString("F4");
        adjustText.PopIn();
        adjustText.FadeOut();
    }

    public override void Flicker()
    {
        /*hologramTime = mainBody.hologramTime;
        hologramAlpha = mainBody.hologramAlpha;
        hologram.SetFloat("_AlphaAdjust", Mathf.SmoothDamp(hologram.GetFloat("_AlphaAdjust"), hologramAlpha, ref speedRef, hologramTime));*/
    }
}
