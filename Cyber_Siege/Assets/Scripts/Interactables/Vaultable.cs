using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaultable : Interactable
{
    public List<Transform> vaultPositions;
    public override void Interact(Player owner)
    {
        List<Vector3> vaultPositionCopy = new List<Vector3>();
        foreach(Transform pos in vaultPositions)
        {
            vaultPositionCopy.Add(pos.position);
        }
        if(Vector3.Distance(owner.transform.position, vaultPositionCopy[0]) > Vector3.Distance(owner.transform.position, vaultPositionCopy[vaultPositionCopy.Count - 1]))
        {
            vaultPositionCopy.Reverse();
        }
        StartCoroutine(owner.GetComponent<Player>().Vault(vaultPositionCopy.ToArray()));
    }
}
