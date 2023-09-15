using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DollCard : MonoBehaviour
{
    public Image icon;
    public Text dollFullName;
    public Text dollStatText;
    public Text buffDesc;

    public void SetupCard(DollInstance di)
    {
        Doll doll = di.theDoll;
        HitBody hb = doll.GetComponent<HitBody>();

        icon.sprite = doll.icon;
        dollFullName.text = di.fullName;

        dollStatText.text = "";
        dollStatText.text += "¦å¶q " + hb.HP_Max + "\n";
        dollStatText.text += "§ðÀ» " + doll.AttackInit + "\n";
        dollStatText.text += "®gµ{ " + doll.SearchRange + "\n";

        buffDesc.text = "";
        List<DollBuffBase> buffList = di.GetBuffList();
        foreach (DollBuffBase buff in buffList)
        {
            buffDesc.text += buff.desc + "\n";
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
