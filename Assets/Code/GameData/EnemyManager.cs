using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string EnemyID;
    public string BaseRef;
    public int Rank;
    public float HP;
    public float ATK;
    public int DropID;
}

public class EnemyManager : MonoBehaviour
{

    public TextAsset csvFile;

    void Awake()
    {
        EnemyData[] enemyDatas = CSVReader.FromCSV<EnemyData>(csvFile.text);
        for (int i = 0; i < enemyDatas.Length; i++)
        {
            print("Enemy " + enemyDatas[i].EnemyID);
        }
    }
}
