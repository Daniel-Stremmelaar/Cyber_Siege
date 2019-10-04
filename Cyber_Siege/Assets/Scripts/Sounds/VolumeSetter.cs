using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSetter : MonoBehaviour {

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider effectsSlider;
    public float masterVolume;
    public float musicVolume;
    public float effectsVolume;
    public AudioMixer musicMixer;
    public AudioMixer effectsMixer;

    // Use this to set references
    private void Awake()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        }
        else
        {
            masterVolume = 0.5f;
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        }
        masterSlider.value = masterVolume;

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            musicVolume = 0.5f;
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        }
        musicSlider.value = musicVolume;

        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            effectsVolume = PlayerPrefs.GetFloat("EffectsVolume");
        }
        else
        {
            effectsVolume = 0.5f;
            PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
        }
        effectsSlider.value = effectsVolume;
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        masterVolume = masterSlider.value;
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
        }
        musicVolume = musicSlider.value;
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            musicMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        }
        effectsVolume = effectsSlider.value;
        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
            effectsMixer.SetFloat("EffectsVolume", PlayerPrefs.GetFloat("EffectsVolume"));
        }
    }
}
