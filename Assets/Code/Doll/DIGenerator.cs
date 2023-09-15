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
    public string text;
}


public class DIGenerator : MonoBehaviour
{
    public string dollID;
    public DollBuffRef[] prefixBuffers;
    public DollBuffRef[] suffixBuffers;
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

        string dollName = "���F";
        DollBuffRef preBuffRef = prefixBuffers[Random.Range(0, prefixBuffers.Length - 1)];
        DollBuffRef sufBuffRef = suffixBuffers[Random.Range(0, suffixBuffers.Length - 1)];
        dollName = preBuffRef.text + "��" + dollName + sufBuffRef.text;
        print("�ͦ����W�r�O: " + dollName);

        //Buff ���ͦ�
        DollBuffBase pBuff = GenerateOneBuff(preBuffRef);
        DollBuffBase sBuff = GenerateOneBuff(sufBuffRef);
        di.AddBuff(pBuff);
        di.AddBuff(sBuff);

        di.fullName = dollName;

        //TODO: di �}�l�ҥ� Buff

        return null;
    }


    protected DollBuffBase GenerateOneBuff(DollBuffRef bRef)
    {
        DollBuffBase newBuff = null;
        switch (bRef.type)
        {
            case DOLL_BUFF_TYPE.DAMAGE:
            case DOLL_BUFF_TYPE.HP:
            case DOLL_BUFF_TYPE.ATTACK_SPEED:
            case DOLL_BUFF_TYPE.MOVE_SPEED:
                newBuff = new DollBuffBase();
                newBuff.InitValue(bRef.target, bRef.value1);
                break;
        }
        return newBuff;
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
