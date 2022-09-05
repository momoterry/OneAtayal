using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffectBase : bullet_base
{
    public float timePeriod = 0.25f;
    public float lifeTime = 4.0f;

    protected List<GameObject> objListInArea = new List<GameObject>();

    protected float timeAfterEffect = 0;
    protected float timeTotal = 0;
    // Start is called before the first frame update

    //private List<GameObject> removeList = new List<GameObject>();

    void Start()
    {

    }


    void DoApplyEffectAll()
    {
        //while (objListInArea.Remove(null)) { }


        //foreach (GameObject o in objListInArea)
        //{
        //    if (o == null)
        //    {
        //        print("XXXXXXXX");
        //    }
        //    if (o.activeInHierarchy)
        //    {
        //        ApplyEffect(o);
        //    }
        //    else
        //    {
        //        removeList.Add(o);
        //    }
        //}

        //foreach (GameObject ro in removeList)
        //{
        //    objListInArea.Remove(ro);
        //}
        //removeList.Clear();

        bool isLoop = true;
        while (isLoop)
        {
            isLoop = false;
            for (int i=0; i<objListInArea.Count; i++)
            {
                GameObject o = objListInArea[i];
                if (o == null || !o.activeInHierarchy)
                {
                    objListInArea.RemoveAt(i);
                    isLoop = true;
                    break;
                }
                else
                {
                    ApplyEffect(o);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        timeAfterEffect += Time.deltaTime;
        if (timeAfterEffect >= timePeriod)
        {
            timeAfterEffect -= timePeriod;

            DoApplyEffectAll();
        }

        timeTotal += Time.deltaTime;
        if (lifeTime >=0 && timeTotal >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CheckGameObject(other.gameObject))
            return;

        //print("In!! " + other.gameObject.name);
        objListInArea.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        objListInArea.Remove(other.gameObject);
    }

    //==================== TODO: 參數化 ===================
    protected virtual bool CheckGameObject(GameObject obj)
    {
        if (obj.CompareTag("Player") || obj.CompareTag("Doll"))
            return true;

        return false;
    }
    //====================== 最主要的繼承實作 ====================
    protected virtual void ApplyEffect(GameObject obj)
    {

    }

}
