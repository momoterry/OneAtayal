using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollProgram : DollBeta
{
    [System.Serializable]
    public class Condition
    {
        public string conditionDesc;
        //public float conditionPara;
        public string[] actionDescs;
    }

    [System.Serializable]
    public class SkillActionDef
    {
        public string actionDesc;
        public SkillBase skillRef;
    }

    public Condition[] conditionActions;
    public SkillActionDef[] actionSkillDefs;

    protected class Action
    {
        public SkillBase skill;
    }
    protected Dictionary<string, Action> actions = new Dictionary<string, Action>();

    protected Condition currCondition = null;
    protected List<GameObject> targets = null;
    protected int currActionIndex = 0;
    protected SkillBase currSkill = null;
    protected Action currAction = null;

    protected enum PROG_PHASE
    {
        NONE,
        RUNNING,
    }
    protected PROG_PHASE currProgPhase = PROG_PHASE.NONE;
    protected PROG_PHASE nextProgPhase = PROG_PHASE.NONE;

    protected float checkContitionWait = 0;

    protected override void OnStateEnterBattle()
    {
        base.OnStateEnterBattle();

        for (int i=0; i<actionSkillDefs.Length; i++)
        {
            if (actionSkillDefs[i].skillRef) 
            {
                GameObject o = Instantiate(actionSkillDefs[i].skillRef.gameObject, transform);
                Action a = new Action();
                a.skill = o.GetComponent<SkillBase>();
                a.skill.InitCasterInfo(gameObject, AttackInit);
                actions.Add(actionSkillDefs[i].actionDesc, a);
             }
        }
    }

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
                    checkContitionWait = 0.5f;
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

        targets = BattleUtility.SearchAllTargets(transform.position, SearchRange);
        for (int i=0; i<conditionActions.Length; i++)
        {
            if (CheckCondition(conditionActions[i]))
            {
                currCondition = conditionActions[i];
                currActionIndex = 0;
                currAction = null;
                return true;
            }
        }

        return false;
    }

    protected bool CheckCondition( Condition condi)
    {
        switch (condi.conditionDesc)
        {
            case "當射程內有敵人時":
                if (targets.Count > 0)
                    return true;
                break;
            case "當射程內有多個敵人時":
                if (targets.Count >= 3)
                    return true;
                break;
        }
        return false;
    }

    protected float actionTimeLeft = 0;
    protected void StartAction(Action a)
    {
        a.skill.DoStart();
        actionTimeLeft = 1.0f;
    }

    protected bool UpdateAction(Action a)
    {
        actionTimeLeft -= Time.deltaTime;
        if (actionTimeLeft <= 0)
            return true;
        return false;
    }
    
    protected void UpdateProgram()
    {
        if (currAction == null)
        {
            currAction = actions[currCondition.actionDescs[currActionIndex]];
            if (currAction == null)
            {
                print("ERROR!!!! 無效的 Action");
                currActionIndex++;
            }
            else
            {
                StartAction(currAction);
            }
        }
        else
        {
            if (UpdateAction(currAction))
            {
                //print("Action 結束 .....");
                currAction = null;
                currActionIndex++;
            }
        }

        if (currActionIndex >= currCondition.actionDescs.Length)
        {
            //print("Condition 結束 .....");
            currCondition = null;
            nextProgPhase = PROG_PHASE.NONE;
            checkContitionWait = 1.0f;
            currActionIndex = 0;
        }
    }

    private void OnGUI()
    {
        string actionStr = "";
        if (currCondition != null)
            actionStr = currCondition.conditionDesc + " " + currActionIndex + " : " + currCondition.actionDescs[currActionIndex];
        Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
        thePoint.y = Camera.main.pixelHeight - thePoint.y;
        GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currProgPhase.ToString() + actionStr);
    }
}
