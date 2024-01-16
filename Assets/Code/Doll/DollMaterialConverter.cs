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
        foreach (Doll d in dolls)
        {
            DollMaterial m = d.GetComponent<DollMaterial>();
            if (m != null)
            {
                m.OnConvertToMaterial();
            }
        }
    }
}
