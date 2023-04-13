using JetBrains.Annotations;
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
    public string ID;
    public GameObject Owner;

    public void Init(float _d, OwnerType _type, string _ID, GameObject _Owner)
    {
        damage = _d;
        type = _type;
        ID = _ID;
        Owner = _Owner;
    }
}

public enum DAMAGE_GROUP
{
    NONE,
    PLAYER,
    ENEMY,
}

public class bullet_base : MonoBehaviour
{
    protected float baseDamage = 60.0f;
    protected Vector3 targetDir = Vector3.up;
    protected GameObject targetObj = null;
    protected DAMAGE_GROUP group = DAMAGE_GROUP.PLAYER;
    protected Damage myDamage;

    public virtual void InitValue(DAMAGE_GROUP g, Damage theDamage, Vector3 targetVec, GameObject targetObject = null)
    {
        group = g;
        targetDir = targetVec.normalized;
        myDamage = theDamage;
        baseDamage = theDamage.damage;
        targetObj = targetObject;
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
    //public void SetGroup(DAMAGE_GROUP g)
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
        if (col.gameObject.CompareTag("Enemy") && group == DAMAGE_GROUP.PLAYER)
        {
            //print("Trigger:  Hit Enemy !!");
            hit = true;
            col.gameObject.SendMessage("OnDamage", myDamage);
        }
        else if ((col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Doll")) && group == DAMAGE_GROUP.ENEMY)
        {
            //print("Trigger:  Hit Player or Doll !!");
            hit = true;
            col.gameObject.SendMessage("OnDamage", myDamage);
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //print("Trigger:  HitWall !!");
            hit = true;
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("DeadZone"))
        {
            destroy = true;
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
