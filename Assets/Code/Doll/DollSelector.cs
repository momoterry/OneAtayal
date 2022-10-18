using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollSelector : MonoBehaviour
{
    public GameObject cancelObjRef;
    protected float cancelButtonHeight = 1.5f;

    protected DollCanceller myCanceller;

    static DollCanceller theCanceller = null;        //只允許一份存在

    // Start is called before the first frame update
    void Start()
    {
        //shilftPos = Vector3.forward * cacelButtonHeight;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: 設定 Canceller 的位置
        if (myCanceller)
        {
            myCanceller.gameObject.transform.position = transform.position + Vector3.forward * cancelButtonHeight;
        }
    }

    private void OnMouseDown_ToRemove()
    {

        if (theCanceller)
        {
            //有存在的 Canceller, 先移除
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
                print("ERROR !! cancelObjRef 沒有 DollCanceller!! ");
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
        //TODO: 叫 Doll 來執行
        Destroy(gameObject);
    }
}
