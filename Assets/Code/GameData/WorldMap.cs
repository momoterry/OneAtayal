using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WorldMapSaveData
{
    public ZonePF[] zones;
}

public class WorldMap : MonoBehaviour
{
    public string forestScene;

    protected Dictionary<Vector2Int, ZonePF> zones = new Dictionary<Vector2Int, ZonePF>();
    protected ZonePF currTravleingZone = null;
    protected Vector2Int currTravelingIndex;
    protected Vector3 currEnterPosition;
    protected float currEnterAngle;

    public void LoadData(WorldMapSaveData savedData)
    {
        ClearAllZones();
        if (savedData.zones != null)
        {
            for (int i = 0; i < savedData.zones.Length; i++)
            {
                zones.Add(savedData.zones[i].worldIndex, savedData.zones[i]);
            }
        }
    }

    public WorldMapSaveData SaveData()
    {
        print("WorldMapSaveData 開始存資料: " + zones.Count);
        WorldMapSaveData savedData = new WorldMapSaveData();
        savedData.zones = new ZonePF[zones.Count];
        int i = 0;
        foreach (KeyValuePair<Vector2Int, ZonePF> pe in zones)
        {
            savedData.zones[i] = pe.Value;
            i++;
        }
        return savedData;
    }

    public void Init()
    {
        ClearAllZones();

        int hCellNum = 30;
        int cellSize = 2;
        int NoiseScaleOn256 = 10;
        int edgeWidth = 6;
        float zWidth = (hCellNum + hCellNum) * cellSize;
        float zHeight = (hCellNum + hCellNum) * cellSize;

        float xShiftStep = (hCellNum + hCellNum - edgeWidth ) * (float)NoiseScaleOn256 / 256.0f;
        float yShiftStep = (hCellNum + hCellNum - edgeWidth ) * (float)NoiseScaleOn256 / 256.0f;

        float xShiftCenter = Random.Range(0, (float)(20 * NoiseScaleOn256) );  //先亂寫
        float yShiftCenter = Random.Range(0, (float)(20 * NoiseScaleOn256) );  //先亂寫
        //print("Random!!")

        int xMin = -1; int xMax = 1;
        int yMin = -1; int yMax = 1;
        for (int y = yMin; y <= yMax; y++)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                ZonePF zone = new ZonePF();
                zone.ID = "WORLD_(" + x + "," + y + ")";
                zone.worldIndex = new Vector2Int(x, y);
                zone.worldPos = new Vector2(x * zWidth, y * zHeight);
                zone.width = zWidth;
                zone.height = zHeight;
                zone.scene = forestScene;
                zone.perlinShiftX = xShiftCenter + ( x * xShiftStep );
                zone.perlinShiftY = yShiftCenter + ( y * yShiftStep );
                zone.cellSize = cellSize;
                zone.edgeWidth = edgeWidth;
                zone.W = (x != xMin);
                zone.E = (x != xMax);
                zone.S = (y != yMin);
                zone.N = (y != yMax);
                zones.Add(zone.worldIndex, zone);

            }
        }
    }

    public void GotoZone(Vector2Int zoneIndex, Vector3 enterPosition, float faceAngel = 0)
    {
        if (zones.ContainsKey(zoneIndex))
        {
            currTravelingIndex = zoneIndex;
            currTravleingZone = zones[zoneIndex];
            currEnterPosition = enterPosition;
            currEnterAngle = faceAngel;
            //print("即將傳送到世界地圖的 " + currTravleingZone.ID);
            BattleSystem.RegisterAwakeCallBack(SetupBattleSystem);
            //SceneManager.LoadScene(currTravleingZone.scene);
            BattleSystem.GetInstance().OnGotoScene(currTravleingZone.scene);
        }
        else
        {
            print("ERRPR!!!! WorldMap 沒有對應的 Zone !!" + zoneIndex);
        }
    }

    void SetupBattleSystem(BattleSystem bs)
    {
        if (bs != null)
        {
            //print("WorldMap:SetupBattleSystem MG = " + bs.theMG);
            MG_PerlinField mgPF = (MG_PerlinField)bs.theMG;
            if (mgPF == null)
            {
                print("ERROR!!!! 無法處理的 MG 類型，必須是 MG_PerlinField");
                return;
            }


            GameObject o = new GameObject("NEW_PLAYER_POS");        //從新創建一個 Dummy 以免影響初始營地結構
            o.transform.position = currEnterPosition;
            bs.initPlayerPos = o.transform;
            bs.initPlayerDirAngle = currEnterAngle;
            print("SetupBattleSystem: " + currEnterPosition + " -- " + currEnterAngle);

            mgPF.SetZone(currTravleingZone);
            currTravleingZone = null;
        }
    }

    protected void ClearAllZones()
    {
        zones.Clear();
        currTravleingZone = null;
    }

    //static public GameObject CreateZoneEdgeTrigger(Vector2Int toZoneIndex, Vector3 center, float width, float height)
    //{
    //    GameObject o = new GameObject("ZoneTrigger_" + toZoneIndex);
    //    o.transform.position = center;
    //    BoxCollider bc = o.AddComponent<BoxCollider>();
    //    bc.size = new Vector3(width, 2.0f, height);
    //    bc.isTrigger = true;
    //    AreaTG atg = o.AddComponent<AreaTG>();
    //    atg.TriggerTargets = new GameObject[1];
    //    atg.TriggerTargets[0] = o;
    //    WorldPortal wp = o.AddComponent<WorldPortal>();
    //    wp.toWorldZoneIndex = toZoneIndex;
    //    wp.messageHint = true;
    //    return o;
    //}

}
