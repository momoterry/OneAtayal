using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class MissionBoardMenu : MonoBehaviour
{
    public Transform MenuRoot;
    public GameObject itemRef;

    protected List<MissionData> missions;
    protected List<MissionItem> items = new List<MissionItem>();
    public void OpenMenu(List<MissionData> missionList)
    {
        missions = missionList;
        CreateItems();
        MenuRoot.gameObject.SetActive(true);
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        ClearItems();
        MenuRoot.gameObject.SetActive(false);
        BattleSystem.GetPC().ForceStop(false);
    }

    public void OnItemClicked(MissionItem item, MissionData mission)
    {
        //print("¥h§a¥h§a");
        MissionManager.StartMission(mission);
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
