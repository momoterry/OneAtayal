using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HealMultiPickup : MonoBehaviour
{
    public GameObject healFX;
    public int healTotal = 100;

    void OnTG(GameObject whoTG)
    {
        print("heal....");

        GameObject bestHealTarget = null;

        float bestTargetHpRatio = 1.0f;
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc.GetHP() < pc.GetHPMax())
        {
            float preHealValue = 0;
            PreHealInfo pi = pc.GetComponent<PreHealInfo>();
            if (pi)
            {
                preHealValue = pi.GetPreHeal();
            }
            bestTargetHpRatio = (pc.GetHP() + preHealValue) / pc.GetHPMax();
            bestHealTarget = pc.gameObject;
        }


        //And Dolls (找血的比例最少的)
        List<Doll> theList = pc.GetDollManager().GetDolls();
        foreach (Doll d in theList)
        {
            if (!d.gameObject.activeInHierarchy)
                continue;
            if (d == this)
                continue;
            HitBody body = d.GetComponent<HitBody>();
            if (body && body.GetHP() < body.GetHPMax())
            {
                float preHealValue = 0;
                PreHealInfo pi = d.GetComponent<PreHealInfo>();
                if (pi)
                {
                    preHealValue = pi.GetPreHeal();
                }

                float hpRatio = (body.GetHP() + preHealValue) / body.GetHPMax();
                if (hpRatio < bestTargetHpRatio)
                {
                    bestHealTarget = body.gameObject;
                    bestTargetHpRatio = hpRatio;
                }
            }
        }

        if (bestHealTarget)
        {
            HealTarget(bestHealTarget, healTotal);
            //whoTG.
            Destroy(gameObject);
        }
    }

    protected void HealTarget(GameObject targetObj, float healValue)
    {
        if (targetObj.activeInHierarchy)
        {
            PlayerControllerBase pc = targetObj.GetComponent<PlayerControllerBase>();
            if (pc)
            {
                pc.DoHeal(healValue);
            }

            HitBody body = targetObj.GetComponent<HitBody>();
            if (body)
            {
                body.DoHeal(healValue);
            }

            if (healFX)
            {
                BattleSystem.SpawnGameObj(healFX, targetObj.transform.position);
            }
        }
    }
}
