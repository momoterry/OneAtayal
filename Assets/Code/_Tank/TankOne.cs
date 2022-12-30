using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankOne : DollAuto
{
    public float hullRotateSpeed = 180.0f;

    protected Vector3 agentDir;

    protected override void UpdateFollow()
    {
        //base.UpdateFollow();

        //agentDir = myAgent.desiredVelocity;

        transform.position = Vector3.MoveTowards(transform.position, mySlot.position, RunSpeed * Time.deltaTime);

        myFace = BattleSystem.GetPC().GetFaceDir();

        if (autoStateTime > 0.1f)
        {

            autoStateTime = 0;

            if (SearchTarget())
            {
                nextAutoState = AutoState.CHASE;
                //ª½±µ¶i§ðÀ»
                float disT = (myTarget.transform.position - transform.position).magnitude;
                if (disT < AttackRangeIn)
                {
                    nextAutoState = AutoState.ATTACK;
                }
            }
        }
    }

    override protected void UpdateGoBack()
    {
        transform.position = Vector3.MoveTowards(transform.position, mySlot.position, RunSpeed * Time.deltaTime);

        float dis = (mySlot.position - transform.position).magnitude;

        myFace = (mySlot.position - transform.position).normalized;

        //print(dis);
        if (dis < PositionRangeIn)
        {
            nextAutoState = AutoState.FOLLOW;
        }
    }

    private void OnGUI()
    {
        Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
        thePoint.y = Camera.main.pixelHeight - thePoint.y;
        GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currAutoState.ToString());

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + agentDir);
    //}
}
