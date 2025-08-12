using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollCollect : MonoBehaviour
{
    //public GameObject dollObject;
    public bool spawnNewDoll = false;
    public bool multipleCollect = false;    //可重複收集，只有在 spawnNewDoll 才有意義
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
                //野生 Doll 可轉化成 Material 時的處理
                DollMaterial m = theDoll.gameObject.AddComponent<DollMaterial>();
            }
        }
        else
        {
            if (collectForever)
            {
                One.ERROR("spawnNewDoll 的方式不支援 collectForever !!!!");
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

            //先測試性放入
            //string[] joinTalks = { "我來了", "是野巫靈喔", "呼....好累", "我幫得上忙!!" };
            //string joinTalk = joinTalks[Random.Range(0, joinTalks.Length)];
            //ComicTalk.StartTalk(joinTalk, o, 2.0f);
            o.SendMessage("OnTalkCondition", TALK_CONDITION.COLLECTED, SendMessageOptions.DontRequireReceiver);

            if (!multipleCollect)
                Destroy(gameObject);
            return;
        }

        //回應 ActionTrigger 是否成功
        bool actionResult = theDoll.TryJoinThePlayer(collectForever?DOLL_JOIN_SAVE_TYPE.FOREVER:DOLL_JOIN_SAVE_TYPE.BATTLE);
        whoTG.SendMessage("OnActionResult", actionResult, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
        if (actionResult)
        {
            //脫離
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
