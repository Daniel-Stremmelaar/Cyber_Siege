using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBeacon : MonoBehaviour
{
    public float triggerDistance;
    private GameObject player;
    private Tutorial tutorial;
    // Start is called before the first frame update
    void Start()
    {
        if(triggerDistance == 0f)
        {
            triggerDistance = 1.0f;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        tutorial = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<Tutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag == "MovementBeacon")
        {
            if (Vector3.Distance(player.transform.position, transform.position) < triggerDistance)
            {
                tutorial.NextStage();
                gameObject.SetActive(false);
            }
        }
    }

    public void Seen()
    {
        print("seen");
        tutorial.NextStage();
        gameObject.SetActive(false);
    }
}
