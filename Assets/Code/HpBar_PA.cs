using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HpBar_PA : MonoBehaviour
{
    public GameObject fillBar;
    public GameObject fillBarMP;

    public int pixelWidth = 16;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SetValue(float hp, float hpMax)
    {
        SetFillRate( hp / hpMax);
    }

    public virtual void SetMPValue(float mp, float mpMax)
    {
        SetManaFillRate(mp / mpMax);
    }

    public void SetFillRate( float rate)
    {
        //無條件捨去
        float pixelWidthF = (float)pixelWidth;
        float fillPixelCountF = Mathf.Floor(rate * pixelWidthF);
        float scaleWidth = fillPixelCountF / pixelWidthF;
        float shiftValue = (pixelWidthF - fillPixelCountF) / 32.0f;

        if (fillBar)
        {
            fillBar.transform.localPosition = new Vector3(-shiftValue, 0, 0);
            fillBar.transform.localScale = new Vector3(scaleWidth, 1.0f, 1.0f);
        }
    }

    public void SetManaFillRate(float rate)
    {
        //無條件捨去
        float pixelWidthF = (float)pixelWidth;
        float fillPixelCountF = Mathf.Floor(rate * pixelWidthF);
        float scaleWidth = fillPixelCountF / pixelWidthF;
        float shiftValue = (pixelWidthF - fillPixelCountF) / 32.0f;

        if (fillBarMP)
        {
            fillBarMP.transform.localPosition = new Vector3(-shiftValue, 0, 0);
            fillBarMP.transform.localScale = new Vector3(scaleWidth, 1.0f, 1.0f);
        }
    }

    public void SetWorldPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }
}
