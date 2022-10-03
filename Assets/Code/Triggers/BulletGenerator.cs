using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    public GameObject bulletRef;
    public Transform dirRef;
    public float initDis = 1.0f;
    public float damage = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTG(GameObject whoTG)
    {
        Vector3 dir = Vector3.back;
        if (dirRef)
        {
            dir = (dirRef.position - transform.position).normalized;
        }

        Vector3 pos = transform.position + dir * initDis;
        if (bulletRef)
        {
            GameObject bObj = BattleSystem.SpawnGameObj(bulletRef, pos);
            bullet_base bullet = bObj.GetComponent<bullet_base>();
            if (bullet)
            {
                bullet.InitValue(DAMAGE_GROUP.ENEMY, damage, dir);
            }
        }
    }
}
