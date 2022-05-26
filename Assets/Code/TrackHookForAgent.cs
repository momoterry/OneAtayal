using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrackHookForAgent : TrackHook
{
    public NavMeshAgent hookAgent;
    public float agentSpeedDuringHook = 10.0f;  //�յ۰���   TODO: �~���]�w?

    protected float agentSpeedOriginal;

    override protected void StartHook()
    {
        if (hookAgent)
        {
            agentSpeedOriginal = hookAgent.speed;
            hookAgent.speed = agentSpeedDuringHook;
        }
    }

    override protected void EndHook()
    {
        if (hookAgent)
            hookAgent.speed = agentSpeedOriginal;
    }

    protected override void UpdateHook()
    {
        if (hookAgent)
        {
            hookAgent.SetDestination(mySlot.transform.position);
        }
    }
}
