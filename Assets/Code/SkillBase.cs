using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    public float damageRatio = 1.0f;
    public float duration = 0.2f;   //�ޯ�I����� (�L�k�ާ@ )
    public float manaCost = 0;

    
    public virtual void DoStart() { print("SkillBase !!!!!!"); }
}
