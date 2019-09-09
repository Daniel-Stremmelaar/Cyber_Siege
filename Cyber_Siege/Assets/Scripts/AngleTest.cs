using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTest : MonoBehaviour
{
    public Transform one, two;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print(Vector3.Angle(one.position, two.position));
    }
}
