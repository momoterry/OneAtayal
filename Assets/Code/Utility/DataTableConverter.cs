using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;


public class DataTableConverter
{
    protected const string ARRAY_LENGTH = "_LENGTH";

    public Dictionary<string, string> stringTable;
    public Dictionary<string, int> intTable;
    public Dictionary<string, float> floatTable;

    public DataTableConverter()
    {
        if (stringTable == null)
            stringTable = new Dictionary<string, string>();
        if (intTable == null)
            intTable = new Dictionary<string, int>();
        if (floatTable == null)
            floatTable = new Dictionary<string, float>();
    }

    virtual public void AddInt(string _id, int value)
    {
        intTable.Add(_id, value);
    }

    virtual public void AddString(string _id, string value)
    {
        stringTable.Add(_id, value);
    }

    virtual public void AddFloat(string _id, float value)
    {
        floatTable.Add(_id, value);
    }

    virtual public int GetInt(string _id)
    {
        if (intTable.ContainsKey(_id))
            return intTable[_id];
        return 0;
    }

    virtual public string GetString(string _id)
    {
        if (stringTable.ContainsKey(_id))
            return stringTable[_id];
        return "";
    }

    virtual public float GetFloat(string _id)
    {
        if (floatTable.ContainsKey(_id))
            return floatTable[_id];
        return 0;
    }

    protected void print(string str) { Debug.Log(str); }

    public void ConvertToTable<T>(T data, string prefix)
    {
        Type theType = typeof(T);
        DataToTable(prefix, theType, data);
    }

    public T FromTable<T>(string prefix)
    {
        return (T)TableToData(prefix, typeof(T));
    }

    //===============================================================================
    //   內部的處理單元們
    //===============================================================================

    //protected int GetIntFromTable(string _id)
    //{
    //    return GetInt(_id);
    //}

    //protected bool GetBoolFromTable(string _id)
    //{
    //    return GetInt(_id) != 0;
    //}

    //protected string GetStringFromTable(string _id)
    //{
    //    return GetString(_id);
    //}

    protected object TableToData(string prefix, Type _type)//, object data)
    {
        //print("TableToData 處理中: " + prefix + "Type: " + _type);
        if (_type == typeof(int))
        {
            int data = GetInt(prefix);
            //print(prefix + " :是一個 int, 值設為: " + (int)data);
            return data;
        }
        else if (_type == typeof(bool))
        {
            bool data = (GetInt(prefix) != 0);
            //print(prefix + " :是一個 bool, 值設為: " + (bool)data);
            return data;
        }
        else if (_type == typeof(float))
        {
            float data = GetFloat(prefix);
            //print(prefix + " :是一個 float, 值設為: " + (float)data);
            return data;
        }
        else if (_type == typeof(string))
        {
            string data = GetString(prefix);
            //print(prefix + " :是一個 string, 值設為: " + (string)data);
            return data;
        }
        else if (_type.IsEnum)
        {
            int data = GetInt(prefix);
            //print(prefix + " :是一個 Enum, 值設為: " + (int)data);
            return data;
        }
        else if (_type.IsArray)
        {
            int size = GetInt(prefix + ARRAY_LENGTH);
            //print(prefix + " :是一個 Array ，基底為:" + _type.GetElementType().Name + " Length: " + size);
            Array data = Array.CreateInstance(_type.GetElementType(), size);
            for (int i = 0; i < size; i++)
            {
                data.SetValue(TableToData(prefix + "_" + i, _type.GetElementType()), i);
            }
            return data;
        }
        else if (_type.IsClass)
        {
            //print(prefix + " :是一個新的 Class ，類型為:" + _type.Name);

            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            object data = Activator.CreateInstance(_type);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(data, TableToData(prefix + "_" + field.Name, field.FieldType));
            }
            return data;
        }
        else
        {
            //print(prefix + " :是一個新的 東西 ，類型為:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            object data = Activator.CreateInstance(_type);
            if (fields.Length < 2)
            {
                print("ERROR!!!! 這個類型無法處理 !! " + prefix + " .... 類型為:" + _type.Name);
                return data;
            }
            foreach (FieldInfo field in fields)
            {
                field.SetValue(data, TableToData(prefix + "_" + field.Name, field.FieldType));
            }
            return data;
        }
    }

    protected void DataToTable(string prefix, Type _type, object data)
    {
        if (data == null)
        {
            //print("DataToTable 發現空資料 !! " + prefix + " ,Type = " + _type.Name);
            return;
        }

        if (_type == typeof(int))
        {
            //print(prefix + " :是一個 int, 值等於: " + (int)data);
            AddInt(prefix, (int)data);
        }
        else if (_type == typeof(string))
        {
            //print(prefix + " :是一個 string, 值等於: " + (string)data);
            AddString(prefix, (string)data);
        }
        else if (_type == typeof(bool))
        {
            //print(prefix + " :是一個 bool, 值等於: " + (bool)data);
            AddInt(prefix, (bool)data == true ? 1 : 0);
        }
        else if (_type == typeof(float))
        {
            //print(prefix + " :是一個 float, 值等於: " + (float)data);
            AddFloat(prefix, (float)data);
        }
        else if (_type.IsArray)
        {
            Array array = (Array)data;
            //print(prefix + " :是一個 Array，的大小為: " + array.Length);
            AddInt(prefix + ARRAY_LENGTH, array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                DataToTable(prefix + "_" + i, _type.GetElementType(), array.GetValue(i));
            }
        }
        else if (_type.IsClass)
        {
            //print(prefix + " :是一個新的 Class ，類型為:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
            }
        }
        else if (_type.IsEnum)
        {
            //print(prefix + " :是一個 int, 值等於: " + (int)data);
            AddInt(prefix, (int)data);
        }
        else
        {
            //print(prefix + " :是一個新的 東西 ，類型為:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (fields.Length < 2)
            {
                print("ERROR!!!! 這個類型無法處理 !! " + prefix + " .... 類型為:" + _type.Name);
                return;
            }
            foreach (FieldInfo field in fields)
            {
                DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
                //print(prefix + "_" + field.Name);
            }
        }
    }
}
