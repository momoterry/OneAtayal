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

    //���o�ƭȬ���
    public float GetHPMax() { return HP_Max; }
    public float GetMPMax() { return MP_Max; }
    public float GetHP() { return hp; }
    public float GetMP() { return mp; }
    public float GetATTACK() { return Attack; }
    public virtual void InitStatus() {}
    public virtual bool IsKilled(){return false;}

    // �ɯŬ��� : TODO: ���q Base �����A���o������������
    //2022/9/22 �ɥR
    //�{�b�w�g���A�ϥγo���������ɪ��� Pick Up �i�H���O�d�@��
    //���d���Ȯ��ܱj������ϥ� (�ܱj�u����P���d�A�b���������)
    public virtual bool DoHpUp(){return true;}

    public virtual bool DoAtkUp(){return true;}


    //Doll ����
    protected DollManager myDollManager;
    public GameObject DollManagerRef;
    public DollManager GetDollManager() { return myDollManager; }

    //���ʡB���ʵ�����
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

    //���F SkillBase ����o������T��
    public virtual Vector3 GetFaceDir() { return Vector3.forward; }
    public virtual FaceFrontType GetFaceFront() { return FaceFrontType.DOWN; }


    // �����欰����
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

