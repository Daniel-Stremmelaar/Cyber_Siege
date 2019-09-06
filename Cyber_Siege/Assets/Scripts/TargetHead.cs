using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHead : Target
{
    public float headshotTime;
    public Target mainBody;

    public override void Hit()
    {
        gameObject.GetComponent<MeshRenderer>().material = red;
        manager.timer -= headshotTime;
        mainBody.Hit();
    }
}
