using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Explosive
{
    [SerializeField] float explosionDelay;
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionDamage;
    [SerializeField] LayerMask explosionLayers;

    [SerializeField] string humanoidTag;

    public IEnumerator StartExplosionTimer()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    public override void Explode()
    {
        Collider[] damagableObjects = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayers);
        if(damagableObjects.Length > 0)
        {
            foreach(Collider damagableObject in damagableObjects)
            {
                GameObject thisObject = damagableObject.gameObject;
                float distancePercentageFromCenter = Vector3.Distance(transform.position, thisObject.transform.position) / explosionRadius;
                if(thisObject.tag == humanoidTag)
                {
                    Target humanoidData = thisObject.GetComponent<Target>();
                    humanoidData.Hit(explosionDamage * distancePercentageFromCenter, 0);
                }
            }
        }
        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        print("BOOM");
        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
