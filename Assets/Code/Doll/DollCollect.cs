using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollCollect : MonoBehaviour
{
    //public GameObject dollObject;
    public bool spawnNewDoll = false;
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

            if (!collectForever)
            {
                //野生 Doll 的處理
                DollMaterial m = theDoll.gameObject.AddComponent<DollMaterial>();
            }
        }
        else
        {
            if (collectForever)
            {
                print("ERROR!!!! spawnNewDoll 的方式不支援 collectForever !!!!");
            }
        }
    }


    public void OnTG(GameObject whoTG)
    {
        //print("DollCollect OnTG");
        if (spawnNewDoll)
        {
            print("Spawn Battle Doll!! ");
            GameObject o = GameSystem.GetDollData().AddBattleDollByID(spawnDollID, transform.position);
            if (!collectForever)
            {
                o.AddComponent<DollMaterial>();
            }
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
