﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTEST : MonoBehaviour
{
    public Transform cam;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(cam.position);
    }
}
