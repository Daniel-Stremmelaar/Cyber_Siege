using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTester : MonoBehaviour
{
    public AudioClip boom;
    public AudioSource speaker;
    public ParticleSystem fire;
    public ParticleSystem smoke;
    public ParticleSystem debris;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            speaker.PlayOneShot(boom);
            fire.Play();
            smoke.Play();
            debris.Play();
        }
    }
}
