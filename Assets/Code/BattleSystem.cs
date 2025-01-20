using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class BattleSystem : MonoBehaviour
{
    public Battle_HUD theBattleHUD;
    public string backScene = "StartMenu";
    public string backEntrance = "";
    public string levelID = "";  //如果有指定的話，就是屬於 LevelManager 中的指定 ID
    //public VPad theVPad;

    public MapGeneratorBase theMG;
    public Transform initPlayerPos;
    public float initPlayerDirAngle = 0;
    public GameObject playerRef;
    public bool ForceUsePlayerRef = false;

    public bool IsBattleLevelUp = false;

    protected GameObject thePlayer;   //TODO Player Character Spawn 較晚，但 PC 應常駐
    protected List<GameObject> enemyList = new List<GameObject>();
    protected List<GameObject> objList = new List<GameObject>();

    protected PlayerControllerBase thePC;

    public int MaxLevel = 5;
    protected int currLevel = 1;

    //Input 相關
    protected int touchLayer;
    protected List<GameObject> touchDownTracing = new List<GameObject>();

    //血瓶
    public int initPotion = 3;
    public int maxPotion = 5;
    public float potionHealRatio = 0.5f;
    protected int currPotion = 3;

    //整體傷害平衡用
    protected float AllEnemyDamageRate = 0.5f;
    protected float AllFriendlyDamageRate = 0.5f;
    static public float GetAllEnemyDamageRate() { return instance.AllEnemyDamageRate; }
    static public float GetAllFriendlyDamageRate() { return instance.AllFriendlyDamageRate; }
    static public void SetAllEnemyDamageRate(float _AllEnemyDamageRate) { instance.AllEnemyDamageRate = _AllEnemyDamageRate; }
    static public void SetAllFriendlyDamageRate(float _AllFriendlyDamageRate) { instance.AllFriendlyDamageRate = _AllFriendlyDamageRate; }


    //Doll Skill 相關
    protected DollSkillManager theDollSkillManager;

    protected enum BATTLE_GAME_STATE
    {
        NONE,
        INIT,
        BATTLE,
        LOCAL_WIN,
        FAIL,
    }
    protected BATTLE_GAME_STATE currState = BATTLE_GAME_STATE.NONE;
    protected BATTLE_GAME_STATE nextState = BATTLE_GAME_STATE.NONE;
    protected float stateTime = 0;

    public int GetEnemyCount() { return enemyList.Count; }

    public delegate void BattleSystemAwakeCB(BattleSystem bs);
    static protected List<BattleSystemAwakeCB> awakeCBs = new List<BattleSystemAwakeCB>();
    static public void RegisterAwakeCallBack(BattleSystemAwakeCB _cb) { awakeCBs.Add(_cb); }

    protected static BattleSystem instance = null;
    public static BattleSystem GetInstance() { return instance; }

    //Fade In/Out 相關
    public FadeBlocker fadeBlocker;

    public void StartFadeOut(float fadeTime, FadeBlockerDelegate _cb = null)
    {
        if (fadeBlocker)
            fadeBlocker.StartFadeOut(fadeTime, _cb);
    }

    public void StartFadeIn(float fadeTime, FadeBlockerDelegate _cb = null)
    {
        if (fadeBlocker)
            fadeBlocker.StartFadeIn(fadeTime, _cb);
    }

    protected void Awake()
    {
        //print("BattleSystem Awake!!");
        StartFadeIn(0.25f);
        GameSystem.Ensure();    //為了讓任何 Scene 都可以直接 Play !!

        if (instance != null)
            print("ERROR !! 超過一份 BattleSystem 存在: ");
        instance = this;

        foreach (BattleSystemAwakeCB cb in awakeCBs)
        {
            cb(this);
        }
        awakeCBs.Clear();

        touchLayer = LayerMask.GetMask("TouchPlane", "UI");
        theDollSkillManager = GetComponent<DollSkillManager>();
    }

    public GameObject GetPlayer() { return thePlayer; }

    public PlayerControllerBase GetPlayerController() { return thePC; }
    public static PlayerControllerBase GetPC() { return instance.thePC; }
    public static Dialogue GetDialogue() { return instance.theBattleHUD.theDialogue; }
    public static Battle_HUD GetHUD() { return instance.theBattleHUD; }
    public static DollSkillManager GetDollSkillManager() { return instance.theDollSkillManager; }
    public VPad GetVPad() { return theBattleHUD.theVPad; }
    public VPad GetDirVPad() { return theBattleHUD.theRightPad; }

    public MapGeneratorBase GetMapGenerator() { return theMG; }

    public int GetCurrLevel() { return currLevel; }

    public bool IsDuringBattle() { return currState == BATTLE_GAME_STATE.BATTLE; }

    public void AddEnemy(GameObject enemyObj)
    {
        enemyList.Add(enemyObj);
    }

    public void OnEnemyKilled(GameObject enemyObj)
    {
        //TODO: 是否把 enemyList 改成 Enemy 而不是 GameObject ?

        if (enemyList.Remove(enemyObj))
        {
            Enemy e = enemyObj.GetComponent<Enemy>();
            if (e)
            {
                thePC.OnKillEnemy(e);
                BattlePlayerData.GetInstance().OnKillEnemy(e);
            }

            //if (GetEnemyCount()==0)
            //{
            //    OnEnemyClear();
            //}      
        }
        else
        {
            print("ERROR !! 不應該的重復擊殺!!: " + enemyObj);
        }
    }

    public void OnBSObjectDestroy(GameObject obj)
    {
        objList.Remove(obj);
    }

    public GameObject SpawnGameplayObject(GameObject objRef, Vector3 pos, bool clearByBS = true)
    {
#if XZ_PLAN
        Quaternion qm = Quaternion.Euler(90, 0, 0);
#else
        Quaternion qm = Quaternion.identity;
#endif
        if (!objRef)
            return null;

        GameObject o = Instantiate(objRef, pos, qm, null);
        if (o && clearByBS)
        {
            objList.Add(o);
            //if (clearByBS)
            //{
            //    o.AddComponent<BSObjectTag>();
            //}
            o.AddComponent<BSObjectTag>();
        }
        o.SetActive(true);
        return o;
    }

    public static GameObject SpawnGameObj(GameObject objRef, Vector3 pos, bool clearByBS = false)
    {
        return instance.SpawnGameplayObject(objRef, pos, clearByBS);
    }

    public void SetInitPosition(Vector3 pos)       //設定遊戲開始的位置
    {
        if (Camera.main)
        {
            Camera.main.transform.position = new Vector3(pos.x, Camera.main.transform.position.y, pos.z);
        }
        if (initPlayerPos)
        {
            initPlayerPos.position = pos;
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //print("==== QualitySettings.vSyncCount: " + QualitySettings.vSyncCount);
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;

        if (!ForceUsePlayerRef)
        {
            GameObject playerInfoToSet = GameSystem.GetPlayerData().GetPlayerCharacterRef();
            if (playerInfoToSet != null)
            {
                playerRef = playerInfoToSet;
            }
            else
            {
                GameSystem.GetPlayerData().SetPlayerCharacterRef(playerRef);
            }
        }

        //GameSystem.GetInstance().SaveData();
        ContinuousBattleDataBase cB = ContinuousBattleManager.GetCurrBattleData();
        if (cB != null && cB.name != "")
        {
            theBattleHUD.EnableLevelText(true);
            theBattleHUD.SetLevelText(cB.name);
        }

        nextState = BATTLE_GAME_STATE.INIT;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        if (nextState != currState)
        {
            OnStateExit();
            OnStateEnter();
            currState = nextState;
            stateTime = 0;
            return;
        }

        stateTime += Time.deltaTime;
        switch (currState)
        {
            case BATTLE_GAME_STATE.INIT:
                UpdateInit();
                break;
            case BATTLE_GAME_STATE.BATTLE:
                UpdateInput();
                break;
            case BATTLE_GAME_STATE.FAIL:
                UpdateFail();
                break;
        }

        UpdateBattleHUD();
    }

    //Loading 後第一次初始化關卡 (非 Fail Reset)
    protected virtual void InitBattle()
    {
        //print("InitBattle");
        SetUpLevel(1);
        //InitPlayerData();           //TODO: 這裡應該改交給 SetUpLevel 去指定
    }

    protected virtual void InitBattleStatus()
    {
        //print("InitBattleData");
        //thePC.InitStatus();
        currPotion = initPotion;
    }

    protected void UpdateInit()
    {
        if (GameSystem.GetPlayerData().IsReady())
        {
            //避免出 Error 時一直跑，先設定 nextState
            nextState = BATTLE_GAME_STATE.BATTLE;
            InitBattle();
        }
        //else
        //{
        //    //TODO: 黑畫面?
        //    print("還沒好哩 ...... 要來打我嗎?");
        //}
    }


    protected virtual void UpdateBattleHUD()
    {
        //if (thePC)
        //{
        //    theBattleHUD.SetPlayerInfo(thePC.GetHP(), thePC.GetHPMax(), thePC.GetMP(), thePC.GetMPMax(), thePC.GetATTACK());
        //    theBattleHUD.SetPotionNum(currPotion, maxPotion);
        //}
    }

    protected virtual void UpdateInput()
    {
        //TODO : 避免 UI 衝突的方法有點暴力, 要改
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.currentSelectedGameObject)
        {
            //print("Mouse Down On : "+ EventSystem.current.currentSelectedGameObject);

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, touchLayer))
            {
                //Debug.Log("Object Hit is " + hitInfo.collider.gameObject.name);
                hitInfo.collider.gameObject.SendMessage("OnBattleTouchDown", hitInfo.point);
                touchDownTracing.Add(hitInfo.collider.gameObject);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            foreach (GameObject o in touchDownTracing)
            {
                if (o)
                {
                    o.SendMessage("OnBattleTouchUp", SendMessageOptions.DontRequireReceiver);
                }
            }
            touchDownTracing.Clear();
        }

    }

    protected void UpdateFail()
    {

    }

    protected virtual void OnStateEnter()
    {
        switch (nextState)
        {
            case BATTLE_GAME_STATE.BATTLE:
                break;
            case BATTLE_GAME_STATE.FAIL:
                stateTime = 3.0f;   //三秒後重開局
                StopGameplayByFail();
                theBattleHUD.OnStartFailUI();
                break;
        }
    }

    protected virtual void OnStateExit()
    {

    }

    protected virtual void ClearLevel()
    {
        foreach (GameObject enemyObj in enemyList)
        {
            Destroy(enemyObj);
            //print("Kill One !!");
        }
        enemyList.Clear();

        foreach (GameObject o in objList)
        {
            Destroy(o);
        }
        objList.Clear();

        DropItem.ClearAllDropItem();
    }

    protected virtual void StopGameplayByFail()
    {
        foreach (GameObject enemyObj in enemyList)
        {
            if (enemyObj)
                enemyObj.SendMessage("OnGameFail", SendMessageOptions.DontRequireReceiver);
        }
    }

    protected virtual void SetUpHud()
    {
        //string levelText = "LEVEL : " + currLevel;
        //theBattleHUD.SetLevelText(levelText);
    }

    protected virtual void SetUpLevel(int level = 1)
    {
        //print("SetUpLevel " + level);
        currLevel = level;
        if (level < 1)
            currLevel = 1;
        else if (level > MaxLevel)
            currLevel = MaxLevel;

        //if (clearGate)
        //    clearGate.SetActive(false);
        theMG.BuildAll(currLevel);
        theMG.PostBuildAll();

        // ================ 戰鬥資料初始化 ====================
        if (level == 1)
        {
            InitBattleStatus();
            if (thePlayer == null)
            {
#if XZ_PLAN
                Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
                Quaternion rm = Quaternion.identity;
#endif

                thePlayer = Instantiate(playerRef, initPlayerPos.position, rm, null);

                thePC = thePlayer.GetComponent<PlayerControllerBase>();
                thePC.initFaceDirAngle = initPlayerDirAngle;
                thePC.SetupFaceDirByAngle(initPlayerDirAngle);  //確保 First Frame 轉向正確
                //thePC.InitStatus(); 會在 PC 的 Start 被呼叫
            }
            else
            {
                //舊有的關卡下一關，不會再用到，應該移掉
                thePC.DoTeleport(initPlayerPos.position, initPlayerDirAngle);
                thePC.InitStatus();
            }
        }
        else
        {
            // 關卡升級
            thePC.DoTeleport(initPlayerPos.position, initPlayerDirAngle);
        }

        SetUpHud();
    }

    protected void ResetLevel(int level = 1)
    {
        print("Reset Level!!");

        ClearLevel();


        SetUpLevel(level);
    }

    public void OnPlayerKilled()
    {
        print("玩家死亡........");
        nextState = BATTLE_GAME_STATE.FAIL;
    }

    public void OnUsePotion()
    {
        if (currPotion > 0)
        {
            //在這裡叫角色補上
            if (thePC)
            {
                float heal = thePC.GetHPMax() * potionHealRatio;
                thePC.DoHeal(heal);
            }
            currPotion--;
        }
    }

    // =================== 拾取相關

    protected GameObject moneyAddFXRef;
    public void OnAddMoney(int moneyAdd)
    {
        if (!moneyAddFXRef)
            moneyAddFXRef = GameData.GetObjectRef("MONEY_ADD");
        GameSystem.GetPlayerData().AddMoney(moneyAdd);
        if (moneyAddFXRef && thePC)
        {
            GameObject fo = SpawnGameplayObject(moneyAddFXRef, thePC.transform.position);
            fo.transform.parent = thePC.transform;
        }
    }

    public bool OnDropItemPickUp(DropItem item)
    {
        DropItem.DROPITEM_TYPE itemType = item.GetItemType();
        bool result = false;
        switch (itemType)
        {
            case DropItem.DROPITEM_TYPE.HEAL_POTION:
                if (currPotion < maxPotion)
                {
                    currPotion++;
                    result = true;
                }
                break;

            //2022/9/22 補充
            //現在已經不再使用這類的直接升物件 Pick Up 可以先保留作為
            //關卡中暫時變強的物件使用 (變強只限於同關卡，在換關後消失)

            case DropItem.DROPITEM_TYPE.POWERUP_MAXHP:
                //print("血量升級 !!");
                result = thePC.DoHpUp();
                break;
            case DropItem.DROPITEM_TYPE.POWERUP_ATTACK:
                //print("攻擊升級 !!");
                result = thePC.DoAtkUp();
                break;
        }

        return result;
    }


    public void OnAddLevelDifficulty(int addLevel = 1)
    {
        currLevel += 1;
        SetUpHud();
    }

    public void OnClearGateEnter()
    {
        //增加一個難度
        ResetLevel(currLevel + 1);
    }


    //整個重開，角色也回到等級一狀態，從 Fail UI 呼叫
    public void OnLevelRestart()
    {
        if (currState != BATTLE_GAME_STATE.FAIL)
        {
            print("ERROR !!!!! OnLevelRestart() called but not in fail state !!");
        }

        //GameSystem.GetInstance().SaveData();
        OnExitBattle();

        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);

    }

    public void OnBackToStartMenu()
    {
        //GameSystem.GetInstance().SaveData();
        OnExitBattle();
        SceneManager.LoadScene("StartMenu");
    }

    public void OnGotoScene(string sceneName)
    {
        //GameSystem.GetInstance().SaveData();
        OnExitBattle();
        //SceneManager.LoadScene(sceneName);
        SceneTraveler.GotoScene(sceneName, "");
    }

    public void OnGotoScene(string sceneName, string entraceName)
    {
        OnExitBattle();
        SceneTraveler.GotoScene(sceneName, entraceName);
    }

    public void OnGotoSceneWithBack(string sceneName, string entraceName, string _backScene, string _backEntrance)
    {
        OnExitBattle();
        SceneTraveler.GotoSceneWithBackInfo(sceneName, entraceName, _backScene, _backEntrance);
    }

    public void OnBackPrevScene()
    {
        OnExitBattle();
        if (backScene == WorldMap.WORLDMAP_SCENE)
        {
            GameSystem.GetWorldMap().GotoCurrZone(backEntrance);
        }
        else
            SceneTraveler.GotoScene(backScene, backEntrance);
    }


    protected void OnExitBattle()
    {
        //print("BattleSystem.OnExitBattle() !!");
        if (theMG)
        {
            theMG.OnEixtMap();
        }
        //if (MissionManager.GetCurrMission() != null)
        //{
        //    MissionManager.FinishCurrMission();
        //}
        ContinuousBattleManager.OnSceneExit();
        GameSystem.GetInstance().SaveData();
    }

    static public string GetCurrScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    //float timeTotal = 0;
    //int frameCount = 0;
    //float FPS = 0;
    //private void OnGUI()
    //{
    //    timeTotal += Time.deltaTime;
    //    frameCount++;
    //    if (timeTotal > 0.5f)
    //    {
    //        FPS = frameCount / timeTotal;
    //        timeTotal = 0;
    //        frameCount = 0;
    //    }
    //    GUI.TextArea(new Rect(new Vector2(10.0f, 10.0f), new Vector2(100.0f, 40.0f)), FPS.ToString());
    //    //GUI.TextArea(new Rect(new Vector2(10.0f, 10.0f), new Vector2(100.0f, 40.0f)), currState.ToString());
    //}
}
