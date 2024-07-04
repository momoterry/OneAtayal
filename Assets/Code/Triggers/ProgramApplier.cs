using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramApplier : MonoBehaviour
{
    public void OnTG(GameObject whoTG)
    {

        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }
}
