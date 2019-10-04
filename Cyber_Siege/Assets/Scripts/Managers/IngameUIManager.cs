﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : MonoBehaviour
{
    public GameObject crosshair;
    public Image hitMarker;
    public float hitmarkerFadeSpeed;
    public float hitmarkerDurationBeforeFade;
    public Coroutine hitmarkerRoutine;

    public IEnumerator Hitmarker()
    {
        Color resetColor = hitMarker.color;
        resetColor.a = 1;
        hitMarker.color = resetColor;
        hitMarker.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitmarkerDurationBeforeFade);
        IngameUIManager uiManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<IngameUIManager>();
        Color hitmarkerColor = hitMarker.color;
        while (hitmarkerColor.a > 0)
        {
            hitmarkerColor.a -= hitmarkerFadeSpeed * Time.deltaTime;
            hitmarkerColor.a = Mathf.Max(0, hitmarkerColor.a);
            hitMarker.color = hitmarkerColor;
            yield return null;
        }
        hitMarker.gameObject.SetActive(false);
        hitmarkerRoutine = null;
    }
}