using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPickup : MonoBehaviour
{
    public int expAdd = 100;

    void OnTG(GameObject whoTG)
    {
        //GameSystem.GetPlayerData().AddMoney(moneyAdd);
        if (BattleSystem.GetInstance().IsBattleLevelUp)
        {
            BattlePlayerData.GetInstance().AddExp(expAdd);
        }
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
