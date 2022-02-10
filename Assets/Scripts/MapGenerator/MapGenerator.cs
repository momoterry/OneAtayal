using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class MapGeneratorBase : MonoBehaviour
{
    public virtual void BuildAll(int buildLevel = 1)
    {
    }
}

public class MapGenerator : MapGeneratorBase
{
    public Tile wall;
    public Tile hole;
    public NavMeshSurface2d theSurface2D;
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
        theSurface2D.BuildNavMesh();

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
