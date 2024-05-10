using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class OneUtility
{
    // ========================= 有關方向
    public static DIRECTION GetReverseDIR(DIRECTION dir)
    {
        switch (dir)
        {
            case DIRECTION.U:
                return DIRECTION.D;
            case DIRECTION.D:
                return DIRECTION.U;
            case DIRECTION.L:
                return DIRECTION.R;
            case DIRECTION.R:
                return DIRECTION.L;
        }
        return DIRECTION.NONE;
    }

    // ========================= 有關隨機排序
    //改寫自 AI
    static public void Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    //改寫自 AI
    static public void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // ========================= 從 N 個數中隨機取不重複
    static public int[] GetRandomNonRepeatNumbers(int minInclude, int maxExclude, int count)
    {
        //Debug.Log("GetRandomNonRepeatNumbers count... " + count);
        int range = maxExclude - minInclude;
        if (range < count)
            return null;
        int[] choose = new int[count];
        int[] all = new int[range];
        for (int i = 0; i < range; i++)
            all[i] = minInclude + i;
        //int loop = count < range ? count : range - 1;
        for (int i = 0; i < count; i++)
        {
            int k = count < range ? Random.Range(i + 1, range) : range-1;
            choose[i] = all[k];
            all[k] = all[i];
            all[i] = choose[i];
            //Debug.Log(choose[i]);
        }
        return choose;
    }

    // ========================= 有關隨機取點 (取自 AI)
    public static Vector2 GetRandomPointInRects(List<Rect> rects)
    {
        // 計算總面機
        float totalArea = 0f;
        foreach (Rect rect in rects)
        {
            totalArea += rect.width * rect.height;
        }

        // 隨機挑中一個矩型
        float randomValue = Random.Range(0f, totalArea);
        float accumulatedArea = 0f;
        Rect selectedRect = rects[0];
        foreach (Rect rect in rects)
        {
            float area = rect.width * rect.height;
            if (randomValue <= accumulatedArea + area)
            {
                selectedRect = rect;
                break;
            }
            accumulatedArea += area;
        }

        // 從挑中的矩型中選一個點
        float x = Random.Range(selectedRect.xMin, selectedRect.xMax);
        float y = Random.Range(selectedRect.yMin, selectedRect.yMax);
        return new Vector2(x, y);
    }

    public static List<Vector3> Get3DRandomPointsInRectBand(Vector3 vCenter, float Width, float Height, float bandWidth, int totalNum)
    {
        List<Vector3> points = new List<Vector3>();
        List<Rect> rects = new List<Rect>();
        rects.Add(new Rect(Width * -0.5f, Height * 0.5f - bandWidth, Width - bandWidth, bandWidth));
        rects.Add(new Rect(Width * 0.5f - bandWidth, Height * -0.5f + bandWidth, bandWidth, Height - bandWidth));
        for (int i = 0; i < totalNum; i++)
        {
            Vector3 rp = GetRandomPointInRects(rects);
            rp = Random.Range(0, 2) == 0 ? rp : -rp;
            points.Add(new Vector3(rp.x, 0, rp.y) + vCenter);

        }
        return points;
    }
    // ========================= 有關其它隨機 ======================================

    public static int FloatToRandomInt(float f)
    {
        int min = Mathf.FloorToInt(f);
        float rd = Random.Range(0.0f, 1.0f);
        if ((f - min) >= rd)
            return min+1;
        return min;
    }

    // ========================= 有關加密 ======================================
    public static byte[] EncryptString(string plainText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.Mode = CipherMode.ECB;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new System.IO.MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                    csEncrypt.FlushFinalBlock();
                    return msEncrypt.ToArray();
                }
            }
        }
    }

    public static string DecryptString(string encryptedText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.Mode = CipherMode.ECB;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new System.IO.MemoryStream(encryptedBytes))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    byte[] plainBytes = new byte[encryptedBytes.Length];
                    int decryptedByteCount = csDecrypt.Read(plainBytes, 0, plainBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes, 0, decryptedByteCount);
                }
            }
        }
    }
    // ================================================== 

    // 有關壓縮 (RLE)
    static public byte[] CompressData(byte[] data)
    {
        List<byte> compressedList = new List<byte>();

        int count = 1;
        for (int i = 1; i < data.Length; i++)
        {
            if (data[i] == data[i - 1])
            {
                count++;
                if (count == 255)
                {
                    compressedList.Add(data[i]);
                    compressedList.Add((byte)255);
                    count = 1;
                    i++;
                }
            }
            else
            {
                compressedList.Add(data[i - 1]);
                compressedList.Add((byte)count);
                count = 1;
            }
        }

        // Add the last run
        compressedList.Add(data[data.Length - 1]);
        compressedList.Add((byte)count);

        // Convert list to array
        return compressedList.ToArray();
    }

    static public byte[] DeCompressData(byte[] compressedData)
    {
        List<byte> decompressedList = new List<byte>();

        for (int i = 0; i < compressedData.Length; i += 2)
        {
            byte value = compressedData[i];
            int count = compressedData[i + 1];

            for (int j = 0; j < count; j++)
            {
                decompressedList.Add(value);
            }
        }

        return decompressedList.ToArray();
    }

    //==================================================================
    // Sprite 合成 (參考 ChatGPT)
    //==================================================================
    static public Sprite BlendSprite(Sprite iconA, Sprite iconB, Material _mat = null )
    {
        // Create a new RenderTexture to render the sprites onto
        Debug.Log("iconA: " + iconA.rect);
        RenderTexture rt = new RenderTexture((int)iconA.rect.width, (int)iconA.rect.height, 0);
        Graphics.Blit(iconA.texture, rt, _mat);

        // Render the second sprite onto the RenderTexture
        Graphics.Blit(iconB.texture, rt, _mat);

        // Create a new Texture2D to read the RenderTexture data
        Texture2D combinedTexture = new Texture2D(rt.width, rt.height);
        combinedTexture.filterMode = FilterMode.Point;
        combinedTexture.wrapMode = TextureWrapMode.Clamp;
        RenderTexture.active = rt;
        combinedTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        combinedTexture.Apply();
        RenderTexture.active = null;

        // Create a new sprite using the combined texture
        Sprite iconC = Sprite.Create(combinedTexture, new Rect(0, 0, combinedTexture.width, combinedTexture.height), new Vector2(0.5f, 0.5f), 16.0f);
        //iconC.f
        return iconC;
    }


    //==================================================================
    // 其它工具
    //==================================================================
    public class DisjointSetUnion
    {
        int size;
        int[] P;
        public void Init(int _size)
        {
            P = new int[_size];
            for (int i = 0; i < _size; i++)
                P[i] = i;
        }

        public int Find(int _id)
        {
            if (_id == P[_id])
                return _id;
            else
                return Find(P[_id]);
        }

        public void Union(int a, int b)
        {
            int Fa = Find(a);
            int Fb = Find(b);
            if (Fa != Fb)
            {
                P[Fb] = P[Fa];
            }
        }
    }
}


