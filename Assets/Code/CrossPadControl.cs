using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossPadControl : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnLeft()
    {
        OnControlDirection(Vector3.left);
    }

    public void OnRight()
    {
        OnControlDirection(Vector3.right);
    }

    public void OnUp()
    {
        OnControlDirection(Vector3.forward);
    }

    public void OnDown()
    {
        OnControlDirection(Vector3.back);
    }

    protected void OnControlDirection(Vector3 dir)
    {
        BattleSystem.GetPC().OnFacePosition(BattleSystem.GetPC().transform.position + dir);
    }
}
