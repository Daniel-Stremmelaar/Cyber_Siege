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


    private void Awake()
    {
        recoilAmount = baseData.recoilPattern.initialRecoil;
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
            if (Input.GetButtonDown("Fire1"))
            {
                if (currentClip == 0 && currentAmmoStore > 0)
                {
                    currentActionRoutine = StartCoroutine(Reload());
                    return;
                }
            }
            CheckFireMode();
        }
        UpdateCrosshair();

    }

    void UpdateCrosshair()
    {
        GameObject crosshair = GameObject.FindGameObjectWithTag("UIManager").GetComponent<IngameUIManager>().crosshair;
        float crosshairOffset;
        if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            crosshairOffset = baseData.walkAccuracy;
        }
        else
        {
            crosshairOffset = baseData.zoomedAccuracy;
        }
        foreach(Transform crosshairStripe in crosshair.transform)
        {
            Vector3 ogPos = crosshairStripe.localPosition;
            Vector3 newDistance = crosshairStripe.localPosition.normalized * crosshairOffset;
            crosshairStripe.localPosition = Vector3.MoveTowards(ogPos, newDistance, baseData.crosshairModifySpeed * Time.deltaTime);
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
            RaycastHit hitData;
            Vector3 accuracyModifier;
            accuracyModifier = new Vector3(Random.Range(baseData.minBulletOffset.x, baseData.maxBulletOffset.x), Random.Range(baseData.minBulletOffset.y, baseData.maxBulletOffset.y));
            float accuracyPercentage = 100 - baseData.baseAccuracy;
            if (accuracyPercentage > 0)
            {
                accuracyPercentage /= 100;
            }
            accuracyModifier *= accuracyPercentage;
            muzzleFlash.Play();
            bulletShot.Play();
            if (Physics.Raycast(owner.playerCamera.position, owner.playerCamera.forward + accuracyModifier, out hitData, baseData.bulletRange))
            {
                if(hitData.transform.tag == humanoidTag)
                {
                    print("Hit Target");
                    IngameUIManager uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<IngameUIManager>();
                    if (uiManager.hitmarkerRoutine != null)
                    {
                        StopCoroutine(uiManager.hitmarkerRoutine);
                        uiManager.hitmarkerRoutine = null;
                    }
                    uiManager.hitmarkerRoutine = StartCoroutine(uiManager.Hitmarker());
                }
            }
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
    public virtual IEnumerator FireCooldown(float cooldown)
    {
        yield return new WaitForSeconds(baseData.shotDelay);
        canFire = true;
        currentActionRoutine = null;
    }
    public virtual IEnumerator Reload()
    {
        print("RELOAD");
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
        yield return null;
        currentActionRoutine = null;
    }
    public virtual IEnumerator AimDownsights()
    {
        yield return null;
    }

    public IEnumerator KnockUp()
    {
        float multiplyAmount = 1 / baseData.knockupSpeed;
        while(remainingRotationAmount != Vector3.zero)
        {

            Vector3 rotateAmount = new Vector3(recoilAmount.x * (Time.deltaTime * multiplyAmount), recoilAmount.y * (Time.deltaTime * multiplyAmount), 0);
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
            print(-rotateAmount.x);
            owner.transform.Rotate(new Vector3(0, rotateAmount.y, 0));
            remainingRotationAmount -= rotateAmount;
            yield return null;
        }
        print(remainingRotationAmount);
        knockupRoutine = null;
        knockdownRoutine = StartCoroutine(KnockDown());
    }

    public IEnumerator KnockDown()
    {
        yield return null;
        totalRotationAmount.x = baseData.recoilPattern.initialRecoil.x;
        while(totalRotationAmount.x >= 0)
        {
            print("O");
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
        recoilResetTimer = null;
    }

    [System.Serializable]
    public enum FireTypes {SemiAutomatic, Automatic, Burst}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector2 accuracyModifier;
        accuracyModifier = new Vector2(Random.Range(baseData.minBulletOffset.x, baseData.maxBulletOffset.x), Random.Range(baseData.minBulletOffset.y, baseData.maxBulletOffset.y));
        float accuracyPercentage = 100 - baseData.baseAccuracy;
        if(accuracyPercentage > 0)
        {
            accuracyPercentage /= 100;
        }
        accuracyModifier *= accuracyPercentage;
        Vector3 cameraLocation = owner.playerCamera.position + owner.playerCamera.forward * baseData.bulletRange;
        cameraLocation.x += owner.playerCamera.forward.x * accuracyModifier.x;
        cameraLocation.y += owner.playerCamera.forward.y * accuracyModifier.y;
        Debug.DrawLine(owner.playerCamera.position, cameraLocation);
    }
}
