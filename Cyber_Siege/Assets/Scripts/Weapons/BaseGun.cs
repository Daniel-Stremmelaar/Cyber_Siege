using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseGun : MonoBehaviour
{
    [Header("TESTING PURPOSES")]
    public AnimationClip reloadAnim;
    public AnimationClip shootAnim;
    public Animation animation;


    [Header("Actually Stuff")]
    public Player owner;
    public string humanoidTag;

    public GameObject crosshair;

    public GunData baseData;

    public int repeatedBulletAmount = 0;
    public Vector3 recoilAmount = Vector3.zero;

    public int currentClip;
    public int currentAmmoStore;
    public ParticleSystem muzzleFlash;
    public AudioSource bulletShot;

    public Coroutine currentActionRoutine;

    bool canFire = true;
    Coroutine knockupRoutine, knockdownRoutine, recoilResetTimer;
    Vector3 remainingRotationAmount;
    Vector3 totalRotationAmount;

    public FireTypes fireType;
    public GunState currentState;

    float crosshairOffset = 0;
    const float spreadPrecisionizer = 0.001f;
    public float spreadModifier;

    GameObject ingameManager;
    private void Awake()
    {
        ingameManager = GameObject.FindGameObjectWithTag("Manager");
        recoilAmount = baseData.recoilPattern.initialRecoil;
        spreadModifier = baseData.spreadData.baseSpreadModifier;
        OnEquip();
    }
    // Start is called before the first frame update
    public void Update()
    {
        if (Input.GetButtonDown("Reload"))
        {
            if (currentClip < baseData.clipCapacity && currentAmmoStore > 0)
            {
                currentActionRoutine = StartCoroutine(Reload());
            }
        }
        if (Input.GetButton("Fire1") && currentActionRoutine == null)
        {
            if(currentClip == 0)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (currentAmmoStore > 0)
                    {
                        currentActionRoutine = StartCoroutine(Reload());
                        return;
                    }
                }
            }
            else
            {
                CheckFireMode();
            }
        }
        UpdateCrosshair();

    }
    public void OnEquip()
    {
        ingameManager.GetComponent<IngameUIManager>().clipAmmo.text = currentClip.ToString();
        ingameManager.GetComponent<IngameUIManager>().storedAmmo.text = currentAmmoStore.ToString();
    }
    GunState CheckGunState()
    {
        GunState newState;
        if(currentState != GunState.Zooming)
        {
            if(!Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
            {
                newState = GunState.Idle;
            }
            else
            {
                if (owner.running)
                {
                    newState = GunState.Running;
                }
                else
                {
                    newState = GunState.Moving;
                }
            }
        }
        else
        {
            newState = GunState.Zooming;
        }
        return newState;
    }
    void UpdateCrosshair()
    {
        currentState = CheckGunState();
        GameObject crosshair = GameObject.FindGameObjectWithTag("Manager").GetComponent<IngameUIManager>().crosshair;
        switch (currentState)
        {
            case GunState.Idle:
                crosshairOffset = baseData.idleSpread;
                break;
            case GunState.Moving:
                crosshairOffset = baseData.movingSpread;
                break;
            case GunState.Running:
                crosshairOffset = baseData.runningSpread;
                break;
            case GunState.Zooming:
                crosshairOffset = baseData.zoomedSpread;
                break;
        }
        crosshairOffset += spreadModifier;
        foreach(Transform crosshairStripe in crosshair.transform)
        {
            Vector3 ogPos = crosshairStripe.transform.localPosition;
            Vector3 newDistance = crosshairStripe.GetChild(0).localPosition.normalized * crosshairOffset;
            crosshairStripe.localPosition = Vector3.LerpUnclamped(ogPos, newDistance, baseData.crosshairModifySpeed * Time.deltaTime);
        }
    }

    public void CheckFireMode()
    {
        if(canFire && knockupRoutine == null)
        {
            switch (fireType)
            {
                case FireTypes.Automatic:
                    FireBullet();
                    break;
                case FireTypes.SemiAutomatic:
                    if (Input.GetButtonDown("Fire1"))
                    {
                        FireBullet();
                    }
                    break;
            }
        }
    }
    void CheckRecoilPattern()
    {
        if(baseData.recoilPattern.patternData.Length > 0 && repeatedBulletAmount != 0)
        {
            foreach (GunData.PatternData patternData in baseData.recoilPattern.patternData)
            {
                if (patternData.bulletIndexChange == repeatedBulletAmount)
                {
                    recoilAmount = patternData.recoilAmount;
                    return;
                }
            }
        }
        else
        {
            recoilAmount = baseData.recoilPattern.initialRecoil;
        }
    }
    public void FireBullet()
    {
        if (currentClip > 0 || baseData.infiniteAmmo)
        {
            canFire = false;
            muzzleFlash.Play();
            bulletShot.Play();

            for(int bulletAmount = 0; bulletAmount < baseData.bulletsPerShot; bulletAmount++)
            {
                RaycastHit hitData;
                Vector3 accuracyModifier;
                accuracyModifier = new Vector3(Random.Range(-crosshairOffset, crosshairOffset) , Random.Range(-crosshairOffset, crosshairOffset));

                Vector3 accuracyChanger = Vector3.zero;
                accuracyChanger += accuracyModifier.x * owner.playerCamera.right;
                accuracyChanger += accuracyModifier.y * owner.playerCamera.up;
                accuracyChanger *= spreadPrecisionizer;
                print(accuracyModifier);
                if (Physics.Raycast(owner.playerCamera.position, owner.playerCamera.forward + accuracyChanger, out hitData, baseData.bulletRange))
                {
                    if (hitData.transform.tag == humanoidTag)
                    {
                        IngameUIManager uiManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<IngameUIManager>();
                        if (uiManager.hitmarkerRoutine != null)
                        {
                            StopCoroutine(uiManager.hitmarkerRoutine);
                            uiManager.hitmarkerRoutine = null;
                        }
                        uiManager.hitmarkerRoutine = StartCoroutine(uiManager.Hitmarker());
                        //hitData.transform.GetComponent<TutorialTarget>().Hit();
                    }
                    IngameManager ingameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<IngameManager>();
                    GameObject newBulletHole = Instantiate(baseData.bulletImpactDecal, hitData.point, Quaternion.LookRotation(hitData.normal));
                    newBulletHole.transform.Translate(new Vector3(0, 0, 0.001f));
                    ingameManager.AddBulletHole(newBulletHole);
                }
            }
            AddSpread();
            CheckRecoilPattern();
            repeatedBulletAmount++;
            if(recoilResetTimer != null)
            {
                StopCoroutine(recoilResetTimer);
            }
            recoilResetTimer = StartCoroutine(ResetPatternCounter());
            //Spawn Particle
            //Knock player camera up
            //Let fire animation play
            //Let fire audio play
            if (!baseData.infiniteAmmo)
            {
                currentClip--;
            }
            ingameManager.GetComponent<IngameUIManager>().clipAmmo.text = currentClip.ToString();
            remainingRotationAmount = recoilAmount;
            if (knockupRoutine == null)
            {
                if (knockdownRoutine != null)
                {
                    StopCoroutine(knockdownRoutine);
                    knockdownRoutine = null;
                }
                knockupRoutine = StartCoroutine(KnockUp());
            }
            currentActionRoutine = StartCoroutine(FireCooldown(baseData.shotDelay));
        }
    }
    void AddSpread()
    {
        if(spreadModifier < baseData.spreadData.maxSpread)
        {
            spreadModifier = Mathf.Min(spreadModifier * baseData.spreadData.spreadSquareAmount, baseData.spreadData.maxSpread);
        }
    }
    public virtual IEnumerator FireCooldown(float cooldown)
    {
        yield return new WaitForSeconds(baseData.shotDelay);
        canFire = true;
        currentActionRoutine = null;
    }
    public virtual IEnumerator Reload()
    {
        //Play reload animation
        //Parent ammoClip to hand
        //Play reload2 animation
        //Parent ammoclip to gun

        int requiredAmmo = baseData.clipCapacity - currentClip;
        if (currentAmmoStore >= requiredAmmo)
        {
            currentClip = baseData.clipCapacity;
            currentAmmoStore -= requiredAmmo;
        }
        else
        {
            currentClip += currentAmmoStore;
            currentAmmoStore = 0;
        }
        ingameManager.GetComponent<IngameUIManager>().clipAmmo.text = currentClip.ToString();
        ingameManager.GetComponent<IngameUIManager>().storedAmmo.text = currentAmmoStore.ToString();
        yield return new WaitForSeconds(baseData.reloadSpeedMultiplier);
        currentActionRoutine = null;
    }
    public virtual IEnumerator AimDownsights()
    {
        yield return null;
    }

    public IEnumerator KnockUp()
    {
        while(remainingRotationAmount != Vector3.zero)
        {

            Vector3 rotateAmount = new Vector3(recoilAmount.x * (Time.deltaTime * baseData.recoilSpeed), recoilAmount.y * (Time.deltaTime * baseData.recoilSpeed), 0);
            if(rotateAmount.x > 0)
            {
                if(rotateAmount.x > remainingRotationAmount.x)
                {
                    rotateAmount.x = remainingRotationAmount.x;
                }
            }
            else
            {
                if(rotateAmount.x < 0)
                {
                    if(rotateAmount.x < remainingRotationAmount.x)
                    {
                        rotateAmount.x = remainingRotationAmount.x;
                    }
                }
            }
            if (rotateAmount.y > 0)
            {
                if (rotateAmount.y > remainingRotationAmount.y)
                {
                    rotateAmount.y = remainingRotationAmount.y;
                }
            }
            else
            {
                if (rotateAmount.y < 0)
                {
                    if (rotateAmount.y < remainingRotationAmount.y)
                    {
                        rotateAmount.y = remainingRotationAmount.y;
                    }
                }
            }
            owner.playerCamera.Rotate(new Vector3(-rotateAmount.x, 0, 0));
            owner.transform.Rotate(new Vector3(0, rotateAmount.y, 0));
            remainingRotationAmount -= rotateAmount;
            yield return null;
        }
        knockupRoutine = null;
        knockdownRoutine = StartCoroutine(KnockDown());
    }

    public IEnumerator KnockDown()
    {
        yield return null;
        totalRotationAmount.x = baseData.recoilPattern.initialRecoil.x * (baseData.knockdownRecoilPercentage / 100);
        while(totalRotationAmount.x >= 0)
        {
            Vector3 rotateAmount = new Vector3(baseData.knockdownSpeed * Time.deltaTime, 0, 0);
            owner.playerCamera.Rotate(rotateAmount);
            totalRotationAmount -= rotateAmount;
            yield return null;
        }
        knockdownRoutine = null;
    }

    public IEnumerator ResetPatternCounter()
    {
        yield return new WaitForSeconds(baseData.recoilPattern.resetTimer + baseData.shotDelay);
        repeatedBulletAmount = 0;
        spreadModifier = baseData.spreadData.baseSpread;
        recoilResetTimer = null;
    }

    [System.Serializable]
    public enum FireTypes {SemiAutomatic, Automatic, Burst}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector2 accuracyModifier;
        accuracyModifier = new Vector2(Random.Range(-crosshairOffset, crosshairOffset), Random.Range(-crosshairOffset, crosshairOffset));
        Vector3 cameraLocation = owner.playerCamera.position + owner.playerCamera.forward * baseData.bulletRange;
        cameraLocation.x += owner.playerCamera.forward.x * accuracyModifier.x;
        cameraLocation.y += owner.playerCamera.forward.y * accuracyModifier.y;
        Debug.DrawLine(owner.playerCamera.position, cameraLocation);
    }
    public enum GunState
    {
        Idle, Moving, Running, Zooming
    }
}
