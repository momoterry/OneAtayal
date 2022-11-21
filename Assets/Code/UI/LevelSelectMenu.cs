using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenu : MonoBehaviour
{
    public GameObject LevelMenuItemRef;
    public Transform LevelMenuRoot;

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

    public void OpenMenu()
    {
        LevelMenuRoot.gameObject.SetActive(true);
        //CreateLevelMenuItems();
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        //ClearAllItems();
        LevelMenuRoot.gameObject.SetActive(false);
        BattleSystem.GetPC().ForceStop(false);
    }

    protected void ClearLevelMenuItems()
    {

    }

    protected void CreateLevelMenuItems()
    {
        for (int i=0; i<4; i++)
        {
            GameObject itemObj = Instantiate(LevelMenuItemRef, LevelMenuRoot);
            itemObj.SetActive(true);
            RectTransform rt = itemObj.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(8.0f, -28.0f - (36.0f * i));
            }
        }
    }
}
