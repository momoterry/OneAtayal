using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructObj : MonoBehaviour
{
    public GameObject debrisRef;
    public void OnDeath()
    {
        if (debrisRef)
        {
            BattleSystem.SpawnGameObj(debrisRef, transform.position);
        }
        Destroy(gameObject);
    }
}
