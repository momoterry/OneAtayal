using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
public class ForgeFormulaJsonData       //寫在 Json 檔中的格式
{
    public ForgeFormula[] formulas;
}

public enum FORGE_RESULT
{
    OK,
    OK_TOBACKPACK,
    NO_MONEY,
    NO_MATERIAL,
    ERROR,
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
            print("ERROR !! 超過一份 ForgeManager 存在 ");
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
        //print("測試 Forge Json");
        //print(JsonUtility.ToJson(jTest));

        for (int i = 0; i < formulaJsons.Length; i++)
        {
            //print("開始 Parse 一個 ForgeFormula Json");
            ForgeFormulaJsonData jFormulas = JsonUtility.FromJson<ForgeFormulaJsonData>(formulaJsons[i].text);
            //print("Parse 完成，找到的 Formula 數量: " + jFormulas.formulas.Length);

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

    static public FORGE_RESULT ForgeOneDoll( ForgeFormula formula)
    {
        PlayerData pData = GameSystem.GetPlayerData();
        if (formula.requireMoney > pData.GetMoney())
            return FORGE_RESULT.NO_MONEY;

        for (int i= 0; i < formula.inputs.Length; i++)
        {
            if (formula.inputs[i].num > pData.GetItemNum(formula.inputs[i].matID))
            {
                print("素材不足..." + formula.inputs[i].matID);
                return FORGE_RESULT.NO_MATERIAL;
            }
        }

        bool isToBackpack = false;
        if (!GameSystem.GetInstance().theDollData.AddDollByID(formula.outputID, ref isToBackpack))
        {
            return FORGE_RESULT.ERROR;
        }

        //減去資源
        pData.AddMoney(-formula.requireMoney);
        for (int i = 0; i < formula.inputs.Length; i++)
        {
            pData.AddItem(formula.inputs[i].matID, -formula.inputs[i].num);
        }

        if (isToBackpack)
            return FORGE_RESULT.OK_TOBACKPACK;

        return FORGE_RESULT.OK;
    }

    //static public FORGE_RESULT ForgeOneBook(ForgeFormula formula)
    //{
    //    PlayerData pData = GameSystem.GetPlayerData();
    //    if (formula.requireMoney > pData.GetMoney())
    //        return FORGE_RESULT.NO_MONEY;

    //    for (int i = 0; i < formula.inputs.Length; i++)
    //    {
    //        if (formula.inputs[i].num > pData.GetItemNum(formula.inputs[i].matID))
    //        {
    //            print("素材不足..." + formula.inputs[i].matID);
    //            return FORGE_RESULT.NO_MATERIAL;
    //        }
    //    }


    //    //減去資源
    //    pData.AddMoney(-formula.requireMoney);
    //    for (int i = 0; i < formula.inputs.Length; i++)
    //    {
    //        pData.AddItem(formula.inputs[i].matID, -formula.inputs[i].num);
    //    }

    //    return FORGE_RESULT.OK;
    //}

}
