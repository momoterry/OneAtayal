using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// �ޯ�Ѳ��;����򥻤���
//=================================================
public class BookEquipEnhancerBase : MonoBehaviour
{

}

//=================================================
// �a���]�k�ĪG�ޯ�Ѳ��;�
//=================================================



public class BookEquipEnhancer : BookEquipEnhancerBase
{
    //�r���r���w�q
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
