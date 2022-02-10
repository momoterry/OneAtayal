using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public enum DROPITEM_TYPE
    {
        NONE,
        HEAL_POTION,
        POWERUP_MAXHP,
        POWERUP_ATTACK
    }
    public DROPITEM_TYPE itemType;
    public DROPITEM_TYPE GetItemType() { return itemType; }

    private float pickupRange = 5.0f;
    private float droppingTime = 0.5f;

    //拾取動畫相關
    private Vector3 posFlyAwayStart;
    private float maxTime = 0.1f;

    //狀態
    enum DROP_STATE
    {
        NONE,
        DROPPING,
        WAIT,
        GONE,   //如果要作拾取動畫
    }
    DROP_STATE currState = DROP_STATE.NONE;
    DROP_STATE nextState = DROP_STATE.NONE;

    float stateTime = 0;

    static private List<DropItem> theList = new List<DropItem>();
    static public void ClearAllDropItem()
    {
        foreach (DropItem o in theList)
        {
            Destroy(o.gameObject);
        }
        theList.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        theList.Add(this);
        nextState = DROP_STATE.DROPPING;
    }

    private void OnDestroy()
    {
        theList.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (currState != nextState)
        {
            //TODO: enter/exit state
            stateTime = 0;
            currState = nextState;
            return;
        }

        stateTime += Time.deltaTime;
        switch (currState)
        {
            case DROP_STATE.DROPPING:
                UpdateDropping();
                break;
            case DROP_STATE.GONE:
                UpdateFlyAway();
                break;
        }
    }

    public void DoDrop()
    {
        Animator myAC = GetComponent<Animator>();
        if (myAC)
            myAC.SetTrigger("DoDrop");
    }

    private void UpdateFlyAway()
    {
        //float maxTime = 0.2f;

        float ratio = stateTime / maxTime;
        if (ratio < 1.0f)
        {
            Vector3 pp = BattleSystem.GetInstance().GetPlayer().transform.position;
            Vector3 p = posFlyAwayStart + (pp - posFlyAwayStart) * ratio;
            transform.position = p;
        }
        else
            Destroy(gameObject);
    }

    private void UpdateDropping()
    {
        if (stateTime > droppingTime)
            nextState = DROP_STATE.WAIT;
    }

    private void OnMouseDown()
    {
        //print("我被點啦");
        if ( currState != DROP_STATE.WAIT)
        {
            //print("我還不能用喔........@-@");
            return;
        }
        GameObject p = BattleSystem.GetInstance().GetPlayer();
        if (p)
        {
            float pDis = Vector3.Distance(p.transform.position, transform.position);
            if (pDis <= pickupRange)
            {
                //print("我被撿走啦");
                bool canPickUp = BattleSystem.GetInstance().OnDropItemPickUp(this);
                if (canPickUp)
                {
                    //TODO: 拾取動畫
                    //Destroy(gameObject);
                    nextState = DROP_STATE.GONE;
                    posFlyAwayStart = transform.position;
                    maxTime = pDis / 30.0f; //TODO: Speed 參數化
                    if (maxTime < 0.1f)
                        maxTime = 0.1f;
                }
                else
                {
                    DoDrop();
                }
            }
            else
            {
                //距離太遠，改成讓玩家走過來 TODO: 走過來撿掉?
                PlayerController pc = p.GetComponent<PlayerController>();
                Vector3 targetPos = transform.position + Vector3.Normalize(p.transform.position - transform.position) * 1.5f;
                if (pc)
                    pc.OnMoveToPosition(targetPos);
            }
        }

    }

}
