using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructObj : MonoBehaviour
{
    public GameObject debrisRef;
    public int DropID = -1;
    public GameObject[] destructTrigger;
    protected bool isDead = false;
    public void OnDeath()
    {
        if (isDead) 
            return;
        isDead = true;
        //print("OnDeath!!");
        if (debrisRef)
        {
            BattleSystem.SpawnGameObj(debrisRef, transform.position);
        }
        if (DropID > 0 && DropManager.GetInstance())
        {
            DropManager.GetInstance().DoDropByID(DropID, transform.position);
        }

        if (destructTrigger != null && destructTrigger.Length > 0) 
        {
            foreach (GameObject obj in destructTrigger) 
            {
                if (obj != null)
                {
                    obj.SendMessage("OnTG", gameObject);
                }
            }
        }

        Destroy(gameObject);
    }
}
