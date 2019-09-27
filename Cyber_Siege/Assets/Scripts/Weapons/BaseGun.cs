using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : MonoBehaviour
{
    public GunData baseData;
    public int currentClip;
    public int currentAmmoStore;
    public Transform bulletSpawnPoint;

    public Coroutine currentActionRoutine;

    Coroutine knockupRoutine;
    Vector3 remainingRotationAmount;
    Vector3 totalRotationAmount;
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
        if (Input.GetButtonDown("Fire1") && currentActionRoutine == null)
        {
            if (currentClip > 0 || baseData.infiniteAmmo)
            {
                currentActionRoutine = StartCoroutine(Fire());
            }
            else
            {
                if (currentAmmoStore > 0)
                {
                    currentActionRoutine = StartCoroutine(Reload());
                }
            }
        }
    }

    public virtual IEnumerator Fire()
    {
        while (Input.GetButton("Fire1"))
        {
            if (currentClip > 0 || baseData.infiniteAmmo)
            {
                GameObject bullet = Instantiate(baseData.bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                Destroy(bullet, baseData.destroyTime);
                //Spawn Particle
                //Knock player camera up
                //Let fire animation play
                //Let fire audio play
                if (!baseData.infiniteAmmo)
                {
                    currentClip--;
                }
            }
            yield return new WaitForSeconds(baseData.shotDelay);
        }
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

            transform.Rotate(rotateAmount);
            remainingRotationAmount -= rotateAmount;
            yield return null;
        }
        knockupRoutine = null;
    }

    public IEnumerator KnockDown()
    {
        while(totalRotationAmount.x >= 0)
        {
            Vector3 rotateAmount = new Vector3(baseData.knockupSpeed * Time.deltaTime, 0, 0);
            yield return null;
        }
    }
}
