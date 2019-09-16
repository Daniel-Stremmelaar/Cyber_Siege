using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramFlicker : MonoBehaviour
{
    public Material hologram;
    public Vector4 v;

    // Start is called before the first frame update
    void Start()
    {
        print(Shader.PropertyToID("AlphaAdjust"));
        hologram = gameObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Renderer>().material.SetFloat("_AlphaAdjust", 0.0f);
    }
}
