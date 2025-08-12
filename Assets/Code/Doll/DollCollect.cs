using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollCollect : MonoBehaviour
{
    //public GameObject dollObject;
    public bool spawnNewDoll = false;
    public bool multipleCollect = false;    //�i���Ʀ����A�u���b spawnNewDoll �~���N�q
    public string spawnDollID;
    public bool collectForever;
    public bool isDollMaterial = true;

    protected Doll theDoll;
    // Start is called before the first frame update
    void Start()
    {
        if (!spawnNewDoll)
        {
            theDoll = GetComponentInChildren<Doll>();
            if (theDoll == null || theDoll.transform.parent != gameObject.transform)
            {
                print("ERROR !!!! There is no Doll as child of DollCollct !!");
            }

            if (!collectForever && isDollMaterial)
            {
                //���� Doll �i��Ʀ� Material �ɪ��B�z
                DollMaterial m = theDoll.gameObject.AddComponent<DollMaterial>();
            }
        }
        else
        {
            if (collectForever)
            {
                One.ERROR("spawnNewDoll ���覡���䴩 collectForever !!!!");
            }
        }
    }


    public void OnTG(GameObject whoTG)
    {
        //print("DollCollect OnTG");
        if (spawnNewDoll)
        {
            //print("Spawn Battle Doll!! ");
            GameObject o = GameSystem.GetDollData().SpawnBattleDollByID(spawnDollID, transform.position);
            if (!collectForever)
            {
                o.AddComponent<DollMaterial>();
            }

            //�����թʩ�J
            //string[] joinTalks = { "�ڨӤF", "�O�����F��", "�I....�n��", "�����o�W��!!" };
            //string joinTalk = joinTalks[Random.Range(0, joinTalks.Length)];
            //ComicTalk.StartTalk(joinTalk, o, 2.0f);
            o.SendMessage("OnTalkCondition", TALK_CONDITION.COLLECTED, SendMessageOptions.DontRequireReceiver);

            if (!multipleCollect)
                Destroy(gameObject);
            return;
        }

        //�^�� ActionTrigger �O�_���\
        bool actionResult = theDoll.TryJoinThePlayer(collectForever?DOLL_JOIN_SAVE_TYPE.FOREVER:DOLL_JOIN_SAVE_TYPE.BATTLE);
        whoTG.SendMessage("OnActionResult", actionResult, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
        if (actionResult)
        {
            //����
            theDoll.transform.SetParent(null);

            //if (collectForever)
            //{
            //    GameSystem.GetPlayerData().AddUsingDoll(theDoll.ID);
            //}
            //else
            //{
            //    ContinuousBattleManager.AddCollectedDoll(theDoll.ID);
            //}

            Destroy(gameObject);
        }
    }
}
