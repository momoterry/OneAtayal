using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class BattleSystem : MonoBehaviour
{
    public Battle_HUD theBattleHUD;
    public string backScene = "StartMenu";
    //public VPad theVPad;

    public MapGeneratorBase theMG;
    public Transform initPlayerPos;
    public float initPlayerDirAngle = 0;
    public GameObject playerRef;
    public bool ForceUsePlayerRef = false;

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

    protected static BattleSystem instance = null;
    public static BattleSystem GetInstance() { return instance; }

    public BattleSystem() : base()
    {
        //if (instance != null)
        //    print("ERROR !! �W�L�@�� BattleSystem �s�b: ");
        //instance = this;
    }

    protected void Awake()
    {
        GameSystem.Ensure();    //���F������ Scene ���i�H���� Play !!

        if (instance != null)
            print("ERROR !! �W�L�@�� BattleSystem �s�b: ");
        instance = this;

        touchLayer = LayerMask.GetMask("TouchPlane", "UI");
    }

    public GameObject GetPlayer() { return thePlayer; }

    public PlayerControllerBase GetPlayerController() { return thePC; }
    public static PlayerControllerBase GetPC() { return instance.thePC; }
    public static Dialogue GetDialogue() { return instance.theBattleHUD.theDialogue; }
    public VPad GetVPad() {return theBattleHUD.theVPad; }

    public MapGeneratorBase GetMapGenerator() { return theMG; }

    public int GetCurrLevel() { return currLevel; }

    public bool IsDuringBattle() { return currState == BATTLE_GAME_STATE.BATTLE; }

    public void AddEnemy( GameObject enemyObj)
    {
        enemyList.Add(enemyObj);
    }

    public void OnEnemyKilled( GameObject enemyObj)
    {
        //TODO: �O�_�� enemyList �令 Enemy �Ӥ��O GameObject ?

       if ( enemyList.Remove(enemyObj))
       {
            Enemy e = enemyObj.GetComponent<Enemy>();
            if (e)
            {
                thePC.OnKillEnemy(e);
            }

            if (GetEnemyCount()==0)
            {
                OnEnemyClear();
            }      
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
        return o;
    }

    public static GameObject SpawnGameObj(GameObject objRef, Vector3 pos, bool clearByBS = false)
    {
        return instance.SpawnGameplayObject(objRef, pos, clearByBS);
    }

    //TODO: �i�H�����F
    protected void OnEnemyClear()
    {
        //if (clearGate)
        //    clearGate.SetActive(true);
    }



    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!ForceUsePlayerRef)
        {
            GameObject playerInfoToSet = GameSystem.GetInstance().GetPlayerCharacterRef();
            if (playerInfoToSet != null)
            {
                playerRef = playerInfoToSet;
            }
            else
            {
                GameSystem.GetInstance().SetPlayerCharacterRef(playerRef);
            }
        }

        GameSystem.GetInstance().SaveData();

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
        //TODO: Loading �����ᵥ�ݴX�� Frame ?
        InitBattle();
        nextState = BATTLE_GAME_STATE.BATTLE;
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
            enemyObj.SendMessage("OnGameFail", SendMessageOptions.DontRequireReceiver);
        }
    }

    protected virtual void SetUpHud()
    {
        string levelText = "LEVEL : " + currLevel;
        theBattleHUD.SetLevelText(levelText);
    }

    protected virtual void SetUpLevel( int level = 1)
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

    protected void ResetLevel( int level = 1 )
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


        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);

        //ResetLevel();
        //nextState = BATTLE_GAME_STATE.BATTLE;
    }

    public void OnBackToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void OnGotoScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnBackPrevScene()
    {
        SceneManager.LoadScene(backScene);
    }

    //private void OnGUI()
    //{
    //    GUI.TextArea(new Rect(new Vector2(10.0f, 10.0f), new Vector2(100.0f, 40.0f)), currState.ToString());
    //}
}
