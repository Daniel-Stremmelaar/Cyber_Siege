using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    public Material blue;
    public Material red;

    public void Hit()
    {
        gameObject.GetComponent<MeshRenderer>().material = red;
    }
}
