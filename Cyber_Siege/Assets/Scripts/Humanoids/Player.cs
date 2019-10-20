using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform directionChecker;
    [SerializeField] Transform relocate;
    [SerializeField] Animator playerAnimator;
    [SerializeField] LayerMask actionMask, terrainMask, interactableMask;
    [SerializeField] string interactableTag;
    public float interactRange;
    [SerializeField] States currentState;
    [SerializeField] Status currentStatus;
    public ActionState currentActionState;
    [SerializeField] Transform feetLocation;

    [Header("Camera")]
    public float rotationModifier;
    [SerializeField] float minCamClamp, maxCamClamp;
    public Transform playerCamera;
    [SerializeField] Transform standingCamPos, crouchingCamPos, slideCamPos;
    [SerializeField] float cameraTransitionModifier;
    public Coroutine currentCameralocationRoutine;

    [Header("Movement")]
    public Vector3 lastMovedAmtNormalized;
    public float ySlopeBoosterAngle, boosterModifier;
    [SerializeField] float movementSpeedModifier;
    [SerializeField] float sidewaysWalkDebuff, backwardsWalkDebuff, crouchWalkDebuff, sprintBuff;
    [SerializeField] public bool crouching, running;
    [SerializeField] float fieldOfViewNormal, fieldOfViewRunning, fovChangeSpeed;

    [Header("Jumping")]
    [SerializeField] Rigidbody playerRigid;
    [SerializeField] Vector3 jumpForce;
    [SerializeField] LayerMask jumpableLayers;
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

    [Header("Inventory")]
    public PlayerInventory inventory;

    [Header("Grenades")]
    [SerializeField] float throwVelocityMultiplier;
    [SerializeField] GameObject grenadee; //Will be replaced with the inventory, same for currentgun.

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void LateUpdate()
    {
        if (currentState != States.Frozen || currentState != States.Disabled)
        {
            RotateCamera();
        }
        if(currentStatus == Status.Normal)
        {
            playerRigid.velocity = new Vector3(0, playerRigid.velocity.y, 0);
        }
    }
    public void FixedUpdate()
    {

    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(ThrowGrenade());
        }
        RaycastHit forwardHit;
        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out forwardHit, interactRange, interactableMask, QueryTriggerInteraction.Ignore))
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
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
                        Collider[] vaultables = Physics.OverlapSphere(transform.position, vaultDetectionRange, actionMask);
                        if (vaultables.Length > 0)
                        {
                            foreach(Collider vaultable in vaultables)
                            {
                                if(vaultable.tag == vaultableTag)
                                {
                                    vaultables[0].GetComponent<Interactable>().Interact(this);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if(Physics.CheckBox(feetLocation.position + jumpCheckPosModifier, jumpChargeCheckSize, transform.rotation, jumpableLayers, QueryTriggerInteraction.Ignore))
                            {
                                Jump();
                            }
                        }
                    }
                }
            }
            CheckCameraLocation();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(Slide(slidePower));
        }
    }

    public void Interact()
    {
        RaycastHit hitData;
        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hitData, 10000))
        {
            if(hitData.transform.tag == interactableTag)
            {
                hitData.transform.gameObject.GetComponent<ItemPickup>().Interact(this);
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
    public void Jump()
    {
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
        Vector3 lastMovedAmt = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        lastMovedAmtNormalized = lastMovedAmt;
        playerAnimator.SetFloat("SidewaysWalking", lastMovedAmt.x);
        playerAnimator.SetFloat("ForwardWalking", lastMovedAmt.z);
        if (lastMovedAmt != Vector3.zero)
        {
            if (lastMovedAmt.z == 0)
            {
                lastMovedAmt *= (1 - (sidewaysWalkDebuff / 100));
                playerCamera.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(playerCamera.GetComponent<Camera>().fieldOfView, fieldOfViewNormal, fovChangeSpeed);
            }
            else
            {
                if (lastMovedAmt.z < 0)
                {
                    lastMovedAmt *= (1 - (backwardsWalkDebuff / 100));
                    playerCamera.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(playerCamera.GetComponent<Camera>().fieldOfView, fieldOfViewNormal, fovChangeSpeed);
                }
                else
                {
                    if (!crouching)
                    {
                        if (running)
                        {
                            playerCamera.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(playerCamera.GetComponent<Camera>().fieldOfView, fieldOfViewRunning, fovChangeSpeed);
                            inSlideGap = true;
                            lastMovedAmt *= 1 + (sprintBuff * Input.GetAxis("Vertical") / 100);
                            if (currentGapTimer != null)
                            {
                                StopCoroutine(currentGapTimer);
                                currentGapTimer = null;
                            }
                        }
                        else
                        {
                            playerCamera.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(playerCamera.GetComponent<Camera>().fieldOfView, fieldOfViewNormal, fovChangeSpeed);
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
                lastMovedAmt *= (1 - (crouchWalkDebuff / 100));
            }
            if (CheckHill())
            {

            }
            /*
            if(CheckHill())
            {
                Vector3 lastValue = lastMovedAmt;
                lastMovedAmt += lastValue.z * -directionChecker.forward;
                lastMovedAmt += lastValue.x * -directionChecker.right;
                lastMovedAmt -= lastValue;
            }*/
            transform.Translate(lastMovedAmt * movementSpeedModifier * Time.deltaTime);
        }
        else
        {
            playerCamera.GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(playerCamera.GetComponent<Camera>().fieldOfView, fieldOfViewNormal, fovChangeSpeed);
        }
    }
    bool CheckHill()
    {
        RaycastHit hitData;
        if(Physics.Raycast(feetLocation.position, -feetLocation.transform.up, out hitData, 10, terrainMask, QueryTriggerInteraction.Ignore))
        {
            if(Vector3.Angle(directionChecker.up, hitData.normal) > ySlopeBoosterAngle)
            {
                print(hitData.transform.name);
                directionChecker.rotation =  Quaternion.LookRotation(directionChecker.forward, hitData.normal);
                return true;
            }
        }
        return false;
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

    void CheckCameraLocation()
    {
        bool gotNewState = false;
        if(currentActionState != ActionState.Sliding)
        {
            if (crouching)
            {
                if(currentActionState != ActionState.Crouching)
                {
                    currentActionState = ActionState.Crouching;
                    gotNewState = true;
                }
            }
            else
            {
                if(currentActionState != ActionState.Walking)
                {
                    currentActionState = ActionState.Walking;
                    gotNewState = true;
                }
            }
        }
        if (gotNewState)
        {
            if(currentCameralocationRoutine != null)
            {
                StopCoroutine(currentCameralocationRoutine);
                currentCameralocationRoutine = null;
            }
            switch (currentActionState)
            {
                case ActionState.Walking:
                    currentCameralocationRoutine = StartCoroutine(MoveCameraToPoint(playerCamera, standingCamPos, cameraTransitionModifier * Time.deltaTime));
                    break;

                case ActionState.Crouching:
                    currentCameralocationRoutine = StartCoroutine(MoveCameraToPoint(playerCamera, crouchingCamPos, cameraTransitionModifier * Time.deltaTime));
                    break;
            }
        }
    }

    public IEnumerator ThrowGrenade()
    {
        //Swap out gun
        inventory.grenadeSlots[0].remainingAmount--;
        inventory.remainingGrenadeText.text = inventory.grenadeSlots[0].remainingAmount.ToString();
        GameObject grenade = Instantiate(grenadee, gunWieldingPoint.position, Quaternion.identity, gunWieldingPoint);
        grenade.GetComponent<Grenade>().owner = this;
        //Play animation of throwing grenade.
        yield return null;
        grenade.transform.parent = null;
        grenade.GetComponent<Rigidbody>().velocity = playerCamera.forward * throwVelocityMultiplier;
        StartCoroutine(grenade.GetComponent<Grenade>().StartExplosionTimer());

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
            if(currentCameralocationRoutine != null)
            {
                StopCoroutine(currentCameralocationRoutine);
            }
            currentCameralocationRoutine = StartCoroutine(MoveCameraToPoint(playerCamera, slideCamPos, cameraTransitionModifier * Time.deltaTime));
            currentActionState = ActionState.Sliding;
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
            currentActionState = ActionState.Return;
        }
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
    public enum Status { Normal, Falling }
    
    public enum ActionState { Return, Walking, Crouching, Aiming, Sliding}
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(feetLocation.position + jumpCheckPosModifier, jumpChargeCheckSize);
        Gizmos.DrawLine(feetLocation.position, feetLocation.position + feetLocation.forward);
    }
    public IEnumerator MoveCameraToPoint(Transform objectToMove, Transform locationToMoveTo, float speed)
    {
        yield return null;
        while(objectToMove.position != locationToMoveTo.position)
        {
            objectToMove.position = Vector3.MoveTowards(objectToMove.position, locationToMoveTo.position, speed * Time.deltaTime);
            yield return null;
        }
        currentCameralocationRoutine = null;
    }
}
