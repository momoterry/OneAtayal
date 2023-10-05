using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

//Doll Instance 的產生器，隨機產生對應的 Buffer

[System.Serializable]
public class DollBuffRef
{
    public DOLL_BUFF_TYPE type;
    public DOLL_BUFF_TARGET target;
    public float value1;            //TODO: 改成 Int
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
        //print("喔喔，要來產生一個 Doll Instance 了 !!");
        GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(dollID);
        if (!dollRef)
        {
            print("ERROR!! GenerateOne : 錯誤的 Doll ID: " + dollID);
            return null;
        }
        GameObject o = BattleSystem.SpawnGameObj(dollRef, transform.position + Vector3.forward * 2);
        Doll doll = o.GetComponent<Doll>();
        DollInstance di = o.AddComponent<DollInstance>();

        string dollName = dollBaseName;
        DollBuffRef preBuffRef = prefixBuffers[Random.Range(0, prefixBuffers.Length - 1)];
        DollBuffRef sufBuffRef = suffixBuffers[Random.Range(0, suffixBuffers.Length - 1)];
        dollName = preBuffRef.text + "的" + dollName + sufBuffRef.text;
        print("生成的名字是: " + dollName);

        //Buff 的生成
        DollBuffBase pBuff = GenerateOneBuff(preBuffRef);
        DollBuffBase sBuff = GenerateOneBuff(sufBuffRef);

        //DollInstace 的設定 TODO: 以下應該包裝成一次性初始化
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

            if (actionResult)
            {
                DollInstance di = dObj.GetComponent<DollInstance>();
                GameSystem.GetPlayerData().AddUsingDI(di.ToData());
            }
            else
            {
                print("ERROR !!!! Doll Insatnce 生出來後無法加入，這部份還沒處理呀 !!!!!");
            }
        }
        whoTG.SendMessage("OnActionResult", actionResult, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }

}
