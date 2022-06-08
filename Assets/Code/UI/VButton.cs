using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnAttack()
    {
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc)
        {
            pc.OnAttack();
        }
    }

    void OnShoot()
    {

    }

    void OnAction()
    {

    }
}
