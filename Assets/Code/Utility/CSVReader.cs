using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    static public T[] FromCSV<T>(string text)
    {
        if (!typeof(T).IsClass)
        {
            One.ERROR(" CSVReader �u��䴩 Class");
            return null;
        }
        string[] lines = text.Split("\n");
        if (lines.Length < 2) {
            One.ERROR(" CSV ������!!!!");
            return null;
        }
        for (int i=0; i<lines.Length; i++)
        {
            if (lines[i].EndsWith("\r"))
            {
                //print("�t�� r ����: " + lines[i]);
                lines[i] = lines[i].Remove(lines[i].Length - 1);
                //print("������: " + lines[i]);
            }
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
            //print("CSV: " + n + " ��: " + lines[n + 1]);
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
                //field.SetValue(data, strValue == "" ? 0 : int.Parse(strValue));
                int value;
                if (!int.TryParse(strValue, out value))
                    value = 0;
                field.SetValue(data, value);

            }
            else if (field.FieldType == typeof(float))
            {
                //print("float value:" + strValue);
                float value;
                if (!float.TryParse(strValue, out value))
                    value = 0.0f;
                field.SetValue(data, value);
            }
            else if (field.FieldType == typeof(bool))
            {
                bool value;
                if (!bool.TryParse(strValue, out value))
                    value = false;
                field.SetValue(data, value);
                //print("bool value:" + strValue + " Result: " + value);
            }
            else
            {
                print("�ثe�L�k�䴩�����: " + field.Name);
            }
        }
        return (T)data;
    }
}
