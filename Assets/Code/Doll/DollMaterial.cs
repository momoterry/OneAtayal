using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//針對「地城野生巫靈」，在破關後能轉換成素材的功能
//直接掛在 Doll 身上
//由 DollCollector 來生成

public class DollMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConvertToMaterial()
    {
        print("OnConvertToMaterial.... " + gameObject.name);
    }
}
