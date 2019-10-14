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
        StartCoroutine(FadeOutDelay());
    }

    public void PopIn()
    {
        if(timeChange != null)
        {
            timeChange.color = originalText;
        }
        if(background != null)
        {
            background.color = originalBack;
        }
        f = 0;
    }

    private IEnumerator FadeOutRoutine()
    {
        for(f = 0.0f; f < fadeOutTime; f += Time.deltaTime)
        {
            if(timeChange != null)
            {
                timeChange.color = Color.Lerp(originalText, Color.clear, Mathf.Min(1, f / fadeOutTime));
            }
            if(background != null)
            {
                background.color = Color.Lerp(originalBack, Color.clear, Mathf.Min(1, f / fadeOutTime));
            }
            yield return null;
        }
    }

    public IEnumerator FadeOutDelay()
    {
        yield return new WaitForSeconds(fullTime);
        StartCoroutine(FadeOutRoutine());
    }
}
