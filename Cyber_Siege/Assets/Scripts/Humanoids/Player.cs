using System.Collections;
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
    [SerializeField] float slidePower;
    [SerializeField] States currentState;

    [Header("Sizes")]
    [SerializeField] Vector3 standColliderSize;
    [SerializeField] Vector3 standColliderPosition;
    [SerializeField] Vector3 crouchColliderSize, crouchColliderPosition;

    [SerializeField] float slideGapDuration;
    [SerializeField] bool canSlide = true;
    [SerializeField] bool inSlideGap;
    [SerializeField] float minimalSlideAngle, maximalSlideAngle;
    [SerializeField] float slideBoostModifier;
    [SerializeField] float slideVelocityLimiter;
    [SerializeField] float minimalSlideVelocity;
    Coroutine currentGapTimer;

    [SerializeField] Transform feetLocation;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {





        if (currentState != States.Disabled)
        {
            if (currentState != States.MovementImpaired)
            {
                Movement();
                MovementAction();
            }
            if (Input.GetButtonDown("Jump"))
            {
                Collider[] vaultables = Physics.OverlapSphere(transform.position, vaultDetectionRange, interactableMask);
                if (vaultables.Length > 0)
                {
                    vaultables[0].GetComponent<Interactable>().Interact(gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(Slide(slidePower));
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
    public void MovementAction()
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
                if (inSlideGap && canSlide)
                {
                    print("OOF X2");
                    StartCoroutine(Slide(slidePower));
                    return;
                }
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
    }
    public void Movement()
    {
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
                    if (!crouching)
                    {
                        if (running)
                        {
                            inSlideGap = true;
                            movementAmount *= 1 + (sprintBuff * Input.GetAxis("Vertical") / 100);
                            if (currentGapTimer != null)
                            {
                                StopCoroutine(currentGapTimer);
                                currentGapTimer = null;
                            }
                        }
                        else
                        {
                            if (currentGapTimer == null)
                            {
                                currentGapTimer = StartCoroutine(DisableSlideGap(slideGapDuration));
                            }
                        }
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
    public IEnumerator Slide(float launchPower)
    {
        print("SLIDE");
        currentState = States.MovementImpaired;
        GetComponent<Rigidbody>().velocity += (transform.forward * launchPower);
        print("Launched");
        while (GetComponent<Rigidbody>().velocity.x > slideVelocityLimiter || GetComponent<Rigidbody>().velocity.z > slideVelocityLimiter)
        {
            yield return null;
        }
        while (Input.GetButton("Crouch"))
        {
            Ray rayForward = new Ray(feetLocation.position, feetLocation.forward);
            Ray rayDownward = new Ray(feetLocation.position, -feetLocation.up);
            RaycastHit hitData;
            if (!Physics.Raycast(rayForward, 100))
            {
                if (Physics.Raycast(rayDownward, out hitData, 1000))
                {
                    float angle = Vector3.Angle(Vector3.up, hitData.normal);
                    if (angle >= minimalSlideAngle && angle <= maximalSlideAngle)
                    {
                        Vector3 velocityBoost = hitData.normal;
                        velocityBoost.y = 0;
                        velocityBoost *= angle / maximalSlideAngle;
                        print(angle / maximalSlideAngle);
                        GetComponent<Rigidbody>().velocity += (velocityBoost * slideBoostModifier * Time.deltaTime);

                        velocityBoost = GetComponent<Rigidbody>().velocity;
                        float missing = velocityBoost.x;
                        missing /= (minimalSlideVelocity * hitData.normal.x);
                        if (missing < 1)
                        {
                            missing = 1 + (1 - missing);
                            velocityBoost.x = GetComponent<Rigidbody>().velocity.x * missing;
                        }
                        missing = GetComponent<Rigidbody>().velocity.z;
                        missing /= (minimalSlideVelocity * hitData.normal.z);
                        if (missing < 1)
                        {
                            missing = 1 + (1 - missing);
                            velocityBoost.z = GetComponent<Rigidbody>().velocity.z * missing;
                        }
                        GetComponent<Rigidbody>().velocity = velocityBoost;
                    }
                }
            }
            yield return null;
        }
        currentState = States.Normal;
    }
    public IEnumerator Vault(Vector3[] vaultPositions)
    {
        playerAnimator.SetTrigger("Action");
        currentState = States.Disabled;
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().useGravity = false;
        for (int i = 0; i < vaultPositions.Length; i++)
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
    IEnumerator DisableSlideGap(float timeBeforeChange)
    {
        yield return new WaitForSeconds(timeBeforeChange);
        inSlideGap = false;
    }
    public enum States { Normal, Disabled, ActionImpaired, MovementImpaired }
}
