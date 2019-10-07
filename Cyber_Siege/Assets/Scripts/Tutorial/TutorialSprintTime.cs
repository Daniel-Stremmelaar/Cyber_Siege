using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSprintTime : MonoBehaviour
{
    public float time;
    private float timeReset;
    public GameObject shutter;
    public bool running;
    private Tutorial tutorial;
    public GameObject player;
    public Vector3 cornerOne;
    public Vector3 cornerTwo;

    void Start()
    {
        tutorial = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<Tutorial>();
        timeReset = time;
    }

    void Update()
    {
        if(player.transform.position.z >= cornerOne.z && player.transform.position.x >= cornerOne.x && player.transform.position.z <= cornerTwo.z && player.transform.position.x <= cornerTwo.x)
        {
            if(time <= 0)
            {
                running = false;
                time = timeReset;
                shutter.SetActive(false);
            }
        }
        if(running == true)
        {
            time -= Time.deltaTime;
        }
        if(time <= 0)
        {
            shutter.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print(tutorial.CheckStage().ToString());
        if(tutorial.CheckStage() == 6)
        {
            running = true;
        }
    }
}
