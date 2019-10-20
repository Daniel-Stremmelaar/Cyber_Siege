using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [TextArea]
    public string hoverText;

    public abstract void Interact(Player owner);
}
