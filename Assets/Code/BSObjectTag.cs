using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���F�� BattleSystem �۰ʲM���� Tag

public class BSObjectTag : MonoBehaviour
{

    private void OnDestroy()
    {
        BattleSystem.GetInstance().OnBSObjectDestroy(gameObject);
    }
}
