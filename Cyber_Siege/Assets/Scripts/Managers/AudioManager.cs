using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource backgroundSource;
    Coroutine currentFadeRoutine;
    [SerializeField]GameObject sfxSpawn;

    public float fadeSpeed, normalBackgroundVolume;

    public delegate void VoidDelegate();
    public VoidDelegate onMusicFadedOut, onMusicFadedIn;


    public void InstantiateSFX(AudioClip clip, Vector3 position, bool threeD = false)
    {
        GameObject sound = Instantiate(sfxSpawn, position, Quaternion.identity);
        AudioSource source = sound.GetComponent<AudioSource>();
        if (threeD)
        {
            source.spatialBlend = 1;
        }
        source.clip = clip;
        source.Play();
        Destroy(sound, clip.length);
    }

    public void fadeBackgroundmusicOut()
    {
        CheckFadeRoutine();
        currentFadeRoutine = StartCoroutine(fadeBackgroundOut());
    }

    public void fadeBackgroundmusicIn()
    {
        CheckFadeRoutine();
        currentFadeRoutine = StartCoroutine(fadeBackgroundIn());
    }

    void CheckFadeRoutine()
    {
        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
        }
    }

    IEnumerator fadeBackgroundOut()
    {
        while(backgroundSource.volume > 0)
        {
            backgroundSource.volume -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        if(onMusicFadedOut != null)
        {
            onMusicFadedOut();
        }
        currentFadeRoutine = null;
    }

    IEnumerator fadeBackgroundIn()
    {
        while (backgroundSource.volume < normalBackgroundVolume)
        {
            backgroundSource.volume += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        if (onMusicFadedIn != null)
        {
            onMusicFadedIn();
        }
        currentFadeRoutine = null;
    }
}
