using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public int currentlyEquippedWeapon;

    public WeaponSlot[] weaponSlots;
    public GrenadeSlot[] grenadeSlots;

    public Text remainingGrenadeText;

    public Player owner;


    public void Start()
    {
        if(grenadeSlots[0] != null)
        {
            remainingGrenadeText.text = grenadeSlots[0].remainingAmount.ToString();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("One"))
        {
            SwapGun(0);
        }
        else
        {
            if (Input.GetButtonDown("Two"))
            {
                SwapGun(1);
            }
        }
    }
    void SwapGun(int newIndex)
    {
        weaponSlots[currentlyEquippedWeapon].remainingClipAmmo = owner.currentGun.GetComponent<BaseGun>().currentClip;
        weaponSlots[currentlyEquippedWeapon].remainingStoredAmmo = owner.currentGun.GetComponent<BaseGun>().currentAmmoStore;
        Destroy(owner.currentGun.gameObject);
        currentlyEquippedWeapon = newIndex;
        if(currentlyEquippedWeapon >= weaponSlots.Length)
        {
            currentlyEquippedWeapon = 0;
        }
        else
        {
            if(currentlyEquippedWeapon < 0)
            {
                currentlyEquippedWeapon = weaponSlots.Length - 1;
            }
        }
        GameObject newGun = Instantiate(weaponSlots[currentlyEquippedWeapon].itemData.itemPrefab, owner.gunWieldingPoint.position, owner.gunWieldingPoint.rotation, owner.gunWieldingPoint);
        owner.currentGun = newGun.transform;
        newGun.GetComponent<BaseGun>().currentClip = weaponSlots[currentlyEquippedWeapon].remainingClipAmmo;
        newGun.GetComponent<BaseGun>().currentAmmoStore = weaponSlots[currentlyEquippedWeapon].remainingStoredAmmo;
        newGun.GetComponent<BaseGun>().owner = owner;
    }
    public void SwapItem(GameObject itemObject, ItemData itemData, int amount)
    {
        switch (itemData.itemType)
        {
            case ItemType.Weapon:
                SwapWeapon(itemObject, itemData, amount);
                break;

            case ItemType.Grenade:
                SwapGrenade(itemObject, itemData, amount);
                break;
        }
    }

    bool CheckStackableOfItem(GameObject ownerObject, ItemSlot[] slotsToCheck, ItemData dataToPlace, int amount)
    {
        foreach (ItemSlot slot in slotsToCheck)
        {
            if (slot.itemData != null)
            {
                if(slot.itemData == dataToPlace)
                {
                    if(slot.holdingAmount < slot.itemData.maxAmount)
                    {
                        int availableAmount = slot.itemData.maxAmount - slot.holdingAmount;
                        if(availableAmount < amount)
                        {
                            amount -= availableAmount;
                            SwapItem(ownerObject, dataToPlace, amount);
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }
    bool CheckAvailableSpots(ItemSlot[] slotsToCheck, ItemData dataToPlace)
    {
        foreach(ItemSlot slot in slotsToCheck)
        {
            if (slot.itemData == null)
            {
                slot.itemData = dataToPlace;
                return true;
            }
        }
        return false;
    }
    void SwapWeapon(GameObject ownerObject, ItemData data, int amount)
    {
        if(!CheckStackableOfItem(ownerObject, weaponSlots, data, amount))
        {
            if (!CheckAvailableSpots(weaponSlots, data))
            {
                WeaponSlot currentlyEquippedSlot = weaponSlots[currentlyEquippedWeapon];
                GameObject newItemObject = Instantiate(currentlyEquippedSlot.itemData.itemDrop, ownerObject.transform.position, ownerObject.transform.rotation, ownerObject.transform.parent);
                newItemObject.GetComponent<WeaponPickup>().pickupData = currentlyEquippedSlot.itemData;
                newItemObject.GetComponent<WeaponPickup>().clipAmmoRemaining = owner.currentGun.GetComponent<BaseGun>().currentClip;
                newItemObject.GetComponent<WeaponPickup>().storedAmmoRemaining = owner.currentGun.GetComponent<BaseGun>().currentAmmoStore;
                currentlyEquippedSlot.itemData = data;
                currentlyEquippedSlot.remainingClipAmmo = ownerObject.GetComponent<WeaponPickup>().clipAmmoRemaining;
                currentlyEquippedSlot.remainingStoredAmmo = ownerObject.GetComponent<WeaponPickup>().storedAmmoRemaining;
                currentlyEquippedSlot.icon.sprite = data.itemIcon;
                GameObject newWeapon = Instantiate(data.itemPrefab, owner.gunWieldingPoint.position, owner.gunWieldingPoint.rotation, owner.gunWieldingPoint);
                newWeapon.GetComponent<BaseGun>().currentClip = ownerObject.GetComponent<WeaponPickup>().clipAmmoRemaining;
                newWeapon.GetComponent<BaseGun>().currentAmmoStore = currentlyEquippedSlot.remainingStoredAmmo;
                newWeapon.GetComponent<BaseGun>().owner = owner;
                Destroy(owner.currentGun.gameObject);
                owner.currentGun = newWeapon.transform;
                Destroy(ownerObject);
            }
        }
    }
    void SwapGrenade(GameObject ownerObject, ItemData data, int amount)
    {
        if (!CheckAvailableSpots(grenadeSlots, data))
        {

        }
    }
}
