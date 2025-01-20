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
    public string levelID = "";  //�p�G�����w���ܡA�N�O�ݩ� LevelManager �������w ID
    //public VPad theVPad;

    public MapGeneratorBase theMG;
    public Transform initPlayerPos;
    public float initPlayerDirAngle = 0;
    public GameObject playerRef;
    public bool ForceUsePlayerRef = false;

    public bool IsBattleLevelUp = false;

    protected GameObject thePlayer;   //TODO Player Character Spawn ���ߡA�� PC ���`�n
    protected List<GameObject> enemyList = new List<GameObject>();
    protected List<GameObject> objList = new List<GameObject>();

    protected PlayerControllerBase thePC;

    public int MaxLevel = 5;
    protected int currLevel = 1;

    //Input ����
    protected int touchLayer;
    protected List<GameObject> touchDownTracing = new List<GameObject>();

    //��~
    public int initPotion = 3;
    public int maxPotion = 5;
    public float potionHealRatio = 0.5f;
    protected int currPotion = 3;

    //����ˮ`���ť�
    protected float AllEnemyDamageRate = 0.5f;
    protected float AllFriendlyDamageRate = 0.5f;
    static public float GetAllEnemyDamageRate() { return instance.AllEnemyDamageRate; }
    static public float GetAllFriendlyDamageRate() { return instance.AllFriendlyDamageRate; }
    static public void SetAllEnemyDamageRate(float _AllEnemyDamageRate) { instance.AllEnemyDamageRate = _AllEnemyDamageRate; }
    static public void SetAllFriendlyDamageRate(float _AllFriendlyDamageRate) { instance.AllFriendlyDamageRate = _AllFriendlyDamageRate; }


    //Doll Skill ����
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

    //Fade In/Out ����
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
        GameSystem.Ensure();    //���F������ Scene ���i�H���� Play !!

        if (instance != null)
            print("ERROR !! �W�L�@�� BattleSystem �s�b: ");
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
        //TODO: �O�_�� enemyList �令 Enemy �Ӥ��O GameObject ?

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
            print("ERROR !! �����Ӫ����_����!!: " + enemyObj);
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

    public void SetInitPosition(Vector3 pos)       //�]�w�C���}�l����m
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

    //Loading ��Ĥ@����l�����d (�D Fail Reset)
    protected virtual void InitBattle()
    {
        //print("InitBattle");
        SetUpLevel(1);
        //InitPlayerData();           //TODO: �o�����ӧ�浹 SetUpLevel �h���w
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
            //�קK�X Error �ɤ@���]�A���]�w nextState
            nextState = BATTLE_GAME_STATE.BATTLE;
            InitBattle();
        }
        //else
        //{
        //    //TODO: �µe��?
        //    print("�٨S�n�� ...... �n�ӥ��ڶ�?");
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
        //TODO : �קK UI �Ĭ𪺤�k���I�ɤO, �n��
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
                stateTime = 3.0f;   //�T��᭫�}��
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

        // ================ �԰���ƪ�l�� ====================
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
                thePC.SetupFaceDirByAngle(initPlayerDirAngle);  //�T�O First Frame ��V���T
                //thePC.InitStatus(); �|�b PC �� Start �Q�I�s
            }
            else
            {
                //�¦������d�U�@���A���|�A�Ψ�A���Ӳ���
                thePC.DoTeleport(initPlayerPos.position, initPlayerDirAngle);
                thePC.InitStatus();
            }
        }
        else
        {
            // ���d�ɯ�
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
        print("���a���`........");
        nextState = BATTLE_GAME_STATE.FAIL;
    }

    public void OnUsePotion()
    {
        if (currPotion > 0)
        {
            //�b�o�̥s����ɤW
            if (thePC)
            {
                float heal = thePC.GetHPMax() * potionHealRatio;
                thePC.DoHeal(heal);
            }
            currPotion--;
        }
    }

    // =================== �B������

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

            //2022/9/22 �ɥR
            //�{�b�w�g���A�ϥγo���������ɪ��� Pick Up �i�H���O�d�@��
            //���d���Ȯ��ܱj������ϥ� (�ܱj�u����P���d�A�b���������)

            case DropItem.DROPITEM_TYPE.POWERUP_MAXHP:
                //print("��q�ɯ� !!");
                result = thePC.DoHpUp();
                break;
            case DropItem.DROPITEM_TYPE.POWERUP_ATTACK:
                //print("�����ɯ� !!");
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
        //�W�[�@������
        ResetLevel(currLevel + 1);
    }


    //��ӭ��}�A����]�^�쵥�Ť@���A�A�q Fail UI �I�s
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
