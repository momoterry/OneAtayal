using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossPadControl : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnLeft()
    {
        DoControlDirection(Vector3.left);
    }

    public void OnRight()
    {
        DoControlDirection(Vector3.right);
    }

    public void OnUp()
    {
        DoControlDirection(Vector3.forward);
    }

    public void OnDown()
    {
        DoControlDirection(Vector3.back);
    }

    public void OnLeftRotation()
    {
        Vector3 dir = BattleSystem.GetPC().GetDollManager().transform.rotation * Vector3.left;
        DoControlDirection(dir);
    }

    public void OnRightRotation()
    {
        Vector3 dir = BattleSystem.GetPC().GetDollManager().transform.rotation * Vector3.right;
        DoControlDirection(dir);
    }


    protected void DoControlDirection(Vector3 dir)
    {
        BattleSystem.GetPC().OnFacePosition(BattleSystem.GetPC().transform.position + dir);
    }
}
