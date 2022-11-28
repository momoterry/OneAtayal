using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle_HUD : MonoBehaviour
{
    // Start is called before the first frame update

    // HP / MP
    public Slider hpBar;
    public Slider mpBar;
    public Text hpText;
    public Text mpText;

    // LV / EXP
    public Text LVText;
    public Slider expBar;

    // Doll 資訊
    public Text DollNumText;
    public Text DollMaxText;

    //關卡資訊
    public Text levelText;

    //失敗畫面
    public GameObject failMenu;

    //血瓶
    public Text PotionNumText;
    public Text PotionMaxText;

    //技能
    public Text AttackText;
    public SkillButton autoAttackButton;
    public SkillButton[] SkillButtons;

    //錢
    public Text moneyText;

    //Dialogue
    public Dialogue theDialogue;

    //虛擬搖桿相關
    public VPad theVPad;

    protected int currMoney = -1;
    protected int currLV = -1;
    protected float currExpRatio = -1.0f;
    protected int currDollNum = -1;
    protected int currDollMax = -1;

    //調整解析度相關
    protected int currSceenWidth = 0;
    protected int currSceenHeight = 0;

    protected void CheckScreenResolution()
    {
        int width = Camera.main.pixelWidth;
        int height = Camera.main.pixelHeight;
        if (width != currSceenWidth || height != currSceenHeight)
        {
            currSceenWidth = width;
            currSceenHeight = height;
            if (theVPad)
            {
                theVPad.OnScreenResolution(currSceenWidth, currSceenHeight);
            }
        }

    }

    void Start()
    {
        CheckScreenResolution();

        failMenu.SetActive(false);
        if (hpBar)
            hpBar.value = 1.0f;
//#if TOUCH_MOVE
        if (!GameSystem.IsUseVpad() && theVPad)
        {
            theVPad.vCenter.gameObject.SetActive(false);
            theVPad.vStick.gameObject.SetActive(false);
            theVPad.gameObject.SetActive(false);
        }
//#endif
    }

    // Update is called once per frame
    void Update()
    {
        //處理鍵盤
        //if (Input.GetKeyDown("1"))
        //{
        //    OnButtonPotion();
        //}

        PlayerData pData = GameSystem.GetPlayerData();
        //print(pData);
        if (pData)
        {
            CharacterStat mData = pData.GetMainChracterData();
            if (LVText)
            {
                if (currLV != mData.LV)
                {
                    currLV = mData.LV;
                    LVText.text = currLV.ToString();
                }
            }
            if (expBar)
            {
                float ratio = (float)mData.Exp / (float)mData.ExpMax;
                if (currExpRatio != ratio)
                {
                    currExpRatio = ratio;
                    expBar.value = currExpRatio;
                }
            }
            if (moneyText)
            {
                int money = pData.GetMoney();
                if (money != currMoney)
                {
                    moneyText.text = money.ToString();
                    currMoney = money;
                }
            }
            if (DollNumText)
            {
                int num = pData.GetCurrDollNum();
                if (num != currDollNum)
                {
                    DollNumText.text = num.ToString();
                    currDollNum = num;
                }
            }
            if (DollMaxText)
            {
                int num = pData.GetMaxDollNum();
                if (num != currDollMax)
                {
                    DollMaxText.text = num.ToString();
                    currDollMax = num;
                }
            }
        }

        //處理解析度變化
        CheckScreenResolution();
    }

    //public void SetSkillIcon(Sprite sprite, int index)
    //{
    //    if (index < 0 || index >= SkillIcons.Length)
    //        return;

    //    //print("SetSkillIcon " + index + "   " + sprite);

    //    if (SkillIcons[index])
    //    {
    //        SkillIcons[index].sprite = sprite;
    //    }
    //}
    public SkillButton GetAutoAttackButton()
    {
        return autoAttackButton;
    }

    public SkillButton GetSkillButton(int index)
    {
        if (index < 0 || index >= SkillButtons.Length)
            return null;

        return SkillButtons[index];
    }

    public void SetPlayerInfo( float hp, float maxHp, float mp, float maxMP, float Attack)
    {
        if (hpBar)
            hpBar.value = hp / maxHp;
        if (mpBar)
            mpBar.value = mp / maxMP;

        if (hpText)
            hpText.text = hp.ToString("F0") + " / " + maxHp.ToString("F0");
        if (mpText)
            mpText.text = mp.ToString("F0") + " / " + maxMP.ToString("F0");

        if (AttackText)
            AttackText.text = "ATK: " + Attack.ToString("F1");

        //Money !! TODO: 移到正確的地方
    }

    public void SetLevelText(string text)
    {
        levelText.text = text;
    }

    public void OnStartFailUI()
    {
        failMenu.SetActive(true) ;
    }

    public void OnFailRetry()
    {
        //print("再來一次 !!!");
        failMenu.SetActive(false);
        BattleSystem.GetInstance().OnLevelRestart();
    }

    public void OnFailBackToStartMenu()
    {
        //print("想回主選單呀，關不掉呀");
        failMenu.SetActive(false);
        BattleSystem.GetInstance().OnBackToStartMenu();
    }

    public void OnBackToScene()
    {
        //print("回指定關卡");
        failMenu.SetActive(false);
        BattleSystem.GetInstance().OnBackPrevScene();
    }

    public void SetPotionNum( int num, int maxNum)
    {
        if (PotionNumText)
            PotionNumText.text = "x " + num.ToString();
        if (PotionMaxText)
            PotionMaxText.gameObject.SetActive(num == maxNum);
    }

    public void OnButtonPotion()
    {
        BattleSystem.GetInstance().OnUsePotion();
    }
}
