using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    private float depth;
    public float depthSpeed;
    private float horizontal;
    public float horizontalSpeed;
    public float rotSpeed;
    private Vector3 translation;
    private Vector3 rotation;
    public Rigidbody r;

	// Use this for initialization
	void Start () {
        r = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            Move();
        }
        if(Input.GetAxis("Mouse X") != 0)
        {
            Turn();
        }
	}

    private void Move()
    {
        depth = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        translation.z = depth * Time.deltaTime * depthSpeed;
        translation.x = horizontal * Time.deltaTime * horizontalSpeed;
        transform.Translate(translation);
    }

    private void Turn()
    {
        rotation.y += Input.GetAxis("Mouse X") * rotSpeed;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotation.y, 0.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            translation.y = 0;
        }
    }
}
