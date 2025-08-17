using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SimpleBloomEffect : MonoBehaviour
{
    [Header("Shader/Material")]
    public Shader bloomShader;
    private Material _mat;

    [Header("Bloom Settings")]
    [Range(0, 5)] public float threshold = 1.0f;
    [Range(0, 1)] public float softKnee = 0.5f;
    [Range(0, 5)] public float intensity = 1.2f;

    [Tooltip("降採樣層級（1 = 1/2, 2 = 1/4, 3 = 1/8 解析度）")]
    [Range(1, 4)] public int downsample = 2;

    [Tooltip("模糊半徑（像素數，實際會乘上 texel）")]
    [Range(0, 8)] public float radius = 2.0f;

    [Tooltip("Blur 迭代次數（每次都做 H+V）")]
    [Range(1, 6)] public int iterations = 3;

    void OnEnable()
    {
        if (bloomShader == null)
        {
            bloomShader = Shader.Find("Custom/SimpleBloom");
        }
        if (_mat == null && bloomShader != null)
        {
            _mat = new Material(bloomShader) { hideFlags = HideFlags.HideAndDontSave };
        }
    }

    void OnDisable()
    {
        if (_mat) DestroyImmediate(_mat);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_mat == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        // 傳 bloom 參數
        _mat.SetFloat("_Threshold", threshold);
        _mat.SetFloat("_SoftKnee", softKnee);
        _mat.SetFloat("_Intensity", intensity);
        _mat.SetFloat("_Radius", radius);

        // —— 避免誤判：在 Prefilter/Blur 前，先把 _BloomTex 清掉 ——
        _mat.SetTexture("_BloomTex", Texture2D.blackTexture);

        // 臨時 RT 建議格式（避免直接複用 src.format 造成 alias）
        bool hdr = (src.format == RenderTextureFormat.DefaultHDR ||
                    src.format == RenderTextureFormat.ARGBHalf ||
                    src.format == RenderTextureFormat.ARGBFloat);
        var rtFormat = hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

        int w = Mathf.Max(2, src.width >> downsample);
        int h = Mathf.Max(2, src.height >> downsample);

        var rtA = RenderTexture.GetTemporary(w, h, 0, rtFormat);
        var rtB = RenderTexture.GetTemporary(w, h, 0, rtFormat);

        // —— 保險：若兩者意外同一張，另外再取一張 ——
        if (ReferenceEquals(rtA, rtB))
        {
            RenderTexture.ReleaseTemporary(rtB);
            rtB = RenderTexture.GetTemporary(w, h, 0, rtFormat);
        }

        // Pass 0: Prefilter（src -> rtA）
        Graphics.Blit(src, rtA, _mat, 0);

        // Pass 1/2: Blur（ping-pong 在 rtA/rtB）
        bool useAasSrc = true;
        for (int i = 0; i < iterations; i++)
        {
            // H
            if (useAasSrc) Graphics.Blit(rtA, rtB, _mat, 1);
            else Graphics.Blit(rtB, rtA, _mat, 1);
            useAasSrc = !useAasSrc;

            // V
            if (useAasSrc) Graphics.Blit(rtA, rtB, _mat, 2);
            else Graphics.Blit(rtB, rtA, _mat, 2);
            useAasSrc = !useAasSrc;
        }

        RenderTexture bloomTex = useAasSrc ? rtA : rtB;

        // —— 只在 Composite 前，把 bloomTex 指回去；前面 Blur 時不要掛著它 ——
        _mat.SetTexture("_BloomTex", bloomTex);

        // Pass 3: Composite（src + bloom -> dest）
        Graphics.Blit(src, dest, _mat, 3);

        RenderTexture.ReleaseTemporary(rtA);
        RenderTexture.ReleaseTemporary(rtB);
    }
}
