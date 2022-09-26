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
        //TODO: �o�䪺�B�z, ���ӥ�Ѩ䥦 Manager �B�z�Ӥ��O�浹 UI

        print("Down!! ID: " + dollID + " currNum = " + dollNum);
        PlayerData pData = GameSystem.GetPlayerData();
        PlayerControllerBase thePC = BattleSystem.GetPC();

        if (pData.GetCurrDollNum() == pData.GetMaxDollNum())
        {
            thePC.SaySomthing("�ڲ{�b�a���F����h.....");
            //print("���F���F");
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

        //TODO: Spawn �S��

        //�O��
        pData.AddUsingDoll(dollID);
        pData.RemoveDollFromBackpack(dollID);

        SetNum(dollNum - 1);
    }
}
