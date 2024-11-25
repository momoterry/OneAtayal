using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Damage 定義的部份
public struct Damage
{
    public enum OwnerType
    {
        NONE,
        DOLL,
        PLAYER,
        ENEMY,
    }
    public float damage;
    public OwnerType type;
    public string ID;           //處理傷害統計的單位
    public GameObject Owner;

    public void Init(float _d, OwnerType _type, string _ID, GameObject _Owner)
    {
        damage = _d;
        type = _type;
        ID = _ID;
        Owner = _Owner;
    }
}


public struct BulletResult
{
    public enum RESULT_TYPE
    {
        HIT_WALL,
        HIT_TARGET,
        EXHAUSTED,
    }
    public RESULT_TYPE result;
    public BulletResult(RESULT_TYPE _r) { result = _r; }
}

public class bullet_base : MonoBehaviour
{
    protected float baseDamage = 60.0f;
    protected Vector3 targetDir = Vector3.up;
    protected GameObject targetObj = null;
    protected FACTION_GROUP group = FACTION_GROUP.PLAYER;
    protected Damage myDamage;

    public delegate void BulletResultCB(BulletResult result);
    protected BulletResultCB bulletResultCB = null;

    public virtual void InitValue(FACTION_GROUP g, Damage theDamage, Vector3 targetVec, GameObject targetObject = null)
    {
        group = g;
        targetDir = targetVec.normalized;
        myDamage = theDamage;
        baseDamage = theDamage.damage;
        targetObj = targetObject;
    }

    public void SetResultCB(BulletResultCB resultCB)
    {
        bulletResultCB = resultCB;
    }
}

public class bullet : bullet_base
{
    public float speed = 20.0f;
    public float lifeTime = 0.5f;
    public GameObject hitFX;

    protected float myTime = 1.0f;

    //protected Damage myDamage;


    // Public Functions
    //public void SetGroup(FACTION_GROUP g)
    //{
    //    group = g;
    //}

    // Start is called before the first frame update
    void Start()
    {
        myTime = lifeTime;
        myDamage.damage = baseDamage;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += targetDir * speed * Time.deltaTime;

        if (myTime < 0.0f)
        {
            //print("Dead!! " + myTime +" / " + lifeTime + "  ... " +Time.deltaTime);
            if (bulletResultCB!=null)
            {
                bulletResultCB(new BulletResult(BulletResult.RESULT_TYPE.EXHAUSTED));
            }
            Destroy(gameObject);
        }
        else
        {
            myTime -= Time.deltaTime;
        }
        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y); //用 Y 值設定Z

    }


    private void OnTriggerEnter(Collider col)
    {
        //print("bullet::OnTriggerEnter : " + col);
        bool hit = false;
        bool destroy = false;
        if ((col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Hittable")) && group == FACTION_GROUP.PLAYER)
        {
            //print("Trigger:  Hit Enemy !!");
            hit = true;
            col.gameObject.SendMessage("OnDamage", myDamage);
            if (bulletResultCB != null)
            {
                bulletResultCB(new BulletResult(BulletResult.RESULT_TYPE.HIT_TARGET));
            }
        }
        else if ((col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Doll")) && group == FACTION_GROUP.ENEMY)
        {
            //print("Trigger:  Hit Player or Doll !!");
            hit = true;
            col.gameObject.SendMessage("OnDamage", myDamage);
            if (bulletResultCB != null)
            {
                bulletResultCB(new BulletResult(BulletResult.RESULT_TYPE.HIT_TARGET));
            }
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //print("Trigger:  HitWall !!");
            hit = true;
            if (bulletResultCB != null)
            {
                bulletResultCB(new BulletResult(BulletResult.RESULT_TYPE.HIT_WALL));
            }
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("DeadZone"))
        {
            destroy = true;
            if (bulletResultCB != null)
            {
                bulletResultCB(new BulletResult(BulletResult.RESULT_TYPE.HIT_WALL));
            }
        }


        if (hit)
        {
            Vector3 hitPos = col.ClosestPoint(transform.position);

            BattleSystem.GetInstance().SpawnGameplayObject(hitFX, hitPos, false);

            destroy = true;
        }

        if (destroy)
        {
            Destroy(gameObject);
        }
    }

}
