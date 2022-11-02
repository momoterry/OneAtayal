using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollManager : MonoBehaviour
{
    public Transform[] DollSlots;

    public bool FixDirection = false;
    public bool AllDirectioin = false;  //不限於四方向
    public float RotateSpeed = -1.0f;

    protected int slotNum = 0;
    protected Doll[] dolls;

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

    //public Transform GetEmptySlot(DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    //{
    //    return AddOneDoll(null, positionType);
    //}

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
    
    public virtual Transform AddOneDoll(Doll doll, DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    {
        float positionRatio = 0;
        switch (positionType)
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
            return DollSlots[bestFound];
        }

        return null;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMasterPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void ForceSetDirection(float angle)
    {
        Quaternion targetRt = Quaternion.Euler(0, angle, 0);

        //if (RotateSpeed > 0.0f)
        //{
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRt, Time.deltaTime* RotateSpeed);
        //}
        //else
        {
            transform.rotation = targetRt;
        }
    }

    public void SetMasterDirection( Vector3 dir, FaceFrontType faceType )
    {
        if (FixDirection)
            return;

        Quaternion targetRt;
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

        if (RotateSpeed > 0.0f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRt, Time.deltaTime * RotateSpeed);
        }
        else
        {
            transform.rotation = targetRt;
        }
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
