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
    //   �������B�z�椸��
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
        //print("TableToData �B�z��: " + prefix + "Type: " + _type);
        if (_type == typeof(int))
        {
            int data = GetInt(prefix);
            //print(prefix + " :�O�@�� int, �ȳ]��: " + (int)data);
            return data;
        }
        else if (_type == typeof(bool))
        {
            bool data = (GetInt(prefix) != 0);
            //print(prefix + " :�O�@�� bool, �ȳ]��: " + (bool)data);
            return data;
        }
        else if (_type == typeof(float))
        {
            float data = GetFloat(prefix);
            //print(prefix + " :�O�@�� float, �ȳ]��: " + (float)data);
            return data;
        }
        else if (_type == typeof(string))
        {
            string data = GetString(prefix);
            //print(prefix + " :�O�@�� string, �ȳ]��: " + (string)data);
            return data;
        }
        else if (_type.IsEnum)
        {
            int data = GetInt(prefix);
            //print(prefix + " :�O�@�� Enum, �ȳ]��: " + (int)data);
            return data;
        }
        else if (_type.IsArray)
        {
            int size = GetInt(prefix + ARRAY_LENGTH);
            //print(prefix + " :�O�@�� Array �A�򩳬�:" + _type.GetElementType().Name + " Length: " + size);
            Array data = Array.CreateInstance(_type.GetElementType(), size);
            for (int i = 0; i < size; i++)
            {
                data.SetValue(TableToData(prefix + "_" + i, _type.GetElementType()), i);
            }
            return data;
        }
        else if (_type.IsClass)
        {
            //print(prefix + " :�O�@�ӷs�� Class �A������:" + _type.Name);

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
            //print(prefix + " :�O�@�ӷs�� �F�� �A������:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            object data = Activator.CreateInstance(_type);
            if (fields.Length < 2)
            {
                print("ERROR!!!! �o�������L�k�B�z !! " + prefix + " .... ������:" + _type.Name);
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
            //print("DataToTable �o�{�Ÿ�� !! " + prefix + " ,Type = " + _type.Name);
            return;
        }

        if (_type == typeof(int))
        {
            //print(prefix + " :�O�@�� int, �ȵ���: " + (int)data);
            AddInt(prefix, (int)data);
        }
        else if (_type == typeof(string))
        {
            //print(prefix + " :�O�@�� string, �ȵ���: " + (string)data);
            AddString(prefix, (string)data);
        }
        else if (_type == typeof(bool))
        {
            //print(prefix + " :�O�@�� bool, �ȵ���: " + (bool)data);
            AddInt(prefix, (bool)data == true ? 1 : 0);
        }
        else if (_type == typeof(float))
        {
            //print(prefix + " :�O�@�� float, �ȵ���: " + (float)data);
            AddFloat(prefix, (float)data);
        }
        else if (_type.IsArray)
        {
            Array array = (Array)data;
            //print(prefix + " :�O�@�� Array�A���j�p��: " + array.Length);
            AddInt(prefix + ARRAY_LENGTH, array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                DataToTable(prefix + "_" + i, _type.GetElementType(), array.GetValue(i));
            }
        }
        else if (_type.IsClass)
        {
            //print(prefix + " :�O�@�ӷs�� Class �A������:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
            }
        }
        else if (_type.IsEnum)
        {
            //print(prefix + " :�O�@�� int, �ȵ���: " + (int)data);
            AddInt(prefix, (int)data);
        }
        else
        {
            //print(prefix + " :�O�@�ӷs�� �F�� �A������:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (fields.Length < 2)
            {
                print("ERROR!!!! �o�������L�k�B�z !! " + prefix + " .... ������:" + _type.Name);
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
