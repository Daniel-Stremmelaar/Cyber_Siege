using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingText : MonoBehaviour
{
    public Text timeChange;
    public float fadeOutTime;
    private Color original;
    private float f;

    private void Start()
    {
        original = timeChange.color;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void PopIn()
    {
        timeChange.color = original;
        f = 0;
    }

    private IEnumerator FadeOutRoutine()
    {
        for(f = 0.0f; f < fadeOutTime; f += Time.deltaTime)
        {
            timeChange.color = Color.Lerp(original, Color.clear, Mathf.Min(1, f / fadeOutTime));
            yield return null;
        }
    }
}
