using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAutoShooter : MonoBehaviour
{
    public GameObject bulletRef;
    public Transform startPos;
    public Vector3 shootDir = Vector3.right;
    public float damage = 20.0f;
    public float timePeriod = 0.1f;
    public float initWait = 0.5f;
    public Damage.OwnerType type = Damage.OwnerType.ENEMY;

    protected float waitTime = 0;
    protected Damage myDamage;

    // Start is called before the first frame update
    void Start()
    {
        waitTime = initWait;
        myDamage.Init(damage, type, gameObject.name, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0)
        {
            waitTime += timePeriod;
            DoOneShoot();
        }
    }

    protected void DoOneShoot()
    {

        if (bulletRef)
        {
            GameObject bObj = BattleSystem.SpawnGameObj(bulletRef, startPos.position);
            bullet_base bullet = bObj.GetComponent<bullet_base>();
            if (bullet)
            {
                DAMAGE_GROUP dg = type == Damage.OwnerType.ENEMY ? DAMAGE_GROUP.ENEMY : DAMAGE_GROUP.PLAYER;
                bullet.InitValue(dg, myDamage, shootDir);
            }
        }
    }
}
