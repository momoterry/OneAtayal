using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuffApplierBase : MonoBehaviour
{
    protected Dictionary<DOLL_BUFF_TYPE, List<DollBuffBase>> buffPools = new Dictionary<DOLL_BUFF_TYPE, List<DollBuffBase>>();


    public void ApplyBuffer(DollBuffBase buff)
    {

    }

}

public class BuffApplierDoll : BuffApplierBase
{

}
