using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerCamera, standingCamPos, crouchingCamPos, crouchWalkCamPos, standingWalkCamPos;
    [SerializeField] float transitionModifier;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float movementSpeedModifier, rotationModifier;
    [SerializeField] float sidewaysWalkDebuff, backwardsWalkDebuff, crouchWalkDebuff;
    [SerializeField] bool crouching;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (crouching)
        {
            if (Input.GetButtonUp("Crouch"))
            {
                crouching = false;
                playerAnimator.SetBool("Crouching", crouching);
            }
        }
        else
        {
            if (Input.GetButtonDown("Crouch"))
            {
                crouching = true;
                playerAnimator.SetBool("Crouching", crouching);
            }
        }
        Vector3 movementAmount = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerAnimator.SetFloat("SidewaysWalking", movementAmount.x);
        playerAnimator.SetFloat("ForwardWalking", movementAmount.z);
        if(movementAmount != Vector3.zero)
        {
            if(movementAmount.z == 0)
            {
                movementAmount *= (1 - (sidewaysWalkDebuff / 100));
            }
            else
            {
                if(movementAmount.z < 0)
                {
                    movementAmount *= (1 - (backwardsWalkDebuff / 100));
                }
            }
            if (crouching)
            {
                movementAmount *= (1 - (crouchWalkDebuff / 100));
                playerCamera.position = Vector3.MoveTowards(playerCamera.position, crouchWalkCamPos.position, transitionModifier * Time.deltaTime);
            }
            else
            {
                playerCamera.position = Vector3.MoveTowards(playerCamera.position, standingWalkCamPos.position, transitionModifier * Time.deltaTime);
            }
            transform.Translate(movementAmount * movementSpeedModifier * Time.deltaTime);
        }
        else
        {
            if (crouching)
            {
                playerCamera.position = Vector3.MoveTowards(playerCamera.position, crouchingCamPos.position, transitionModifier * Time.deltaTime);
            }
            else
            {
                playerCamera.position = Vector3.MoveTowards(playerCamera.position, standingCamPos.position, transitionModifier * Time.deltaTime);
            }
        }
        Vector2 cameraRotationAmount = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        transform.Rotate(new Vector3(0, cameraRotationAmount.y, 0) * Time.deltaTime * rotationModifier);
        playerCamera.Rotate(new Vector3(cameraRotationAmount.x, 0, 0) * Time.deltaTime * rotationModifier);
    }
}
