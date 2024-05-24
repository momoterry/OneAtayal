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
    public string mapName;      //�Ψ��ѧO�a�Ϧs��

    public NavMeshSurface theSurface2D;
    public MapEntraceData[] entraceList;

    protected MapSaveDataBase mapDataBase;      //�p�G�����ܡA��ܦ��a�Ϧs�ɡA�ثe�@���򩳥D�n�O�������a�����G

    protected string entranceID;

    public virtual void BuildAll(int buildLevel = 1) {}

    //public virtual void OnEixtMap() { }
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
                print("���J�f" + _ID);
                //BattleSystem.GetInstance().initPlayerPos = entraceList[i].pos;
                BattleSystem.GetInstance().initPlayerDirAngle = entraceList[i].faceAngle;
                //if (Camera.main)    //�ɤO�k���ʦ�m�A���ӳz�L BattleCamera
                //{
                //    Vector3 newPos = entraceList[i].pos.position;
                //    Camera.main.transform.position = new Vector3(newPos.x, Camera.main.transform.position.y, newPos.z);
                //}
                BattleSystem.GetInstance().SetInitPosition(entraceList[i].pos.position);
            }
        }
    }

    virtual protected void CreateMapSaveData()
    {
        mapDataBase = new MapSaveDataBase();
    }

    virtual protected void SaveMap()
    {
        if (mapName == null || mapName == "")
            return;

        if (mapDataBase == null)
            CreateMapSaveData();
    }

    virtual protected void LoadMap()
    {
        mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
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

    // ================================ �����a�ϰO������
    public virtual void OnEixtMap()
    {
        SaveExploreMap();
    }

    protected void SaveExploreMap()
    {
        //MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
        if (mapDataBase == null)
        {
            print("SaveExploreMap: �S���s�ɸ�� MapSaveData�A���B�z");
            return;
        }

        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (theMiniMap)
        {
            //MapSaveMazeOne mapData = (MapSaveMazeOne)mapDataBase;
            mapDataBase.mapMask64 = theMiniMap.EncodeMaskTexture();
        }
    }

    protected void LoadExploreMap()
    {
        mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
        if (mapDataBase == null)
        {
            print("LoadExploreMap : �S���s�ɸ�ơA���B�z");
            return;
        }
        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (!theMiniMap)
        {
            return;
        }

        //MapSaveMazeOne mapData = (MapSaveMazeOne)mapDataBase;
        if (mapDataBase.mapMask64 == null || mapDataBase.mapMask64 == "")
        {
            print("�Ū��a�ϱ�����T: " + mapDataBase.mapMask64);
            return;
        }

        print("LoadExploreMap: ��쪺��r���Y��ơAByte �`�q: " + mapDataBase.mapMask64.Length);
        //print("��쪺��r���Y��Ƥ��e: " + mapData.mapMask64);

        theMiniMap.DecodeMaskTexture(mapDataBase.mapMask64);
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
            buildLevel = 5; //�ȩw�̤j������

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

        //TODO: �ɤO�k
        if (enemyCount == 0)
        {
            GenerateRandomEnemies(enemyInfo);
        }
    }
}
