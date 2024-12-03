using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionBoardMenu : MonoBehaviour
{
    public Transform MenuRoot;
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
        if (currMission == null)
            CreateItems();

    }

    public void CloseMenu()
    {
        ClearItems();
        MenuRoot.gameObject.SetActive(false);
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
