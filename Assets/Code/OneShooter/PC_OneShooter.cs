using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_OneShooter : PlayerController
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


        if (bMove)
        {
            Vector3 newPos = transform.position + moveVec * WalkSpeed * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, xMin, xMax);
            newPos.z = Mathf.Clamp(newPos.z, zMin, zMax);
            transform.position = newPos;
        }
    }
}
