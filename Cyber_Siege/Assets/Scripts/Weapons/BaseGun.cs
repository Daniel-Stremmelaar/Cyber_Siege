using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : MonoBehaviour
{
    public Player owner;
    public string humanoidTag;

    public GunData baseData;
    public int currentClip;
    public int currentAmmoStore;
    public ParticleSystem muzzleFlash;
    public AudioSource bulletShot;

    public Coroutine currentActionRoutine;

    bool canFire = true;
    Coroutine knockupRoutine, knockdownRoutine;
    Vector3 remainingRotationAmount;
    Vector3 totalRotationAmount;

    public FireTypes fireType;
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
                }
            }
            //Spawn Particle
            //Knock player camera up
            //Let fire animation play
            //Let fire audio play
            if (!baseData.infiniteAmmo)
            {
                currentClip--;
            }
            remainingRotationAmount.x += baseData.shotKnockupX;
            totalRotationAmount.x += baseData.shotKnockupX;
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
        while(remainingRotationAmount.x >= 0)
        {
            Vector3 rotateAmount = new Vector3(baseData.knockupSpeed * Time.deltaTime, 0, 0);
            print("A");
            owner.playerCamera.Rotate(-rotateAmount);
            remainingRotationAmount -= rotateAmount;
            yield return null;
        }
        knockupRoutine = null;
        knockdownRoutine = StartCoroutine(KnockDown());
    }

    public IEnumerator KnockDown()
    {
        totalRotationAmount.x = baseData.shotKnockdownX;
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
