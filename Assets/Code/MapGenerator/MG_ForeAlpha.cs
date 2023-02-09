using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//=================================================================
//
//  以 4x4 的 Block 隨機擺放的隨機地圖，以 Dirt 的部份來隨機產生敵人
//
//=================================================================

public class MG_ForeAlpha : MG_ForestRD
{
    public float blockRatio = 0.2f;
    public float dirtRatio = 0.1f;

    public GameObject EnemySpawnerRef;

    protected int blockSize = 4;

    protected List<GameObject> eSpawnerList = new List<GameObject>();
    protected float eSpawnTimer = 0;
    [SerializeField]protected float eSpawnStep = 0.4f;

    private void Update()
    {
        eSpawnTimer += Time.deltaTime;
        if (eSpawnTimer >= eSpawnStep)
        {
            eSpawnerList[Random.Range(0, eSpawnerList.Count)].SendMessage("OnTG", gameObject);
            eSpawnTimer -= eSpawnStep;
        }
    }

    protected void CrossFixInMap()
    {
        for (int x = theMap.xMin; x <= theMap.xMax-1; x++)
        {
            for (int y = theMap.yMin; y <= theMap.yMax-1; y++)
            {
                bool b11 = theMap.GetValue(x, y) == (int)TILE_TYPE.BLOCK;
                bool b12 = theMap.GetValue(x + 1, y) == (int)TILE_TYPE.BLOCK;
                bool b21 = theMap.GetValue(x, y + 1) == (int)TILE_TYPE.BLOCK;
                bool b22 = theMap.GetValue(x + 1, y + 1) == (int)TILE_TYPE.BLOCK;

                if (b11 && b22 && !b21 && !b12)
                {
                    theMap.SetValue(x, y, (int)TILE_TYPE.GRASS);
                    theMap.SetValue(x + 1, y + 1, (int)TILE_TYPE.GRASS);
                }
                else if (!b11 && !b22 && b21 && b12)
                {
                    theMap.SetValue(x, y + 1, (int)TILE_TYPE.GRASS);
                    theMap.SetValue(x + 1, y, (int)TILE_TYPE.GRASS);
                }
            }
            
        }
    }

    protected override void CreateForestMap()
    {
        int xNum = mapWidth / blockSize;
        int yNum = mapHeight / blockSize;

        int dirtCount = 0;
        int blockCount = 0;
        int x = -mapWidth / 2 + blockSize / 2;
        for (int ix =0; ix < xNum; ix++)
        {
            int y = -mapHeight / 2 + blockSize / 2;
            for (int iy=0; iy < yNum; iy++)
            {
                float rd = Random.Range(0, 1.0f);
                if (rd < blockRatio)
                {
                    FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter + new Vector3Int(x, y, 0), 4, 4);
                    blockCount++;
                }
                else if (rd < blockRatio + dirtRatio)
                {
                    Vector3Int coord = mapCenter + new Vector3Int(x, y, 0);
                    FillSquareInMap((int)TILE_TYPE.DIRT, coord, 4, 4);
                    if (EnemySpawnerRef)
                    {
                        eSpawnerList.Add(Instantiate(EnemySpawnerRef, new Vector3(coord.x, 0, coord.y), Quaternion.identity));
                    }
                    dirtCount++;
                }
                y += blockSize;
            }
            x += blockSize;
        }

        print("Dirt Count: " + dirtCount);

        //確保中央是空的
        FillSquareInMap((int)TILE_TYPE.GRASS, Vector3Int.zero, blockSize, blockSize);

        //修正角角對接問題
        CrossFixInMap();
    }
}
