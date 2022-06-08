using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealObject : MonoBehaviour
{
    public GameObject healFX;
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
        //print("DoHeal !!!!!" + whoTG);

        bool result = false;
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc.GetHP() < pc.GetHPMax())
        {
            pc.DoHeal(pc.GetHPMax()*0.5f);
            if (healFX)
            {
#if XZ_PLAN
                Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
                Quaternion rm = Quaternion.identity;
#endif
                Instantiate(healFX, pc.transform.position, rm, pc.transform);
            }
            result = true;
        }

        whoTG.SendMessage("OnActionResult", result, SendMessageOptions.DontRequireReceiver);

        if (result)
            Destroy(gameObject);
    }
}
