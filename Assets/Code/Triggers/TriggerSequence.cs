using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSequence: MonoBehaviour
{
    public float timePeriod = 2.0f;
    public bool Shuffle = false;
    //public bool Loop = false;
    public int LoopCount = 1;

    public GameObject[] TriggerTargetList;
    public GameObject TriggerWhenFinish;

    public bool InstanceTriggerTarget = false;      //�ɤO�k�A���F�䴩����Ĳ�o EnemySpawner....
    public GameObject TriggerWhenAllDone;   //�ɤO�k�A���F�t�X EenmySpawner

    protected GameObject[] triggerSequence;
    protected int sequenceNum;
    protected int currIndex = 0;
    protected float currTime = 0;
    protected int currLoopCount = 0;

    //����^�Ǫ��u�����v Trigger: (TODO: �Ψ䥦���Y�Ԫ��覡�B�z)
    protected int totalDoneCount;
    protected int recivedDoneCount = 0;

    protected enum PHASE
    {
        NONE,
        RUNNING,
        END,
    }
    PHASE currPhase = PHASE.NONE;

    // Start is called before the first frame update
    void Start()
    {
        sequenceNum = TriggerTargetList.Length;
        triggerSequence = new GameObject[sequenceNum];

        for (int i=0; i<sequenceNum; i++)
        {
            triggerSequence[i] = TriggerTargetList[i];
        }
        if (InstanceTriggerTarget)
        {
            CloneTriggerTargets();
        }

        if (Shuffle)
        {
            ShuffleSequence();
        }

        totalDoneCount = sequenceNum * LoopCount;
    }

    void CloneTriggerTargets()
    {
        for (int i = 0; i < sequenceNum; i++)
        {
            GameObject o = triggerSequence[i];
            if (o) 
            {
                triggerSequence[i] = Instantiate(o, o.transform.position, o.transform.rotation, o.transform.parent);
            }
        }
    }

    void ShuffleSequence()
    {
        for (int i = sequenceNum-1; i>=0; i--)
        {
            int rd = Random.Range(0, i);
            GameObject tmp = triggerSequence[rd];
            triggerSequence[rd] = triggerSequence[i];
            triggerSequence[i] = tmp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currPhase == PHASE.RUNNING)
        {
            currTime -= Time.deltaTime;
            if (currTime <= 0)
            {
                DoOneTrigger();
                currTime = timePeriod;
            }
        }
    }

    void DoOneTrigger()
    {
        GameObject target = triggerSequence[currIndex];
        if (target)
        {
            target.SendMessage("OnTG", gameObject);
        }

        currIndex++;
        if (currIndex >= sequenceNum)
        {
            currLoopCount++;
            if (currLoopCount < LoopCount)
            {
                currIndex = 0;
                if (InstanceTriggerTarget)
                {
                    CloneTriggerTargets();
                }
                if (Shuffle)
                {
                    ShuffleSequence();
                }
            }
            else
            {
                currPhase = PHASE.END;
                //enabled = false;      //TODO: �O�_�� Total Done ���� Disable?
                if (TriggerWhenFinish)
                {
                    print("TriggerWhenFinish");
                    TriggerWhenFinish.SendMessage("OnTG", gameObject);
                }
            }
        }
    }

    void OnTG(GameObject whoTG)
    {
        //print("GO!!");
        if (currPhase == PHASE.NONE)
        {
            currPhase = PHASE.RUNNING;
        }
        else if ( currPhase == PHASE.RUNNING || currPhase == PHASE.END)
        {
            if (recivedDoneCount < totalDoneCount)
            {
                //����^�Ǫ��u�����v Trigger: (TODO: �Ψ䥦���Y�Ԫ��覡�B�z)
                recivedDoneCount++;
                print("Sequence Done: " + recivedDoneCount + " / " + totalDoneCount);
                if (recivedDoneCount == totalDoneCount)
                {
                    print("Total Done !!");
                    if (TriggerWhenAllDone)
                        TriggerWhenAllDone.SendMessage("OnTG", gameObject);
                }
            }

        }
    }

}
