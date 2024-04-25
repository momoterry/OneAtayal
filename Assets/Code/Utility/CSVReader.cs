using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Compilation;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    static public T[] FromCSV<T>(string text)
    {
        if (!typeof(T).IsClass)
        {
            print("ERROR!!!!  CSVReader 只能支援 Class");
            return null;
        }
        string[] lines = text.Split("\n");
        if (lines.Length < 2) {
            print("ERROR!!!!  CSV 不到兩行!!!!");
            return null;
        }

        Dictionary<string, int> fieldIndexMap = new Dictionary<string, int>();
        string[] fieldNames = lines[0].Split(",");
        for (int i = 0; i < fieldNames.Length; i++)
        {
            fieldIndexMap.Add(fieldNames[i], i);
        }

        T[] values = new T[lines.Length-1];
        for (int n = 0; n < lines.Length - 1; n++)
        {
            print("CSV: " + n + " 行: " + lines[n + 1]);
            values[n] = FromCSVLine<T>(lines[n + 1], fieldIndexMap);
        }

        return values;
    }

    static protected T FromCSVLine<T>(string line, Dictionary<string, int> fIndexMap)
    {
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        object data = Activator.CreateInstance(typeof(T));
        string[] strValues = line.Split(",");

        foreach (FieldInfo field in fields)
        {
            int fIndex = -1;
            if (fIndexMap.ContainsKey(field.Name))
                fIndex = fIndexMap[field.Name];

            if (fIndex < 0 || fIndex >= strValues.Length)
                continue;
            string strValue = strValues[fIndex];
            
            if (field.FieldType == typeof(string))
            {
                field.SetValue(data, strValue);
            }
            else if (field.FieldType == typeof(int))
            {
                field.SetValue(data, int.Parse(strValue));
            }
            else if (field.FieldType == typeof(float))
            {
                field.SetValue(data, float.Parse(strValue));
            }
            else
            {
                print("目前無法支援的欄位: " + field.Name);
            }
        }
        return (T)data;
    }
}
