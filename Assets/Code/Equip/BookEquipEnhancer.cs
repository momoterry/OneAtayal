using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// �ޯ�Ѳ��;����򥻤���
//=================================================
public class BookEquipEnhancerBase : MonoBehaviour
{
    virtual public void DoEnhance(ref BookEquipSave rEquip) 
    {
        SkillDollSummonEx skill = BookEquipManager.GetInstance().GetSkillByID(rEquip.skillID);
        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(skill.dollID);
        rEquip.bookName = dInfo.dollName + "��";
    }
}

//=================================================
// �a���]�k�ĪG�ޯ�Ѳ��;�
//=================================================

//====�r��
//���    ���� 50
//�g��    ���� 100
//�j��    HP  50
//���j    HP  100
//====�r��
//�Ԥh    ���� 50
//�x��    ���� 100
//�Z�J    HP  50
//����    HP  100

public class BookEquipEnhancer : BookEquipEnhancerBase
{
    //�r���r���w�q
    [System.Serializable]
    public class EnhanceInfo
    {
        public DOLL_BUFF_TYPE type;
        public int value;
        public string text;
    }
    public EnhanceInfo[] enhancePrefixs;
    public EnhanceInfo[] enhanceSuffixs;

    public override void DoEnhance(ref BookEquipSave rEquip)
    {
        int atkAdd = 0;
        int hpAdd = 0;
        string prefix = "";
        string suffix = "";
        EnhanceInfo preEnhance = null;
        EnhanceInfo sufEnhance = null;
        //int prefixCount = 0;
        //int suffixCount = 0;
        float twoEnhanceRate = 40.0f;
        float hRate = twoEnhanceRate / 2.0f;

        if (enhancePrefixs.Length > 0)
        {
            preEnhance = enhancePrefixs[Random.Range(0, enhancePrefixs.Length)];
            switch (preEnhance.type)
            {
                case DOLL_BUFF_TYPE.DAMAGE:
                    atkAdd += preEnhance.value;
                    break;
                case DOLL_BUFF_TYPE.HP:
                    hpAdd += preEnhance.value;
                    break;
            }
            prefix = preEnhance.text + "��";
        }
        if (enhanceSuffixs.Length > 0)
        {
            sufEnhance = enhanceSuffixs[Random.Range(0, enhanceSuffixs.Length)];
            switch (sufEnhance.type)
            {
                case DOLL_BUFF_TYPE.DAMAGE:
                    atkAdd += sufEnhance.value;
                    break;
                case DOLL_BUFF_TYPE.HP:
                    hpAdd += sufEnhance.value;
                    break;
            }
            suffix = sufEnhance.text;
        }

        rEquip.HP_Percent = hpAdd + 100;
        rEquip.ATK_Percent = atkAdd + 100;

        SkillDollSummonEx skill = BookEquipManager.GetInstance().GetSkillByID(rEquip.skillID);
        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(skill.dollID);
        rEquip.bookName = prefix + dInfo.dollName + suffix;
    }

}
