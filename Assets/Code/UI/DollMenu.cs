using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DollMenu : MonoBehaviour
{
    public GameObject buttonRef;
    public Transform buttonRoot;
    public RectTransform DissmissButtonRT;

    protected Dictionary<string, ButtonDollBackpack> buttonMap = new Dictionary<string, ButtonDollBackpack>();

    // Start is called before the first frame update
    void Start()
    {
        if (buttonRoot)
        {
            buttonRoot.gameObject.SetActive(false);
        }
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

    public void OpenMenu()
    {
        if (buttonRoot)
        {
            buttonRoot.gameObject.SetActive(true);
            ClearButtons(); //�H�K���_�}�ҳy�����D
            CreateButtons();
        }
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        ClearButtons();
        if (buttonRoot)
        {
            buttonRoot.gameObject.SetActive(false);
        }
        BattleSystem.GetPC().ForceStop(false);
    }

 

    protected void CreateButtons()
    {
        PlayerData pData = GameSystem.GetPlayerData();
        Dictionary<string, int> backPackInfo = pData.GetDollBackPack();

        int i = 0;
        int ih = 0;
        int width = 4;

        foreach (KeyValuePair<string, int> k in backPackInfo)
        {
            GameObject bo = Instantiate(buttonRef, buttonRoot.transform);
            RectTransform rt = bo.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(-54.0f + (36 * i), 92.0f - (36 * ih));
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
            if (i >= width)
            {
                i = 0;
                ih++;
            }
        }

        if (DissmissButtonRT)
        {
            DissmissButtonRT.anchoredPosition = new Vector2(-56.0f + (36 * (width-1)), 92.0f - (36 * 2));
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
        //TODO: �o�䪺�B�z, ���ӥ�Ѩ䥦 Manager �B�z�Ӥ��O�浹 UI

        DollManager dm = BattleSystem.GetPC().GetDollManager();
        PlayerData pData = GameSystem.GetPlayerData();
        List<Doll> dList = dm.GetDolls();
        foreach (Doll d in dList)
        {
            //TODO: �����S��
            Destroy(d.gameObject);
            pData.AddDollToBackpack(d.ID);
        }
        pData.RemoveAllUsingDolls();

        ResetAllButtons();
    }

    //TODO: ���ӥi�H�����M CreateButtons ��X
    protected void ResetAllButtons()
    {
        ClearButtons();
        CreateButtons();
    }

}
