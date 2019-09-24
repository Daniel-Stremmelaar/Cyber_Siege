using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform relocate;
    [SerializeField] Animator playerAnimator;
    [SerializeField] LayerMask interactableMask;
    [SerializeField] States currentState;
    [SerializeField] Status currentStatus;
    [SerializeField] Transform feetLocation;

    [Header("Camera")]
    [SerializeField] float rotationModifier;
    [SerializeField] float minCamClamp, maxCamClamp;
    [SerializeField] Transform playerCamera, standingCamPos, crouchingCamPos, crouchWalkCamPos, standingWalkCamPos;
    [SerializeField] float cameraTransitionModifier;

    [Header("Movement")]
    Vector3 lastMovedAmount;
    [SerializeField] float movementSpeedModifier;
    [SerializeField] float sidewaysWalkDebuff, backwardsWalkDebuff, crouchWalkDebuff, sprintBuff;
    [SerializeField] bool crouching, running;

    [Header("Jumping")]
    [SerializeField] Vector3 jumpForce;
    [SerializeField] int resetJumps = 1;
    [SerializeField] int remainingJumps = 1;
    [SerializeField] Vector3 jumpChargeCheckSize;
    [SerializeField] Vector3 jumpCheckPosModifier;

    [Header("Vaulting")]
    [SerializeField] string vaultableTag;
    [SerializeField] float vaultDetectionRange;
    [SerializeField] float vaultSpeed;

    [Header("Sliding")]
    [SerializeField] float slidePower;
    [SerializeField] float slideGapDuration;
    [SerializeField] bool canSlide = true;
    [SerializeField] bool inSlideGap;
    [SerializeField] float minimalSlideAngle, maximalSlideAngle;
    [SerializeField] float slideBoostModifier;
    [SerializeField] float slideVelocityLimiter;
    [SerializeField] float minimalSlideVelocity;
    [SerializeField] float rotateToNormalSpeed;
    Coroutine currentGapTimer;

    [Header("ColliderData")]
    [SerializeField] Vector3 standColliderSize;
    [SerializeField] Vector3 standColliderPosition;
    [SerializeField] Vector3 crouchColliderSize, crouchColliderPosition;
    [SerializeField] Vector3 slideColliderSize, slideColliderPosition;

    [Header("GunData")]
    public Transform gunWieldingPoint;
    public Transform currentGun;

    [Header("Inverse Kinematics")]
    public bool ikEnabled;
    [SerializeField] Transform backbone;
    float backupX;

    // Start is called before the first frame update
    private void OnAnimatorIK(int layerIndex)
    {
        if (enabled && currentGun)
        {
            /*
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, playerCamera.rotation);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, playerCamera.rotation);
            */
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, currentGun.GetComponent<BaseGun>().handlePosition.position);
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, currentGun.GetComponent<BaseGun>().triggerPosition.position);
            /*playerAnimator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
            playerAnimator.SetIKHintPosition(AvatarIKHint.LeftElbow, currentGun.GetComponent<BaseGun>().leftElbowPosition.position);
            playerAnimator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
            playerAnimator.SetIKHintPosition(AvatarIKHint.RightElbow, currentGun.GetComponent<BaseGun>().rightElbowPosition.position);
            */
        }
    }
    private void Awake()
    {
        backupX = backbone.localEulerAngles.x;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void LateUpdate()
    {
        
        if (currentStatus == Status.Normal)
        {
            Rigidbody rigid = GetComponent<Rigidbody>();
            rigid.velocity = Vector3.zero;
            RotateCamera();
        }
    }
    public void FixedUpdate()
    {
        if (currentStatus == Status.Normal)
        {
            Rigidbody rigid = GetComponent<Rigidbody>();
            rigid.velocity = Vector3.zero;
        }
    }
    public void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            transform.position = relocate.position;
        }
        if (currentState != States.Disabled)
        {
            if (currentState != States.Frozen)
            {
                if (currentState != States.MovementImpaired)
                {
                    Movement();
                    MovementAction();
                }
                if (currentState != States.ActionImpaired)
                {
                    ;
                    if (Input.GetButtonDown("Jump"))
                    {
                        Collider[] vaultables = Physics.OverlapSphere(transform.position, vaultDetectionRange, interactableMask);
                        if (vaultables.Length > 0)
                        {
                            vaultables[0].GetComponent<Interactable>().Interact(gameObject);
                        }
                        else
                        {
                            if (Physics.OverlapBox(feetLocation.position + jumpCheckPosModifier, jumpChargeCheckSize / 2).Length > 0)
                            {
                                Jump();
                            }
                        }
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(Slide(slidePower));
        }
        if (currentStatus == Status.Normal)
        {
            Rigidbody rigid = GetComponent<Rigidbody>();
            rigid.velocity = Vector3.zero;
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
    public void Jump()
    {
        remainingJumps--;
        GetComponent<Rigidbody>().AddForce(jumpForce);
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
                if (inSlideGap && canSlide && currentState != States.ActionImpaired)
                {
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
        lastMovedAmount = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerAnimator.SetFloat("SidewaysWalking", lastMovedAmount.x);
        playerAnimator.SetFloat("ForwardWalking", lastMovedAmount.z);
        if (lastMovedAmount != Vector3.zero)
        {
            if (lastMovedAmount.z == 0)
            {
                lastMovedAmount *= (1 - (sidewaysWalkDebuff / 100));
            }
            else
            {
                if (lastMovedAmount.z < 0)
                {
                    lastMovedAmount *= (1 - (backwardsWalkDebuff / 100));
                }
                else
                {
                    if (!crouching)
                    {
                        if (running)
                        {
                            inSlideGap = true;
                            lastMovedAmount *= 1 + (sprintBuff * Input.GetAxis("Vertical") / 100);
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
                lastMovedAmount *= (1 - (crouchWalkDebuff / 100));
                playerCamera.position = Vector3.MoveTowards(playerCamera.position, crouchingCamPos.position, cameraTransitionModifier * Time.deltaTime);
            }
            else
            {
                playerCamera.position = Vector3.MoveTowards(playerCamera.position, standingCamPos.position, cameraTransitionModifier * Time.deltaTime);
            }
            transform.Translate(lastMovedAmount * movementSpeedModifier * Time.deltaTime);
        }
        else
        {
            if (crouching)
            {
                playerCamera.position = Vector3.MoveTowards(playerCamera.position, crouchingCamPos.position, cameraTransitionModifier * Time.deltaTime);
            }
            else
            {
                playerCamera.position = Vector3.MoveTowards(playerCamera.position, standingCamPos.position, cameraTransitionModifier * Time.deltaTime);
            }
        }
    }

    public void RotateCamera()
    {
        //backbone.localEulerAngles = new Vector3(backupX, backbone.localEulerAngles.y, backbone.localEulerAngles.z);
        Vector2 cameraRotationAmount = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        transform.Rotate(new Vector3(0, cameraRotationAmount.y, 0) * Time.deltaTime * rotationModifier);
        playerCamera.Rotate(new Vector3(cameraRotationAmount.x, 0, 0) * Time.deltaTime * rotationModifier);
        //backbone.Rotate(new Vector3(Input.GetAxis("Mouse Y") * Time.deltaTime * rotationModifier, 0, 0));
        //backupX = backbone.localEulerAngles.x;
        //playerCamera.localEulerAngles = new Vector3(Mathf.Clamp(playerCamera.localEulerAngles.x, minCamClamp, maxCamClamp), playerCamera.localEulerAngles.y, playerCamera.localEulerAngles.z);
    }
    public IEnumerator Slide(float launchPower)
    {
        inSlideGap = false;
        Vector3 ogUpwards = transform.up;
        RaycastHit hitData;
        Ray rayDownward;
        Ray rayForward = new Ray(feetLocation.position, feetLocation.forward);
        Vector3 lastHitNormal = Vector3.zero;
        Vector3 localCameraEuler;
        if (!Physics.Raycast(rayForward, 1))
        {
            BoxCollider playerCollider = GetComponent<BoxCollider>();
            playerCollider.size = slideColliderSize;
            playerCollider.center = slideColliderPosition;
            currentStatus = Status.Falling;
            currentState = States.MovementImpaired;
            GetComponent<Rigidbody>().velocity += (transform.forward * launchPower);
            print("Launched");
            while (GetComponent<Rigidbody>().velocity.x > slideVelocityLimiter || GetComponent<Rigidbody>().velocity.z > slideVelocityLimiter)
            {
                rayDownward = new Ray(feetLocation.position, -feetLocation.up);
                if (Physics.Raycast(rayDownward, out hitData, 1000))
                {
                    lastHitNormal = hitData.normal;
                    playerCamera.rotation = Quaternion.RotateTowards(playerCamera.rotation, Quaternion.FromToRotation(hitData.normal, transform.up) * playerCamera.rotation, rotateToNormalSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(transform.up, hitData.normal) * transform.rotation, rotateToNormalSpeed * Time.deltaTime);
                    localCameraEuler = playerCamera.localEulerAngles;
                    localCameraEuler = new Vector3(localCameraEuler.x, 0, 0);
                    playerCamera.localEulerAngles = localCameraEuler;
                }
                yield return null;
            }
            while (Input.GetButton("Crouch"))
            {
                rayDownward = new Ray(feetLocation.position, -feetLocation.up);
                if (Physics.Raycast(rayDownward, out hitData, 100))
                {
                    lastHitNormal = hitData.normal;
                    playerCamera.rotation = Quaternion.RotateTowards(playerCamera.rotation, Quaternion.FromToRotation(hitData.normal, transform.up) * playerCamera.rotation, rotateToNormalSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(transform.up, hitData.normal) * transform.rotation, rotateToNormalSpeed * Time.deltaTime);
                    localCameraEuler = playerCamera.localEulerAngles;
                    localCameraEuler = new Vector3(localCameraEuler.x, 0, 0);
                    playerCamera.localEulerAngles = localCameraEuler;
                    float angle = Mathf.Abs(Vector3.Angle(Vector3.up, hitData.normal));
                    if (angle >= minimalSlideAngle && angle <= maximalSlideAngle)
                    {
                        GetComponent<Rigidbody>().velocity += transform.forward * slideBoostModifier * Time.deltaTime;
                    }
                }
                yield return null;
            }
            //USE THE PART OF SLIDE FOR THIS
            while (transform.rotation != transform.rotation * Quaternion.FromToRotation(transform.up, ogUpwards))
            {
                playerCamera.rotation = Quaternion.RotateTowards(playerCamera.transform.rotation, Quaternion.FromToRotation(ogUpwards, transform.up) * playerCamera.rotation, rotateToNormalSpeed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(transform.up, ogUpwards) * transform.rotation, rotateToNormalSpeed * Time.deltaTime);
                localCameraEuler = playerCamera.localEulerAngles;
                localCameraEuler = new Vector3(localCameraEuler.x, 0, 0);
                playerCamera.localEulerAngles = localCameraEuler;
                yield return null;
            }
            Quaternion oldRot = playerCamera.rotation;
            localCameraEuler = playerCamera.localEulerAngles;
            localCameraEuler = new Vector3(localCameraEuler.x, 0, 0);
            playerCamera.localEulerAngles = localCameraEuler;
            localCameraEuler = playerCamera.localEulerAngles;
            currentState = States.Normal;
            currentStatus = Status.Normal;
            playerCollider.size = standColliderSize;
            playerCollider.center = standColliderPosition;
            CheckMovement();
        }
        print("OVER");
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
    public enum States { Normal, Disabled, ActionImpaired, MovementImpaired, Frozen }
    public enum Status { Normal, Falling}
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(feetLocation.position + jumpCheckPosModifier, jumpChargeCheckSize);
        Gizmos.DrawLine(feetLocation.position, feetLocation.position + feetLocation.forward);
    }
}
