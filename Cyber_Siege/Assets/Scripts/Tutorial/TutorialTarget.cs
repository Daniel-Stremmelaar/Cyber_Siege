using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTarget : MonoBehaviour
{
    [Header("Hologram flicker")]
    public float minTime;
    public float maxTime;
    public float minAlpha;
    public float maxAlpha;
    public Material hologram;
    public float hologramAlpha;
    public float hologramTime;
    public float speedRef;

    [Header("Mechanics")]
    public float headshotTime;
    private Tutorial tutorial;
    private TutorialTargetChecker checker;

    private void Start()
    {
        hologram = gameObject.GetComponent<Renderer>().material;
        hologramTime = Random.Range(minTime, maxTime);
        tutorial = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<Tutorial>();
        checker = GameObject.FindGameObjectWithTag("TutorialChecker").GetComponent<TutorialTargetChecker>();
    }

    private void Update()
    {
        Flicker();
    }

    public virtual void Hit()
    {
        if (!tutorial.CheckRunning())
        {
            tutorial.ChangeRunning();
        }
        if(gameObject.tag == "HeadshotTutorial")
        {
            tutorial.ChangeTimer(-headshotTime);
        }
        checker.targets.Remove(gameObject.GetComponent<TutorialTarget>());
        Destroy(gameObject);
    }

    public virtual void Flicker()
    {
        hologramTime -= Time.deltaTime;
        if (hologramTime <= 0)
        {
            hologramAlpha = Random.Range(minAlpha, maxAlpha);
            hologramTime = Random.Range(minTime, maxTime);
            hologram.SetFloat("_AlphaAdjust", Mathf.SmoothDamp(hologram.GetFloat("_AlphaAdjust"), hologramAlpha, ref speedRef, hologramTime));
        }
    }
}
