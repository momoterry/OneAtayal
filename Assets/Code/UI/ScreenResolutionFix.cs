using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenResolutionFix : MonoBehaviour
{
    [SerializeField]protected bool alsoFixMainCamera = false;
    protected CanvasScaler theScaler;
    protected int currSceenWidth = 0;
    protected int currSceenHeight = 0;
    protected float targetRatio = 0.5f;
    protected float cameraDefaultSize = 10.0f;


    private void Awake()
    {
        theScaler = GetComponent<CanvasScaler>();
        if (theScaler != null)
        {
            targetRatio = theScaler.referenceResolution.x / theScaler.referenceResolution.y;
            //print("ScreenResolutionFix => Target Ratio = " + targetRatio);
        }
        if (alsoFixMainCamera)
        {
            cameraDefaultSize = Camera.main.orthographicSize;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        CheckScreenResolution();
    }

    // Update is called once per frame
    void Update()
    {
        CheckScreenResolution();
    }

    protected void CheckScreenResolution()
    {
        if (theScaler == null)
            return;
        int width = Camera.main.pixelWidth;
        int height = Camera.main.pixelHeight;
        if (width != currSceenWidth || height != currSceenHeight)
        {
            currSceenWidth = width;
            currSceenHeight = height;
            float currRatio = (float)currSceenWidth / (float)currSceenHeight;
            if (currRatio < targetRatio)
            {
                theScaler.matchWidthOrHeight = 0;
                if (alsoFixMainCamera)
                {
                    Camera.main.orthographicSize = cameraDefaultSize * targetRatio / currRatio; //太細的螢幕得調整主 Camera
                }
            }
            else
            {
                theScaler.matchWidthOrHeight = 1.0f;
                if (alsoFixMainCamera)
                {
                    Camera.main.orthographicSize = cameraDefaultSize;
                }
            }
            //print("Reset UI Resolution to "+theScaler.matchWidthOrHeight + ", ratio = "+ (float)currSceenWidth / (float)currSceenHeight);
        }
    }
}
