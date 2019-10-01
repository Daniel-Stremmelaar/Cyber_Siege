using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public int maxBulletHoleAmount;
    List<GameObject> bulletHoles = new List<GameObject>();

    public void AddBulletHole(GameObject bulletHole)
    {
        if(maxBulletHoleAmount > 0)
        {
            if (bulletHoles.Count == maxBulletHoleAmount)
            {
                Destroy(bulletHoles[0]);
                bulletHoles.RemoveAt(0);
            }
            bulletHoles.Add(bulletHole);
        }
        else
        {
            Destroy(bulletHole);
        }
    }
}
