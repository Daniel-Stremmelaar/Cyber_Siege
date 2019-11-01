using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Lights : MonoBehaviour
{
    public Light[] lightsList;
    float distance;
    float distance2;
    public float rightAmount;
    public float notRightAmount;
    public float radius;
    public Transform pivotpoint;
    RaycastHit hit;
    public LayerMask wall;
    public List<float> minDistance = new List<float>();
    bool stairs;
    int index;

    void Start()
    {
        // find all objects with lights
        lightsList = FindObjectsOfType(typeof(Light)) as Light[];
        foreach (Light light in lightsList)
        {
            if (light.type != LightType.Directional)
            {
                light.enabled = false;
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < lightsList.Length; i++)
        {
            Light test = lightsList[i];
            distance = Vector3.Distance(pivotpoint.position, test.transform.position);
            //distance = distance - 1;
            if (test.type != LightType.Directional)
            {
                pivotpoint.LookAt(test.transform.position);

                if (Physics.Raycast(pivotpoint.position, pivotpoint.transform.forward, out hit, distance, wall))
                {
                    if (hit.transform.tag == "Flore" && distance > rightAmount)
                    {
                        CheckStairs(test.transform);
                    }
                    else if (hit.transform.tag == "Flore" && distance < rightAmount)
                    {
                        print(distance);
                        test.enabled = false;
                    }
                    //distance = Vector3.Distance(transform.position, hit.point);
                }
                else
                {
                    test.enabled = true;
                }
                Debug.DrawRay(pivotpoint.position, pivotpoint.transform.forward * distance, Color.green);
            }
        }
        //minDistance.Clear();
    }

    void CheckStairs(Transform pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos.transform.position, radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].transform.tag == "Stairs" && colliders.Length == 0)
            {
                print(pos+"/"+ );
                minDistance.Add(Vector3.Distance(pos.transform.position, colliders[i].transform.position));
                index = minDistance.IndexOf(minDistance.Min());
                lightsList[index].enabled = true;
                return;
            }
            else
            {
                lightsList[index].enabled = false;
            }
        }
    }
}

