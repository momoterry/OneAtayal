using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_OneShooter : PC_One
{
    public float xMin = -8.0f;
    public float xMax = 8.0f;
    public float zMin = -8.0f;
    public float zMax = 6.0f;

    protected override void UpdateMoveControl()
    {
        Vector3 moveVec = Vector3.zero;
        bool bMove = false;

        Vector2 inputVec = theInput.TheHero.Move.ReadValue<Vector2>();

        if (inputVec.magnitude > 0.5)
        {
            bMove = true;
            moveVec = new Vector3(inputVec.x, 0, inputVec.y);   //XZ Plan
            moveVec.Normalize();
        }

        //VPad
        Vector2 vPadVec = Vector2.zero;
        if (BattleSystem.GetInstance().GetVPad())
        {
            vPadVec = BattleSystem.GetInstance().GetVPad().GetCurrVector();
        }
        if (vPadVec.magnitude > 0.2f)
        {
            bMove = true;
            moveVec = new Vector3(vPadVec.x, 0, vPadVec.y);   //XZ Plan
            moveVec.Normalize();
        }


        if (bMove)
        {
            Vector3 newPos = transform.position + moveVec * WalkSpeed * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, xMin, xMax);
            newPos.z = Mathf.Clamp(newPos.z, zMin, zMax);
            transform.position = newPos;

            MoveDollManager();
        }
    }

    public override void OnShootTo()
    {
        //print("PC_OneShooter::OnShootTo");
    }

    public override void OnShoot()
    {
        //print("PC_OneShooter::OnShoot");
    }

    public override void OnAttack()
    {
        //print("PC_OneShooter::OnAttack");
    }

    protected override void DoDeath()
    {
        base.DoDeath();

        //¼É¤Oªk
        Doll[] dolls = gameObject.GetComponentsInChildren<Doll>();
        foreach (Doll d in dolls)
        {
            d.OnPlayerDead();
        }
    }

}
