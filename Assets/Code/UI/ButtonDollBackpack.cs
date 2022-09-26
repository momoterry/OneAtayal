using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDollBackpack : MonoBehaviour
{
    public Image icon;
    public Text numText;
    // Start is called before the first frame update

    protected int dollNum;
    protected string dollID; 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDollInfo( string _dollID, Sprite iconSprite)
    {
        dollID = _dollID;
        if (icon)
        {
            icon.sprite = iconSprite;
        }
    }

    //public void SetIcon(Sprite iconSprite)
    //{
    //    if (icon)
    //    {
    //        icon.sprite = iconSprite;
    //    }
    //}

    public void SetNum( int num )
    {
        dollNum = num;
        numText.text = "" + num;
        if (dollNum == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnButtonDown()
    {
        //TODO: 這邊的處理, 應該交由其它 Manager 處理而不是交給 UI

        print("Down!! ID: " + dollID + " currNum = " + dollNum);
        PlayerData pData = GameSystem.GetPlayerData();
        PlayerControllerBase thePC = BattleSystem.GetPC();

        if (pData.GetCurrDollNum() == pData.GetMaxDollNum())
        {
            thePC.SaySomthing("我現在帶不了那麼多.....");
            //print("滿了滿了");
            return;
        }

        GameObject dollRef = pData.GetDollRefByID(dollID);
        if (!dollRef)
        {
            print("ERROR!! Wrong ID: " + dollID);
            return;
        }

        Vector3 pos = thePC.transform.position + Vector3.forward * 2.0f;
        GameObject dObj = BattleSystem.SpawnGameObj(dollRef, pos);
        Doll d = dObj.GetComponent<Doll>();
        if (!d)
        {
            print("ERROR!! No Doll: " + dollID);
            return;
        }
        if (!d.TryJoinThePlayer())
        {
            print("ERROR!! Can not Join Player !!!!");
        }

        //TODO: Spawn 特效

        //記錄
        pData.AddUsingDoll(dollID);
        pData.RemoveDollFromBackpack(dollID);

        SetNum(dollNum - 1);
    }
}
