using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�N�Ҧ��C�����|�ϥΨ쪺�a����ưѼưO���_�Ӫ��a��
//��B���H Json �ɮת��覡�ӽs��U�Ӧa�����Ѽ�

[System.Serializable]
public class DungeonListJsonData
{
    public CMazeJsonData[] dungeons;
}

public class DungeonData : MonoBehaviour
{
    public TextAsset[] jsonFiles;
    public GameObject[] objectRefs;

    protected Dictionary<string , CMazeJsonData> allDungeons = new Dictionary<string, CMazeJsonData>();
    protected Dictionary<string, GameObject> objRefMap = new Dictionary<string, GameObject>();

    private void Awake()
    {
        for (int i = 0; i < objectRefs.Length; i++)
        {
            objRefMap.Add(objectRefs[i].name, objectRefs[i]);
        }

        for (int i=0; i<jsonFiles.Length; i++)
        {
            print("�}�l Parse �@�� DungeonData Json");
            DungeonListJsonData dgList = JsonUtility.FromJson<DungeonListJsonData>(jsonFiles[i].text);
            print("Parse �����A��쪺 Dungeon �ƶq: " + dgList.dungeons.Length);
            for (int j=0; j<dgList.dungeons.Length; j++)
            {
                dgList.dungeons[j].Convert(objRefMap);
                allDungeons.Add(dgList.dungeons[j].ID, dgList.dungeons[j]);
                print("�[�J�F�a��: " + dgList.dungeons[j].name);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
