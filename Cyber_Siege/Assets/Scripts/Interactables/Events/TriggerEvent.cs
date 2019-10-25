using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public delegate void VoidEvent();
    public VoidEvent onTrigger;
    public string humanoidTag;



    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == humanoidTag)
        {
            if(onTrigger != null)
            {
                onTrigger();
            }
        }
    }
}
