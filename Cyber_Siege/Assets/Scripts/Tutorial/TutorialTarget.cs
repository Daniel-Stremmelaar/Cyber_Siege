using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTarget : Target
{

    [Header("Mechanics")]   
    public float headshotTime;
    public float hostageTime;
    public Animation projector;
    private Tutorial tutorial;
    private TutorialTargetChecker checker;

    private void Start()
    {
        hologram = gameObject.GetComponent<Renderer>().material;
        hologramTime = Random.Range(minTime, maxTime);
        tutorial = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<Tutorial>();
        checker = GameObject.FindGameObjectWithTag("TutorialChecker").GetComponent<TutorialTargetChecker>();
        projector.Play();
    }

    private void Update()
    {
        Flicker();
    }

    public override void Death(float timeChange)
    {
        projector.Stop();
        IngameManager manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<IngameManager>();
        if (manager.currentTimer == null)
        {
            manager.StartTimer();
        }
        manager.ChangeTime(timeChange);
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
