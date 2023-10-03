using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class SaveDataTable
{
    public Dictionary<string, string> stringTable;
    public Dictionary<string, int> intTable;

    public SaveDataTable()
    {
        if (stringTable == null)
            stringTable = new Dictionary<string, string>();
        if (intTable == null)
            intTable = new Dictionary<string, int>();
    }

    protected void print(string str) { Debug.Log(str); }

    public void ConvertToTable<T>(T data)
    {
        Type theType = typeof(T);
        //FieldInfo[] fileds = theType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        DataToTable("Root", theType, data);
    }

    public T FromTable<T>()
    {
        //object o = new();
        //TableToData("Root", typeof(T), o);
        //return (T)o;


        //T data = default(T);
        //T data = Activator.CreateInstance<T>();
        //TableToData("Root", typeof(T), data);
        //return data;

        return (T)TableToData("Root", typeof(T));
    }

    //===============================================================================
    //   內部的處理單元們
    //===============================================================================

    protected int GetIntFromTable(string _id)
    {
        if (intTable.ContainsKey(_id))
            return intTable[_id];
        return 0;
    }

    protected bool GetBoolFromTable(string _id)
    {
        return GetIntFromTable(_id) != 0;
    }

    protected string GetStringFromTable(string _id)
    {
        if (stringTable.ContainsKey(_id))
            return stringTable[_id];
        return "";
    }

    protected object TableToData(string prefix, Type _type)//, object data)
    {
        print("TableToData 處理中: " + prefix + "Type: " + _type);
        if (_type == typeof(int))
        {
            int data = GetIntFromTable(prefix);
            print(prefix + " :是一個 int, 值設為: " + (int)data);
            return data;
        }
        else if (_type == typeof(bool))
        {
            bool data = GetBoolFromTable(prefix);
            print(prefix + " :是一個 bool, 值設為: " + (bool)data);
            return data;
        }
        else if (_type == typeof(string))
        {
            string data = GetStringFromTable(prefix);
            print(prefix + " :是一個 string, 值設為: " + (string)data);
            return data;
        }
        else if (_type.IsArray)
        {
            print(prefix + " :是一個 Array ，基底為:" + _type.GetElementType().Name);
            Array data = Array.CreateInstance(_type.GetElementType(), 0);
            return data;
        }
        else if(_type.IsClass)
        {
            print(prefix + " :是一個新的 Class ，類型為:" + _type.Name);

            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            object data = Activator.CreateInstance(_type);
            foreach (FieldInfo field in fields)
            {
                //object instance = Activator.CreateInstance(field.FieldType);
                //TableToData(prefix + "_" + _type.Name, field.FieldType, instance);
                field.SetValue(data, TableToData(prefix + "_" + field.Name, field.FieldType));
            }
            return data;
        }
        else 
        {
            print("還不能處理的 type ......");
            object data = Activator.CreateInstance(_type);
            return data;
        }
    }

    protected void DataToTable(string prefix, Type _type, object data)
    {
        if (data == null)
        {
            print("發現空資料 !!");
            return;
        }

        if (_type == typeof(int))
        {
            //print(prefix + " :是一個 int, 值等於: " + (int)data);
            intTable.Add(prefix, (int)data);
        }
        else if (_type == typeof(string))
        {
            //print(prefix + " :是一個 string, 值等於: " + (string)data);
            stringTable.Add(prefix, (string)data);
        }
        else if (_type == typeof(bool))
        {
            //print(prefix + " :是一個 bool, 值等於: " + (bool)data);
            intTable.Add(prefix, (bool)data == true ? 1:0);
        }
        else if (_type.IsArray)
        {
            Array array = (Array)data;
            //print(prefix + " :是一個 Array，的大小為: " + array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                DataToTable(prefix + "_" + i, _type.GetElementType(), array.GetValue(i));
            }
        }
        else if (_type.IsClass)
        {
            //print(prefix + " :是一個新的 Class ，類型為:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach( FieldInfo field in fields)
            {
                DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
            }
        }
        else
        {
            //print(prefix + " :是一個新的 東西 ，類型為:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (fields.Length < 2)
            {
                print(">>>>這個類型無法處理 !!");
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
