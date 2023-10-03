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

    protected void DataToTable(string prefix, Type _type, object data)
    {
        if (data == null)
        {
            print("祇瞷戈 !!");
            return;
        }

        if (_type == typeof(int))
        {
            print(prefix + " :琌 int, 单: " + (int)data);
            //print(prefix + " 单: " + (int)data);
        }
        else if (_type == typeof(string))
        {
            print(prefix + " :琌 string, 单: " + (string)data);
            //print(prefix + " 单: " + (string)data);
        }
        else if (_type == typeof(bool))
        {
            print(prefix + " :琌 bool, 单: " + (bool)data);
            //print(prefix + " 单: " + (bool)data);
        }
        else if (_type.IsArray)
        {
            Array array = (Array)data;
            print(prefix + " :琌 Array: " + array.Length);
            //print(prefix + " : " + array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                DataToTable(prefix + "_" + i, _type.GetElementType(), array.GetValue(i));
            }
        }
        else if (_type.IsClass)
        {
            print(prefix + " :琌穝 Class 摸:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach( FieldInfo field in fields)
            {
                DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
            }
        }
        else
        {
            print(prefix + " :琌穝 狥﹁ 摸:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (fields.Length < 2)
            {
                print(">>>>硂摸礚猭矪瞶 !!");
                return;
            }
            foreach (FieldInfo field in fields)
            {
                DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
                //print(prefix + "_" + field.Name);
            }
        }
    }

    protected void HandleArrayElement(string prefix, Type _type, object data)
    {
        if (data == null)
        {
            print("祇瞷戈 !!");
            return;
        }

        if (_type == typeof(int))
        {
            print(">> " + prefix + " :琌 int");
            print(">> " + prefix + " 单: " + (int)data);
        }
        else if (_type == typeof(string))
        {
            print(">> " + prefix + " :琌 string");
            print(">> " + prefix + " 单: " + (string)data);
        }
        else if (_type.IsArray)
        {
            print(">> " + prefix + " :琌 Array膀┏: " + _type.GetElementType().Name);
            Array array = (Array)data;
            print(">> " + prefix + " : " + array.Length);
            //for (int i = 0; i < array.Length; i++)
            //{
            //    DataToTable(prefix + "_" + i, _type.GetElementType(), array.GetValue(i));
            //}
        }
        else if (_type.IsClass)
        {
            print(">> " + prefix + " :琌穝 Type 摸:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                print(">>>> " + field.Name);
                //DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
            }
        }
    }

    //protected void DataToTable(string prefix, Type _type, object data)
    //{
    //    FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
    //    foreach (FieldInfo field in fields)
    //    {
    //        if (field.FieldType == typeof(int))
    //        {
    //            print(prefix + "_" + field.Name + " :琌 int");
    //            print(prefix + "_" + field.Name + " 单: " + (int)field.GetValue(data));
    //        }
    //        else if (field.FieldType == typeof(string))
    //        {
    //            print(prefix + "_" + field.Name + " :琌 string");
    //            print(prefix + "_" + field.Name + " 单: " + (string)field.GetValue(data));
    //        }
    //        else if (field.FieldType.IsArray)
    //        {
    //            print(prefix + "_" + field.Name + " :琌 Array膀┏: " + field.FieldType.GetElementType().Name);
    //            Array array = (Array)field.GetValue(data);
    //            print(prefix + "_" + field.Name + " : " + array.Length);
    //            for (int i = 0; i < array.Length; i++)
    //            {
    //                print(prefix + "_" + field.Name + "_" + i + "_:" + array.GetValue(i));
    //                //DataToTable(prefix + "_" + field.Name + "_" + i + "_", field.FieldType.GetElementType(), array.GetValue(i));
    //            }
    //        }
    //        else
    //        {
    //            print(prefix + "_" + field.Name + " :琌穝 Type");
    //            DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
    //        }
    //    }
    //}

}
