using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingText : MonoBehaviour
{
    public Text timeChange;
    public Image background;
    public float fadeOutTime;
    public float fullTime;
    private Color originalText;
    private Color originalBack;
    private float f;
    private Coroutine routine;

    private void Start()
    {
        if(timeChange != null)
        {
            originalText = timeChange.color;
            timeChange.color = Color.clear;
        }
        if(background != null)
        {
            originalBack = background.color;
            background.color = Color.clear;
        }
    }

    public void FadeOut()
    {
        PopIn();
        StartCoroutine(FadeOutDelay());
    }

    public void PopIn()
    {
        if(routine != null)
        {
            StopCoroutine(routine);
            f = 0.0f;
        }
        if (timeChange != null)
        {
            timeChange.color = originalText;
        }
        if(background != null)
        {
            background.color = originalBack;
        }
    }

    private IEnumerator FadeOutRoutine()
    {
        for(f = 0.0f; f < fadeOutTime; f += Time.deltaTime)
        {
            if(timeChange != null)
            {
                timeChange.color = Color.Lerp(timeChange.color, Color.clear, Mathf.Min(1, f / fadeOutTime));
            }
            if(background != null)
            {
                background.color = Color.Lerp(background.color, Color.clear, Mathf.Min(1, f / fadeOutTime));
            }
            yield return null;
        }
    }

    public IEnumerator FadeOutDelay()
    {
        yield return new WaitForSeconds(fullTime);
        routine = StartCoroutine(FadeOutRoutine());
    }
}
