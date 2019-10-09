using System.Collections;
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
    public GameObject doorBlock;
    public List<GameObject> tutorialObjects = new List<GameObject>();

    [Header("Tutorial text")]
    public Text subtitleText;
    private FadingText subtitleFade;
    public List<string> subtitles = new List<string>();
    public List<float> subtitleTimes = new List<float>();

    [Header("Tutorial objectives")]
    public Text objectiveText;
    public List<string> objectives = new List<string>();

    [Header("Tutorial world UI")]
    public List<GameObject> uiGuides = new List<GameObject>();

    [Header("Doorblock colours")]
    public Material open;

    [Header("Time")]
    public Text timeText;
    public Text adjustText;
    public float startDelay;
    private float timer;
    private bool running;
    // Start is called before the first frame update
    void Start()
    {
        stage = 0;
        next = false;
        //image.SetActive(false);
        StartCoroutine(StartDelay());
        subtitleFade = subtitleText.gameObject.GetComponent<FadingText>();
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
        if(stage == 6)
        {
            doorBlock.GetComponent<Collider>().isTrigger = true;
            doorBlock.gameObject.GetComponent<MeshRenderer>().material = open;
        }
        if(stage == 3)
        {
            
        }
        running = false;
        next = false;
        //image.SetActive(false);
        foreach(GameObject g in tutorialObjects)
        {
            if(g != null)
            {
                g.SetActive(false);
            }
        }
        objectiveText.text = objectives[stage];
        announcerSource.PlayOneShot(voiceLines[stage]);
        //image.GetComponent<Image>().sprite = tutorialImages[stage];
        //image.SetActive(true);
        if(uiGuides[stage] != null)
        {
            uiGuides[stage].SetActive(true);
        }
        subtitleText.text = subtitles[stage];
        subtitleFade.fadeOutTime = subtitleTimes[stage];
        subtitleText.gameObject.GetComponent<FadingText>().PopIn();
        subtitleText.gameObject.GetComponent<FadingText>().FadeOut();
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
