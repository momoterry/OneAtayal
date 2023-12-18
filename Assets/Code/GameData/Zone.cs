using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneBase
{
    public string ID;
    public Vector2Int worldIndex;       //以世界中心為 (0,0) 的 Zone 座標
    public Vector2 worldPos;            //以世界中心為 (0,0,0) 的 World 座標
    public float width;
    public float height;
}

public class ZonePF: ZoneBase           //以 Perlin Filed 為基底的 Zone
{
    public string scene;
    public float perlinShiftX;
    public float perlinShiftY;
    public int cellSize;
    public int edgeWidth;
}


