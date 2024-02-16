using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;

[System.Serializable]
public class MapEntraceData
{
    public string name;
    public Transform pos;
    public float faceAngle;
}

public class MapGeneratorBase : MonoBehaviour
{
    public NavMeshSurface theSurface2D;
    public MapEntraceData[] entraceList;

    protected string entranceID;

    public virtual void BuildAll(int buildLevel = 1) {}

    public virtual void OnEixtMap() { }
    public virtual void SetEntrance(string _ID) { 
        entranceID = _ID;
        //print("SetEntrance: " + _ID);
        if (entraceList == null)
        {
            return;
        }
        for (int i=0; i < entraceList.Length;i++)
        {
            if (_ID == entraceList[i].name)
            {
                print("找到入口" + _ID);
                //BattleSystem.GetInstance().initPlayerPos = entraceList[i].pos;
                BattleSystem.GetInstance().initPlayerDirAngle = entraceList[i].faceAngle;
                //if (Camera.main)    //暴力法移動位置，應該透過 BattleCamera
                //{
                //    Vector3 newPos = entraceList[i].pos.position;
                //    Camera.main.transform.position = new Vector3(newPos.x, Camera.main.transform.position.y, newPos.z);
                //}
                BattleSystem.GetInstance().SetInitPosition(entraceList[i].pos.position);
            }
        }
    }

    public void RebuildNavmesh()
    {
        if (theSurface2D)
            GenerateNavMesh(theSurface2D);
    }

    static public void GenerateNavMesh(NavMeshSurface surface)
    {
        if (surface)
        {
            surface.hideEditorLogs = true;
            surface.BuildNavMesh();
        }
    }
}

public class MapGenerator : MapGeneratorBase
{
    public Tile wall;
    public Tile hole;
    public Tilemap theTM;
    public GameObject wallBlocker;

    public GameObject enemyNormal;
    public GameObject enemyStrong;
    public GameObject enemyRanger;

    private List<GameObject> wallList = new List<GameObject>();

    struct EnemyRateAll
    {
        public float nomal;
        public float strong;
        public float ranger;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void BuildAll( int buildLevel = 1)
    {
        ClearAll();
        GenerateRandomWalls();
        GenerateNavMesh(theSurface2D);

        if (buildLevel >= 5)
            buildLevel = 5; //暫定最大五難度

        EnemyRateAll enemyInfo;
        enemyInfo.nomal = 5.0f + (float)((buildLevel-1) * 2);

        enemyInfo.strong = 0.0f;
        enemyInfo.ranger = 0.0f;
        if (buildLevel > 2)
            enemyInfo.strong = 3.0f +(float)((buildLevel - 3) * 2);
        GenerateRandomEnemies(enemyInfo);
    }

    private void ClearAll()
    {
        foreach (GameObject w in wallList)
        {
            Destroy(w);
        }
        wallList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateRandomWalls(float wallRate = 15.0f, float holeRate = 15.0f)
    {
        for (int i = -10; i < 10; i++)
        {
            for (int j = -5; j < 5; j++)
            {
                float rd = Random.Range(0.0f, 100.0f);
                Vector3Int tileCoordinate = new Vector3Int(i, j, 0);
                if (rd < wallRate)
                {
                    theTM.SetTile(tileCoordinate, wall);
                    Vector3 pos = theTM.CellToWorld(tileCoordinate);
                    pos += new Vector3(0.5f, 0.5f, 0.0f);
                    GameObject w = Instantiate(wallBlocker, pos, Quaternion.identity, null);
                    wallList.Add(w);
                }
                else if (rd < wallRate + holeRate)
                {
                    theTM.SetTile(tileCoordinate, hole);
                }
                else
                    theTM.SetTile(tileCoordinate, null);
            }
        }
    }

    private void GenerateRandomEnemies(EnemyRateAll enemyInfo)
    {
        int enemyCount = 0;
        for (int i = -10; i < 10; i++)
        {
            for (int j = -2; j < 5; j++)
            {
                Vector3Int tileCoordinate = new Vector3Int(i, j, 0);
                if (theTM.GetTile(tileCoordinate) == null)
                {
                    Vector3 pos = theTM.CellToWorld(tileCoordinate);
                    pos += new Vector3(0.5f, 0.5f, 0.0f);                  
                    float rd = Random.Range(0.0f, 100.0f);
                    if (rd < enemyInfo.nomal)
                    {
                        Instantiate(enemyNormal, pos, Quaternion.identity, null);
                        enemyCount++;
                    }
                    else if (rd < enemyInfo.nomal + enemyInfo.strong)
                    {
                        Instantiate(enemyStrong, pos, Quaternion.identity, null);
                        enemyCount++;
                    }
                    else if (rd < enemyInfo.nomal + enemyInfo.strong + enemyInfo.ranger)
                    {
                        Instantiate(enemyRanger, pos, Quaternion.identity, null);
                        enemyCount++;
                    }
                }

            }
        }
        print("GenerateRandomEnemies() Total Enemy = " + enemyCount);

        //TODO: 暴力法
        if (enemyCount == 0)
        {
            GenerateRandomEnemies(enemyInfo);
        }
    }
}
