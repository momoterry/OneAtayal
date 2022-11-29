using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionTG : MonoBehaviour
{
    public GameObject TriggerTarget = null;
    public GameObject hint = null;

    public bool deleteAfterAction = false;
    public bool continueAction = false;

    protected PlayerControllerBase playerToActive = null;
    protected PlayerControllerBase pendingPlayer = null;

    protected float tryRegisterTime = 0;

    private void Awake()
    {
        if (hint)
            hint.SetActive(false);
    }
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

    virtual protected void OnAction()
    {
        if (playerToActive)
        {
            if (TriggerTarget)
            {
                TriggerTarget.SendMessage("OnTG", gameObject);
            }
        }
    }

    virtual protected void OnActionResult( bool result)
    {
        if (result)
        {
            if (!deleteAfterAction)
            {
                if (!continueAction)
                {
                    if (hint)
                    {
                        hint.gameObject.SetActive(false);
                    }

                    if (playerToActive)
                    {
                        playerToActive.OnUnregisterActionObject(gameObject);
                        playerToActive = null;
                    }
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnGameObjectIn(GameObject obj)
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

    protected virtual void OnGameObjectOut(GameObject obj)
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