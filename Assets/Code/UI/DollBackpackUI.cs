using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DollBackpackUI: MonoBehaviour
{
    public GameObject buttonRef;
    public Transform buttonRoot; 

    protected Dictionary<string, ButtonDollBackpack> buttonMap = new Dictionary<string, ButtonDollBackpack>();

    // Start is called before the first frame update
    void Start()
    {

    }

    protected float testTime = 0.2f;

    // Update is called once per frame
    void Update()
    {
        if (testTime > 0)
        {
            testTime -= Time.deltaTime;
            if (testTime <= 0)
            {
                //OpenBackpackUI();
                testTime = -1.0f;
            }
        }
    }

    public void OpenBackpackUI()
    {
        ////== Debug
        //PlayerData pData = GameSystem.GetPlayerData();
        //pData.AddDollToBackpack("DollOne");
        //pData.AddDollToBackpack("DollOne");
        //pData.AddDollToBackpack("DollOne");
        //pData.AddDollToBackpack("DollFire");
        //pData.AddDollToBackpack("DollStone");
        //pData.AddDollToBackpack("DollStone");
        ////==

        if (buttonRoot)
        {
            buttonRoot.gameObject.SetActive(true);
            CreateButtons();
        }
    }

    public void CloseBackpackUI()
    {
        ClearButtons();
        if (buttonRoot)
        {
            buttonRoot.gameObject.SetActive(false);
        }
    }

    protected void CreateButtons()
    {
        PlayerData pData = GameSystem.GetPlayerData();
        Dictionary<string, int> backPackInfo = pData.GetDollBackPack();

        int i = 0;
        int ih = 0;  //
        foreach ( KeyValuePair<string, int> k in backPackInfo)
        {
            GameObject bo = Instantiate(buttonRef, buttonRoot.transform);
            RectTransform rt = bo.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(56.0f - (36 * ih), 92.0f - (36 * i));
            }

            ButtonDollBackpack bDoll = bo.GetComponent<ButtonDollBackpack>();
            if (bDoll)
            {
                GameObject dObj = pData.GetDollRefByID(k.Key);
                Doll d = dObj.GetComponent<Doll>();
                bDoll.SetNum(k.Value);
                bDoll.SetDollInfo(k.Key, d.icon);

                buttonMap.Add(k.Key, bDoll);
            }

            i++;
            if ( i >= 4)
            {
                i = 0;
                ih++;
            }
        }

    }

    protected void ClearButtons()
    {
        foreach (KeyValuePair<string, ButtonDollBackpack> k in buttonMap)
        {
            Destroy(k.Value.gameObject);
        }
        buttonMap.Clear();
    }

    public void OnDismissAllDoll()
    {
        //TODO: 這邊的處理, 應該交由其它 Manager 處理而不是交給 UI

        DollManager dm = BattleSystem.GetPC().GetDollManager();
        PlayerData pData = GameSystem.GetPlayerData();
        List<Doll> dList = dm.GetDolls();
        foreach ( Doll d in dList)
        {
            //TODO: 消失特效
            Destroy(d.gameObject);
            pData.AddDollToBackpack(d.ID);
        }
        pData.RemoveAllUsingDolls();

        ResetAllButtons();
    }

    //TODO: 應該可以直接和 CreateButtons 整合
    protected void ResetAllButtons()
    {
        ClearButtons();
        CreateButtons();
    }

}
