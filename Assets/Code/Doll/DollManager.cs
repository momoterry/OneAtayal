using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollManager : MonoBehaviour
{
    public Transform[] DollSlots;

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

    public Transform GetEmptySlot(DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    {
        return AddOneDoll(null, positionType);
    }

    public Transform AddOneDoll(Doll doll, DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
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

    public void SetMasterDirection( Vector3 dir, FaceFrontType faceType )
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
        transform.rotation = Quaternion.Euler(0, angle, 0);
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

    //?N???a?????????F?? Doll ??
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
