using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIRemoveAll : MonoBehaviour
{
    //�@�����ե�
    public void OnTG(GameObject whoTG)
    {
        print("�}�l���ղ����Ҧ� DI !!");
        List<Doll> dList = BattleSystem.GetPC().GetDollManager().GetDolls();
        foreach (Doll d in dList)
        {
            DollInstance di = d.gameObject.GetComponent<DollInstance>();
            if (di)
            {
                print("���@�� DI !! " + di.fullName);
                //GameSystem.GetPlayerData().RemoveUsingDI(di.ToData());
                Destroy(d.gameObject);
            }
        }

        GameSystem.GetPlayerData().RemoveAllUsingDIs();
    }
}
