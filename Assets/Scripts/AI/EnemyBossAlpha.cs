using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossAlpha : Enemy
{
    public GameObject bulletRef;

    //�����j�ۼu��
    public float specialDamageRatio = 0.7f;
    public float specialRate = 0.3f;
    public float specialWait = 0.3f;
    public float shootPeriod = 0.1f;
    public int shotsPerLine = 12;
    public float angleStep = 10.0f;

    private float checkSkillTime = 0.0f;

    private float specialTime = 0.0f;
    private int shootCount = 0;
    private Vector3 shootTarget;

    enum ATTACK_TYPE
    {
        NORMAL,
        SPECIAL_SHOT,
    }
    ATTACK_TYPE attckType = ATTACK_TYPE.NORMAL;

    //private void OnGUI()
    //{
    //    Vector3 sPos = Camera.main.WorldToScreenPoint(transform.position);
    //    Rect debugR = new Rect(sPos.x, Camera.main.pixelHeight - sPos.y, 200, 50);
    //    GUI.TextArea(debugR, currState.ToString());
    //}

    //public override void SetUpLevel(int iLv = 1)
    //{
    //    base.SetUpLevel(iLv);
    //    float r = Mathf.Pow(LvUpRatio, (float)(iLv - 1));
    //    specialDamage *= r;
    //}

    protected override void UpdateChase()
    {
        //base.UpdateChase();

        //�b�l�ĤH�A�w���˴��O�_�o���{�j�ۡA�N�� State ���ܤ]�����m�p��
        //TODO: �ɶ��V�[�A���v�V�����i���
        bool doSpecial = false;
        checkSkillTime += Time.deltaTime;
        if ( checkSkillTime > 1.0f)
        {
            //�o���˴�
            float rd = Random.Range(0, 1.0f);
            //print("�j�� Check !!");
            if (rd < specialRate) 
            {
                print("�o��!!");
                doSpecial = true;
            }
            else
            {
                print("��F..........");
            }
            checkSkillTime = 0.0f;
        }

        if (doSpecial)
        {
            attckType = ATTACK_TYPE.SPECIAL_SHOT;
            specialTime = 0;
            shootCount = 0;
            shootTarget = targetObj.transform.position;
            myAgent.SetDestination(transform.position);   //����
            nextState = AI_STATE.ATTACK;
        }
        else
        {
            base.UpdateChase();
        }

    }

    protected override void UpdateAttack()
    {
        if ( attckType == ATTACK_TYPE.NORMAL)
        {
            base.UpdateAttack();
        }
        else
        {
            UpdateSpecialShoot();
        }
    }

    private void UpdateSpecialShoot()
    {
        ////TODO: �ѼƤ�
        //float specialWait = 0.3f;
        //float shootPeriod = 0.1f;
        //int shotsPerLine = 12;

        int shootPhase = 0;
        //int shootCount = 0;

        specialTime += Time.deltaTime;
        // TEST
        if (specialTime > specialWait)
        {
            shootPhase = (int)((specialTime - specialWait) / shootPeriod) + 1;
            if (shootPhase <= shotsPerLine)
            {
                if (shootPhase > shootCount)
                {
                    DoOneSpecialShoot(shootCount, shotsPerLine);
                    shootCount = shootCount + 1;
                }
            }
            else
            {
                nextState = AI_STATE.IDLE;
            }
        }
    }

    private void DoOneSpecialShoot( int shootIndex, int shotsPerLine )
    {
        //float angleStep = 10.0f;    //TODO: �ѼƤ�
        float halfTotalAngle = angleStep * (float)(shotsPerLine - 1) * 0.5f;

        Vector3 shootTo = shootTarget - transform.position;
        shootTo.z = 0;
        shootTo.Normalize();
        float rAngle = (float) shootIndex * angleStep - halfTotalAngle;

        //Quaternion rM = new Quaternion(0, 0, rAngle, 1.0f);
        Quaternion rM = Quaternion.AngleAxis(rAngle, Vector3.forward);
        shootTo = rM * shootTo;

        if (bulletRef)
        {
            GameObject newObj = Instantiate(bulletRef, gameObject.transform.position, Quaternion.identity, null);
            if (newObj)
            {
                bullet newBullet = newObj.GetComponent<bullet>();
                if (newBullet)
                {
                    newBullet.SetGroup(DAMAGE_GROUP.ENEMY);
                    //Vector3 td = targetObj.transform.position - newObj.transform.position;
                    //td.z = 0;
                    newBullet.targetDir = shootTo;
                    newBullet.phyDamage = Attack * specialDamageRatio;
                }
            }
        }
    }
}
