using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMap : MonoBehaviour
{
    public string forestScene;

    protected Dictionary<Vector2Int, ZonePF> zones = new Dictionary<Vector2Int, ZonePF>();
    protected KeyValuePair<Vector2Int, ZonePF> currTravelingZoneP;
    protected ZonePF currTravleingZone = null;
    protected Vector2Int currTravelingIndex;
    protected Vector3 currEnterPosition;
    protected float currEnterAngle;

    public void Init()
    {
        ClearAllZones();

        int hCellNum = 30;
        int cellSize = 2;
        int NoiseScaleOn256 = 10;
        int edgeWidth = 6;
        float zWidth = (hCellNum + hCellNum) * cellSize;
        float zHeight = (hCellNum + hCellNum) * cellSize;

        float xShiftStep = (hCellNum + hCellNum - edgeWidth - edgeWidth) * (float)NoiseScaleOn256 / 256.0f;
        float yShiftStep = (hCellNum + hCellNum - edgeWidth - edgeWidth) * (float)NoiseScaleOn256 / 256.0f;

        float xShiftCenter = 10.5f;
        float yShiftCenter = 10.5f;

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
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

                zones.Add(zone.worldIndex, zone);
            }
        }
    }

    public void GotoZone(Vector2Int zoneIndex, Vector2 enterPosition, float faceAngel = 0)
    {
        if (zones.ContainsKey(zoneIndex))
        {
            currTravelingIndex = zoneIndex;
            currTravleingZone = zones[zoneIndex];
            currEnterPosition = enterPosition;
            currEnterAngle = faceAngel;
            //print("即將傳送到世界地圖的 " + currTravleingZone.ID);
            BattleSystem.RegisterAwakeCallBack(SetupBattleSystem);
            SceneManager.LoadScene(currTravleingZone.scene);
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

            mgPF.SetZone(currTravleingZone);
            currTravleingZone = null;
        }
    }

    protected void ClearAllZones()
    {
        zones.Clear();
    }

}
