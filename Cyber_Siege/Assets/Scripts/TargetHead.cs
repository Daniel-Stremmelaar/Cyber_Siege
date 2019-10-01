using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHead : Target
{
    [Header("Head data")]
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

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public override void Flicker()
    {
        
    }
}
