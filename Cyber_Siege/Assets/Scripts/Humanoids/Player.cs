using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float movementSpeedModifier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movementAmount = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerAnimator.SetFloat("SidewaysWalking", movementAmount.x);
        playerAnimator.SetFloat("ForwardWalking", movementAmount.z);
        if(movementAmount != Vector3.zero)
        {
            transform.Translate(movementAmount * movementSpeedModifier * Time.deltaTime);
        }
    }
}
