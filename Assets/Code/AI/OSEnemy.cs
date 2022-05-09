using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSEnemy : MonoBehaviour
{
    public float MaxHP = 100.0f;
    public GameObject deadFX;
    public float Attack = 20.0f;

    protected float hp;
    protected Hp_BarHandler myHPHandler;

    public float SpawnWaitTime = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        hp = MaxHP;
        myHPHandler = GetComponent<Hp_BarHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (myHPHandler && currState != AI_STATE.SPAWN_WAIT && currState != AI_STATE.NONE)
        {
            myHPHandler.SetHP(hp, MaxHP);
        }
    }

    void OnDamage(Damage theDamage)
    {

        hp -= theDamage.damage;
        if (hp < 0)
        {
            hp = 0;
            DoDeath();
        }


    }

    protected void DoDeath()
    {

        Quaternion rm = Quaternion.Euler(90, 0, 0);
       
        if (deadFX)
        {

            Instantiate(deadFX, transform.position, rm, null);
        }

        Destroy(gameObject);
    }


}
