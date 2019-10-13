using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public bool instaKill;
    public float damageMultiplier;
    public float timeChange;

    Target owner;

    public void Hit(float damage)
    {
        if (instaKill)
        {
            owner.Death(timeChange);
        }
        else
        {
            owner.Hit(damage * damageMultiplier, timeChange);
        }
    }
}
