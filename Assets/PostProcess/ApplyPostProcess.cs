using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ApplyPostProcess : MonoBehaviour
{
    public Material grayscaleMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (grayscaleMaterial != null)
        {
            Graphics.Blit(src, dest, grayscaleMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
