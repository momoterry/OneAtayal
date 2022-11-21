using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuItem : MonoBehaviour
{
    public Text nameText;
    public Text descText;

    protected LevelItemInfo myInfo;
    protected LevelSelectMenu myMenu;

    public LevelItemInfo GetInfo() { return myInfo; }

    public void InitInfo(LevelItemInfo info, LevelSelectMenu theMenu)
    {
        myInfo = info;
        myMenu = theMenu;

        if (nameText)
        {
            nameText.text = info.name;
        }
        if (descText)
        {
            descText.text = info.desc;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonDown()
    {
        myMenu.OnLevelItemDown(this);
    }


}
