using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ForgeMaterialInfo
{
    public string matID;
    public int num;
}

[System.Serializable]
public class ForgeFormula
{
    public string outputID;
    public ITEM_TYPE outputType;
    public int requireMoney;
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
    protected List<ForgeFormula> formulaList = new List<ForgeFormula>();

    static protected ForgeManager instance;
    public static ForgeManager GetInstance() { return instance; }

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� ForgeManager �s�b ");
        instance = this;

        //ForgeFormulaJsonData jTest = new ForgeFormulaJsonData();
        //jTest.formulas = new ForgeFormula[1];
        //jTest.formulas[0] = new ForgeFormula();
        //jTest.formulas[0].outputID = "TestDollItem";
        //jTest.formulas[0].outputType = ITEM_TYPE.DOLL;
        //jTest.formulas[0].inputs = new ForgeMaterialInfo[1];
        //jTest.formulas[0].inputs[0] = new ForgeMaterialInfo();
        //jTest.formulas[0].inputs[0].matID = "TestMatOne";
        //jTest.formulas[0].inputs[0].num = 3;
        //print("���� Forge Json");
        //print(JsonUtility.ToJson(jTest));

        for (int i = 0; i < formulaJsons.Length; i++)
        {
            print("�}�l Parse �@�� ForgeFormula Json");
            ForgeFormulaJsonData jFormulas = JsonUtility.FromJson<ForgeFormulaJsonData>(formulaJsons[i].text);
            print("Parse �����A��쪺 Formula �ƶq: " + jFormulas.formulas.Length);

            for (int j = 0; j < jFormulas.formulas.Length; j++)
            {
                //print("Output:" + jFormulas.formulas[j].outputID + "Input:" + jFormulas.formulas[j].inputs);
                formulaList.Add(jFormulas.formulas[j]);
                //for (int k=0; k< jFormulas.formulas[j].inputs.Length; k++)
                //{
                //    print("Input: " + jFormulas.formulas[j].inputs[k].matID);
                //}
            }
        }
    }

    public List<ForgeFormula> GetValidFormulas()
    {
        return formulaList;
    }

}
