using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankOne : DollAuto
{
    public GameObject hull;
    public GameObject turret;
    public float turretLength = 1.0f;
    public float hullRotateSpeed = 180.0f;
    public float turretRotateSpeed = 240.0f;

    protected Vector3 hullBaceDir = Vector3.forward;
    protected Vector3 hullDir = Vector3.forward;
    protected float hullAngle = 0;

    protected Vector3 turretBaseDir = Vector3.forward;
    protected Vector3 turretDir = Vector3.forward;
    protected float turretAngle = 0;

    protected void UpdateTankMoave()
    {
        //transform.position = Vector3.MoveTowards(transform.position, mySlot.position, RunSpeed * Time.deltaTime);

        Vector3 toDir = mySlot.position - transform.position;
        toDir.y = 0;
        toDir.Normalize();
        //float toAngle = Vector3.SignedAngle(hullBaceDir, toDir, Vector3.down);

        //float diffAngle = toAngle - hullAngle;
        //if (diffAngle > 180.0f)
        //    diffAngle -= 360.0f;
        //else if (diffAngle < -180.0f)
        //    diffAngle += 360.0f;

        //if (Mathf.Abs(diffAngle) < 1.0f)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, mySlot.position, RunSpeed * Time.deltaTime);
        //}

        float diffAngle = Vector3.Angle(hullDir, toDir);
        if (diffAngle < 3.0f)
        {
            //對準了才移動
            transform.position = Vector3.MoveTowards(transform.position, mySlot.position, RunSpeed * Time.deltaTime);
        } 

        hullDir = Vector3.RotateTowards(hullDir, toDir, Time.deltaTime * hullRotateSpeed * Mathf.Deg2Rad, 0);
        hullAngle = Vector3.SignedAngle(hullBaceDir, hullDir, Vector3.down);
        hull.transform.localRotation = Quaternion.Euler(0, 0, hullAngle);
    }

    protected override void UpdateFollow()
    {
        myFace = BattleSystem.GetPC().GetFaceDir();
        //transform.position = Vector3.MoveTowards(transform.position, mySlot.position, RunSpeed * Time.deltaTime);
        UpdateTankMoave();

        if (autoStateTime > 0.1f)
        {

            autoStateTime = 0;

            if (SearchTarget())
            {
                //Tank 直接進攻擊
                nextAutoState = AutoState.ATTACK;
            }
        }
    }

    protected override void UpdateAttack()
    {
        bool waitRotate = false;
        if (myTarget)
        {
            Vector3 toDir = myTarget.transform.position - transform.position;
            toDir.y = 0;
            toDir.Normalize();

            float diffAngle = Vector3.Angle(turretDir, toDir);
            if (diffAngle > 3.0f)
            {
                waitRotate = true;
            }

            turretDir = Vector3.RotateTowards(turretDir, toDir, Time.deltaTime * turretRotateSpeed * Mathf.Deg2Rad, 0);
            turretAngle = Vector3.SignedAngle(turretBaseDir, turretDir, Vector3.down);
            turret.transform.localRotation = Quaternion.Euler(0, 0, turretAngle);
        }
        if (!waitRotate)
            base.UpdateAttack();
    }

    override protected void UpdateGoBack()
    {
        myFace = (mySlot.position - transform.position).normalized;
        UpdateTankMoave();

        float dis = (mySlot.position - transform.position).magnitude;

        if (dis < PositionRangeIn)
        {
            nextAutoState = AutoState.FOLLOW;
        }
    }

    protected override void DoOneAttack()
    {
        Vector3 td = turretDir;

        GameObject bulletObj = BattleSystem.SpawnGameObj(bulletRef, transform.position + turretDir * turretLength);

        bullet_base b = bulletObj.GetComponent<bullet_base>();
        if (b)
        {
            b.InitValue(DAMAGE_GROUP.PLAYER, AttackInit, turretDir, myTarget);
        }
    }

    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currAutoState.ToString() + " " + hullAngle);

    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + agentDir);
    //}
}
