using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTG(GameObject whoTG)
    {
        print("DoHeal !!!!!" + whoTG);

        bool result = false;
        PlayerController pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc.GetHP() < pc.GetHPMax())
        {
            pc.DoHeal(pc.GetHPMax()*0.5f);
            result = true;
        }

        whoTG.SendMessage("OnActionResult", result);

        if (result)
            Destroy(gameObject);
    }
}
