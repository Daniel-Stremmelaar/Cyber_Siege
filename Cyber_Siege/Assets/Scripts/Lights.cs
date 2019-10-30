using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    public Light[] lightsList;
    float distance;
    public float rightAmount;

    void Start()
    {
        // find all objects with lights
        lightsList = FindObjectsOfType(typeof(Light)) as Light[];
        foreach (Light light in lightsList)
        {
            light.enabled = false;
        }
    }

    void Update()
    {
        for (int i = 0; i < lightsList.Length; i++)
        {
            Light test = lightsList[i];
            distance = Vector3.Distance(transform.position, test.transform.position);
            if (distance < rightAmount)
            {
                test.enabled = true;
            }
            else
            {
                test.enabled = false;
            }
        }
    }
}
