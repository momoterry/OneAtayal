using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Doll Instance 的產生器，隨機產生對應的 Buffer

[System.Serializable]
public class DollBuffRef
{
    public DOLL_BUFF_TYPE type;
    public DOLL_BUFF_TARGET target;
    public float value1;
}


public class DIGenerator : MonoBehaviour
{
    public string dollID;
    public DollBuffRef[] possibleBuffs;
    public int buffNum = 2;


    public GameObject GenerateOne()
    {
        print("喔喔，要來產生一個 Doll Instance 了 !!");
        return null;
    }

    //作為測試用
    public void OnTG(GameObject whoTG)
    {
        bool actionResult = false;
        GameObject dObj = GenerateOne();
        if (dObj)
        {
            Doll d = dObj.GetComponent<Doll>();
            if (d)
            {
                actionResult = d.TryJoinThePlayer();
            }
        }
        whoTG.SendMessage("OnActionResult", actionResult, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }

}
