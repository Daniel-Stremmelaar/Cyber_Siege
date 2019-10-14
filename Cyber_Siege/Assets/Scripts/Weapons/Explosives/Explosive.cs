using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Explosive : MonoBehaviour
{
    public GameObject explosionParticle;
    public abstract void Explode();
}
