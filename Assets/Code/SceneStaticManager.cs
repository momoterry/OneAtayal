using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStaticManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //把所有物件的 Z 設為和 Y 同值
        for ( int i=0; i<transform.childCount; i++)
        {
            Transform tm = transform.GetChild(i);
            tm.position = new Vector3(tm.position.x, tm.position.y, tm.position.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
