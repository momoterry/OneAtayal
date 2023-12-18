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
        float zWidth = (hCellNum + hCellNum) * cellSize;
        float zHeight = (hCellNum + hCellNum) * cellSize;

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
                zone.perlinShiftX = 0.5f;
                zone.perlinShiftY = 0.5f;
                zone.cellSize = cellSize;

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
            print("�Y�N�ǰe��@�ɦa�Ϫ� " + currTravleingZone.ID);
            BattleSystem.RegisterAwakeCallBack(SetupBattleSystem);
            SceneManager.LoadScene(currTravleingZone.scene);
        }
        else
        {
            print("ERRPR!!!! WorldMap �S�������� Zone !!" + zoneIndex);
        }
    }

    void SetupBattleSystem(BattleSystem bs)
    {
        if (bs != null)
        {
            print("WorldMap:SetupBattleSystem MG = " + bs.theMG);
            MG_PerlinField mgPF = (MG_PerlinField)bs.theMG;
            if (mgPF == null)
            {
                print("ERROR!!!! �L�k�B�z�� MG �����A�����O MG_PerlinField");
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
