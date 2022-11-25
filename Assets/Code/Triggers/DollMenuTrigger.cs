using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollMenuTrigger : MonoBehaviour
{
    public DollMenu theMenu;

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
        if (theMenu)
        {
            theMenu.OpenMenu();
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
        }
    }

}
