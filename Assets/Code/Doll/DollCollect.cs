using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollCollect : MonoBehaviour
{
    //public GameObject dollObject;
    public bool collectForever;


    protected Doll theDoll;
    // Start is called before the first frame update
    void Start()
    {
        //if (dollObject)
        //theDoll = dollObject.GetComponent<Doll>();
        theDoll = GetComponentInChildren<Doll>();
        if (theDoll == null || theDoll.transform.parent != gameObject.transform)
        {
            print("ERROR !!!! There is no Doll as child of DollCollct !!");
        }

        if (!collectForever)
        {
            //���� Doll ���B�z
            DollMaterial m = theDoll.gameObject.AddComponent<DollMaterial>();
        }
    }


    public void OnTG(GameObject whoTG)
    {
        //print("DollCollect OnTG");


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