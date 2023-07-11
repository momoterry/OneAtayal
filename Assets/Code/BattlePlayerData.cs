using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�b���d�԰��������a��ơA�b�԰�������|�Q�M�����]

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
            print("ERROR !! �W�L�@�� BattlePlayerData �s�b ");
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
        //�ɤO�k  TODO: �Ϊ��]�w�g���
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
            currExpMax = (int)(currExpMax * 1.2f);  //TODO: �άd���覡?
        }
        if (currBattleLV != originalLV)
        {

        }
    }

    protected void DoBattleLVUp(int addLV)
    {
        print("�ɯŰաA�ɤF" + addLV + " �šA�{�b�O " + currBattleLV + " ��");
    }
}
