using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public ItemData pickupData;
    public int amount;

    public override void Interact(Player owner)
    {
        owner.inventory.SwapItem(gameObject, pickupData, amount);
    }
}
