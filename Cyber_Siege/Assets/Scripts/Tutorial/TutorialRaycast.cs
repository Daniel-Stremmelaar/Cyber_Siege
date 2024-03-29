﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRaycast : MonoBehaviour
{
    public GameObject lookBar;
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
            lookBar.SetActive(false);
            if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                if(hit.transform.gameObject.tag == "LookBeacon")
                {
                    hit.transform.gameObject.GetComponent<TutorialBeacon>().Seen();
                }
            }
        }

        /*if (Input.GetButton("Fire1"))
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                if(hit.transform.gameObject.tag == "Humanoid")
                {
                    print("hit");
                    //hit.transform.gameObject.GetComponent<TutorialTarget>().Hit();
                }
            }
        }*/

        if (Input.GetKeyDown(KeyCode.E) && tutorial.CheckStage() == 2)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, 3))
            {
                if (hit.transform.gameObject.tag == "Interactable")
                {
                    tutorial.NextStage();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.R) && tutorial.CheckStage() == 4)
        {
            print("reload");
            tutorial.NextStage();
        }

        if(Input.GetKeyDown(KeyCode.G) && tutorial.CheckStage() == 10)
        {
            print("grenade");
            tutorial.NextStage();
        }
    }
}
