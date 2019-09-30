using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GunData", menuName = "OOF/YES")]
public class GunData : ScriptableObject
{
    public GameObject gunPrefab;
    public string gunName;
    public int clipCapacity;
    public int ammoCapacity;
    public float reloadSpeedMultiplier;
    public float bulletDamage;
    public float shotKnockupX, shotKnockdownX;
    public float knockupSpeed, knockdownSpeed;
    public GameObject bulletPrefab;
    public float movementSpeedModifier;
    public float baseAccuracy;
    public float destroyTime;
    public float shotDelay;
    public float bulletRange;
    public Vector2 maxBulletOffset, minBulletOffset;

    public AudioClip shotSound;

    public bool infiniteAmmo;
}
