using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollManager : MonoBehaviour
{
    public Transform[] DollSlots;

    public bool FixDirection = false;
    public bool DirectionByMove = true;
    public bool AllDirectioin = false;  //不限於四方向
    public float RotateSpeed = -1.0f;

    //protected DollSkillManager theSkillManager;

    protected int slotNum = 0;
    protected Doll[] dolls;
    protected Quaternion targetRt = Quaternion.identity;

    //當 DollRecovery 還在進行時，不能加入 Doll
    //protected bool waitDollRecovery = false;

    //public void SetIsWaitDollRecovery(bool isWait) { waitDollRecovery = isWait; }
    //public DollSkillManager GetDollSkillManager() { return theSkillManager; }

    //private void Awake()
    //{
    //    theSkillManager = GetComponent<DollSkillManager>();
    //}

    //只抓取還在隊伍中活耀的，還沒死掉的
    public List<Doll> GetActiveDolls()
    {
        List<Doll> theList = new List<Doll>();
        foreach (Doll d in dolls)
        {
            if (d && d.gameObject.activeInHierarchy)
            {
                theList.Add(d);
            }
        }
        return theList;
    }

    //只抓取還在隊伍中活耀的，不論死活
    public List<Doll> GetDolls()
    {
        List<Doll> theList = new List<Doll>();
        foreach (Doll d in dolls)
        {
            if (d)
            {
                theList.Add(d);
            }
        }
        return theList;
    }


    public virtual bool HasEmpltySlot(DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    {
        for (int i=0; i<slotNum; i++)
        {
            if (dolls[i] == null)
            {
                return true;
            }
        }
        return false;
    }
    
    public virtual void GetDollGroupAndIndex(Doll doll, ref int group, ref int index) { group = -1; index = -1; }


    public virtual bool AddOneDollWithGivenPosition(Doll doll, int group, int index)
    {
        return AddOneDoll(doll);
    }

    public virtual bool AddOneDoll(Doll doll/*, DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT*/)
    {
        float positionRatio = 0;
        switch (doll.positionType)
        {
            case DOLL_POSITION_TYPE.MIDDLE:
                positionRatio = 0.5f;
                break;
            case DOLL_POSITION_TYPE.BACK:
                positionRatio = 1.0f;
                break;
        }

        float minDis = 2.0f;
        int bestFound = -1;
        for (int i=0; i<slotNum; i++)
        {
            if ( dolls[i] == null && DollSlots[i] != null)
            {
                float ratio = (float)i / (float)(slotNum-1);
                float dis = Mathf.Abs(positionRatio - ratio);
                if (dis < minDis)
                {
                    bestFound = i;
                    minDis = dis;
                }
                //dolls[i] = doll;
                //return DollSlots[i];
            }
        }

        if (bestFound >= 0)
        {
            dolls[bestFound] = doll;
            doll.SetSlot(DollSlots[bestFound]);
            //return DollSlots[bestFound];
            return true;
        }

        return false;
    }

    public virtual void OnDollTempDeath(Doll doll)
    {

    }

    public virtual void OnDollRevive( Doll doll)
    {

    }

    public virtual void OnDollDestroy( Doll doll)
    {

    }

    // Start is called before the first frame update
    virtual protected void Start()
    {
        foreach (Transform tm in DollSlots)
        {
            if (tm == null)
            {
                print("Error !!! Empty Slot !!");
            }
        }
        slotNum = DollSlots.Length;
        dolls = new Doll[slotNum];

        BattleSystem.GetHUD().RegisterDollLayoutUI(this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (RotateSpeed > 0.0f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRt, Time.deltaTime * RotateSpeed);
        }
        else
        {
            transform.rotation = targetRt;
        }
    }

    public void SetMasterPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void ForceSetDirection(float angle)      //通常是針對 Trigger 等強制換方向的時候
    {
        //Quaternion targetRt = Quaternion.Euler(0, angle, 0);
        targetRt = Quaternion.Euler(0, angle, 0);

        transform.rotation = targetRt;

    }

    public void TrySetDirection(Vector3 dir)           //這是針對一般操控行為
    {
        if (FixDirection)
            return;
        if (AllDirectioin)
        {
            targetRt = Quaternion.LookRotation(dir);

        }
        else
        {
            float angle = 0;
            if (dir.x > dir.z)
            {
                if (dir.x > -dir.z)
                    angle = 90;
                else
                    angle = 180;
            }
            else if (dir.x < -dir.z)
                angle = 270;
            targetRt = Quaternion.Euler(0, angle, 0);
        }
        transform.rotation = targetRt;
    }

    public void SetMasterDirection( Vector3 dir, FaceFrontType faceType )
    {
        if (FixDirection || !DirectionByMove)
            return;

        //Quaternion targetRt;
        if (AllDirectioin)
        {
            targetRt = Quaternion.LookRotation(dir);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRt, Time.deltaTime * rotateSpeed);

            //transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir);
        }
        else
        {
            float angle = 0;

            switch (faceType)
            {
                case FaceFrontType.RIGHT:
                    angle = 90.0f;
                    break;
                case FaceFrontType.DOWN:
                    angle = 180.0f;
                    break;
                case FaceFrontType.LEFT:
                    angle = 270.0f;
                    break;
            }
            //transform.rotation = Quaternion.Euler(0, angle, 0);
            targetRt = Quaternion.Euler(0, angle, 0);
        }

        //if (RotateSpeed > 0.0f)
        //{
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRt, Time.deltaTime * RotateSpeed);
        //}
        //else
        //{
        //    transform.rotation = targetRt;
        //}
    }

    public void ForceMoveAll()
    {
        for (int i=0; i<dolls.Length; i++)
        {
            if (dolls[i])
            {
                dolls[i].transform.position = DollSlots[i].position;
            }
        }
    }

    //將玩家的行為傳達給 Doll 們
    public void OnPlayerAttack(Vector3 target)
    {
        foreach ( Doll d in dolls)
        {
            if (d)
            {
                d.OnPlayerAttack(target);
            }
        }
    }

    public void OnPlayerShoot(Vector3 target)
    {
        foreach (Doll d in dolls)
        {
            if (d)
            {
                d.OnPlayerShoot(target);
            }
        }
    }

    public void OnPlayerDead()
    {
        foreach (Doll d in dolls)
        {
            if (d)
            {
                d.OnPlayerDead();
            }
        }
    }

}
