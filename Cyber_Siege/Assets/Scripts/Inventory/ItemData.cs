using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;

    public GameObject itemPrefab, itemDrop;
    public Sprite itemIcon;
    public string itemName;
    public int maxAmount;
}
[System.Serializable]
public enum ItemType { Weapon, Grenade}
