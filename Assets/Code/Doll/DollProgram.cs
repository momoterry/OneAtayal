using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollProgram : DollBeta
{
    [System.Serializable]
    public class Condition
    {
        public string conditionDesc;
        public float conditionPara;
        public string[] actionDescs;
    }

    [System.Serializable]
    public class SkillAction
    {
        public string actionDesc;
        public SkillBase skill;
    }

    public Condition[] conditionActions;
    public SkillAction[] actionSkills;

    protected Condition currCondition = null;
    protected ActionTG currAction = null;

    protected enum PROG_PHASE
    {
        NONE,
        RUNNING,
    }
    protected PROG_PHASE currProgPhase = PROG_PHASE.NONE;
    protected PROG_PHASE nextProgPhase = PROG_PHASE.NONE;

    protected float checkContitionWait = 0;
    protected override void UpdateBattle()
    {
        if (currProgPhase != nextProgPhase)
        {
            currProgPhase = nextProgPhase;
        }

        switch (currProgPhase)
        {
            case PROG_PHASE.NONE:
                FollowSlot();
                if (checkContitionWait < 0)
                {
                    checkContitionWait = 1.0f;
                    if (CheckAllConditions())
                    {
                        nextProgPhase = PROG_PHASE.RUNNING;
                    }
                }
                else
                    checkContitionWait -= Time.deltaTime;
                break;
            case PROG_PHASE.RUNNING:
                UpdateProgram();
                break;
        }

    }


    protected bool CheckAllConditions()
    {
        currCondition = null;
        currAction = null;
        return false;
    }


    protected void UpdateProgram()
    {

    }

    private void OnGUI()
    {
        Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
        thePoint.y = Camera.main.pixelHeight - thePoint.y;
        GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currProgPhase.ToString() + currAction);
    }
}
