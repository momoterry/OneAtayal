using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenResolutionFix : MonoBehaviour
{
    protected CanvasScaler theScaler;
    protected int currSceenWidth = 0;
    protected int currSceenHeight = 0;
    protected float targetRatio = 0.5f;

    private void Awake()
    {
        theScaler = GetComponent<CanvasScaler>();
        if (theScaler != null)
        {
            targetRatio = theScaler.referenceResolution.x / theScaler.referenceResolution.y;
            //print("ScreenResolutionFix => Target Ratio = " + targetRatio);
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
            if ((float)currSceenWidth / (float)currSceenHeight < targetRatio)
            {
                theScaler.matchWidthOrHeight = 0;
            }
            else
            {
                theScaler.matchWidthOrHeight = 1.0f;
            }
            //print("Reset UI Resolution to "+theScaler.matchWidthOrHeight + ", ratio = "+ (float)currSceenWidth / (float)currSceenHeight);
        }
    }
}
