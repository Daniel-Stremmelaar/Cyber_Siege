using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeFetcher : MonoBehaviour {

    public AudioMixer masterMixer;

    // Use this for initialization
    void Start () {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterMixer.SetFloat("masterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        }
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            masterMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        }
        if (PlayerPrefs.HasKey("EffectsVolume"))
        {
            masterMixer.SetFloat("sfxVolume", PlayerPrefs.GetFloat("EffectsVolume"));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
