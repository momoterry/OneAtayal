using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelItemInfo
{
    public string scene;
    public string name;
    public string desc;
}

public class LevelSelectMenu : MonoBehaviour
{
    public GameObject LevelMenuItemRef;
    public Transform LevelMenuRoot;

    protected List<GameObject> itemList = new List<GameObject>();
    protected LevelItemInfo[] allLevelInfos;


    // Start is called before the first frame update
    void Start()
    {
        //LevelMenuRoot.gameObject.SetActive(true);
        //CreateLevelMenuItems();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenMenu(LevelItemInfo[] levelInfos)
    {
        LevelMenuRoot.gameObject.SetActive(true);
        ClearLevelMenuItems();

        allLevelInfos = levelInfos;
        CreateLevelMenuItems();
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        ClearLevelMenuItems();
        LevelMenuRoot.gameObject.SetActive(false);
        BattleSystem.GetPC().ForceStop(false);
    }

    protected void ClearLevelMenuItems()
    {
        foreach (GameObject item in itemList)
        {
            Destroy(item);
        }
        itemList.Clear();
    }

    protected void CreateLevelMenuItems()
    {
        for (int i=0; i< allLevelInfos.Length; i++)
        {
            GameObject itemObj = Instantiate(LevelMenuItemRef, LevelMenuRoot);
            itemObj.SetActive(true);
            RectTransform rt = itemObj.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(8.0f, -28.0f - (36.0f * i));
            }
            itemList.Add(itemObj);

            LevelMenuItem item = itemObj.GetComponent<LevelMenuItem>();
            item.InitInfo(allLevelInfos[i], this);
        }
    }
}
