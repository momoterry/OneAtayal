using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemInfo
{
    public float RandomPercent;
    public GameObject[] SpawnRefs;
}

public class RandomSpawner : MonoBehaviour
{
    public ItemInfo[] itemInfos;

    // Start is called before the first frame update
    void Start()
    {
        //���v�ե�
        float randomTotal = 0;
        foreach (ItemInfo o in itemInfos)
        {
            randomTotal += o.RandomPercent;
            if (o.RandomPercent <= 0 || o.RandomPercent >= 100.0f)
            {
                print("ERRROR!! RandomSpanwer �����D���H���ȡA�����b 0 - 100 ����!! " + gameObject.name);
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

        //����X�{�`�X��n���p�� 100 �B rd = 100 �����p
        if (result < 0)
            result = itemInfos.Length - 1;

        if (result >= 0)
        {
#if XZ_PLAN
            Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
            Quaternion rm = Quaternion.identity;
#endif
            foreach (GameObject o in itemInfos[result].SpawnRefs)
            {
                Instantiate(o, transform.position, rm, null);
            }
        }
    }
}
