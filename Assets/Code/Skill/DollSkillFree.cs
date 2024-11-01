using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollSkillFree : DollSkillBase
{
    protected float NewSearchRange = 12.0f;
    protected float RunSpeed = 20.0f;

    protected float oldSearchRange;
    protected float oldPositionRangeIn;
    protected float oldPositionRangeOut;
    protected float oldRunSpeed;

    protected NavMeshAgent myAgent;

    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnStartSkill(bool active = true)
    {
        base.OnStartSkill(active);

        //TODO: DollSkill 不再針對 DollAuto 支援，以下的實作需要重新實現
        
        //if (active)
        //{
        //    oldSearchRange = doll.SearchRange;
        //    oldPositionRangeIn = doll.PositionRangeIn;
        //    oldPositionRangeOut = doll.PositionRangeOut;
        //    oldRunSpeed = doll.RunSpeed;

        //    doll.SearchRange = NewSearchRange;
        //    doll.PositionRangeIn = 1000.0f;
        //    doll.PositionRangeOut = 1001.0f;
        //    doll.RunSpeed = RunSpeed;
        //    myAgent.speed = RunSpeed;
        //}
        //else
        //{
        //    doll.SearchRange = oldSearchRange;
        //    doll.PositionRangeIn = oldPositionRangeIn;
        //    doll.PositionRangeOut = oldPositionRangeOut;
        //    doll.RunSpeed = oldRunSpeed;
        //    myAgent.speed = oldRunSpeed;
        //}
    }
}
