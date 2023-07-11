using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//在關卡戰鬥中的玩家資料，在戰鬥結束後會被清除重設

public class BattlePlayerData : MonoBehaviour
{
    const int INIT_EXP_MAX = 100;
    protected int currExp = 0;
    protected int currExpMax = INIT_EXP_MAX;
    protected int currBattleLV = 1;

    static private BattlePlayerData instance;
    static public BattlePlayerData GetInstance() { return instance; }
    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 BattlePlayerData 存在 ");
        instance = this;
    }

    // Public Functions
    public int GetLeftBattlePoints() { return 0; }
    public void AddBattlePoints( int point ) { }
    public int GetBattleLevel() { return currBattleLV; }
    public int GetBattleExp() { return currExp; }
    public int GetBattleExpMax() { return currExpMax; }

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
        //暴力法  TODO: 用表格設定經驗值
        if (BattleSystem.GetInstance().IsBattleLevelUp)
        {
            AddExp(30);
        }
    }
    public void AddExp(int value)
    {
        currExp += value;
        int originalLV = currBattleLV;
        while (currExp > currExpMax)
        {
            currExp -= currExpMax;
            currBattleLV++;
            currExpMax = (int)(currExpMax * 1.2f);  //TODO: 用查表的方式?
        }
        if (currBattleLV != originalLV)
        {

        }
    }

    protected void DoBattleLVUp(int addLV)
    {
        print("升級啦，升了" + addLV + " 級，現在是 " + currBattleLV + " 級");
    }
}
