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
public class ForgeFormulaJsonData       //寫在 Json 檔中的格式
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
        print("測試 Forge Json");
        print(JsonUtility.ToJson(jTest));

        for (int i = 0; i < formulaJsons.Length; i++)
        {
            print("開始 Parse 一個 ForgeFormula Json");
            ForgeFormulaJsonData jFormulas = JsonUtility.FromJson<ForgeFormulaJsonData>(formulaJsons[i].text);
            print("Parse 完成，找到的 Formula 數量: " + jFormulas.formulas.Length);

            for (int j = 0; j < jFormulas.formulas.Length; j++)
            {
            }
        }
    }
}
