﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerCamera, standingCamPos, crouchingCamPos, crouchWalkCamPos, standingWalkCamPos;
    [SerializeField] float transitionModifier;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float movementSpeedModifier, rotationModifier;
    [SerializeField] float sidewaysWalkDebuff, backwardsWalkDebuff, crouchWalkDebuff, sprintBuff;
    [SerializeField] bool crouching, running;
    [SerializeField] LayerMask interactableMask;
    [SerializeField] string vaultableTag;
    [SerializeField] float vaultDetectionRange;
    [SerializeField] float vaultSpeed;
    [SerializeField] States currentState;

    [Header("Sizes")]
    [SerializeField] Vector3 standColliderSize;
    [SerializeField] Vector3 standColliderPosition;
    [SerializeField] Vector3 crouchColliderSize, crouchColliderPosition;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        if(currentState != States.Disabled)
        {
            Movement();
            if (Input.GetButtonDown("Jump"))
            {
                Collider[] vaultables = Physics.OverlapSphere(transform.position, vaultDetectionRange, interactableMask);
                if (vaultables.Length > 0)
                {
                    vaultables[0].GetComponent<Interactable>().Interact(gameObject);
                }
            }
        }
    }
    public void CheckMovement()
    {
        if (Input.GetButton("Crouch"))
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }
        if (Input.GetButton("Sprint") && !crouching)
        {
            running = true;
        }
        else
        {
            running = false;
        }
        playerAnimator.SetBool("Running", running);
        playerAnimator.SetBool("Crouching", crouching);
    }
    public void Movement()
    {
        if (crouching)
        {
            if (Input.GetButtonUp("Crouch"))
            {
                if (Input.GetButton("Sprint"))
                {
                    running = true;
                    playerAnimator.SetBool("Running", running);
                }
                crouching = false;
                playerAnimator.SetBool("Crouching", crouching);
                BoxCollider playerCollider = GetComponent<BoxCollider>();
                playerCollider.center = standColliderPosition;
                playerCollider.size = standColliderSize;
            }
        }
        else
        {
            if (Input.GetButtonDown("Crouch"))
            {
                if (running)
                {
                    running = false;
                    playerAnimator.SetBool("Running", running);
                }
                crouching = true;
                playerAnimator.SetBool("Crouching", crouching);
                BoxCollider playerCollider = GetComponent<BoxCollider>();
                playerCollider.center = crouchColliderPosition;
                playerCollider.size = crouchColliderSize;
            }
        }
        if (running)
        {
            if (Input.GetButtonUp("Sprint"))
            {
                running = false;
                playerAnimator.SetBool("Running", running);
            }
        }
        else
        {
            if (Input.GetButtonDown("Sprint") && !crouching)
            {
                running = true;
                playerAnimator.SetBool("Running", running);
            }
        }
        Vector3 movementAmount = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerAnimator.SetFloat("SidewaysWalking", movementAmount.x);
        playerAnimator.SetFloat("ForwardWalking", movementAmount.z);
        if (movementAmount != Vector3.zero)
        {
            if (movementAmount.z == 0)
            {
                movementAmount *= (1 - (sidewaysWalkDebuff / 100));
            }
            else
            {
                if (movementAmount.z < 0)
                {
                    movementAmount *= (1 - (backwardsWalkDebuff / 100));
                }
                else
                {
                    if(!crouching && running)
                    {
                        movementAmount *= 1 + (sprintBuff * Input.GetAxis("Vertical") / 100);
                    }
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
    public IEnumerator Vault(Vector3[] vaultPositions)
    {
        playerAnimator.SetTrigger("Action");
        currentState = States.Disabled;
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().useGravity = false;
        for(int i = 0; i < vaultPositions.Length; i++)
        {
            while (Vector3.Distance(transform.position, vaultPositions[i]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, vaultPositions[i], vaultSpeed * Time.deltaTime);
                yield return null;
            }
        }
        //yield return new WaitForSeconds(0.1f);
        currentState = States.Normal;
        GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().useGravity = true;
        CheckMovement();
        playerAnimator.SetTrigger("Action");
    }
    public enum States { Normal, Disabled, ActionImpaired}
}
