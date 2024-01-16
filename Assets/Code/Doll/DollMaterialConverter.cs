using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollMaterialConverter : MonoBehaviour
{

    public void OnTG(GameObject whoTG)
    {
        ConvertAll();
    }

    protected void ConvertAll()
    {
        List<Doll> dolls = BattleSystem.GetPC().GetDollManager().GetDolls();
        List<DollMaterial> dmList = new List<DollMaterial>();
        foreach (Doll d in dolls)
        {
            DollMaterial m = d.GetComponent<DollMaterial>();
            if (m != null)
            {
                dmList.Add(m);
                //m.OnConvertToMaterial();
            }
        }

        foreach (DollMaterial dm in dmList)
        {
            dm.OnConvertToMaterial();
        }
    }
}
