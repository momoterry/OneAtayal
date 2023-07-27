using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DollSkillOne : DollSkillBase
{
    protected float attackSpeedRate = 1.5f;    //�g�t�W�[
    protected float AttackRangeAdd = -1.0f;     //�g�{�W�[

    protected GameObject bulletRef;
    protected bool isActive = false;
    protected float attackCD;
    protected float attackRange;

    protected float timeToShoot;

    // Start is called before the first frame update
    void Start()
    {
        bulletRef = doll.bulletRef;
        attackRange = doll.AttackRangeIn + AttackRangeAdd;
        attackCD = doll.attackCD / attackSpeedRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            UpdateHoldShoot();
        }
    }

    protected GameObject SearchTarget()
    {
        GameObject foundEnemy = null;
        float minDistance = Mathf.Infinity;

        Collider[] cols = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Character"));
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                float dis = Vector3.Distance(col.gameObject.transform.position, transform.position);

                if (dis < minDistance)
                {
                    minDistance = dis;
                    foundEnemy = col.gameObject;
                }
            }
        }

        return foundEnemy;
    }

    protected void DoOneAttack(GameObject target)
    {
        GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.Euler(90, 0, 0));
        Vector3 td = target.transform.position - transform.position;
        td.y = 0;
        td.Normalize();

        myDamage.damage = doll.AttackInit;
        bullet_base b = bulletObj.GetComponent<bullet_base>();
        if (b)
        {
            b.InitValue(DAMAGE_GROUP.PLAYER, myDamage, td, target);
        }

        doll.SetFace(td);
    }

    protected void UpdateHoldShoot()
    {
        timeToShoot -= Time.deltaTime;
        if (timeToShoot <= 0)
        {
            timeToShoot += attackCD;
            GameObject target = SearchTarget();
            if (target)
            {
                DoOneAttack(target);
            }
        }
    }

    protected void StartHoldShoot()
    {
        doll.StartHoldPosition(doll.transform.position);
        timeToShoot = 0;
    }

    protected void StopHoldShoot()
    {
        doll.StopHoldPosition();
    }

    public override void OnStartSkill(bool active = true)
    {
        base.OnStartSkill(active);

        isActive = active;
        if (active)
        {
            StartHoldShoot();
        }
        else
        {
            StopHoldShoot();
        }
        //print("DollSkillOne::OnStartSkill " + active);
    }

}
