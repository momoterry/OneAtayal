using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�t�d�޲z���d�}�񶶧ǡA�H�ΰO�����d��ƪ��t��
//����}��P�_���s�ɡA�浹 PlayerData �� Event �ӰO��

[System.Serializable]
public class LevelInfo
{
    public string ID;
    public string sceneName;
    public string name;
    public int requireLevel;    //-1 ��� ??
}

public class LevelManager : MonoBehaviour
{
    public LevelInfo[] mainLevels;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
