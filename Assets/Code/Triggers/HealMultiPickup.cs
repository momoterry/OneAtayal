using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealMultiPickup : MonoBehaviour
{
    public GameObject healFX;
    //public int healTotal = 100;
    public int healCount = 10;
    public float healEach = 10;             //–干﹀荡癸秖
    public float healPercentEach = 5;       //–干﹀κだ秖 (HPMax 禫蔼干禫)

    protected class HealInfo
    {
        public GameObject obj;
        public float hp;
        public float hpMax;
        public float hpToHeal = 0;
        public float hpRatio;
        public HealInfo(GameObject obj, float hp, float hpMax, float hpToHeal, float hpRatio)
        {
            this.obj = obj;
            this.hp = hp;
            this.hpMax = hpMax;
            this.hpToHeal = hpToHeal;
            this.hpRatio = hpRatio;
        }
    }
    public void OnTG(GameObject whoTG)
    {
        List<HealInfo> hList = new List<HealInfo>();

        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();

        if (pc.GetHP() < pc.GetHPMax())
        {
            float preHealValue = 0;
            PreHealInfo pi = pc.GetComponent<PreHealInfo>();
            if (pi)
            {
                preHealValue = pi.GetPreHeal();
            }
            float ratio = (pc.GetHP() + preHealValue) / pc.GetHPMax();

            hList.Add(new HealInfo(pc.gameObject, (pc.GetHP() + preHealValue), pc.GetHPMax(), 0, ratio));
        }


        //And Dolls (т﹀ゑㄒ程ぶ)
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
                hList.Add(new HealInfo(body.gameObject, (body.GetHP() + preHealValue), body.GetHPMax(), 0, hpRatio));
            }
        }

        if (hList.Count > 0)
        {
            for (int i=0; i<healCount; i++)
            {
                HealInfo theHeal = hList.OrderBy(obj => (float)obj.hpRatio).FirstOrDefault();

                if (theHeal.hpToHeal + theHeal.hp >= theHeal.hpMax)
                {
                    //print("⊿Τ挡");
                    break;
                }
                //print("hList Best !!" + theHeal.obj + " hp " + (theHeal.hp + theHeal.hpToHeal) + " / max: " + theHeal.hpMax);
                float healOnce = healEach + theHeal.hpMax * healPercentEach * 0.01f;
                theHeal.hpToHeal = Mathf.Min(theHeal.hpToHeal + healOnce, theHeal.hpMax - theHeal.hp);
                //print("heal to " + theHeal.hpToHeal);
                theHeal.hpRatio = (theHeal.hp + theHeal.hpToHeal) / theHeal.hpMax;
            }

            foreach (HealInfo h in hList)
            {
                if (h.hpToHeal > 0)
                {
                    HealTarget(h.obj, h.hpToHeal);
                }
            }

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
                GameObject f = BattleSystem.SpawnGameObj(healFX, targetObj.transform.position);
                f.transform.parent = targetObj.transform;
            }
        }
    }
}
