using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionTG : MonoBehaviour
{
    public GameObject TriggerTarget = null;
    public GameObject hint = null;

    public bool deleteAfterAction = false;

    private PlayerController whoActiveMe = null;


    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnAction()
    {
        if (whoActiveMe)
        {
            if (TriggerTarget)
            {
                TriggerTarget.SendMessage("OnTG", gameObject);
                //if (deleteAfterAction)
                //{
                //    Destroy(gameObject);
                //}
            }
        }
    }

    void OnActionResult( bool result)
    {
        if (result)
        {
            if (deleteAfterAction)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //print("I got picked !!");
            whoActiveMe = col.gameObject.GetComponent<PlayerController>();
            //whoActiveMe.SendMessage("OnRegisterActionObject", gameObject);
            if (whoActiveMe && hint && whoActiveMe.OnRegisterActionObject(gameObject))
            {
                hint.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //print("I got picked !!");
            whoActiveMe = col.gameObject.GetComponent<PlayerController>();
            //whoActiveMe.SendMessage("OnRegisterActionObject", gameObject);
            if (whoActiveMe && hint && whoActiveMe.OnRegisterActionObject(gameObject))
            {
                hint.gameObject.SetActive(true);
            }
            //TODO: 如果無法跟玩家註冊，需要在 Update 時 Check 是否
        }

    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == whoActiveMe.gameObject)
        {
            //print("You leave me........ " + whoActiveMe);
            //whoActiveMe.SendMessage("OnUnregisterActionObject", gameObject);
            if (!whoActiveMe.OnUnregisterActionObject(gameObject))
            {
                print("ERROR: ActionTG OnTriggerExit Fail........");
            }
            whoActiveMe = null;
            if (hint)
                hint.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject == whoActiveMe)
        {
            //print("You leave me........ " + whoActiveMe);
            //whoActiveMe.SendMessage("OnUnregisterActionObject", gameObject);
            if (!whoActiveMe.OnUnregisterActionObject(gameObject))
            {
                print("ERROR: ActionTG OnTriggerExit Fail........");
            }
            whoActiveMe = null;
            if (hint)
                hint.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (whoActiveMe)
        {
            //whoActiveMe.SendMessage("OnUnregisterActionObject", gameObject);
            whoActiveMe.OnUnregisterActionObject(gameObject);
        }
    }

}