using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�b���d�԰��������a��ơA�b�԰�������|�Q�M�����]

public class BattlePlayerData : MonoBehaviour
{
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
        //�ɤO�k  TODO: �Ϊ��]�w�g���
        if (BattleSystem.GetInstance().IsBattleLevelUp)
        {
            AddExp(10);
        }
    }
    public void AddExp(int value)
    {


    }
}
