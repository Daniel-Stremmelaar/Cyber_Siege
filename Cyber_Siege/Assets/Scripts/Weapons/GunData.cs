using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GunData", menuName = "Guns/GunData")]
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
    public float destroyTime;
    public float shotDelay;
    public float bulletRange;
    public float idleSpread, movingSpread, runningSpread, zoomedSpread;
    public Spread spreadData;
    public float crosshairModifySpeed;
    public int bulletsPerShot = 1;

    [Header("Recoil")]
    public RecoilPattern recoilPattern;
    [Range(1, 100)]
    public float knockdownRecoilPercentage;
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
    [System.Serializable]
    public struct Spread
    {
        public float baseSpread, maxSpread;
        public float baseSpreadModifier, spreadSquareAmount;
    }
}
