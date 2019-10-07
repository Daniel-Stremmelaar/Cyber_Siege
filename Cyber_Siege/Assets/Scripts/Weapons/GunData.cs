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
    public float shotKnockdownX;
    public float knockupSpeed, knockdownSpeed;
    public GameObject bulletImpactDecal;
    public float movementSpeedModifier;
    public float baseAccuracy, walkAccuracy, sprintAccuracy, zoomedAccuracy;
    public float destroyTime;
    public float shotDelay;
    public float bulletRange;
    public Vector2 maxBulletOffset, minBulletOffset;
    public float crosshairModifySpeed;

    [Header("Recoil")]
    public RecoilPattern recoilPattern;

    public AudioClip shotSound;

    public bool infiniteAmmo;

    [System.Serializable]
    public struct PatternData
    {
        public int bulletIndexChange;
        public Vector3 recoilAmount;
    }
    [System.Serializable]
    public struct RecoilPattern
    {
        public float resetTimer;
        public Vector3 initialRecoil;
        public PatternData[] patternData;
    }
}
