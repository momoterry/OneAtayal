using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MG_PerlinEnemy : MG_PerlinNoise
{
    public GameObject stoneSpawnerRef;
    public Image MiniMapImageTest;
    protected override void FillTiles()
    {
        base.FillTiles();

        //for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        //{
        //    for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
        //    {
        //        if (theCellMap.GetValue(x, y) == (int)MY_VALUE.LOW)
        //        {
        //            if (Random.value < 0.1f) 
        //            {
        //                Vector2Int c = theCellMap.GetCellCenterCoord(x, y);
        //                Vector3 pos = new Vector3(c.x, 0, c.y);
        //                BattleSystem.SpawnGameObj(stoneSpawnerRef, pos);
        //            }
        //        }
        //    }
        //}

        if (MiniMapImageTest)
        {
            float tSep = Time.realtimeSinceStartup;
            //Texture2D newTexture = CreateTextures();
            MiniMapImageTest.sprite = CreateMiniMapSprite();
            tSep = Time.realtimeSinceStartup - tSep;
            print("小地圖創建時間: " + tSep);
        }
    }

    Sprite CreateMiniMapSprite()
    {
        OneMap theMap = theCellMap.GetOneMap();
        int tWidth = Mathf.NextPowerOfTwo(theMap.mapWidth);
        int tHeight = Mathf.NextPowerOfTwo(theMap.mapHeight);
        Color[] colorMap = new Color[tWidth * tHeight];

        for (int y = 0; y < theMap.mapHeight; y++)
        {
            for (int x = 0; x < theMap.mapWidth; x++)
            {
                int value = theMap.GetValue(x + theMap.xMin, y + theMap.yMin);
                Color color = Color.black;
                switch (value)
                {
                    case (int)MY_VALUE.NORMAL:
                        color = Color.green;
                        break;
                    case (int)MY_VALUE.LOW:
                        color = Color.yellow;
                        break;
                    case (int)MY_VALUE.HIGH:
                    case (int)MY_VALUE.HIGH_2:
                    case (int)MY_VALUE.HIGH_3:
                        color = new Color(0, 0.5f, 0);
                        break;

                }
                //color = new Color(0, 0.5f, 0);
                colorMap[y * tWidth + x] = color;
            }
        }

        Texture2D texture = new Texture2D(tWidth, tHeight);
        texture.SetPixels(colorMap);
        texture.Apply();
        Sprite s = Sprite.Create(texture, new Rect(0, 0, theMap.mapWidth, theMap.mapHeight), Vector2.zero);
        return s;
    }

    Texture2D CreateTextures()
    {
        OneMap theMap = theCellMap.GetOneMap();
        int tWidth = Mathf.NextPowerOfTwo(theMap.mapWidth);
        int tHeight = Mathf.NextPowerOfTwo(theMap.mapHeight);
        Color[] colorMap = new Color[tWidth * tHeight];

        
        for (int y = 0; y < theMap.mapHeight; y++)
            {
            for (int x = 0; x < theMap.mapWidth; x++)
                {
                int value = theMap.GetValue(x + theMap.xMin, y + theMap.yMin);
                Color color = Color.black;
                switch (value)
                {
                    case (int)MY_VALUE.NORMAL:
                        color = Color.green;
                        break;
                    case (int)MY_VALUE.LOW:
                        color = Color.yellow;
                        break;
                    case (int)MY_VALUE.HIGH:
                    case (int)MY_VALUE.HIGH_2:
                    case (int)MY_VALUE.HIGH_3:
                        color = new Color(0, 0.5f, 0);
                        break;

                }
                //color = new Color(0, 0.5f, 0);
                colorMap[y * tWidth + x] = color;
            }
        }

        Texture2D texture = new Texture2D(tWidth, tHeight);
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    //Texture2D CreateTextures_Old()
    //{
    //    OneMap theMap = theCellMap.GetOneMap();
    //    int tWidth = Mathf.NextPowerOfTwo(theMap.mapWidth);
    //    int tHeight = Mathf.NextPowerOfTwo(theMap.mapHeight);
    //    print("CreateTextures:" + theMap.mapWidth + ", " + theMap.mapHeight + " =>" + tWidth + ", " + tHeight);
    //    Texture2D texture = new Texture2D(tWidth, tHeight, TextureFormat.ARGB32, false);
    //    for (int x = 0; x < theMap.mapWidth; x++)
    //    {
    //        for (int y = 0; y < theMap.mapHeight; y++)
    //        {
    //            int value = theMap.GetValue(x+theMap.xMin, y+theMap.yMin);
    //            Color color = Color.black;
    //            switch (value)
    //            {
    //                case (int)MY_VALUE.NORMAL:
    //                    color = Color.green;
    //                    break;
    //                case (int)MY_VALUE.LOW:
    //                    color = Color.yellow;
    //                    break;
    //                case (int)MY_VALUE.HIGH:
    //                case (int)MY_VALUE.HIGH_2:
    //                case (int)MY_VALUE.HIGH_3:
    //                    color = new Color(0, 0.5f, 0);
    //                    break;

    //            }
    //            //color = new Color(0, 0.5f, 0);
    //            texture.SetPixel(x, y, color);
    //        }
    //    }
    //    texture.Apply();
    //    return texture;
    //}

}
