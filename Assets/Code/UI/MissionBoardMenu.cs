using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionBoardMenu : MonoBehaviour
{
    public Text hintText;
    public Transform MenuRoot;
    public MissionCard currMissionCard;
    public GameObject itemRef;

    public Color[] typeColors;

    public GameObject MissionTitle;
    public Text MissionTitleText;

    protected List<MissionData> missions;
    protected List<MissionItem> items = new List<MissionItem>();
    public void OpenMenu(List<MissionData> missionList)
    {
        missions = missionList;
        MenuRoot.gameObject.SetActive(true);
        BattleSystem.GetPC().ForceStop(true);

        MissionData currMission = MissionManager.GetCurrMission();
        if (currMission != null)
        {
            hintText.text = "現在承接中的任務";
            currMissionCard.gameObject.SetActive(true);
            currMissionCard.InitValue(currMission, this);
        }
        else
        {
            hintText.text = "選擇要前往的任務";
            currMissionCard.gameObject.SetActive(false);
            CreateItems();
        }

    }

    public void CloseMenu()
    {
        ClearItems();
        MenuRoot.gameObject.SetActive(false);
        currMissionCard.gameObject.SetActive(false);
        BattleSystem.GetPC().ForceStop(false);
    }

    public void OnItemClicked(MissionItem item, MissionData mission)
    {
        print("接下任務喔");
        //MissionManager.StartMission(mission);
        MissionManager.AcceptMission(mission);
        CloseMenu();

        if (MissionTitleText)
        {
            MissionTitleText.color = typeColors[(int)mission.type];
            MissionTitleText.text = mission.Title;
        }
        if (MissionTitle)
            MissionTitle.SetActive(true);
    }

    public void OnCancelCurrMission()
    {
        MissionManager.CancelCurrMission();
        if (MissionTitle)
            MissionTitle.SetActive(false);
        CloseMenu();
    }


    protected void CreateItems()
    {
        RectTransform rrt = itemRef.GetComponent<RectTransform>();
        float x = rrt.anchoredPosition.x;
        float y = rrt.anchoredPosition.y;
        float yStep = rrt.rect.height + 4.0f;
        foreach (MissionData data in missions)
        {
            GameObject o = Instantiate(itemRef, MenuRoot);
            RectTransform rt = o.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(x, y);
            }
            o.SetActive(true);

            MissionItem mItem = o.GetComponent<MissionItem>();
            mItem.InitValue(data, this);
            items.Add(mItem);

            y -= yStep;
        }
    }

    protected void ClearItems()
    {
        foreach (MissionItem item in items) 
        {
            Destroy(item.gameObject);
        }
        items.Clear();
    }

}
