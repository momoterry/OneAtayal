using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//高度控制器
//為了模擬 3D 的高度，把設定的高度資訊 (理論上是 Y 軸轉換成 Z 軸高度去控制)

public class HeightController : MonoBehaviour
{
    public GameObject mainBody;

    protected float cuuuHeight = 0;     //所有在地面上預設都是 0
    protected Vector3 mainBodyOffset;


    void Awake()
    {
        if (mainBody)
        {
            mainBodyOffset = mainBody.transform.position - transform.position;
        }    
    }

    virtual public void SetHeight(float height)
    {
        cuuuHeight = height;
        mainBody.transform.position = transform.position + mainBodyOffset + Vector3.forward * height;
    }
}
