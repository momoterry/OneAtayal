using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Doll Instance �����;��A�H�����͹����� Buffer

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
        print("���A�n�Ӳ��ͤ@�� Doll Instance �F !!");
        GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(dollID);
        if (!dollRef)
        {
            print("ERROR!! GenerateOne : ���~�� Doll ID: " + dollID);
            return null;
        }
        GameObject o = BattleSystem.SpawnGameObj(dollRef, transform.position + Vector3.forward * 2);
        Doll doll = o.GetComponent<Doll>();
        DollInstance di = o.AddComponent<DollInstance>();

        return null;
    }

    //�@�����ե�
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
        whoTG.SendMessage("OnActionResult", actionResult, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
    }

}
