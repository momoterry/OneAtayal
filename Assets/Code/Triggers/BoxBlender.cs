using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBlender : MonoBehaviour
{
    public bool blendHori = true;   //垂直改變為 false

    protected BoxCollider areaBox;
    protected PlayerControllerBase thePlayerIn = null;

    protected float v1;
    protected float v2;

    protected float ratio;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        areaBox = GetComponent<BoxCollider>();
        if (!areaBox)
        {
            print("ERROR!! BoxBlender need a BoxCollider !!!!!");
            return;
        }

        if (blendHori)
        {
            v1 = areaBox.transform.position.x + areaBox.center.x - (areaBox.size.x * 0.5f);
            v2 = areaBox.transform.position.x + areaBox.center.x + (areaBox.size.x * 0.5f);
        }
        else
        {
            v1 = areaBox.transform.position.z + areaBox.center.z - (areaBox.size.z * 0.5f);
            v2 = areaBox.transform.position.z + areaBox.center.z + (areaBox.size.z * 0.5f);
        }

    }

    protected virtual void CalcBlendRatio()
    {
        if (thePlayerIn && areaBox)
        {
            float vo;
            if (blendHori)
            {
                vo = thePlayerIn.transform.position.x;
            }
            else
            {
                vo = thePlayerIn.transform.position.z;
            }

            ratio = (vo - v1) / (v2 - v1);

            ratio = Mathf.Clamp(ratio, 0, 1.0f);
        }
    }

    protected virtual void ApplyBlendResult() { }
    protected virtual void OnStartBlend() { }
    protected virtual void OnEndBlend() { }

    // Update is called once per frame
    void Update()
    {
        if (thePlayerIn)
        {
            CalcBlendRatio();
            ApplyBlendResult();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        thePlayerIn = other.GetComponent<PlayerControllerBase>();

        OnStartBlend();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (thePlayerIn)
        {
            //離開時,確保一定是極端值
            CalcBlendRatio();
            ratio = ratio > 0.5f ? 1.0f : 0;
            ApplyBlendResult();

            OnEndBlend();
        }
        thePlayerIn = null;
    }
}
