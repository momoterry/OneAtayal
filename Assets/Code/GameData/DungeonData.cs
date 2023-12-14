using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//盢┮Τ笴栏い穦ㄏノ戈把计癘魁癬ㄓよ
//˙ Json 郎よΑㄓ絪胯把计

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
            print("秨﹍ Parse  DungeonData Json");
            DungeonListJsonData dgList = JsonUtility.FromJson<DungeonListJsonData>(jsonFiles[i].text);
            print("Parse ЧΘт Dungeon 计秖: " + dgList.dungeons.Length);
            for (int j=0; j<dgList.dungeons.Length; j++)
            {
                dgList.dungeons[j].Convert(objRefMap);
                allDungeons.Add(dgList.dungeons[j].ID, dgList.dungeons[j]);
                print(": " + dgList.dungeons[j].name);
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
