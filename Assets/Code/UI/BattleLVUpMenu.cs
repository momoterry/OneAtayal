using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BattleLVUpMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject menuRoot;
    public GameObject menuEntryButton;

    protected bool isMenuOn = false;

    void Start()
    {
        //CheckBattlePoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnToggleBattleLveUpMenu()
    {
        isMenuOn = !isMenuOn;
        menuRoot.SetActive(isMenuOn);
    }

    public void OnSetBattlePoint(int battlePoints)
    {
        bool entryOn = false;
        //if (BattleSystem.GetInstance().IsBattleLevelUp)
        {
            entryOn = battlePoints > 0 ? true : false;
            if (!entryOn)
            {
                isMenuOn = false;
                menuRoot.SetActive(false);
            }
        }
        menuEntryButton.SetActive(entryOn);
    }

    protected void CheckBattlePoint()
    {

    }

}
