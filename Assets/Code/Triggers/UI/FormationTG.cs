using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationTG : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //print("Formation Trigger IN");
            BattleSystem.GetHUD().OnOffDollLayoutAll(true);
        }
    }


    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //print("Formation Trigger OUT");
            BattleSystem.GetHUD().OnOffDollLayoutAll(false);
        }
    }
}
