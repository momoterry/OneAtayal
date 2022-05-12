using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 為了讓 BattleSystem 自動清除的 Tag

public class BSObjectTag : MonoBehaviour
{

    private void OnDestroy()
    {
        BattleSystem.GetInstance().OnBSObjectDestroy(gameObject);
    }
}
