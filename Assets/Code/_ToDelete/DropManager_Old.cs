using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager_Old : MonoBehaviour
{
    [System.Serializable]
    public struct DropMapping_Old {
        public DropItem.DROPITEM_TYPE type;
        public GameObject objRef;
    }
    public DropMapping_Old[] dropMappingArray;


    public Dictionary<DropItem.DROPITEM_TYPE, GameObject> dropObjMap = new Dictionary<DropItem.DROPITEM_TYPE, GameObject>();

    //---- Instance ----
    private static DropManager_Old instance = null;
    public static DropManager_Old GetInstance() { return instance; }
    //public DropManager_Old() : base()
    //{
    //    if (instance != null)
    //    {
    //        print("ERROR !! 超過一份 DropManager 存在 ");
    //    } 
    //    instance = this;
    //}
    //----


    private void Awake()
    {
        if (instance != null)
        {
            print("ERROR !! 超過一份 DropManager 存在 ");
        }
        instance = this;

        foreach (DropMapping_Old dm in dropMappingArray)
        {
            dropObjMap.Add(dm.type, dm.objRef);
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

    public void OnTryDropByEnemyKilled( Enemy e)
    {
        //int enemyID = e.GetID();

        //float dropRate = 0;
        //DropItem.DROPITEM_TYPE toDrop = DropItem.DROPITEM_TYPE.NONE;

        //switch (enemyID){
        //    case 3001:
        //        dropRate = 1.0f;
        //        toDrop = DropItem.DROPITEM_TYPE.HEAL_POTION;
        //        break;
        //    case 1001:
        //        dropRate = 0.2f;
        //        toDrop = DropItem.DROPITEM_TYPE.HEAL_POTION;
        //        break;
        //    case 1002:
        //        dropRate = 0.1f;
        //        toDrop = DropItem.DROPITEM_TYPE.POWERUP_MAXHP;
        //        break;
        //    case 2001:
        //        dropRate = 0.2f;
        //        toDrop = DropItem.DROPITEM_TYPE.POWERUP_ATTACK;
        //        break;
        //    default:
        //        dropRate = 0.1f;
        //        break;
        //}

        //if (toDrop == DropItem.DROPITEM_TYPE.NONE)
        //    return;

        //bool isDrop = (Random.Range(0, 1.0f) <= dropRate);

        ////TODO: 列表查詢
        ////DropItem.DROPITEM_TYPE toDrop = DropItem.DROPITEM_TYPE.HEAL_POTION;

        //GameObject dropRef = dropObjMap[toDrop];
        //if (isDrop && dropRef)
        //{
        //    GameObject o = Instantiate(dropRef, e.transform.position, Quaternion.identity, null);
        //    DropItem di = o.GetComponent<DropItem>();
        //    di.DoDrop();

        //}
    }
}
