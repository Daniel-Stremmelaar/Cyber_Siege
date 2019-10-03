using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Tutorial stage management")]
    public int stage;
    public bool next;

    [Header("Tutorial images")]
    public List<Sprite> tutorialImages = new List<Sprite>();
    public GameObject image;

    [Header("Voice lines")]
    public AudioSource announcerSource;
    public List<AudioClip> voiceLines = new List<AudioClip>();
    private int index;

    [Header("Time")]
    private float timer;
    public float startDelay;
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        next = true;
        timer = startDelay;
        image.SetActive(false);
        StartCoroutine("StartDelay");
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
        announcerSource.PlayOneShot(voiceLines[stage]);
        image.GetComponent<Image>().sprite = tutorialImages[stage];
        image.SetActive(true);
    }

    IEnumerator StartDelay()
    {
        next = true;
        yield return new WaitForSeconds(startDelay);
    }
}
