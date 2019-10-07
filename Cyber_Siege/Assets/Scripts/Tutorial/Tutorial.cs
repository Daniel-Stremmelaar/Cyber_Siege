﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Tutorial stage management")]
    private int stage;
    public bool next;

    [Header("Tutorial images")]
    public GameObject image;
    public List<Sprite> tutorialImages = new List<Sprite>();

    [Header("Voice lines")]
    public AudioSource announcerSource;
    public List<AudioClip> voiceLines = new List<AudioClip>();

    [Header("Tutorial objects")]
    public List<GameObject> tutorialObjects = new List<GameObject>();

    [Header("Time")]
    private float timer;
    private bool running;
    public Text timeText;
    public Text adjustText;
    public float startDelay;
    // Start is called before the first frame update
    void Start()
    {
        stage = 0;
        next = false;
        image.SetActive(false);
        StartCoroutine(StartDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if(next)
        {
            StageCaller();
        }
        if (running)
        {
            timer += Time.deltaTime;
        }
        if (timer > 0.001f)
        {
            timeText.text = timer.ToString("F4");
        }
        else
        {
            timeText.text = "";
        }
    }

    public void StageCaller()
    {
        if(stage != 4 && stage != 6)
        {
            timer = 0.000f;
        }
        running = false;
        next = false;
        image.SetActive(false);
        foreach(GameObject g in tutorialObjects)
        {
            if(g != null)
            {
                g.SetActive(false);
            }
        }
        announcerSource.PlayOneShot(voiceLines[stage]);
        image.GetComponent<Image>().sprite = tutorialImages[stage];
        image.SetActive(true);
        if(tutorialObjects[stage] != null)
        {
            tutorialObjects[stage].SetActive(true);
        }
    }

    public void NextStage()
    {
        stage++;
        next = true;
    }

    public int CheckStage()
    {
        return stage;
    }

    public void ChangeTimer(float change)
    {
        timer += change;
    }

    public void ChangeAdjust(float change)
    {
        if(change > 0)
        {
            adjustText.text = "+" + change.ToString("F4");
        }
        else
        {
            adjustText.text = change.ToString("F4");
        }
    }

    public void ChangeRunning()
    {
        running = !running;
    }

    public bool CheckRunning()
    {
        return running;
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        next = true;
    }
}
