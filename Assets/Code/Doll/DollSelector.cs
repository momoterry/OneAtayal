using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollSelector : MonoBehaviour
{
    public GameObject cancelObjRef;
    protected float cancelButtonHeight = 1.5f;

    protected DollCanceller myCanceller;

    static DollCanceller theCanceller = null;        //�u���\�@���s�b

    // Start is called before the first frame update
    void Start()
    {
        //shilftPos = Vector3.forward * cacelButtonHeight;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: �]�w Canceller ����m
        if (myCanceller)
        {
            myCanceller.gameObject.transform.position = transform.position + Vector3.forward * cancelButtonHeight;
        }
    }

    private void OnMouseDown_ToRemove()
    {

        if (theCanceller)
        {
            //���s�b�� Canceller, ������
            theCanceller.OnCancel();
            theCanceller = null;
        }

        if (cancelObjRef && !myCanceller)
        {
            GameObject myCancelObj = BattleSystem.GetInstance().SpawnGameplayObject(cancelObjRef, transform.position + Vector3.forward * cancelButtonHeight);
            myCanceller = myCancelObj.GetComponent<DollCanceller>();
            if (myCanceller)
            {
                myCanceller.InitSelector(this);                
                theCanceller = myCanceller;

            }
            else
            {
                print("ERROR !! cancelObjRef �S�� DollCanceller!! ");
                Destroy(myCancelObj);
            }
        }
    }

    //====
    public void OnCancellerCancel()
    {
        if (myCanceller == theCanceller)
        {
            theCanceller = null;
        }
        myCanceller = null;
    }

    public void OnCancellerOK()
    {
        if (myCanceller == theCanceller)
        {
            theCanceller = null;
        }

        myCanceller = null;
        //TODO: �s Doll �Ӱ���
        Destroy(gameObject);
    }
}
