using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeFetcher : MonoBehaviour {

    public AudioMixer musicMixer;
    public AudioMixer effectsMixer;

    // Use this for initialization
    void Start () {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
        }
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        }
        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            effectsMixer.SetFloat("EffectsVolume", PlayerPrefs.GetFloat("EffectsVolume"));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
