using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionTG : MonoBehaviour
{
    public GameObject TriggerTarget = null;
    public GameObject hint = null;

    public bool deleteAfterAction = false;

    protected PlayerControllerBase playerToActive = null;
    protected PlayerControllerBase pendingPlayer = null;

    protected float tryRegisterTime = 0;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (pendingPlayer)
        {
            tryRegisterTime += Time.deltaTime;
            if (tryRegisterTime > 0.1f)
            {
                if (pendingPlayer.OnRegisterActionObject(gameObject))
                {
                    playerToActive = pendingPlayer;
                    pendingPlayer = null;
                    if (hint)
                    {
                        hint.gameObject.SetActive(true);
                    }
                }

                tryRegisterTime = 0;
            }
        }
    }

    void OnAction()
    {
        if (playerToActive)
        {
            if (TriggerTarget)
            {
                TriggerTarget.SendMessage("OnTG", gameObject);
            }
        }
    }

    void OnActionResult( bool result)
    {
        if (result)
        {
            if (hint) //TODO: 是否加一個參數來關掉提示?
            {
                hint.gameObject.SetActive(false);
            }
            if (playerToActive)
            {
                playerToActive.OnUnregisterActionObject(gameObject);
                playerToActive = null;
            }

            if (deleteAfterAction)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnGameObjectIn(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {

            PlayerControllerBase pc = obj.GetComponent<PlayerControllerBase>();
            if (pc)
            {
                //TODO: 往後要考慮如果有多個 PlayerController 的情況 ( if playerToActive != null )

                if (pc.OnRegisterActionObject(gameObject))
                {
                    playerToActive = pc;
                    if (hint)
                    {
                        hint.gameObject.SetActive(true);
                    }
                }
                else
                {
                    pendingPlayer = pc;
                }
            }
        }
    }

    void OnGameObjectOut(GameObject obj)
    {
        PlayerControllerBase pc = obj.GetComponent<PlayerControllerBase>();
        if (pc)
        {
            if (pc == playerToActive)
            {
                if (!playerToActive.OnUnregisterActionObject(gameObject))
                {
                    print("ERROR: ActionTG OnGameObjectOut Fail........OnUnregisterActionObject Fail ");
                }
                if (hint)
                    hint.gameObject.SetActive(false);

                playerToActive = null;
            }
            else
            {
                if (pc == pendingPlayer)
                {
                    pendingPlayer = null;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        OnGameObjectIn(col.gameObject);
    }

    private void OnTriggerEnter(Collider col)
    {
        OnGameObjectIn(col.gameObject);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        OnGameObjectOut(col.gameObject);
    }

    private void OnTriggerExit(Collider col)
    {
        OnGameObjectOut(col.gameObject);
    }

    private void OnDestroy()
    {
        if (playerToActive)
        {
            playerToActive.OnUnregisterActionObject(gameObject);
        }
    }

}