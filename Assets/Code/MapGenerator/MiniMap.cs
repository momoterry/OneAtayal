using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Image MiniMapImage;
    public Image MaskImage;

    public int alignH = 2;
    public int cRange = 10;

    protected Sprite miniMapSprite;
    protected Texture2D miniMapTexture;

    protected Sprite maskSprite;
    protected Texture2D maskTexture;

    protected int width;
    protected int height;
    protected int tWidth;
    protected int tHeight;
    protected int xMin;
    protected int yMin;

    protected int align;
    protected float alignF;

    // Start is called before the first frame update
    void Start()
    {
        align = alignH + alignH;
        alignF = (float)align;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerControllerBase pc = BattleSystem.GetPC();
        if (!pc)
            return;
        Vector3 pos = BattleSystem.GetPC().transform.position;
        int x = (int)pos.x;
        int y = (int)pos.z;
        RevealMap(x, y);
    }

    protected int px, py;

    public void RevealMap(int _x, int _y)
    {
        //Align
        _x = (int)Mathf.Floor((_x + alignH) / alignF) * align;
        _y = (int)Mathf.Floor((_y + alignH) / alignF) * align; ;

        int x = _x - xMin;
        int y = _y - yMin;

        for (int i=-cRange; i<= cRange; i++)
        {
            for (int j = -cRange; j <= cRange; j++)
            {
                int xi = x + i; int yj = y + j;
                if (xi >= 0 && xi < tWidth && yj >= 0 && yj < tHeight)
                    maskTexture.SetPixel(xi, yj, new Color(0, 0, 0, 0));
            }
        }

        //maskTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
        maskTexture.Apply();
    }

    public void CreateMinMap(OneMap theMap)
    {
        width = theMap.mapWidth;
        height = theMap.mapHeight;
        xMin = theMap.xMin;
        yMin = theMap.yMin;
        tWidth = Mathf.NextPowerOfTwo(width);
        tHeight = Mathf.NextPowerOfTwo(height);
        Color[] colorMap = new Color[tWidth * tHeight];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int value = theMap.GetValue(x + theMap.xMin, y + theMap.yMin);
                Color color = Color.black;
                switch (value)
                {
                    case 1:
                        color = new Color(0, 0.8f, 0);
                        break;
                    case 2:
                        color = new Color(1.0f, 0.8f, 0);
                        break;
                    case 3:
                        color = new Color(0, 0.65f, 0);
                        break;
                    case 4:
                        color = new Color(0, 0.5f, 0);
                        break;
                    case 5:
                        color = new Color(0, 0.35f, 0);
                        break;
                    case 6:
                        color = new Color(0, 0.2f, 0);
                        break;

                }
                //color = new Color(0, 0.5f, 0);
                colorMap[y * tWidth + x] = color;
            }
        }

        miniMapTexture = new Texture2D(tWidth, tHeight);
        miniMapTexture.SetPixels(colorMap);
        miniMapTexture.Apply();
        miniMapSprite = Sprite.Create(miniMapTexture, new Rect(0, 0, width, height), Vector2.zero);

        if (MiniMapImage)
        {
            MiniMapImage.sprite = miniMapSprite;
        }

        //Mask
        for (int y = 0; y < tHeight; y++)
        {
            for (int x = 0; x < tWidth; x++)
            {
                colorMap[y * tWidth + x] = Color.black;
            }
        }
        maskTexture = new Texture2D(tWidth, tHeight);
        maskTexture.SetPixels(colorMap);
        maskTexture.Apply();
        maskSprite = Sprite.Create(maskTexture, new Rect(0, 0, width, height), Vector2.zero);
        if (MaskImage)
        {
            MaskImage.sprite = maskSprite;
        }
    }

    //private void OnGUI()
    //{
    //    GUI.TextArea(new Rect(100, 100, 100, 50), "( " + px + ", " + py + " )");
    //}
}
