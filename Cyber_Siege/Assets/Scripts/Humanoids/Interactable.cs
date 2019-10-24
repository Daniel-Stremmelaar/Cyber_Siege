using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [TextArea]
    public string hoverText;
    public string requiredInput;

    public void CheckInteract(string input, Player owner)
    {
        if(input == requiredInput)
        {
            Interact(owner);
        }
    }

    public abstract void Interact(Player owner);
}
