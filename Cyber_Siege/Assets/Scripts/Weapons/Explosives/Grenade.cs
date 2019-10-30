using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Explosive
{
    [SerializeField] float explosionDelay;
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionDamage;
    [SerializeField] float explosionForce;
    [SerializeField] LayerMask explosionLayers;
    public AudioClip pinSound, throwSound, explosionSound;
    public AudioSource audioSource;

    [SerializeField] string humanoidTag, velocityObjectTag;

    public IEnumerator StartExplosionTimer()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    public override void Explode()
    {
        audioSource.clip = explosionSound;
        audioSource.Play();
        Collider[] damagableObjects = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayers);
        if(damagableObjects.Length > 0)
        {
            foreach(Collider damagableObject in damagableObjects)
            {
                GameObject thisObject = damagableObject.gameObject;
                float distancePercentageFromCenter = Vector3.Distance(transform.position, thisObject.transform.position) / explosionRadius;
                if(thisObject.tag == humanoidTag)
                {
                    if (thisObject.GetComponent<Target>())
                    {
                        Target humanoidData = thisObject.GetComponent<Target>();
                        humanoidData.Hit(explosionDamage * distancePercentageFromCenter, 0);
                    }

                }
                else
                {
                    if(thisObject.tag == velocityObjectTag)
                    {
                        thisObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);
                        print("YES");
                    }
                }
            }
            IngameUIManager playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<IngameUIManager>();
            playerUI.StartCoroutine(playerUI.Hitmarker());
        }
        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
