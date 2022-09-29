using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class RandomSpawner : MonoBehaviour
{
    [System.Serializable]    
    public struct ItemInfo
    {
        public float RandomPercent;
        public GameObject[] SpawnRefs;
    }

    public ItemInfo[] itemInfos;
    public bool deleteSpawnedObjectOnDestory = true;

    protected List<GameObject> spawnedObjList;

    // Start is called before the first frame update
    void Start()
    {
        //機率校正
        float randomTotal = 0;
        foreach (ItemInfo o in itemInfos)
        {
            randomTotal += o.RandomPercent;
            if (o.RandomPercent < 0 || o.RandomPercent > 100.0f)
            {
                print("ERRROR!! RandomSpanwer 有問題的隨機值，必須在 0 - 100 之間!! " + gameObject.name);
            }
        }
        float adjustRatio = 100.0f / randomTotal;

        for (int i = 0; i < itemInfos.Length; i++)
        {
            itemInfos[i].RandomPercent *= adjustRatio;
        }
    }

    void OnTG(GameObject whoTG)
    {
        float rdSum = 0;
        float rd = Random.Range(0, 100.0f);
        int result = -1;
        for ( int i=0; i<itemInfos.Length-1; i++)
        {
            rdSum += itemInfos[i].RandomPercent;
            if (rd < rdSum)
            {
                result = i;
                break;
            }
        }

        //防止出現總合剛好略小於 100 且 rd = 100 的情況
        if (result < 0)
            result = itemInfos.Length - 1;

        spawnedObjList = new List<GameObject>();
        if (result >= 0)
        {
#if XZ_PLAN
            Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
            Quaternion rm = Quaternion.identity;
#endif
            foreach (GameObject o in itemInfos[result].SpawnRefs)
            {
                GameObject newObj = Instantiate(o, transform.position, rm, null);
                if (newObj)
                {
                    spawnedObjList.Add(newObj);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (deleteSpawnedObjectOnDestory && spawnedObjList!=null)
        {
            // 回收產生的物件
            foreach (GameObject o in spawnedObjList)
            {
                if (o)
                {
                    Destroy(o);
                }
            }

            spawnedObjList = null;
        }
    }
}
