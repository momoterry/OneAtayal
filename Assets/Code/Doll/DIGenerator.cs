using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

//Doll Instance �����;��A�H�����͹����� Buffer

[System.Serializable]
public class DollBuffRef
{
    public DOLL_BUFF_TYPE type;
    public DOLL_BUFF_TARGET target;
    public float value1;            //TODO: �令 Int
    public string text;
}


public class DIGenerator : MonoBehaviour
{
    public string dollID;
    public string dollBaseName;
    public DollBuffRef[] prefixBuffers;
    public DollBuffRef[] suffixBuffers;

    public DollCardMenu showResultMenu;

    public GameObject GenerateOne()
    {
        //print("���A�n�Ӳ��ͤ@�� Doll Instance �F !!");
        GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(dollID);
        if (!dollRef)
        {
            print("ERROR!! GenerateOne : ���~�� Doll ID: " + dollID);
            return null;
        }
        GameObject o = BattleSystem.SpawnGameObj(dollRef, transform.position + Vector3.forward * 2);
        Doll doll = o.GetComponent<Doll>();
        DollInstance di = o.AddComponent<DollInstance>();

        string dollName = dollBaseName;
        DollBuffRef preBuffRef = prefixBuffers[Random.Range(0, prefixBuffers.Length - 1)];
        DollBuffRef sufBuffRef = suffixBuffers[Random.Range(0, suffixBuffers.Length - 1)];
        dollName = preBuffRef.text + "��" + dollName + sufBuffRef.text;
        print("�ͦ����W�r�O: " + dollName);

        //Buff ���ͦ�
        DollBuffBase pBuff = GenerateOneBuff(preBuffRef);
        DollBuffBase sBuff = GenerateOneBuff(sufBuffRef);

        //DollInstace ���]�w TODO: �H�U���ӥ]�˦��@���ʪ�l��
        di.Init(dollName, doll);
        di.AddBuff(pBuff);
        di.AddBuff(sBuff);

        if (showResultMenu)
        {
            showResultMenu.ShowOneDollCard(di);
        }

        return o;
    }


    protected DollBuffBase GenerateOneBuff(DollBuffRef bRef)
    {
        DollBuffBase newBuff = null;
        switch (bRef.type)
        {
            case DOLL_BUFF_TYPE.DAMAGE:
                newBuff = new DollBuffDamage();
                break;
            case DOLL_BUFF_TYPE.HP:
                newBuff = new DollBuffHP();
                break;
            case DOLL_BUFF_TYPE.ATTACK_SPEED:
                newBuff = new DollBuffAttackSpeed();
                break;
            case DOLL_BUFF_TYPE.MOVE_SPEED:
                newBuff = new DollBuffMoveSpeed();
                break;
        }
        if (newBuff != null)
        {
            newBuff.InitValue(bRef.type, bRef.target, (int)bRef.value1);
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

            if (actionResult)
            {
                DollInstance di = dObj.GetComponent<DollInstance>();
                GameSystem.GetPlayerData().AddUsingDI(di.ToData());
            }
            else
            {
                print("ERROR !!!! Doll Insatnce �ͥX�ӫ�L�k�[�J�A�o�����٨S�B�z�r !!!!!");
            }
        }
        whoTG.SendMessage("OnActionResult", actionResult, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
    }

}
