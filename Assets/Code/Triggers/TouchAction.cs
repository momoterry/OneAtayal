using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //if (GameSystem.IsUseVpad())
        //{
        //    gameObject.SetActive(false);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBattleTouchDown(Vector3 pos)
    {
        BattleSystem.GetPC().OnActionKey();
    }
}
