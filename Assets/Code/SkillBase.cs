using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    public float damageRatio = 1.0f;
    public float duration = 0.2f;   //技能施放期間 (無法操作 )
    public float manaCost = 0;
    public float coolDown = 1.0f;

    public Sprite icon;

    protected GameObject theCaster;

    public virtual void InitCasterInfo(GameObject oCaster) { theCaster = oCaster; }
    public virtual bool DoStart() { 
        print("SkillBase !!!!!!");
        return true;
    }
}
