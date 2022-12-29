using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelItemInfo
{
    public string ID;
    public string scene;
    public string name;
    public string desc;
}

public class LevelSelectMenu : MonoBehaviour
{
    public GameObject LevelMenuItemRef;
    public Transform LevelMenuRoot;
    public Image FadeBlockImage;

    protected List<GameObject> itemList = new List<GameObject>();
    protected LevelItemInfo[] allLevelInfos;

    protected LevelItemInfo levelToGo;

    protected float fadeTime = 0.5f;
    protected float timeToFade = 0;

    // Start is called before the first frame update
    void Start()
    {
        //LevelMenuRoot.gameObject.SetActive(true);
        //CreateLevelMenuItems();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToFade > 0)
        {
            timeToFade -= Time.deltaTime;
            if (timeToFade <= 0)
            {
                timeToFade = 0;
                DoGoToLevel();
            }

            if (FadeBlockImage)
            {
                FadeBlockImage.color = new Color(0, 0, 0, 1.0f - (timeToFade / fadeTime));
            }
        }
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


    public void OnLevelItemDown( LevelMenuItem item)
    {
        //ClearLevelMenuItems();
        //LevelMenuRoot.gameObject.SetActive(false);
        levelToGo = item.GetInfo();

        timeToFade = fadeTime;
        if (FadeBlockImage)
            FadeBlockImage.enabled = true;
    }

    protected void DoGoToLevel()
    {
        BattleSystem.GetInstance().OnGotoScene(levelToGo.scene);
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
        int currLine = 0;
        for (int i=0; i< allLevelInfos.Length; i++)
        {
            if (allLevelInfos[i] == null)
                continue;
            if (!GameSystem.GetLevelManager().IsLevelOpen(allLevelInfos[i].ID))
            {
                //print("關卡還沒開放 .. " + allLevelInfos[i].ID);
                continue;
            }

            GameObject itemObj = Instantiate(LevelMenuItemRef, LevelMenuRoot);
            itemObj.SetActive(true);
            RectTransform rt = itemObj.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(8.0f, -28.0f - (36.0f * currLine));
            }
            itemList.Add(itemObj);

            LevelMenuItem item = itemObj.GetComponent<LevelMenuItem>();
            item.InitInfo(allLevelInfos[i], this);
            currLine++;
        }
    }

}
