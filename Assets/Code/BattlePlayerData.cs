using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//在關卡戰鬥中的玩家資料，在戰鬥結束後會被清除重設

public class BattlePlayerData : MonoBehaviour
{
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
    public int GetBattleLevel() { return 99; }
    public int GetBattleExp() { return 0; }
    public int GetBattleExpMax() { return 100; }

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
            AddExp(10);
        }
    }
    public void AddExp(int value)
    {


    }
}
