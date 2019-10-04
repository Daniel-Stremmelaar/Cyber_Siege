using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRaycast : MonoBehaviour
{
    private Tutorial tutorial;
    private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        tutorial = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<Tutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial.CheckStage() == 1)
        {
            print("cast");
            if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                print("hit");
                if(hit.transform.gameObject.tag == "LookBeacon")
                {
                    print("see");
                    hit.transform.gameObject.GetComponent<TutorialBeacon>().Seen();
                }
            }
        }
    }
}
