using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollSell : MonoBehaviour
{
    public GameObject dollRef;
    public GameObject SpawnFX;
    public float SpawnDistance = 4.0f;

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


        Vector3 pos = transform.position + Vector3.back * SpawnDistance;
        //Vector3 pos = availableSlot.position;

        if (SpawnFX)
            BattleSystem.GetInstance().SpawnGameplayObject(SpawnFX, pos, false);

        //GameObject dollObj = BattleSystem.GetInstance().SpawnGameplayObject(dollRef, pos, false);

        GameObject dollObj = Instantiate(dollRef, pos, Quaternion.Euler(90.0f, 0, 0), null);
        //print("M!! " + dollObj.transform.rotation);

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

        //TODO �b�o�䦩��

        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
    }
}
