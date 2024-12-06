using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealer : EnemyBeta
{
    protected override bool SearchTarget(float givenRange = -1)
    {
        float searchRange = givenRange > 0 ? givenRange : ChaseRangeIn;

        //尋找 Enemy
        GameObject foundTarget = null;
        float bestTargetHpRatio = 1.0f;  //找血的比例最低的
        Collider[] cols = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Character"));
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Enemy") && col.gameObject != gameObject)
            {

                Enemy enemy = col.gameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    float hpRatio = enemy.GetHP() /  enemy.MaxHP;
                    if (hpRatio < bestTargetHpRatio)
                    {
                        bestTargetHpRatio = hpRatio;
                        foundTarget = enemy.gameObject;
                    }
                }
            }
        }

        if (foundTarget)
        {
            //print("Enemy Found Target: " + foundTarget);
            SetTarget(foundTarget);
            //print("foundTarget Pos: " + foundTarget.transform.position);
            return true;
        }
        return false;
    }

    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currState.ToString() + "\n" + currState.ToString());
    //}
}
