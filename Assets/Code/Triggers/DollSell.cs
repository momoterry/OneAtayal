using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollSell : MonoBehaviour
{
    public GameObject dollRef;
    public GameObject SpawnFX;
    public int CostMoney = 200;
    public float SpawnDistance = 4.0f;
    public Talk theTalk;

    private void OnTG(GameObject whoTG)
    {
        //================================

        DollManager dm = BattleSystem.GetInstance().GetPlayerController().GetDollManager();
        if (dollRef == null || dm == null)
        {
            return;
        }

        Doll refDoll = dollRef.GetComponent<Doll>();
        if (!refDoll)
        {
            return;
        }



        if (!dm.HasEmpltySlot(refDoll.positionType))
        {
            print("Doll Manager �S���Ŷ��F......");
            return;
        }

        if (GameSystem.GetPlayerData().GetMoney() < CostMoney)
        {
            if (theTalk)
                theTalk.AddSentence("�A�n�������Ӱ��F�r.....");
            return;
        }



        if (GameSystem.GetPlayerData().GetCurrDollNum() >= GameSystem.GetPlayerData().GetMaxDollNum())
        {
            if (theTalk)
            {
                theTalk.AddSentence("���F�A�����I�]...... ");
                //theTalk.AddSentence("����A�[�H�F�A�h�ɯŦA��...... ");
            }

            GameSystem.GetPlayerData().AddDollToBackpack(refDoll.ID);
            //return;
        }
        else
        {
            Vector3 pos = transform.position + Vector3.back * SpawnDistance;

            if (SpawnFX)
                BattleSystem.GetInstance().SpawnGameplayObject(SpawnFX, pos, false);

            GameObject dollObj = Instantiate(dollRef, pos, Quaternion.Euler(90.0f, 0, 0), null);

            Doll theDoll = dollObj.GetComponent<Doll>();
            if (theDoll == null)
            {
                print("Error!! There is no Doll in dollRef !!");
                Destroy(dollObj);
                return;
            }

            //TODO: ���ɤO�k�סA�] Action Ĳ�o�� Doll Spawn �A�i��|�� NavAgent �� Update
            NavMeshAgent dAgent = theDoll.GetComponent<NavMeshAgent>();
            if (dAgent)
            {
                dAgent.updateRotation = false;
                dAgent.updateUpAxis = false;
                dAgent.enabled = false;
            }

            if (!theDoll.TryJoinThePlayer())
            {
                print("Woooooooooops.......");
                return;
            }

            GameSystem.GetPlayerData().AddUsingDoll(theDoll.ID);
            if (theTalk)
                theTalk.AddSentence("���¥��{ !!");

        }

        GameSystem.GetPlayerData().AddMoney(-CostMoney);
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
    }
}
