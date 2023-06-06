using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HScrollCamera : MonoBehaviour
{
    protected float cameraSize = 5.0f;
    protected Camera theCamera;

    protected float targetRatio = 16.0f / 9.0f;

    private void Awake()
    {
        theCamera = GetComponent<Camera>();
        cameraSize = theCamera.orthographicSize;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        int width = Camera.main.pixelWidth;
        int height = Camera.main.pixelHeight;
        float currRatio = (float)width / (float)height;
        theCamera.orthographicSize = Mathf.Max( cameraSize * targetRatio / currRatio, cameraSize );
    }
}
