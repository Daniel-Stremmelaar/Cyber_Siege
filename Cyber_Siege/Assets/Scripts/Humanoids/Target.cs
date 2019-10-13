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
    public GeneralManager manager;
    public TargetHead head;
    public float health;

    private void Start()
    {
        hologram = gameObject.GetComponent<Renderer>().material;
        hologramTime = Random.Range(minTime, maxTime);
        //manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GeneralManager>();
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
        Destroy(gameObject);
        //manager.targets.Remove(gameObject.GetComponent<Target>());
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
