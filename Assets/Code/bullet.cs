using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Damage 定義的部份
public struct Damage
{
    public float damage;
}

public enum DAMAGE_GROUP
{
    NONE,
    PLAYER,
    ENEMY,
}

public class bullet : MonoBehaviour
{
    public float phyDamage = 60.0f;
    public float speed = 20.0f;
    public Vector3 targetDir = Vector3.up;
    public float lifeTime = 0.5f;
    public GameObject hitFX;

    private DAMAGE_GROUP group = DAMAGE_GROUP.PLAYER;

    private float myTime = 1.0f;

    private Damage myDamage;


    // Public Functions
    public void SetGroup(DAMAGE_GROUP g)
    {
        group = g;
    }

    // Start is called before the first frame update
    void Start()
    {
        myTime = lifeTime;
        myDamage.damage = phyDamage;
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        print("OnCollisionEnter2D : " + col);
        if (col.gameObject.CompareTag("Enemy"))
        {
            //print("Hit Enemy !!  .. Collision");
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //print("OnTriggernEnter2D : " + col);
        bool hit = false;
        bool flashShift = true;// 打中角色的話, 擊中特效往鏡頭放以免被擋
        if (col.gameObject.CompareTag("Enemy") && group == DAMAGE_GROUP.PLAYER)
        {
            //print("Trigger:  Hit Enemy !!");
            hit = true;
            col.gameObject.SendMessage("DoDamage", myDamage);
        }
        else if (col.gameObject.CompareTag("Player") && group == DAMAGE_GROUP.ENEMY)
        {
            //print("Trigger:  Hit Player !!");
            hit = true;
            col.gameObject.SendMessage("DoDamage", myDamage);
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //print("Trigger:  HitWall !!");
            hit = true;
            flashShift = false;
        }


        if (hit)
        {
            Vector3 hitPos = col.ClosestPoint(transform.position);
            if (flashShift)
            {
                hitPos.z = col.transform.position.z - 0.125f;   //角色的話用對方的 Z 來調整
            }
            else
                hitPos.z = hitPos.y;

            Instantiate(hitFX, hitPos, Quaternion.identity, null);
            Destroy(gameObject);
        }

    }

}
