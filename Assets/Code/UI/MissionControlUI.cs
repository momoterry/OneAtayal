using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class MissionControlUI : MonoBehaviour
{
    public Text titleText;
    public Text msgText;
    public Text countText;
    public GameObject MissionMessageBox;
    public GameObject MissionCoompleteWindowRoot;

    public GameObject MissionRewardItemRef;

    float missionMessageBoxTimeLeft = 0;
    //float missionCompleteWindowTimeLeft = 0;

    public class MissionRewardData
    {
        public string itemID;
        public int num;
    }

    public void ShowObjectiveDoneMessage( string title, string objective, int objDone, int objTotal)
    {
        countText.text = objDone.ToString() + " / " + objTotal.ToString();
        msgText.text = objective;
        titleText.text = title;
        MissionMessageBox.SetActive(true);
        missionMessageBoxTimeLeft = 4.0f;
    }

    public void ShowMissionCompleteWindow( MissionData _mission, MissionManager.MissionRewardResult rewardResult = null)
    {
        if (rewardResult != null)
        {
            CreateMissionCompleteRewardItems(rewardResult);
        }
        MissionCoompleteWindowRoot.SetActive(true);
        //missionCompleteWindowTimeLeft = 4.0f;
    }


    void Update()
    {
        if (missionMessageBoxTimeLeft > 0)
        {
            missionMessageBoxTimeLeft -= Time.deltaTime;
            if (missionMessageBoxTimeLeft < 0)
            {
                MissionMessageBox.SetActive(false);
            }
        }
        //if (missionCompleteWindowTimeLeft > 0)
        //{
        //    missionCompleteWindowTimeLeft -= Time.deltaTime;
        //    if (missionCompleteWindowTimeLeft < 0)
        //    {
        //        //MissionCoompleteWindowRoot.SetActive(false);
        //        CloseMissionCompleteMenu();
        //    }
        //}
    }

    protected List<MissionRewardItem> rewardItemList = new();
    protected void CreateMissionCompleteRewardItems(MissionManager.MissionRewardResult rewardResult)
    {
        Transform itemRoot = MissionRewardItemRef.gameObject.transform.parent;
        RectTransform rrt = MissionRewardItemRef.GetComponent<RectTransform>();
        float x = rrt.anchoredPosition.x;
        float y = rrt.anchoredPosition.y;
        float yStep = rrt.rect.height + 4.0f;
        for (int i=0; i< rewardResult.itemList.Count + 1; i++)
        {
            GameObject o = Instantiate(MissionRewardItemRef, itemRoot);
            RectTransform rt = o.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(x, y);
            }
            o.SetActive(true);

            MissionRewardItem rewardItem = o.GetComponent<MissionRewardItem>();
            if (i < rewardResult.itemList.Count)
                rewardItem.InitValue(rewardResult.itemList[i].itemID, rewardResult.itemList[i].num);
            else
            {

                //錢的部份
                rewardItem.InitValue(null, rewardResult.Money);
            }
            rewardItemList.Add(rewardItem);

            y -= yStep;
        }

    }

    public void CloseMissionCompleteMenu()
    {
        foreach (MissionRewardItem item in rewardItemList)
        {
            Destroy(item.gameObject);
        }
        rewardItemList.Clear();
        MissionCoompleteWindowRoot.SetActive(false);
    }

}
