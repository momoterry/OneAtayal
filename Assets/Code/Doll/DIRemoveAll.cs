using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIRemoveAll : MonoBehaviour
{
    //作為測試用
    public void OnTG(GameObject whoTG)
    {
        print("開始嘗試移除所有 DI !!");
        List<Doll> dList = BattleSystem.GetPC().GetDollManager().GetDolls();
        foreach (Doll d in dList)
        {
            DollInstance di = d.gameObject.GetComponent<DollInstance>();
            if (di)
            {
                print("找到一個 DI !! " + di.fullName);
                //GameSystem.GetPlayerData().RemoveUsingDI(di.ToData());
                Destroy(d.gameObject);
            }
        }

        GameSystem.GetPlayerData().RemoveAllUsingDIs();
    }
}
