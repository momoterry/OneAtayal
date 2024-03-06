using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorModifier : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // �n�����C�⪺ SpriteRenderer
    public float HueAdjust = 30.0f;

    void Start()
    {
        // �ˬd�O�_�����w SpriteRenderer�A�p�G�S���A�h�չϱq�o�Ӫ��骺�ե����
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // ��� Sprite �����z
            Texture2D originalTexture = spriteRenderer.sprite.texture;
            int width = originalTexture.width;
            int height = originalTexture.height;

            // �Ыؤ@�i�s�� Texture2D �Ӧs�x�ܦ�᪺����
            Texture2D newTexture = new Texture2D(width, height);
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.filterMode = FilterMode.Point;

            Color[] originalPixels = originalTexture.GetPixels();
            Color[] newPixels = new Color[originalPixels.Length];

            // �M���C�ӹ����A�ק� HUE ��
            for (int i = 0; i < originalPixels.Length; i++)
            {
                // �N�C�ӹ����ഫ�� HSB ��m�Ŷ�
                Color.RGBToHSV(originalPixels[i], out float h, out float s, out float v);

                // �W�[ HUE �� 60
                h = (h + HueAdjust / 360f) % 1f;

                // �N�ק�᪺ HSB ��m�Ŷ��ഫ�^ RGB
                newPixels[i] = Color.HSVToRGB(h, s, v);
                newPixels[i].a = originalPixels[i].a;
            }

            // �N�ܦ�᪺�����]�m��s�� Texture2D ��
            newTexture.SetPixels(newPixels);
            newTexture.Apply();

            // �N�s�� Texture2D �]�m�� SpriteRenderer
            spriteRenderer.sprite = Sprite.Create(newTexture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);

        }
    }
}
