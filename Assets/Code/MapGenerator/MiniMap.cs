using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Image BigMapImage;
    public Image BigMaskImage;
    public Image SmallMapImage;
    public Image SmallMaskImage;
    public Image SmallUIMaskImage;
    public GameObject BigMapRoot;
    public RectTransform bigMapPlayerPoint;

    public delegate Color GetColorCB(int value);

    public int alignH = 2;
    public int cRange = 10;

    //Test
    public Camera renderCamera;
    public bool useRenderCamera = false;

    protected Sprite miniMapSprite;
    protected Texture2D miniMapTexture;

    protected Sprite maskSprite;
    protected Texture2D maskTexture;

    protected GetColorCB getColorCB = null;

    protected Vector2 mapCenter;

    protected int width;
    protected int height;
    protected int tWidth;
    protected int tHeight;
    protected int xMin;
    protected int yMin;

    protected int align;
    protected float alignF;

    //CurrMap 相關
    protected Texture2D currMapTexture;
    protected int currMapWidth = 64;
    protected int currMapHeight = 64;
    protected float CMScaleX = 1.0f;
    protected float CMScaleY = 1.0f;
    protected Vector2 CMmoveScale;      //小地圖移動的比例
    protected Vector2 BMmoveScale;      //大地圖移動的比例

    protected float currTime = 0;

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
        UpdateCurrMap(pos.x, pos.z);

        currTime += Time.deltaTime;
        if (currTime > 0.2f) 
        {
            currTime = 0;
            int x = (int)pos.x;
            int y = (int)pos.z;
            RevealMap(x, y);
        }
    }


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

        maskTexture.Apply();
    }

    protected Color DefaultGetColor(int value)
    {
        switch (value)
        {
            case 1:
                return new Color(0, 0.8f, 0);
            case 2:
                return new Color(1.0f, 0.8f, 0);
            case 3:
            case 4:
            case 5:
            case 6:
                return new Color(0, 0.2f, 0);
        }
        return Color.black;
    }

    public void CreateMiniMapByCamera(OneMap theMap)
    {
        mapCenter = theMap.mapCenter;
        width = theMap.mapWidth;
        height = theMap.mapHeight;

        tWidth = tHeight = Mathf.NextPowerOfTwo(Mathf.Max(theMap.mapWidth, theMap.mapHeight));
        width = height = tWidth;
        xMin = theMap.mapCenter .x - tWidth/2;
        yMin = theMap.mapCenter .y - tHeight/2;
        miniMapTexture = new Texture2D(tWidth, tHeight);

        RenderTexture rtBackUp = RenderTexture.active;
        RenderTexture miniMapRT = new RenderTexture(tWidth, tHeight, 24);

        //Camera renderCamera = new();
        //print("renderCamera:" + renderCamera);
        renderCamera.orthographic = true;
        renderCamera.transform.rotation = Quaternion.Euler(90.0f, 0, 0);
        renderCamera.targetTexture = miniMapRT;
        renderCamera.orthographicSize = tWidth / 2;
        renderCamera.transform.position = new Vector3(theMap.mapCenter.x, 10, theMap.mapCenter.y);
        RenderTexture.active = renderCamera.targetTexture;

        renderCamera.Render();
        miniMapTexture.ReadPixels(new Rect(0, 0, tWidth, tHeight), 0, 0);

        miniMapTexture.Apply();
        miniMapSprite = Sprite.Create(miniMapTexture, new Rect(0, 0, tWidth, tHeight), Vector2.zero);

        //Destroy(renderCamera.gameObject);
        RenderTexture.active = rtBackUp;
    }


    public void CreateMiniMap(OneMap theMap, GetColorCB _getColorCB = null)
    {
        gameObject.SetActive(true);

        getColorCB = _getColorCB;
        if (getColorCB == null)
            getColorCB = DefaultGetColor;

        if (renderCamera && useRenderCamera)
        {
            CreateMiniMapByCamera(theMap);
            renderCamera.gameObject.SetActive(false);
        }
        else
        {
            mapCenter = theMap.mapCenter;
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
                    Color color = getColorCB(value);
                    colorMap[y * tWidth + x] = color;
                }
            }
            miniMapTexture = new Texture2D(tWidth, tHeight);
            miniMapTexture.SetPixels(colorMap);
            miniMapTexture.Apply();
            miniMapSprite = Sprite.Create(miniMapTexture, new Rect(0, 0, width, height), Vector2.zero);
        }


        if (BigMapImage)
        {
            BigMapImage.sprite = miniMapSprite;
            //BMmoveScale.x = M
            //Vector2 bigMapSize = BigMapImage.rectTransform.rect.size;
            BMmoveScale = BigMapImage.rectTransform.rect.size / new Vector2(width, height);
        }

        //Mask
        Color[] colorMask = new Color[tWidth * tHeight];
        for (int y = 0; y < tHeight; y++)
        {
            for (int x = 0; x < tWidth; x++)
            {
                colorMask[y * tWidth + x] = Color.black;
            }
        }
        maskTexture = new Texture2D(tWidth, tHeight);
        maskTexture.SetPixels(colorMask);
        maskTexture.Apply();
        maskSprite = Sprite.Create(maskTexture, new Rect(0, 0, width, height), Vector2.zero);
        if (BigMaskImage)
        {
            BigMaskImage.sprite = maskSprite;
        }

        //小型地圖
        //currMapTexture = new Texture2D(currMapWidth, currMapHeight);
        //Sprite currMapSprite = Sprite.Create(currMapTexture, new Rect(0, 0, currMapWidth, currMapHeight), Vector2.zero);

        CMScaleX = (float)width / (float)currMapWidth;
        CMScaleY = (float)height / (float)currMapHeight;
        if (SmallMapImage)
        {
            SmallMapImage.sprite = miniMapSprite;
            CMmoveScale = SmallMapImage.rectTransform.rect.size / new Vector2(currMapWidth, currMapHeight);
            //print("CMmoveScale: " + CMmoveScale);
            SmallMapImage.rectTransform.localScale = new Vector2(CMScaleX, CMScaleY);
            if (SmallMaskImage)
            {
                SmallMaskImage.sprite = maskSprite;
            }
        }

        //小地圖底色
        if (SmallUIMaskImage)
        {
            SmallUIMaskImage.color = Camera.main.backgroundColor;
        }
    }

    protected void UpdateCurrMap(float x, float y)
    {
        Vector2 relativePos = new Vector2(x, y) - mapCenter;
        if (SmallMapImage)
        {
            SmallMapImage.rectTransform.localPosition = -relativePos * CMmoveScale;
        }
        if (bigMapPlayerPoint)
        {
            bigMapPlayerPoint.localPosition = relativePos * BMmoveScale;
        }
    }

    public void OnOpenBigMap()
    {
        if (BigMapRoot)
            BigMapRoot.SetActive(true);
    }

    public void OnCloseBigMap()
    {
        if (BigMapRoot)
            BigMapRoot.SetActive(false);
    }

    //private void OnGUI()
    //{
    //    //GUI.TextArea(new Rect(100, 100, 100, 50), "( " + px + ", " + py + " )");
    //    //GUI.TextArea(new Rect(100, 100, 100, 50), "Time: " + pTime);
    //}
}
