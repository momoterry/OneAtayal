using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTG : MonoBehaviour
{
    public GameObject[] TriggerTargets;

    public bool triggerOnce = true;

    public bool triggerOnlyPlayerMove = false;

    protected bool isTriggered = false;
    protected PlayerControllerBase pcToCheckMove;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pcToCheckMove != null)
        {
            if (pcToCheckMove.IsMoving())
            {
                //print("123 木頭人!! 你動了");
                DoTrigger(pcToCheckMove.gameObject);
                pcToCheckMove = null;
            }
        }
    }


    protected void DoTrigger(GameObject whoTriggered)
    {
        foreach (GameObject o in TriggerTargets)
        {
            o.SendMessage("OnTG", whoTriggered);
        }
        if (triggerOnce)
        {
            isTriggered = true;
            enabled = false;
        }
    }


    protected void CheckTriggerEnter(GameObject obj)
    {
        if (obj.CompareTag("Player") && isTriggered == false)
        {
            if (triggerOnlyPlayerMove)
            {
                PlayerControllerBase pc = obj.GetComponent<PlayerControllerBase>();
                if (!pc || !pc.IsMoving())
                {
                    //print("玩家沒在動，先等等 .....");
                    pcToCheckMove = pc;
                    return;
                }
            }
            //print("Player In !!");
            //foreach (GameObject o in TriggerTargets)
            //{
            //    o.SendMessage("OnTG", obj);
            //}
            //if (triggerOnce)
            //{
            //    isTriggered = true;
            //    enabled = false;
            //}
            DoTrigger(obj);
        }
    }

    protected void CheckTriggerExit(GameObject obj)
    {
        if (pcToCheckMove && pcToCheckMove.gameObject == obj)
        {
            //print("追縱中的玩家離開了.....");
            pcToCheckMove = null;
        }
    }


    protected void OnTriggerEnter2D(Collider2D col)
    {
        CheckTriggerEnter(col.gameObject);
        //if (col.gameObject.CompareTag("Player") && isTriggered == false)
        //{
        //    //print("Player In !!");
        //    foreach (GameObject o in TriggerTargets)
        //    {
        //        o.SendMessage("OnTG", col.gameObject);
        //    }
        //    if (triggerOnce)
        //    {
        //        isTriggered = true;
        //        enabled = false;
        //    }
        //}
    }

    protected void OnTriggerEnter(Collider other)
    {
        CheckTriggerEnter(other.gameObject);
        //if (other.gameObject.CompareTag("Player") && isTriggered == false)
        //{
        //    //print("Player In !!");
        //    foreach (GameObject o in TriggerTargets)
        //    {
        //        if (o)
        //            o.SendMessage("OnTG", other.gameObject);
        //    }
        //    if (triggerOnce)
        //    {
        //        isTriggered = true;
        //        enabled = false;
        //    }
        //}
    }

    protected void OnTriggerExit(Collider col)
    {
        CheckTriggerExit(col.gameObject);
    }

    protected void OnTriggerExit2D(Collider2D col)
    {
        CheckTriggerExit(col.gameObject);
    }
}
