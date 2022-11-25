using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;




public class DollShopItem : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public Text descText;
    public Text moneyText;
    public GameObject SpawnFX;  //TODO: 似乎應該放到別的地方更好

    protected string dollID;
    protected int CostMoney;
    protected DollShopMenu myMenu;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitInfo(DollShopItemInfo info, DollShopMenu theMenu)
    {
        myMenu = theMenu;

        if (nameText)
        {
            nameText.text = info.name;
        }
        if (descText)
        {
            descText.text = info.desc;
        }
        if (moneyText)
        {
            moneyText.text = info.cost.ToString();
        }

        CostMoney = info.cost;
        dollID = info.ID;
        GameObject dObj = GameSystem.GetPlayerData().GetDollRefByID(dollID);
        if (!dObj)
        {
            print("ERROR!! Wrong Doll ID in Shop !! : " + dollID);
        }
        Doll d = dObj.GetComponent<Doll>();
        if (icon)
        {
            icon.sprite = d.icon;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnButtonDown()
    {
        //print("來了喔!! " + dollID);
        PlayerData pData = GameSystem.GetPlayerData();
        DollManager dm = BattleSystem.GetInstance().GetPlayerController().GetDollManager();

        GameObject dollRef = pData.GetDollRefByID(dollID);
        if (dollRef == null || dm == null)
        {
            return;
        }

        if (pData.GetMoney() < CostMoney)
        {
            //print("你好像錢不太夠了呀.....");
            myMenu.ShowTempMessage("你好像錢不太夠了呀.....");
            return;
        }


        bool isToBackpack = false;
        if (pData.GetCurrDollNum() >= pData.GetMaxDollNum())
        {
            //print("無論如何，先放到背包...... ");

            pData.AddDollToBackpack(dollID);
            //return;
            //myMenu.ShowTempMessage("購買的巫靈放到背包了");
            isToBackpack = true;
        }
        else
        {
            Vector3 pos = dm.transform.position + Vector3.back * 1.0f;

            if (SpawnFX)
                BattleSystem.GetInstance().SpawnGameplayObject(SpawnFX, pos, false);

            GameObject dollObj = BattleSystem.SpawnGameObj(dollRef, pos);

            Doll theDoll = dollObj.GetComponent<Doll>();
            if (theDoll == null)
            {
                print("Error!! There is no Doll in dollRef !!");
                Destroy(dollObj);
                return;
            }

            //TODO: 先暴力法修，因 Action 觸發的 Doll Spawn ，可能會讓 NavAgent 先 Update
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

            pData.AddUsingDoll(dollID);
            //myMenu.ShowTempMessage("購買成功");
        }

        int totalNum = pData.GetDollNumByID(dollID);
        string msg;
        if (isToBackpack)
        {
            msg = "購買成功並放到背包, 總共有 " + totalNum + "個 " + nameText.text + ".";
        }
        else
        {
            msg = "購買成功並召喚, 總共有 " + totalNum + "個 " + nameText.text + ".";
        }
        myMenu.ShowTempMessage(msg);

        pData.AddMoney(-CostMoney);
    }
}
