using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ForgeMaterialInfo
{
    public string itemID;
    public int requireNum;
}

[System.Serializable]
public class ForgeFormula
{
    public string outputID;
    public ITEM_TYPE outputType;
    public ForgeMaterialInfo[] inputs;
}

[System.Serializable]
public class ForgeFormulaJsonData       //�g�b Json �ɤ����榡
{
    public ForgeFormula[] formulas;
}


public class ForgeManager : MonoBehaviour
{
    public TextAsset[] formulaJsons;

    private void Awake()
    {
        ForgeFormulaJsonData jTest = new ForgeFormulaJsonData();
        jTest.formulas = new ForgeFormula[1];
        jTest.formulas[0] = new ForgeFormula();
        jTest.formulas[0].outputID = "TestDollItem";
        jTest.formulas[0].outputType = ITEM_TYPE.DOLL;
        print("���� Forge Json");
        print(JsonUtility.ToJson(jTest));

        for (int i = 0; i < formulaJsons.Length; i++)
        {
            print("�}�l Parse �@�� ForgeFormula Json");
            ForgeFormulaJsonData jFormulas = JsonUtility.FromJson<ForgeFormulaJsonData>(formulaJsons[i].text);
            print("Parse �����A��쪺 Formula �ƶq: " + jFormulas.formulas.Length);

            for (int j = 0; j < jFormulas.formulas.Length; j++)
            {
            }
        }
    }
}
