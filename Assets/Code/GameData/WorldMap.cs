using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    protected Dictionary<Vector2, ZonePF> filedZones = new Dictionary<Vector2, ZonePF>();

    public void CreateZones()
    {
        int hCellNum = 20;
        float zWidth = (hCellNum + hCellNum) * 2;
        float zHeight = (hCellNum + hCellNum) * 2;

        for (int y = -1; y <= 1; y++)
        {
            for (int x=-1; x<=1; x++)
            {
                ZonePF zone = new ZonePF();
                zone.ID = "WORLD_(" + x + "," + y + ")";
                zone.worldIndex = new Vector2Int(x, y);
                zone.worldPos = new Vector2(x * zWidth, y * zHeight);
                zone.width = zWidth;
                zone.height = zHeight;

                filedZones.Add(zone.worldIndex, zone);
            }
        }
    }
}
