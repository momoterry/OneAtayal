using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    public Text fpsText;
    // Start is called before the first frame update
    float timeTotal = 0;
    int frameCount = 0;
    float fFPS = 0;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeTotal += Time.deltaTime;
        frameCount++;
        if (timeTotal > 0.5f)
        {
            fFPS = frameCount / timeTotal;
            timeTotal = 0;
            frameCount = 0;
            if (fpsText)
                fpsText.text = fFPS.ToString("F2");
        }
    }
}
