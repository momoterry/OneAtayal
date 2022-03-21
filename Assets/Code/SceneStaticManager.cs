using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStaticManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool runAgainAtStart = false;
    void Start()
    {
        if (runAgainAtStart)
            SetupSorting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupSorting()
    {
        //把所有物件的 Z 設為和 Y 同值
        for ( int i=0; i<transform.childCount; i++)
        {
            //順便做 Snap
            Transform tm = transform.GetChild(i);
            float mx = tm.position.x;
            float my = tm.position.y;
            float mz = tm.position.z;
            mx = Mathf.Round(mx * 4.0f) * 0.25f;
            my = Mathf.Round(my * 4.0f) * 0.25f;
            mz = Mathf.Round(mz * 4.0f) * 0.25f;
#if XZ_PLAN
            tm.position = new Vector3(mx, 0, mz);
#else
            tm.position = new Vector3(mx, my, 0);
#endif
        }

        SpriteRenderer[] allSprite = GetComponentsInChildren<SpriteRenderer>();
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
