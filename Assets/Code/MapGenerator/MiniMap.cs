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
    public int border = 6;


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

        int x1 = Mathf.Max(x - cRange - border, 0);
        int x2 = Mathf.Min(x + cRange + border, tWidth);
        int y1 = Mathf.Max(y - cRange - border, 0);
        int y2 = Mathf.Min(y + cRange + border, tHeight);
        Color[] maskColors = maskTexture.GetPixels(x1, y1, x2 - x1, y2 - y1);

        int kValue = cRange;
        float b = (float)border;
        for (int i = x1; i < x2; i++)
        {
            for (int j = y1; j < y2; j++)
            {
                int d = Mathf.Max(0, Mathf.Abs(i - x) - kValue) + Mathf.Max(0, Mathf.Abs(j - y) - kValue);
                float a = Mathf.Min(1.0f, d / b);
                //float a = Mathf.Max(Mathf.Max(Mathf.Abs(i - x), Mathf.Abs(j - y)) - kValue, 0) / (float)b;
                maskColors[(j - y1) * (x2 - x1) + (i - x1)].a = Mathf.Min(a, maskColors[(j - y1) * (x2 - x1) + (i - x1)].a);
            }
        }
        maskTexture.SetPixels(x1, y1, x2 - x1, y2 - y1, maskColors);

        /*/
        for (int i=-cRange; i<= cRange; i++)
        {
            for (int j = -cRange; j <= cRange; j++)
            {
                int xi = x + i; int yj = y + j;
                if (xi >= 0 && xi < tWidth && yj >= 0 && yj < tHeight)
                {
                    if (i<-cRange+2||i>=cRange-2|| j < -cRange + 2 || j >= cRange - 2) 
                    {
                        Color c = maskTexture.GetPixel(xi, yj);
                        c.a = Mathf.Min(c.a, 0.5f);
                        maskTexture.SetPixel(xi, yj, c);
                    }
                    else
                        maskTexture.SetPixel(xi, yj, new Color(0, 0, 0, 0));
                }
            }
        }
        //*/

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
        miniMapTexture.filterMode = FilterMode.Point;
        miniMapTexture.wrapMode = TextureWrapMode.Clamp;

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

        if (BattleSystem.GetInstance().fadeBlocker)
            BattleSystem.GetInstance().fadeBlocker.gameObject.SetActive(false);

        renderCamera.Render();
        miniMapTexture.ReadPixels(new Rect(0, 0, tWidth, tHeight), 0, 0);

        if (BattleSystem.GetInstance().fadeBlocker)
            BattleSystem.GetInstance().fadeBlocker.gameObject.SetActive(true);

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
            miniMapTexture.filterMode = FilterMode.Point;
            miniMapTexture.wrapMode = TextureWrapMode.Clamp;
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
        maskTexture.filterMode = FilterMode.Point;
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
            SmallUIMaskImage.color = new Color(Camera.main.backgroundColor.r, Camera.main.backgroundColor.g, Camera.main.backgroundColor.b);
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


    public Texture2D GetMaskTexture() { return maskTexture; }


    public string EncodeMaskTexture()
    {
        if (maskTexture == null)
            return null;
        // 獲取Texture2D的所有像素
        Color[] pixels = maskTexture.GetPixels();

        // 初始化alphaData數組
        byte[] alphaData = new byte[pixels.Length];

        // 將每個像素的alpha值轉換為字節數據
        for (int i = 0; i < pixels.Length; i++)
        {
            alphaData[i] = (byte)(pixels[i].a * 255);
        }
        //print("Byte 總量: " + alphaData.Length);

        byte[] compressedAlphaData = OneUtility.CompressData(alphaData);
        //print("壓縮後 Byte 總量" + compressedAlphaData.Length);
        //print("壓縮後內容: " + compressedAlphaData);

        string compressedAlpha64Text = System.Convert.ToBase64String(compressedAlphaData);
        //print("SaveExploreMap 壓縮後的文字量: " + compressedAlpha64Text.Length);
        //print("TEXT: " + compressedAlpha64Text);

        return compressedAlpha64Text;
    }

    public void DecodeMaskTexture(string codeStr)
    {

        byte[] compressedAlphaData = System.Convert.FromBase64String(codeStr);
        //print("找到的壓縮資料，Byte 總量: " + compressedAlphaData.Length);
        //print("壓縮資料: " + compressedAlphaData);

        byte[] alphaData = OneUtility.DeCompressData(compressedAlphaData);
        //print("解壓縮資料，Byte 總量: " + alphaData.Length);

        //Texture2D maskT = theMiniMap.GetMaskTexture();
        Color[] pixels = maskTexture.GetPixels();
        if (pixels.Length != alphaData.Length)
        {
            print("解壓縮錯誤，載入的探索地圖大小和實際不符 !! " + alphaData.Length + " / " + pixels.Length);
            return;
        }

        //print("開始改寫探索地圖....");
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].a = alphaData[i] / 255.0f;
        }
        maskTexture.SetPixels(pixels);
        maskTexture.Apply();
    }

    //private void OnGUI()
    //{
    //    //GUI.TextArea(new Rect(100, 100, 100, 50), "( " + px + ", " + py + " )");
    //    //GUI.TextArea(new Rect(100, 100, 100, 50), "Time: " + pTime);
    //}
}
