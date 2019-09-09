using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GeneralManager manager;
    public Material red;

    public virtual void Hit()
    {
        gameObject.GetComponent<MeshRenderer>().material = red;
        manager.targets.Remove(gameObject.GetComponent<Target>());
        Destroy(gameObject);
    }
}
