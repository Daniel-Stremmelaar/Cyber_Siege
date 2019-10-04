using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void EndSliderDragEventHandler(float val);

public class VolumeTester : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    public AudioSource source;
    public AudioClip clip;
    bool dragging;

    //public event EndSliderDragEventHandler EndDrag;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonUp("Fire1"))
        {
            if (dragging == true)
            {
                print("yay");
                dragging = false;
                source.PlayOneShot(clip);
            }
        }
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("mouse down");
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
