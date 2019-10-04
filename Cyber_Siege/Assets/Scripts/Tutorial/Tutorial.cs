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
    public List<GameObject> tutorialObjects = new List<GameObject>();

    [Header("Time")]
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
            StageCaller(stage);
        }
    }

    public void StageCaller(int i)
    {
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

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        next = true;
    }
}
