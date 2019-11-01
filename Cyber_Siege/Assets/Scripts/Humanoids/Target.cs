using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
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
    public IngameManager manager;
    public TargetHead head;
    public float health;
    public float timePenaltyOnAlive;

    private void Start()
    {
        hologram = gameObject.GetComponent<Renderer>().material;
        hologramTime = Random.Range(minTime, maxTime);
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<IngameManager>();
    }

    private void Update()
    {
        Flicker();
    }

    public void Hit(float damage, float timeReductionOnDeath)
    {
        health -= damage;
        if(health <= 0)
        {
            Death(timeReductionOnDeath);
        }
    }

    public virtual void Death(float timeChange)
    {
        GameObject.FindGameObjectWithTag("Manager").GetComponent<IngameManager>().targets.Remove(this);
        GameObject.FindGameObjectWithTag("Manager").GetComponent<IngameManager>().ChangeTime(timeChange);
        manager.targets.Remove(GetComponent<Target>());
        DestroyImmediate(gameObject);
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
