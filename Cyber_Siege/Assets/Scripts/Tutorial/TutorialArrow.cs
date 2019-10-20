using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    public float distance;
    public float speed;
    private float traveled;
    private bool forward;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(traveled > distance)
        {
            forward = false;
        }
        if(traveled < -distance)
        {
            forward = true;
        }

        if (forward)
        {
            gameObject.transform.Translate(-Vector3.right * Time.deltaTime * speed);
            traveled += Time.deltaTime;
        }
        else
        {
            gameObject.transform.Translate(Vector3.right * Time.deltaTime * speed);
            traveled -= Time.deltaTime;
        }
        
    }
}
