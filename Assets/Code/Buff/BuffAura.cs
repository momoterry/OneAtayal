using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ثe����@�� Enemy �M�Ϊ� Buff �F��
// ���M DollBuff ��X��A�i�H�����q�Ϊ� BuffAura

public class BuffAura : MonoBehaviour
{
    public BuffBase[] buffs;

    protected List<GameObject> objListInArea = new List<GameObject>();

    protected FACTION_GROUP group = FACTION_GROUP.ENEMY;    //TODO: ����]�䴩���a��

    private void OnTriggerEnter(Collider other)
    {
        if (!CheckGameObject(other.gameObject))
            return;

        //print("Enemuy In !!");

        foreach (BuffBase buff in buffs)
        {
            other.gameObject.SendMessage("AddBuff", buff);
        }

        objListInArea.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!CheckGameObject(other.gameObject))
            return;

        foreach (BuffBase buff in buffs)
        {
            other.gameObject.SendMessage("RemoveBuff", buff);
        }
        objListInArea.Remove(other.gameObject);
        print("EnmeyOut Total = " + objListInArea.Count);
    }

    //==================== TODO: �ѼƤ� ===================
    protected virtual bool CheckGameObject(GameObject obj)
    {
        if (obj.CompareTag("Enemy"))
            return true;

        return false;
    }

}
