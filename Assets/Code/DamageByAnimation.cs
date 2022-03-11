using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageByAnimation : MonoBehaviour
{
    public float phyDamage = 10.0f; //方便初期測試用
    //TODO: 支援圓形和 Box
    public Vector2 BoxSize;
    public GameObject hitFX;

    private DAMAGE_GROUP group = DAMAGE_GROUP.PLAYER;
    private Damage myDamage;

    //Public Functions
    public void SetGroup(DAMAGE_GROUP g)
    {
        group = g;
    }

    public void SetDamage(Damage d)
    {
        myDamage = d;
        phyDamage = d.damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        myDamage.damage = phyDamage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDamage()
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, BoxSize, 0.0f);

        foreach (Collider2D col in cols)
        {
            bool hit = false;
            if (col.gameObject.CompareTag("Enemy") && group == DAMAGE_GROUP.PLAYER)
            {
                //print("Trigger:  Hit Enemy !! ");
                col.gameObject.SendMessage("DoDamage", myDamage);
                hit = true;
            }
            else if (col.gameObject.CompareTag("Player") && group == DAMAGE_GROUP.ENEMY)
            {
                //print("Trigger:  Hit Player !!");
                col.gameObject.SendMessage("DoDamage", myDamage);
                hit = true;
            }
            if (hit)
            {
                Vector3 hitPos = col.ClosestPoint(transform.position);
                hitPos.z = col.transform.position.z - 0.125f;   //角色的話用對方的 Z 來調整

                Instantiate(hitFX, hitPos, Quaternion.identity, null);
            }
        }
    }
}
