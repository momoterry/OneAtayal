using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class RoomGameplayBase : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroupInfo
    {
        public GameObject[] enemys;
        public float totalNumMin = 2;
        public float totalNumMax = 3;
    }

    public virtual void Build( MazeGameManager.RoomInfo room ) { }

    static public GameObject SpawnEnemyGroupObject(EnemyGroupInfo info, Vector3 vCenter, int width=4, int height=4, float diffAddRate=0)
    {
        float numF = ((info.totalNumMax - info.totalNumMin) * diffAddRate + info.totalNumMin);
        //int num = Mathf.RoundToInt(numF);
        int num = OneUtility.FloatToRandomInt(numF);
        //print("EG: float: " + numF + "int: "+ num);
        GameObject o = new GameObject();
        o.transform.position = vCenter;
        EnemyGroup enemyGroup = o.AddComponent<EnemyGroup>();

        enemyGroup.width = width;
        enemyGroup.height = height;
        enemyGroup.isRandomEnemyTotal = true;
        enemyGroup.randomEnemyTotal = num;
        enemyGroup.enemyInfos = new EnemyGroup.EnemyInfo[info.enemys.Length];
        for (int i = 0; i < info.enemys.Length; i++)
        {
            enemyGroup.enemyInfos[i] = new EnemyGroup.EnemyInfo();
            enemyGroup.enemyInfos[i].enemyRef = info.enemys[i];
        }
        return o;
    }
}

//public class MR_NodeBase : MonoBehaviour
//{
//    public const float ROOM_RELATIVE_SIZE = 10.0f;     //縮放等的基準
//    protected float widthRatio = 1;
//    protected float heightRatio = 1;
//    virtual public void OnSetupByRoom(MazeGameManager.RoomInfo room)
//    {
//        //TODO: Local 位置校正
//        widthRatio = room.width / ROOM_RELATIVE_SIZE;
//        heightRatio = room.height / ROOM_RELATIVE_SIZE;
//    }
//}

public class RoomGameplay : RoomGameplayBase
{
    public GameObject centerGame;

    private void Awake()
    {
        //centerGame.SetActive(false);
    }
    public override void Build( MazeGameManager.RoomInfo room ) 
    {
        if (!centerGame)
            return;
        GameObject o = BattleSystem.SpawnGameObj(centerGame, room.vCenter);
        o.SetActive(true);

        foreach (MR_Node node in o.GetComponentsInChildren<MR_Node>())
        {
            node.OnSetupByRoom(room);
        }
    }

}
