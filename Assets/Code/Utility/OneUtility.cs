using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO;

class OneUtility
{
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
    //
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

}