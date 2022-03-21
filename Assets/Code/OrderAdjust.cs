using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAdjust : MonoBehaviour
{
    public float bias = 0.0f;

    protected float updatePeriod = 0.2f;
    protected float currTime = 0;

    protected SpriteRenderer[] allSprite;
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y + zBias);

        allSprite = GetComponentsInChildren<SpriteRenderer>();
        SetupOrder();
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
        foreach (SpriteRenderer sr in allSprite)
        {
#if XZ_PLAN
            sr.sortingOrder = -(int)(sr.transform.position.z * 10.0f);
#else
            sr.sortingOrder = -(int)(sr.transform.position.y * 10.0f);
#endif
        }
    }
}
