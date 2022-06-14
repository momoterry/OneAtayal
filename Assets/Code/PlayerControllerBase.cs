using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBase : MonoBehaviour
{
    public float initFaceDirAngle = 180.0f;

    protected float HP_Max = 100.0f;
    protected float MP_Max = 100.0f;
    protected float hp = 100.0f;
    protected float mp = 100.0f;
    protected float Attack = 50.0f;

    //取得數值相關
    public float GetHPMax() { return MP_Max; }
    public float GetMPMax() { return MP_Max; }
    public float GetHP() { return hp; }
    public float GetMP() { return mp; }
    public float GetATTACK() { return Attack; }
    public virtual void InitStatus() {}
    public virtual bool IsKilled(){return false;}

    // 升級相關 : TODO: 應從 Base 移除，但得拿掉對應物件
    public virtual bool DoHpUp(){return true;}

    public virtual bool DoAtkUp(){return true;}


    //Doll 相關
    protected DollManager myDollManager;
    public GameObject DollManagerRef;
    public DollManager GetDollManager() { return myDollManager; }

    //互動、移動等相關
    public virtual void OnActionKey() { }
    public virtual bool OnRegisterActionObject(GameObject obj) { return false; }
    public virtual bool OnUnregisterActionObject(GameObject obj) { return false; }
    public virtual void OnMoveToPosition(Vector3 target) { }
    public virtual void ForceStop(bool stop = true) { }
    public virtual void DoTeleport(Vector3 position, float faceAngle) { }
    public virtual void SetupFaceDir(Vector3 dir) { }
    public virtual void SetInputActive(bool enable) { }

    //為了 SkillBase 能取得相關資訊用
    public virtual Vector3 GetFaceDir() { return Vector3.forward; }


    // 攻擊行為相關
    public virtual void OnAttack() { }
    public virtual void OnShoot() { }
    public virtual void OnShootTo() { }
    public virtual void OnAttackTo(Vector3 target) { }
    public virtual void DoShootTo(Vector3 target) { }
    public virtual void DoHeal(float healNum) { }
    public virtual void DoUseMP(float mpCost) 
    {
        mp -= mpCost;
        if (mp < 0)
        {
            mp = 0;
        }
    }
    public virtual void OnSkill( int index) {}
}

