using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralManager : MonoBehaviour
{
    public GameObject player;
    public float timer;
    public bool running;
    public Text timeText;
    public Vector3 start;
    public Vector3 start2;
    public Vector3 end;
    public Vector3 end2;
    public float targetMissTime;
    public List<Target> targets = new List<Target>();

    // Start is called before the first frame update
    void Start()
    {
        timeText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        if (start.x < player.transform.position.x && player.transform.position.x < start2.x && start.z < player.transform.position.z && player.transform.position.z < start2.z)
        {
            timer = 0.00f;
            running = true;
        }

        if (running)
        {
            timer += Time.deltaTime;
            timeText.text = timer.ToString("F4");
        }

        if (end.x < player.transform.position.x && player.transform.position.x < end2.x && end.z < player.transform.position.z && player.transform.position.z < end2.z && running)
        {
            timer += targetMissTime * targets.Count;
            timeText.text = timer.ToString("F4");
            running = false;
        }
    }
}
