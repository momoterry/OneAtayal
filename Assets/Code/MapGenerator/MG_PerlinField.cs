using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapSavePerlinField : MapSaveDataBase
{
    public int CellSize = 4;
    public int mapCellWidthH = 15;
    public int mapCellHeightH = 10;
    public int NoiseScaleOn256 = 5;
    public float highRatio = 0.35f;
    public float lowRatio = 0.35f;
    public float randomShiftX = -1;
    public float randomShiftY = -1;
    public string mapMask64 = null;
}


public class MG_PerlinField : MG_PerlinNoise
{
    public string mapName;
    public DungeonEnemyManagerBase enemyManager;
    public MapDecadeGenerator decadeGenerator;
    public GameObject initGameplay;
    public int edgeWidth = 4;

    protected Vector2Int initCell = Vector2Int.zero;

    //�H���O��������
    protected float randomShiftX = -1;
    protected float randomShiftY = -1;


    public override void OnEixtMap()
    {
        SaveExploreMap();
    }

    protected override int GetMapValue(float perlinValue)
    {
        if (perlinValue > 1.0f - highRatio)
        {
            return (int)MY_VALUE.HIGH;
        }
        else if (perlinValue < lowRatio)
        {
            return (int)MY_VALUE.LOW;
        }
        else
        {
            return (int)MY_VALUE.NORMAL;
        }
    }

    protected override void GenerateCellMap()
    {
        float saveMargin = 0.01f;
        List<Vector2Int> saveList = new List<Vector2Int>(); ;
        saveList.Add(Vector2Int.zero);

        float noiseScale = (float)NoiseScaleOn256 / 256.0f;
        float randomSscale = 10.0f;
        float xShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        float yShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        if (randomShiftX > 0 && randomShiftY > 0)
        {
            xShift = randomShiftX;
            yShift = randomShiftY;
        }
        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
            {
                float rd = Mathf.PerlinNoise((float)x * noiseScale + xShift, (float)y * noiseScale + yShift);
                theCellMap.SetValue(x, y, GetMapValue(rd));

                if (rd < 0.5f + saveMargin && rd > 0.5f - saveMargin 
                    && x > theCellMap.GetXMin() + 2 && x < theCellMap.GetXMax() - 2
                    && y > theCellMap.GetYMin() + 2 && y < theCellMap.GetYMax() - 2)
                {
                    saveList.Add(new Vector2Int(x, y));
                }
            }
        }
        randomShiftX = xShift;
        randomShiftY = yShift;

        ModifyMapEdge();

        //��a�ϰ�M��
        float minDis = Mathf.Infinity;
        int saveAreaSizeH = 2;
        Vector2Int bestFound = Vector2Int.zero;
        bool isBestPositionFound = false;
        foreach( Vector2Int savePos in saveList)
        {
            float dis = (savePos.x * savePos.x)  + (savePos.y * savePos.y);
            if (dis < minDis)
            {
                bool isCheck = true;
                for (int ix = savePos.x - saveAreaSizeH; ix <= savePos.x + saveAreaSizeH; ix++)
                {
                    for (int iy = savePos.y - saveAreaSizeH; iy <= savePos.y + saveAreaSizeH; iy++)
                    {
                        if (theCellMap.GetValue(ix, iy) != (int)MY_VALUE.NORMAL)
                        {
                            isCheck = false;
                            continue;
                        }
                    }
                }
                if (isCheck)
                {
                    bestFound = savePos;
                    minDis = dis;
                    isBestPositionFound = true;
                    //print("����n���I " + dis);
                }
            }
        }


