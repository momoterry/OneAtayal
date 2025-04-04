using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageByAnimation : bullet_base
{
    //public float baseDamage = 10.0f; //方便初期測試用
    //TODO: 支援圓形和 Box
    public Vector2 BoxSize;
    public GameObject hitFX;

    //private FACTION_GROUP group = FACTION_GROUP.PLAYER;
    //protected Damage myDamage;

    ////Public Functions
    //public void SetGroup(FACTION_GROUP g)
    //{
    //    group = g;
    //}

    //public void SetDamage(Damage d)
    //{
    //    myDamage = d;
    //    baseDamage = d.damage;
    //}

    // Start is called before the first frame update
    //void Start()
    //{
    //    myDamage.damage = baseDamage;
    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    void OnDoDamage()
    {
        myDamage.damage = baseDamage;
#if XZ_PLAN
        Collider[] cols = Physics.OverlapBox(transform.position, new Vector3(BoxSize.x*0.5f, 1.0f, BoxSize.y*0.5f));
        foreach (Collider col in cols)
#else
        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, BoxSize, 0.0f);
        foreach (Collider2D col in cols)
#endif

        {
            bool hit = false;
            if ((col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Hittable")) && group == FACTION_GROUP.PLAYER)
            {
                //print("Trigger:  Hit Enemy !! ");
                col.gameObject.SendMessage("OnDamage", myDamage);
                hit = true;
            }
            else if ((col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Doll")) && group == FACTION_GROUP.ENEMY)
            {
                //print("Trigger:  Hit Player !!");
                col.gameObject.SendMessage("OnDamage", myDamage);
                hit = true;
            }

            //打中牆的情況，可以跟擊中對手並存
            if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                //print("Trigger:  HitWall !!");
                if (bulletResultCB != null)
                {
                    bulletResultCB(new BulletResult(BulletResult.RESULT_TYPE.HIT_WALL));
                }
            }

            if (hit&& hitFX)
            {
                Vector3 hitPos = col.ClosestPoint(transform.position);

                BattleSystem.GetInstance().SpawnGameplayObject(hitFX, hitPos, false);
            }
        }
    }

    void OnFinish()
    {
        Destroy(gameObject);
    }

}
