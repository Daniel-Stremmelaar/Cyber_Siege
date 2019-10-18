using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBeacon : MonoBehaviour
{
    public float triggerDistance;
    public float lookTime;
    public float maxAway;
    public GameObject lookBar;
    private Slider slider;
    private GameObject player;
    private Tutorial tutorial;
    private float awayTime;
    private float startLook;
    // Start is called before the first frame update
    void Start()
    {
        startLook = lookTime;
        if(triggerDistance == 0f)
        {
            triggerDistance = 1.0f;
        }
        if(lookBar != null)
        {
            slider = lookBar.GetComponent<Slider>();
        }
        player = GameObject.FindGameObjectWithTag("Player");
        tutorial = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<Tutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        awayTime += Time.deltaTime;
        if(awayTime > maxAway && lookBar != null)
        {
            lookTime = startLook;
            slider.value = 1-lookTime;
        }
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
        awayTime = 0;
        lookTime -= Time.deltaTime;
        slider.value = 1-lookTime;
        lookBar.SetActive(true);
        if(lookTime <= 0)
        {
            tutorial.NextStage();
            lookBar.SetActive(false);
            gameObject.SetActive(false);
        }
    }

}
