using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaHealObject : MonoBehaviour
{
    public float manaToHeal = 50.0f;
    public GameObject healFX;

    void OnTG(GameObject whoTG)
    {
        //print("DoHeal !!!!!" + whoTG);

        bool result = false;
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc.GetMP() < pc.GetMPMax())
        {
            pc.DoHealMana(manaToHeal);
            if (healFX)
            {
#if XZ_PLAN
                Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
                Quaternion rm = Quaternion.identity;
#endif
                Instantiate(healFX, pc.transform.position, rm, pc.transform);
            }
            result = true;
        }

        whoTG.SendMessage("OnActionResult", result, SendMessageOptions.DontRequireReceiver);

        if (result)
            Destroy(gameObject);
    }
}
