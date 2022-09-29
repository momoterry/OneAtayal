using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSpawner : MonoBehaviour
{
    public int MinNum = 2;
    public int MaxNum = 3;
    public GameObject objRef;

    public float AreaWidth = 0;
    public float AreaHeight = 0;

    void OnTG(GameObject whoTG)
    {
        if (MinNum < MaxNum)
            MinNum = MaxNum;

        int num = Random.Range(MinNum, MaxNum + 1);
        for (int i=0; i<num; i++)
        {
            float rH = Random.Range(-AreaWidth * 0.5f, AreaWidth * 0.5f);
            float rV = Random.Range(-AreaHeight * 0.5f, AreaHeight * 0.5f);

            Vector3 pos = new Vector3(rH, 0, rV) + transform.position;
            GameObject o = BattleSystem.SpawnGameObj(objRef, pos, true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(AreaWidth, 2.0f, AreaHeight));
    }
}
