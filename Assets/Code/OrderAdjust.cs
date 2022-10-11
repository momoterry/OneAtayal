using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAdjust : MonoBehaviour
{
    public float bias = 0.0f;
    public bool onlyAdjustOnStart = false;

    protected float updatePeriod = 0.2f;
    protected float currTime = 0;

    //2022/0620: 使用 Renderer 而不是 SpriteRenderer, 以支援 TextMesh
    protected Renderer[] allSprite;
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y + zBias);

        allSprite = GetComponentsInChildren<Renderer>(true);
        SetupOrder();
        if (onlyAdjustOnStart)
            enabled = false;
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void LateUpdate()
    {
        currTime += Time.deltaTime;
        if (currTime >= updatePeriod)
        {
            SetupOrder();
            currTime = 0;
        }
    }

    private void SetupOrder()
    {
#if XZ_PLAN
        int order = -(int)((transform.position.z - bias) * 10.0f);
#else
        int order = -(int)(transform.position.y * 10.0f);
#endif
        foreach (Renderer sr in allSprite)
        {
            sr.sortingOrder = order;
//#if XZ_PLAN
//            sr.sortingOrder = -(int)((sr.transform.position.z - bias) * 10.0f);
//#else
//            sr.sortingOrder = -(int)(sr.transform.position.y * 10.0f);
//#endif
        }

    }
}
