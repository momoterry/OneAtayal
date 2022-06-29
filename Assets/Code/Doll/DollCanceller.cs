using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollCanceller : MonoBehaviour
{
    protected float clickRange = 1.0f;
    protected DollSelector mySelector;

    public void InitSelector(DollSelector selector)
    {
        mySelector = selector;
    }

    private void OnMouseDown()
    {

        Vector3 mPos = Input.mousePosition;
        Vector3 wPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wPos.y = 0;

        Vector3 myPos = transform.position;
        wPos.y = 0;
        myPos.y = 0;

        float dis2 = (wPos - myPos).sqrMagnitude;
        if (dis2 < clickRange * clickRange)
        {
            OnOK();
        }
        else
        {
            OnCancel();
        }

    }

    public void OnCancel()
    {
        if (mySelector)
        {
            mySelector.OnCancellerCancel();
        }
        Destroy(gameObject);
    }

    public void OnOK()
    {
        if (mySelector)
        {
            mySelector.OnCancellerOK();
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (mySelector)
            mySelector.OnCancellerCancel();
    }

}
