using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHintSquare : MonoBehaviour
{
    public GameObject LU, UU, RU;
    public GameObject LL, CC, RR;
    public GameObject LD, DD, RD;

    protected float step = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSize(float width, float length)
    {
        float wScale = (width - step - step) / step;
        float hScale = (length - step - step) / step;
        float wPos = (wScale * 0.5f + 0.5f) * step;
        float hPos = (hScale * 0.5f + 0.5f) * step;
        //transform.localScale = new Vector3 (width, length, 1.0f);
        UU.transform.localScale = DD.transform.localScale = new Vector3(wScale, 1.0f, 1.0f);
        LL.transform.localScale = RR.transform.localScale = new Vector3(1.0f, hScale, 1.0f);
        CC.transform.localScale = new Vector3(wScale, hScale, 1.0f);

        LU.transform.localPosition = new Vector3(-wPos, hPos, 0);
        UU.transform.localPosition = new Vector3(0, hPos, 0);
        RU.transform.localPosition = new Vector3(wPos, hPos, 0);

        LL.transform.localPosition = new Vector3(-wPos, 0, 0);
        RR.transform.localPosition = new Vector3(wPos, 0, 0);

        LD.transform.localPosition = new Vector3(-wPos, -hPos, 0);
        DD.transform.localPosition = new Vector3(0, -hPos, 0);
        RD.transform.localPosition = new Vector3(wPos, -hPos, 0);

    }
}
