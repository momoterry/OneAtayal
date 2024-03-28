using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// 技能書產生器的基本介面
//=================================================
public class BookEquipEnhancerBase : MonoBehaviour
{

}

//=================================================
// 帶有魔法效果技能書產生器
//=================================================



public class BookEquipEnhancer : BookEquipEnhancerBase
{
    //字首字尾定義
    [System.Serializable]
    public class EnhanceInfo
    {
        public DOLL_BUFF_TYPE type;
        public int value;
        public string text;
    }
    public EnhanceInfo[] enhancePrefixs;
    public EnhanceInfo[] enhanceSuffixs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
