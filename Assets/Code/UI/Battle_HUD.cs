using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle_HUD : MonoBehaviour
{
    // Start is called before the first frame update

    // LV / EXP
    public GameObject BasicLevelRoot;
    public Text LVText;
    public Slider expBar;

    //戰鬥升級
    public GameObject BattleLevelUpRoot;
    public Text BattleLVText;
    public Slider BattleExpBar;
    public GameObject BattlePointRoot;
    public Text BattlePointNum;
    public BattleLVUpMenu bLVMenu;


    // Doll 資訊
    public Text DollNumText;
    public Text DollMaxText;

    //關卡資訊
    public GameObject levelRoot;    //關卡名稱的 Root，用來開關
    public Text levelText;

    //Mini Map
    public MiniMap miniMap;

    //勝敗畫面
    public GameObject winMenu;
    public GameObject failMenu;

    //技能
    public Text AttackText;
    public SkillButton autoAttackButton;
    public SkillButton[] SkillButtons;

    //Doll 技能
    public DollSkillButton[] DollSkillButtons;

    //錢
    public Text moneyText;

    //Dialogue
    public Dialogue theDialogue;

    //虛擬搖桿相關
    public VPad theVPad;

    //雙搖桿情況
    public VPad theLeftPad;
    public VPad theRightPad;
    public CrossPadControl crossPadControl;

    //陣型編輯相關
    public DollLayoutUIBase[] dLayoutUIs;
    protected DollLayoutUIBase currDollLayout;

    protected int currMoney = -1;
    protected int currLV = -1;
    protected float currExpRatio = -1.0f;
    protected int currDollNum = -1;
    protected int currDollMax = -1;

    //調整解析度相關
    protected int currSceenWidth = 0;
    protected int currSceenHeight = 0;

    protected float targetRatio = 0.5f;
    protected float ratioMax = 1.0f;    //螢幕太寬就進行 Camera + UI 縮放
    protected float cameraDefaultSize = 10.0f;
    protected CanvasScaler theScaler;

    //勝敗頁面相關
    protected System.Action winMenuCB;




    private void Awake()
    {
        cameraDefaultSize = Camera.main.orthographicSize;
        //print("Main Camera Default Size = " + cameraDefaultSize);
        theScaler = GetComponent<CanvasScaler>();
        if (theScaler != null)
        {
            targetRatio = theScaler.referenceResolution.x / theScaler.referenceResolution.y;
        }
    }

    protected void CheckScreenResolution()
    {
        int width = Camera.main.pixelWidth;
        int height = Camera.main.pixelHeight;
        if (width != currSceenWidth || height != currSceenHeight)
        {
            currSceenWidth = width;
            currSceenHeight = height;

            //CanvasScaler theScaler = GetComponent<CanvasScaler>();
            if (theScaler != null)
            {
                float currRatio = (float)currSceenWidth / (float)currSceenHeight;
                if (currRatio < targetRatio)
                {
                    theScaler.matchWidthOrHeight = 0;
                    Camera.main.orthographicSize = cameraDefaultSize * targetRatio / currRatio; //太細的螢幕得調整主 Camera
                }
                else if (currRatio > ratioMax)
                {
                    //float uiScaleRatio = 0.5f;    //縮放速度的調整
                    //float cameraScaleRatio = 0.75f;
                    //float matchWidthRatio = 16.0f / 9.0f;
                    //float maxWidthRatio = 16.0f / 9.0f;
                    //float calRatio = Mathf.Min(currRatio, maxWidthRatio);           //最大縮放到 16/9

                    //float uiRatio = (calRatio - ratioMax) * uiScaleRatio + ratioMax;
                    //float cameraRatio = (calRatio - ratioMax) * cameraScaleRatio + ratioMax;

                    //float matchRatio = Mathf.Min(1.0f, (uiRatio - ratioMax) / (matchWidthRatio - ratioMax));
                    //theScaler.matchWidthOrHeight = (1.0f - matchRatio);
                    //print("Match Ratio: " + matchRatio);
                    //Camera.main.orthographicSize = cameraDefaultSize * ratioMax / cameraRatio; //太寬的螢幕得調整主 Camera
                }
                else
                {
                    theScaler.matchWidthOrHeight = 1.0f;
                    Camera.main.orthographicSize = cameraDefaultSize;
                }
            }

            if (theVPad)
            {
                theVPad.OnScreenResolution(currSceenWidth, currSceenHeight);
            }
            if (theRightPad)
            {
                theRightPad.OnScreenResolution(currSceenWidth, currSceenHeight);
            }
        }

    }

    void Start()
    {
        winMenu.SetActive(false);
        failMenu.SetActive(false);
        //if (hpBar)
        //    hpBar.value = 1.0f;

        bool isVPad = GameSystem.IsUseVpad();
        bool isDual = isVPad && GameSystem.IsUseDirectionControl();

        //isDual = false;

        if (isDual)
        {
            if (theVPad)
            {
                theVPad.gameObject.SetActive(false);
                theVPad.vCenter.gameObject.SetActive(false);
                theVPad.vStick.gameObject.SetActive(false);
            }
            if (theLeftPad)
            {
                theLeftPad.vCenter.gameObject.SetActive(isDual);
                theLeftPad.vStick.gameObject.SetActive(isDual);
                theLeftPad.gameObject.SetActive(isDual);
            }
            if (theRightPad)
            {
                theRightPad.vCenter.gameObject.SetActive(isDual);
                theRightPad.vStick.gameObject.SetActive(isDual);
                theRightPad.gameObject.SetActive(isDual);
            }
            theVPad = theLeftPad;
        }

        if (crossPadControl)
        {
            crossPadControl.gameObject.SetActive(isDual);
        }

        if (theVPad)
        {


            theVPad.vCenter.gameObject.SetActive(isVPad);
            theVPad.vStick.gameObject.SetActive(isVPad);
            theVPad.gameObject.SetActive(isVPad);

        }

        if (BattleLevelUpRoot)
            BattleLevelUpRoot.SetActive(BattleSystem.GetInstance().IsBattleLevelUp);
        if (BasicLevelRoot)
        {
            BasicLevelRoot.SetActive(GameSystem.GetInstance().showBasicLevelUp && (!BattleSystem.GetInstance().IsBattleLevelUp));
        }

        CheckScreenResolution();

    }

    // Update is called once per frame
    void Update()
    {

        PlayerData pData = GameSystem.GetPlayerData();
        //print(pData);
        if (pData)
        {
            CharacterStat mData = pData.GetMainChracterData();
            if (!BattleSystem.GetInstance().IsBattleLevelUp)
            {
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

            if (moneyText)
            {
                int money = pData.GetMoney();
                if (money != currMoney)
                {
                    moneyText.text = money.ToString();
                    currMoney = money;
                }
            }
        }

        if (BattleSystem.GetInstance().IsBattleLevelUp)
        {
            //TODO: 不用每個 Frame 做
            bool isMax = BattlePlayerData.GetInstance().GetBattleLevel() == BattlePlayerData.GetInstance().GetMaxBattleLevel();
            if (BattleLVText)
            {
                //if (currLV != mData.LV)
                //{
                //    currLV = mData.LV;
                //    LVText.text = currLV.ToString();
                //}
                BattleLVText.text = isMax ? "Max" : BattlePlayerData.GetInstance().GetBattleLevel().ToString();
            }
            if (BattleExpBar)
            {
                float ratio = isMax? 1.0f : (float)BattlePlayerData.GetInstance().GetBattleExp() / (float)BattlePlayerData.GetInstance().GetBattleExpMax();
                if (currExpRatio != ratio)
                {
                    currExpRatio = ratio;
                    BattleExpBar.value = currExpRatio;
                }
            }
            if (BattlePointRoot)
            {
                int bp = BattlePlayerData.GetInstance().GetBattleLVPoints();
                
                if (bp > 0)
                {
                    BattlePointRoot.SetActive(true);
                    if (BattlePointNum)
                        BattlePointNum.text = bp.ToString();
                }
                else
                {
                    BattlePointRoot.SetActive(false);
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

    public DollSkillButton GetDollSkillButton(int index)
    {
        if (index < 0 || index >= DollSkillButtons.Length)
            return null;

        return DollSkillButtons[index];
    }

    //public void SetPlayerInfo( float hp, float maxHp, float mp, float maxMP, float Attack)
    //{
    //    if (hpBar)
    //        hpBar.value = hp / maxHp;
    //    if (mpBar)
    //        mpBar.value = mp / maxMP;

    //    if (hpText)
    //        hpText.text = hp.ToString("F0") + " / " + maxHp.ToString("F0");
    //    if (mpText)
    //        mpText.text = mp.ToString("F0") + " / " + maxMP.ToString("F0");

    //    if (AttackText)
    //        AttackText.text = "ATK: " + Attack.ToString("F1");

    //    //Money !! TODO: 移到正確的地方
    //}

    public void SetLevelText(string text)
    {
        if (levelText)
            levelText.text = text;
    }

    public void EnableLevelText(bool enable)
    {
        if (levelRoot)
            levelRoot.SetActive(enable);
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

    public void OnOpenWinMenu(System.Action cb)
    {
        if (winMenu)
        {
            winMenu.SetActive(true);
        }
        winMenuCB = cb;
    }

    public void OnCloseWinMenu()
    {
        if (winMenu)
        {
            winMenu.SetActive(false);
        }
        //BattleSystem.GetInstance().OnBackPrevScene();
        if (winMenuCB != null)
        {
            winMenuCB();
        }

    }

    public void OnBackToScene()
    {
        //print("回指定關卡");
        failMenu.SetActive(false);
        BattleSystem.GetInstance().OnBackPrevScene();
    }

    //陣型編輯相關 ===============================
    public void RegisterDollLayoutUI( DollManager dm )
    {
        //TODO: 根據 ID 來找出對應的 Menu
        currDollLayout = dLayoutUIs[0];
        currDollLayout.SetupDollManager(dm);
    }

    public void OnOnOffDollLayoutUI()
    {
        if (currDollLayout)
        {

            if (currDollLayout.IsMenuActive())
            {
                currDollLayout.CloseMenu();
            }
            else
            {
                currDollLayout.OpenMenu();
            }
        }
    }

    public void OnButtonPotion()
    {
        BattleSystem.GetInstance().OnUsePotion();
    }



}
