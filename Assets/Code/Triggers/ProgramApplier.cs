using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramApplier : MonoBehaviour
{
    public void OnTG(GameObject whoTG)
    {

        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
    }
}
