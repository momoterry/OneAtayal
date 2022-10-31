using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreHealInfo : MonoBehaviour
{
    protected float preHealValue = 0;
   
    public float GetPreHeal() { return preHealValue; }
    public void AddPreHeal( float addValue)
    {
        preHealValue += addValue;
        if (preHealValue < 0.5f) //太小就刪去
        {
            Destroy(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(preHealValue / 100.0f, 0.5f, 0.5f));
    }

}
