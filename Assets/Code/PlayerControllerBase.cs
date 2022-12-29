using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FaceFrontType
{
    UP,
    RIGHT,
    DOWN,
    LEFT,
}
public class PlayerControllerBase : MonoBehaviour
{
    public float initFaceDirAngle = 180.0f;
    public CharacterData theCharData;

    protected float HP_Max = 100.0f;
    protected float MP_Max = 100.0f;
    protected float hp = 100.0f;
    protected float mp = 100.0f;
    protected float Attack = 50.0f;

    //取得數值相關
    public float GetHPMax() { return HP_Max; }
    public float GetMPMax() { return MP_Max; }
    public float GetHP() { return hp; }
    public float GetMP() { return mp; }
    public float GetATTACK() { return Attack; }
    public virtual void InitStatus() {}
    public virtual bool IsKilled(){return false;}

    // 升級相關 : TODO: 應從 Base 移除，但得拿掉對應物件
    //2022/9/22 補充
    //現在已經不再使用這類的直接升物件 Pick Up 可以先保留作為
    //關卡中暫時變強的物件使用 (變強只限於同關卡，在換關後消失)
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
    public virtual void SetupFaceDirByAngle(float angle) { }
    public virtual void SetInputActive(bool enable) { }
    public virtual void SaySomthing(string str) { }

    //為了 SkillBase 能取得相關資訊用
    public virtual Vector3 GetFaceDir() { return Vector3.forward; }
    public virtual FaceFrontType GetFaceFront() { return FaceFrontType.DOWN; }


    // 攻擊行為相關
    public virtual void OnAttack() { }
    public virtual void OnShoot() { }
    public virtual void OnShootTo() { }
    public virtual void OnAttackTo(Vector3 target) { }
    public virtual void DoShootTo(Vector3 target) { }
    public virtual void DoHeal(float healNum) { }
    public virtual float DoHeal(float healAbsoluteNum, float healRatio) { return 0; }
    public virtual void DoUseMP(float mpCost) 
    {
        mp -= mpCost;
        if (mp < 0)
        {
            mp = 0;
        }
    }
    public virtual void DoHealMana(float healNum)
    {
        mp += healNum;
        if ( mp > MP_Max)
        {
            mp = MP_Max;
        }
    }
    public virtual void OnSkill( int index) {}

    public virtual void OnKillEnemy(Enemy e) {}
}

