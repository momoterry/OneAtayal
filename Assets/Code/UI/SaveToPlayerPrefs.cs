using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveToPlayerPrefs : DataTableConverter
{
    public SaveData LoadData()
    {
        SaveData data = FromTable<SaveData>("One");
        return data;
    }

    public void SaveData(SaveData data)
    {
        ConvertToTable<SaveData>(data, "One");
        PlayerPrefs.Save();
    }

    public override void AddInt(string _id, int value)
    {
        PlayerPrefs.SetInt(_id, value);
    }

    public override void AddString(string _id, string value)
    {
        PlayerPrefs.SetString(_id, value);
    }

    public override int GetInt(string _id)
    {
        return PlayerPrefs.GetInt(_id, 0);
    }

    public override string GetString(string _id)
    {
        return PlayerPrefs.GetString(_id, "");
    }

}


