using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookUpDown : MonoBehaviour {

    [Header ("Look")]
    private Vector3 r;
    public float rotSpeed;

    [Header("Fire")]
    public Text ammoText;
    public int ammo;
    public int ammoCap;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ammoText.text = ammo.ToString() + "/" + ammoCap.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        //rotation look
        r.x += Input.GetAxis("Mouse Y") * rotSpeed;
        r.x = Mathf.Clamp(r.x, -50.0f, 50.0f);
        transform.eulerAngles = (new Vector3(r.x, transform.eulerAngles.y, 0.0f));

        if (Input.GetButtonDown("Fire1") && ammo > 0)
        {
            Fire();
        }

        if (Input.GetButtonDown("Reload"))
        {
            ammo = ammoCap;
            ammoText.text = ammo.ToString() + "/" + ammoCap.ToString();
        }
    }

    public void Fire()
    {
        ammo--;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            print("hit");
            if(hit.transform.gameObject.tag == "Target" || hit.transform.gameObject.tag == "Head")
            {
                Debug.Log("hit target");
                hit.transform.gameObject.GetComponent<Target>().Hit();
            }
        }
        ammoText.text = ammo.ToString() + "/" + ammoCap.ToString();
    }
}
