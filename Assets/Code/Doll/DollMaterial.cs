using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�w��u�a�����ͧ��F�v�A�b�}������ഫ���������\��
//�������b Doll ���W
//�� DollCollector �ӥͦ�

public class DollMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConvertToMaterial()
    {
        print("OnConvertToMaterial.... " + gameObject.name);
        Doll doll = GetComponent<Doll>();
        if (doll)
        {
            string matID = ItemDef.GetDollMaterialID(doll.ID);
            GameSystem.GetPlayerData().AddItem(matID);

            print("�[�J�F Item: " + matID + ", �W�٬�:" + ItemDef.GetInstance().GetItemInfo(matID).Name);

            gameObject.SendMessage("OnLeavePlayer", SendMessageOptions.DontRequireReceiver);
            BattleSystem.GetPC().GetDollManager().OnDollDestroy(doll);
            Destroy(gameObject);
        }
    }
}
