using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //print("Gate Opend !!");
            BattleSystem.GetInstance().OnClearGateEnter();
        }
    }

    void OnMouseDown()
    {
        BattleSystem.GetInstance().GetPlayerController().OnMoveToPosition(transform.position);
    }
}
