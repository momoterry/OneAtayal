using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DollBackpackUI: MonoBehaviour
{
    //public calss DollItemIcon
    //{
    //    public  string dollID;
    //    public 
    //}

    public GameObject buttonRef;

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
                //CreateButtons();
                testTime = -1.0f;
            }
        }
    }

    void CreateButtons()
    {
        PlayerData pData = GameSystem.GetPlayerData();

        //==Debug
        pData.AddDollToBackpack("DollOne");
        pData.AddDollToBackpack("DollOne");
        pData.AddDollToBackpack("DollOne");
        pData.AddDollToBackpack("DollFire");
        pData.AddDollToBackpack("DollStone");
        pData.AddDollToBackpack("DollStone");
        //==

        Dictionary<string, int> backPackInfo = pData.GetDollBackPack();

        int i = 0;
        foreach ( KeyValuePair<string, int> k in backPackInfo)
        {
            GameObject bo = Instantiate(buttonRef, transform);
            RectTransform rt = bo.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(56.0f, 92.0f - (36 * i));
            }

            ButtonDollBackpack bDoll = bo.GetComponent<ButtonDollBackpack>();
            if (bDoll)
            {
                GameObject dObj = pData.GetDollRefByID(k.Key);
                Doll d = dObj.GetComponent<Doll>();
                bDoll.SetNum(k.Value);
                bDoll.SetDollInfo(k.Key, d.icon);
            }

            i++;
        }


        //for (int i=0; i<4; i++)
        //{
        //    GameObject bo = Instantiate(buttonRef, transform);
        //    RectTransform rt = bo.GetComponent<RectTransform>();
        //    if (rt)
        //    {
        //        rt.anchoredPosition = new Vector2(56.0f, 92.0f -( 36 * i));
        //    }
        //}

    }

}