        //�ɤO�k����a
        if (!isBestPositionFound)
        {
            print("�䤣���I�A�w����a !!");
            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    theCellMap.SetValue(x, y, (int)MY_VALUE.NORMAL);
                }
            }
        }
        else
        {
            initCell = bestFound;
            if (initGameplay)
            {
                initGameplay.transform.position = theCellMap.GetCellCenterPosition(initCell.x, initCell.y);
                print("������a�� :" + initCell);
            }
        }
    }

    protected override void FillTiles()
    {
        base.FillTiles();

        if (decadeGenerator)
        {
            DecadeGenerateParameter p = new DecadeGenerateParameter();
            p.mapValue = (int)MY_VALUE.HIGH;
            decadeGenerator.BuildAll(theCellMap.GetOneMap(), p);
        }

    }

    protected override void PreBuild()
    {
        LoadMap();

        base.PreBuild();
    }

    protected override void PostBuild()
    {
        base.PostBuild();

        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (theMiniMap)
        {
            theMiniMap.CreateMiniMap(theCellMap.GetOneMap());
        }


        if (enemyManager)
        {
            float dis2Max = Mathf.Sqrt((mapCellHeightH * mapCellHeightH) + (mapCellWidthH * mapCellWidthH));
            for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
            {
                for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
                {
                    if (theCellMap.GetValue(x, y) == (int)MY_VALUE.LOW)
                    {
                        Vector2Int iPos = theCellMap.GetCellCenterCoord(x, y);
                        Vector3 vPos = new Vector3(iPos.x + 0.5f, 0, iPos.y + 0.5f);

                        float dis2Ratio = Mathf.Sqrt((x - initCell.x)* (x - initCell.x) + (y - initCell.y)* (y - initCell.y)) / dis2Max;
                        //print("dis2Ratio:" + dis2Ratio);
                        enemyManager.AddNormalPosition(vPos, dis2Ratio);
                    }
                }
            }
            enemyManager.BuildAllGameplay();
        }
        //���J�w��������T
        LoadExploreMap();

        //�a�Ϧs��
        SaveMap();

    }

    protected void ModifyMapEdge()
    {
        //���
        if (edgeWidth >= mapCellWidthH || edgeWidth > mapCellHeightH)
            return;
        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int i = 0; i < edgeWidth; i++)
            {
                theCellMap.SetValue(x, theCellMap.GetYMin() + i, (int)MY_VALUE.HIGH);
                theCellMap.SetValue(x, theCellMap.GetYMax() - i, (int)MY_VALUE.HIGH);
            }
            if (theCellMap.GetValue(x, theCellMap.GetYMin() + edgeWidth) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(x, theCellMap.GetYMin() + edgeWidth, (int)MY_VALUE.NORMAL);
            if (theCellMap.GetValue(x, theCellMap.GetYMax() - edgeWidth) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(x, theCellMap.GetYMax() - edgeWidth, (int)MY_VALUE.NORMAL);
        }

        for (int y = theCellMap.GetYMin() + edgeWidth; y <= theCellMap.GetYMax() - edgeWidth; y++)
        {
            for (int i = 0; i < edgeWidth; i++)
            {
                theCellMap.SetValue(theCellMap.GetXMin() +i, y, (int)MY_VALUE.HIGH);
                theCellMap.SetValue(theCellMap.GetXMax() -i, y, (int)MY_VALUE.HIGH);
            }
            if (theCellMap.GetValue(theCellMap.GetXMin() + edgeWidth, y) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(theCellMap.GetXMin() + edgeWidth, y, (int)MY_VALUE.NORMAL);
            if (theCellMap.GetValue(theCellMap.GetXMax() - edgeWidth, y) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(theCellMap.GetXMax() - edgeWidth, y, (int)MY_VALUE.NORMAL);
        }
    }

    protected void SaveMap()
    {
        MapSavePerlinField mapData = new MapSavePerlinField();
        mapData.className = "MG_PerlinField";
        mapData.mapName = mapName;
        mapData.mapCellWidthH = mapCellWidthH;
        mapData.mapCellHeightH = mapCellHeightH;
        mapData.CellSize = CellSize;
        mapData.NoiseScaleOn256 = NoiseScaleOn256;
        mapData.highRatio = highRatio;
        mapData.lowRatio = lowRatio;
        mapData.randomShiftX = randomShiftX;
        mapData.randomShiftY = randomShiftY;

        GameSystem.GetPlayerData().SaveMap(mapName, mapData);

    }

    protected void LoadMap()
    {
        MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
        if (mapDataBase == null || mapDataBase.className != "MG_PerlinField")
        {
            print("LoadMap: �S���s�ɸ��");
            return;
        }

        print("LoadMap: ���s�ɸ�� !!!!");
        MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
        print("LoaMap Size: " + new Vector2Int(mapData.mapCellWidthH, mapData.mapCellHeightH));
        //print("LoaMap RandomShift: " + new Vector2(mapData.randomShiftX, mapData.randomShiftY));
        //print("LoadMap map64: " + mapData.mapMask64);

        mapCellWidthH = mapData.mapCellWidthH;
        mapCellHeightH = mapData.mapCellHeightH;
        CellSize = mapData.CellSize;
        NoiseScaleOn256 = mapData.NoiseScaleOn256;
        highRatio = mapData.highRatio;
        lowRatio = mapData.lowRatio;
        randomShiftX = mapData.randomShiftX;
        randomShiftY = mapData.randomShiftY;

    }
    // ======= ���������a�Ϫ��O���M�^�_ =======================


    protected void SaveExploreMap()
    {
        MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
        if (mapDataBase == null || mapDataBase.className != "MG_PerlinField")
        {
            print("SaveExploreMap: �S���s�ɸ�� MapSaveData�A���B�z");
            return;
        }

        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (theMiniMap)
        {
            //print("�}�l�����x�s MiniMap");

            Texture2D maskT = theMiniMap.GetMaskTexture();
            // ���Texture2D���Ҧ�����
            Color[] pixels = maskT.GetPixels();

            // ��l��alphaData�Ʋ�
            byte[] alphaData = new byte[pixels.Length];

            // �N�C�ӹ�����alpha���ഫ���r�`�ƾ�
            for (int i = 0; i < pixels.Length; i++)
            {
                alphaData[i] = (byte)(pixels[i].a * 255);
            }
            //print("Byte �`�q: " + alphaData.Length);

            byte[] compressedAlphaData = OneUtility.CompressData(alphaData);
            //print("���Y�� Byte �`�q" + compressedAlphaData.Length);
            //print("���Y�᤺�e: " + compressedAlphaData);

            string compressedAlpha64Text = System.Convert.ToBase64String(compressedAlphaData);
            print("SaveExploreMap ���Y�᪺��r�q: " + compressedAlpha64Text.Length);
            //print("TEXT: " + compressedAlpha64Text);


            MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
            mapData.mapMask64 = compressedAlpha64Text;
        }
    }

    protected void LoadExploreMap()
    {
        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (!theMiniMap)
        {
            return;
        }
        MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
        if (mapDataBase == null || mapDataBase.className != "MG_PerlinField")
        {
            print("LoadExploreMap : �S���s�ɸ�ơA���B�z");
            return;
        }

        MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
        if (mapData.mapMask64 == null || mapData.mapMask64 == "")
        {
            print("�Ū��a�ϱ�����T: " + mapData.mapMask64);
            return;
        }

        print("LoadExploreMap: ��쪺��r���Y��ơAByte �`�q: " + mapData.mapMask64.Length);
        //print("��쪺��r���Y��Ƥ��e: " + mapData.mapMask64);
        byte[] compressedAlphaData = System.Convert.FromBase64String(mapData.mapMask64);
        //print("��쪺���Y��ơAByte �`�q: " + compressedAlphaData.Length);
        //print("���Y���: " + compressedAlphaData);

        byte[] alphaData = OneUtility.DeCompressData(compressedAlphaData);
        //print("�����Y��ơAByte �`�q: " + alphaData.Length);

        Texture2D maskT = theMiniMap.GetMaskTexture();
        Color[] pixels = maskT.GetPixels();
        if (pixels.Length != alphaData.Length)
        {
            print("�����Y���~�A���J�������a�Ϥj�p�M��ڤ��� !! " + alphaData.Length + " / " + pixels.Length);
            return;
        }

        //print("�}�l��g�����a��....");
        for (int i=0; i<pixels.Length; i++)
        {
            pixels[i].a = alphaData[i] / 255.0f;
        }
        maskT.SetPixels(pixels);
        maskT.Apply();
        //print("����....");
    }

}
