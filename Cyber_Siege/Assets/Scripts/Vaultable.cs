using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaultable : Interactable
{
    [SerializeField] Transform[] vaultPositions;
    public override void Interact(GameObject player)
    {
        List<Vector3> vaultPositionCopy = new List<Vector3>();
        foreach(Transform pos in vaultPositions)
        {
            vaultPositionCopy.Add(pos.position);
        }
        if(Vector3.Distance(player.transform.position, vaultPositionCopy[0]) > Vector3.Distance(player.transform.position, vaultPositionCopy[vaultPositionCopy.Count - 1]))
        {
            vaultPositionCopy.Reverse();
        }
        StartCoroutine(player.GetComponent<Player>().Vault(vaultPositionCopy.ToArray()));
    }
}
