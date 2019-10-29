using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSprintTime : MonoBehaviour
{
    public float time;
    private float timeReset;
    public GameObject shutter;
    public GameObject shutterText;
    public bool running;
    private Tutorial tutorial;
    public GameObject player;
    public Vector3 cornerOne;
    public Vector3 cornerTwo;
    public Text text;

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
                text.text = time.ToString("F4");
                shutter.SetActive(false);
                shutterText.SetActive(false);
            }
        }
        if(running == true)
        {
            time -= Time.deltaTime;
            text.text = time.ToString("F4");
        }
        if(time <= 0)
        {
            time = 0.0000f;
            time.ToString("F4");
            shutter.SetActive(true);
            shutterText.SetActive(true);
        }
        if(tutorial.CheckStage() == 7)
        {
            text.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(tutorial.CheckStage() == 6)
        {
            running = true;
            text.gameObject.SetActive(true);
        }
    }
}
