using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDasher : Enemy
{
    //public float DashSpeed = 10.0f;
    public float DashTime = 0.1f;
    public float BackTime = 0.1f;
    public float MinDashLength = 1.0f;
    public float DashToTargetDistance = 0.5f;

    public GameObject hitFX;

    //protected float originalSpeed = 3.0f;
    //protected float originalAcc = 10.0f;

    public Transform myDashBody;

    protected float dashStateTime = 0;
    protected Vector3 dashVector;
    protected Vector3 bodyOriginalRelativePos;

    public enum DASH_STATE
    {
        NONE,
        DASHING,
        BACK,
    }

    DASH_STATE currDashState = DASH_STATE.NONE;
    DASH_STATE nextDashState = DASH_STATE.NONE;

    protected override void DoOneAttack()
    {
        if (myDashBody)
            bodyOriginalRelativePos = myDashBody.transform.position - transform.position;

        dashVector = targetObj.transform.position - transform.position;
        dashVector.y = 0;

        float dashLength = Mathf.Max(MinDashLength, dashVector.magnitude - DashToTargetDistance);
        dashVector = dashVector.normalized * dashLength;
        //print("DashLength : " + dashLength);

        nextDashState = DASH_STATE.DASHING;
    }

    protected void DoFinishDash()
    {
        if (myDashBody)
        {
            myDashBody.position = transform.position + bodyOriginalRelativePos;
        }
    }

    protected void DoApplyDashDamage()
    {
        if (targetObj)
        {
            targetObj.SendMessage("OnDamage", myDamage);

            if (hitFX)
            {
                Vector3 pos = targetObj.transform.position;
                BattleSystem.GetInstance().SpawnGameplayObject(hitFX, pos, false);
            }
        }
    }

    protected override void UpdateAttack()
    {
        //base.UpdateAttack();

        if (nextDashState != currDashState)
        {
            switch (nextDashState)
            {
                case DASH_STATE.DASHING:
                    DoApplyDashDamage();
                    break;
                case DASH_STATE.BACK:
                    break;
                case DASH_STATE.NONE:
                    DoFinishDash();
                    break;
            }
            dashStateTime = 0;

            currDashState = nextDashState;
        }
        else
        {
            dashStateTime += Time.deltaTime;
        }


        switch (currDashState)
        {
            case DASH_STATE.NONE:
                base.UpdateAttack();
                break;
            case DASH_STATE.DASHING:
                stateTime -= Time.deltaTime;
                UpdateDash();
                break;
            case DASH_STATE.BACK:
                stateTime -= Time.deltaTime;
                UpdateBack();
                break;
        }

    }

    protected void UpdateDash()
    {
        float posRatio = dashStateTime / DashTime;
        if (posRatio  >= 1.0f)
        {
            nextDashState = DASH_STATE.BACK;
            posRatio = 1.0f;
        }

        Vector3 localPos = dashVector * posRatio;
        if (myDashBody)
        {
            myDashBody.position = transform.position + bodyOriginalRelativePos + localPos;
        }
    }

    protected void UpdateBack()
    {
        float posRatio = 1.0f - (dashStateTime / BackTime);
        if (posRatio <=.0)
        {
            nextDashState = DASH_STATE.NONE;
            posRatio = 0;
        }

        Vector3 localPos = dashVector * posRatio;
        if (myDashBody)
        {
            myDashBody.position = transform.position + bodyOriginalRelativePos + localPos;
        }
    }

    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currState.ToString() + "\n" + currDashState.ToString());
    //}
}
