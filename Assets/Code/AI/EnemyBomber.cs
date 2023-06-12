using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomber : Enemy
{
    public float expRadius = 4.0f;
    public GameObject expFX;
    public GameObject hitFX;

    protected float bombTime = 1.0f;

    protected override void UpdateAttack()
    {
        bombTime -= Time.deltaTime;
        if (bombTime < 0)
        {
            DoExplosion();
            Destroy(gameObject);
        }
    }

    protected override void OnStartAttack()
    {
        //print("Bomb OnStartAttack");
        base.OnStartAttack();
        if (mySPAimator)
        {
            mySPAimator.PlaySpecific("Hint");
        }
    }

    protected void DoExplosion()
    {
        Vector3 expPos = transform.position;

        BattleSystem.SpawnGameObj(expFX, expPos);

        //Ãz¬µ³B²z
        Collider[] cols = Physics.OverlapSphere(expPos, expRadius);
        foreach (Collider co in cols)
        {
            if (co.gameObject.CompareTag("Player") || co.gameObject.CompareTag("Doll"))
            {
                co.gameObject.SendMessage("OnDamage", myDamage);
                BattleSystem.SpawnGameObj(hitFX, co.transform.position);
            }
        }
    }
}
