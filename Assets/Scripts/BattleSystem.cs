using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    public Battle_HUD theBattleHUD;

    public GameObject clearGate;
    public MapGeneratorBase theMG;
    public Transform initPlayerPos;
    public GameObject playerRef;

    private GameObject thePlayer;   //TODO Player Character Spawn ���ߡA�� PC ���`�n
    private List<GameObject> enemyList = new List<GameObject>();

    private PlayerController thePC;

    public int MaxLevel = 5;
    private int currLevel = 1;

    //��~
    public int initPotion = 3;
    public int maxPotion = 5;
    public float potionHealRatio = 0.5f;
    private int currPotion = 3;

    enum BATTLE_GAME_STATE
    {
        NONE,
        INIT,
        BATTLE,
        LOCAL_WIN,
        FAIL,
    }
    BATTLE_GAME_STATE currState = BATTLE_GAME_STATE.NONE;
    BATTLE_GAME_STATE nextState = BATTLE_GAME_STATE.NONE;
    float stateTime = 0;

    public int GetEnemyCount() { return enemyList.Count ; }

    private static BattleSystem instance;
    public static BattleSystem GetInstance() { return instance; }

    public BattleSystem() : base()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� BattleSystem �s�b ");
        instance = this;
    }

    public GameObject GetPlayer() { return thePlayer; }

    public PlayerController GetPlayerController() { return thePC; }

    public void AddEnemy( GameObject enemyObj)
    {
        enemyList.Add(enemyObj);
    }

    public void OnEnemyKilled( GameObject enemyObj)
    {
       if ( !enemyList.Remove(enemyObj))
       {
            print("ERROR !! OnEnemyKilled() : ���w���ĤH���b�M�椤 : " + enemyObj);
       }
       else
        {
            if (GetEnemyCount()==0)
            {
                OnEnemyClear();
            }
        }
    }

    private void Awake()
    {
        GameSystem.Ensure();    //���F������ Scene ���i�H���� Play !!
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerInfoToSet = GameSystem.GetInstance().GetPlayerCharacterRef();
        if (playerInfoToSet != null)
        {
            playerRef = playerInfoToSet;
        }

        nextState = BATTLE_GAME_STATE.INIT;
    }

    // Update is called once per frame
    void Update()
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
                break;
            case BATTLE_GAME_STATE.FAIL:
                UpdateFail();
                break;
        }

        UpdateBattleHUD();
    }

    //�Ĥ@����l�����d
    private void InitGameData()
    {
        SetUpLevel(1);

        //thePC.InitStatus();
        //currPotion = initPotion;
        InitPlayerData();
    }

    private void InitPlayerData()
    {
        thePC.InitStatus();
        currPotion = initPotion;
    }

    private void UpdateInit()
    {
        //TODO: Loading �����ᵥ�ݴX�� Frame ?
        InitGameData();
        nextState = BATTLE_GAME_STATE.BATTLE;
    }

    private void UpdateBattleHUD()
    {
        if (thePC)
        {
            theBattleHUD.SetPlayerInfo(thePC.GetHP(), thePC.GetHPMax(), thePC.GetMP(), thePC.GetMPMax(), thePC.GetATTACK());
            theBattleHUD.SetPotionNum(currPotion, maxPotion);
        }
    }

    private void UpdateFail()
    {
    //    stateTime -= Time.deltaTime;
    //    if (stateTime < 0)
    //    {
    //        //TODO: ��ӹC�����}
    //        ResetLevel();
    //        thePC.InitStatus();
    //        nextState = BATTLE_GAME_STATE.BATTLE;
    //    }
    }

    private void OnStateEnter()
    {
        switch (nextState)
        {
            case BATTLE_GAME_STATE.BATTLE:
                break;
            case BATTLE_GAME_STATE.FAIL:
                stateTime = 3.0f;   //�T��᭫�}��
                theBattleHUD.OnStartFailUI();
                break;
        }
    }

    private void OnStateExit()
    {

    }

    public void OnEnemyClear()
    {
        clearGate.SetActive(true);
    }

    public void OnClearGateEnter()
    {
        //�W�[�@������
        ResetLevel(currLevel+1);
    }

    //��ӭ��}�A����]�^�쵥�Ť@���A�A�q Fail UI �I�s
    public void OnLevelRestart()
    {
        if (currState != BATTLE_GAME_STATE.FAIL)
        {
            print("ERROR !!!!! OnLevelRestart() called but not in fail state !!");
        }

        ResetLevel();
        //thePC.InitStatus();
        InitPlayerData();
        nextState = BATTLE_GAME_STATE.BATTLE;
    }

    public void OnBackToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

  
    private void ClearLevel()
    {
        foreach (GameObject enemyObj in enemyList)
        {
            Destroy(enemyObj);
            print("Kill One !!");
        }
        enemyList.Clear();

        DropItem.ClearAllDropItem();
    }


    private void SetUpLevel( int level = 1)
    {
        currLevel = level;
        if (level < 1)
            currLevel = 1;
        else if (level > MaxLevel)
            currLevel = MaxLevel;

        clearGate.SetActive(false);
        theMG.BuildAll(currLevel);

        if (thePlayer == null)
        {
            thePlayer = Instantiate(playerRef, initPlayerPos.position, Quaternion.identity, null);
            thePC = thePlayer.GetComponent<PlayerController>();
        }
        NavMeshAgent pAgnet = thePlayer.GetComponent<NavMeshAgent>();
        pAgnet.Warp(initPlayerPos.position);

        string levelText = "LEVEL : " + currLevel;
        theBattleHUD.SetLevelText(levelText);
    }

    public void ResetLevel( int level = 1 )
    {
        print("Reset Level!!");
        //currLevel = level;
        //if (level < 1)
        //    currLevel = 1;
        //else if (level > MaxLevel)
        //    currLevel = MaxLevel;

        ClearLevel();
        //if (clearEnemy)
        //{
        //    foreach (GameObject enemyObj in enemyList)
        //    {
        //        Destroy(enemyObj);
        //        print("Kill One !!");
        //    }
        //    enemyList.Clear();
        //}

        SetUpLevel(level);
        //clearGate.SetActive(false);
        //theMG.BuildAll(currLevel);

        //if (thePlayer == null)
        //{
        //    thePlayer = Instantiate(playerRef, initPlayerPos.position, Quaternion.identity, null);
        //    thePC = thePlayer.GetComponent<PlayerController>();
        //}
        //NavMeshAgent pAgnet = thePlayer.GetComponent<NavMeshAgent>();
        //pAgnet.Warp(initPlayerPos.position);

        //string levelText = "LEVEL : " + currLevel;
        //theBattleHUD.SetLevelText(levelText);
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
}
