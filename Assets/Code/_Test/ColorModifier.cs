using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorModifier : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // 要改變顏色的 SpriteRenderer
    public float HueAdjust = 30.0f;

    void Start()
    {
        // 檢查是否有指定 SpriteRenderer，如果沒有，則試圖從這個物體的組件中獲取
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // 獲取 Sprite 的紋理
            Texture2D originalTexture = spriteRenderer.sprite.texture;
            int width = originalTexture.width;
            int height = originalTexture.height;

            // 創建一張新的 Texture2D 來存儲變色後的像素
            Texture2D newTexture = new Texture2D(width, height);
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.filterMode = FilterMode.Point;

            Color[] originalPixels = originalTexture.GetPixels();
            Color[] newPixels = new Color[originalPixels.Length];

            // 遍歷每個像素，修改 HUE 值
            for (int i = 0; i < originalPixels.Length; i++)
            {
                // 將每個像素轉換為 HSB 色彩空間
                Color.RGBToHSV(originalPixels[i], out float h, out float s, out float v);

                // 增加 HUE 值 60
                h = (h + HueAdjust / 360f) % 1f;

                // 將修改後的 HSB 色彩空間轉換回 RGB
                newPixels[i] = Color.HSVToRGB(h, s, v);
                newPixels[i].a = originalPixels[i].a;
            }

            // 將變色後的像素設置到新的 Texture2D 中
            newTexture.SetPixels(newPixels);
            newTexture.Apply();

            // 將新的 Texture2D 設置給 SpriteRenderer
            spriteRenderer.sprite = Sprite.Create(newTexture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);

        }
    }
}
