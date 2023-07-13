using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�b���d�԰��������a��ơA�b�԰�������|�Q�M�����]

public class BattlePlayerCrossSceneData
{
    public int currExp;
    public int currExpMax;
    public int currBattleLV;
    public int battleLVPoint;
}

public class BattlePlayerData : MonoBehaviour
{
    const int MAX_BATTLE_LEVEL = 20;
    const int INIT_EXP_MAX = 100;
    const float EXP_MAX_STEP = 1.2f;

    protected int currExp = 0;
    protected int currExpMax = INIT_EXP_MAX;
    protected int currBattleLV = 1;
    protected int battleLVPoint = 0;
    //protected BattlePlayerCrossSceneData data;

    protected int[] maxExpArray = new int[MAX_BATTLE_LEVEL];

    static private BattlePlayerData instance;
    static public BattlePlayerData GetInstance() { return instance; }
    private void Awake()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� BattlePlayerData �s�b ");
        instance = this;

        maxExpArray[0] = 1;
        maxExpArray[1] = INIT_EXP_MAX;
        for (int i=2; i<MAX_BATTLE_LEVEL; i++)
        {
            maxExpArray[i] = (int)(maxExpArray[i - 1] * EXP_MAX_STEP);
        }

        if (ContinuousBattleManager.GetInstance() != null)
        {
            BattlePlayerCrossSceneData data = ContinuousBattleManager.GetBattlePlayerCrossSceneData();
            if (data != null)
            {
                //print("�� BattlePlayerCrossSceneData�A�_����� !!");
                currBattleLV = data.currBattleLV;
                currExp = data.currExp;
                currExpMax = data.currExpMax;
                battleLVPoint = data.battleLVPoint;
            }
        }
    }

    // Public Functions
    public BattlePlayerCrossSceneData GetCrossSceneData()
    {
        BattlePlayerCrossSceneData data = new BattlePlayerCrossSceneData();
        data.currBattleLV = currBattleLV;
        data.currExp = currExp;
        data.currExpMax = currExpMax;
        data.battleLVPoint = battleLVPoint;
        return data;
    }
    public int GetBattleLVPoints() { return battleLVPoint; }
    public void AddBattleLVPoints( int point ) { 
        battleLVPoint += point;
        BattleSystem.GetPC().OnBattlePointsChange();
    }
    public int GetBattleLevel() { return currBattleLV; }
    public int GetBattleExp() { return currExp; }
    public int GetBattleExpMax() { return currExpMax; }
    public int GetMaxBattleLevel() { return MAX_BATTLE_LEVEL; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnKillEnemy(Enemy e)
    {
        //�ɤO�k  TODO: �Ϊ��]�w�g���
        if (BattleSystem.GetInstance().IsBattleLevelUp)
        {
            AddExp(30);
        }
    }
    public void AddExp(int value)
    {
        if (currBattleLV == MAX_BATTLE_LEVEL)
            return;
        currExp += value;
        int originalLV = currBattleLV;
        while (currExp >= currExpMax)
        {
            currBattleLV++;
            battleLVPoint++;
            if (currBattleLV < MAX_BATTLE_LEVEL)
            {
                currExp -= currExpMax;
                currExpMax = maxExpArray[currBattleLV];
            }
            else
            {
                currBattleLV = MAX_BATTLE_LEVEL;    //�T�O
                currExp = 0;       //���ŤF
            }
        }
        if (currBattleLV != originalLV)
        {
            DoBattleLVUp(currBattleLV - originalLV);
        }
    }

    protected void DoBattleLVUp(int addLV)
    {
        BattleSystem.GetPC().OnBattlePointsChange();
        //print("�ɯŰաA�ɤF" + addLV + " �šA�{�b�O " + currBattleLV + " ��");
    }
}
