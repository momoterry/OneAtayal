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
            print("�o�{�Ÿ�� !!");
            return;
        }

        if (_type == typeof(int))
        {
            print(prefix + " :�O�@�� int, �ȵ���: " + (int)data);
            //print(prefix + " ���ȵ���: " + (int)data);
        }
        else if (_type == typeof(string))
        {
            print(prefix + " :�O�@�� string, �ȵ���: " + (string)data);
            //print(prefix + " ���ȵ���: " + (string)data);
        }
        else if (_type == typeof(bool))
        {
            print(prefix + " :�O�@�� bool, �ȵ���: " + (bool)data);
            //print(prefix + " ���ȵ���: " + (bool)data);
        }
        else if (_type.IsArray)
        {
            Array array = (Array)data;
            print(prefix + " :�O�@�� Array�A���j�p��: " + array.Length);
            //print(prefix + " ���j�p��: " + array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                DataToTable(prefix + "_" + i, _type.GetElementType(), array.GetValue(i));
            }
        }
        else if (_type.IsClass)
        {
            print(prefix + " :�O�@�ӷs�� Class �A������:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach( FieldInfo field in fields)
            {
                DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
            }
        }
        else
        {
            print(prefix + " :�O�@�ӷs�� �F�� �A������:" + _type.Name);
            FieldInfo[] fields = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (fields.Length < 2)
            {
                print(">>>>�o�������L�k�B�z !!");
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
            print("�o�{�Ÿ�� !!");
            return;
        }

        if (_type == typeof(int))
        {
            print(">> " + prefix + " :�O�@�� int");
            print(">> " + prefix + " ���ȵ���: " + (int)data);
        }
        else if (_type == typeof(string))
        {
            print(">> " + prefix + " :�O�@�� string");
            print(">> " + prefix + " ���ȵ���: " + (string)data);
        }
        else if (_type.IsArray)
        {
            print(">> " + prefix + " :�O�@�� Array�A�򩳬�: " + _type.GetElementType().Name);
            Array array = (Array)data;
            print(">> " + prefix + " ���j�p��: " + array.Length);
            //for (int i = 0; i < array.Length; i++)
            //{
            //    DataToTable(prefix + "_" + i, _type.GetElementType(), array.GetValue(i));
            //}
        }
        else if (_type.IsClass)
        {
            print(">> " + prefix + " :�O�@�ӷs�� Type �A������:" + _type.Name);
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
    //            print(prefix + "_" + field.Name + " :�O�@�� int");
    //            print(prefix + "_" + field.Name + " ���ȵ���: " + (int)field.GetValue(data));
    //        }
    //        else if (field.FieldType == typeof(string))
    //        {
    //            print(prefix + "_" + field.Name + " :�O�@�� string");
    //            print(prefix + "_" + field.Name + " ���ȵ���: " + (string)field.GetValue(data));
    //        }
    //        else if (field.FieldType.IsArray)
    //        {
    //            print(prefix + "_" + field.Name + " :�O�@�� Array�A�򩳬�: " + field.FieldType.GetElementType().Name);
    //            Array array = (Array)field.GetValue(data);
    //            print(prefix + "_" + field.Name + " ���j�p��: " + array.Length);
    //            for (int i = 0; i < array.Length; i++)
    //            {
    //                print(prefix + "_" + field.Name + "_" + i + "_:" + array.GetValue(i));
    //                //DataToTable(prefix + "_" + field.Name + "_" + i + "_", field.FieldType.GetElementType(), array.GetValue(i));
    //            }
    //        }
    //        else
    //        {
    //            print(prefix + "_" + field.Name + " :�O�@�ӷs�� Type");
    //            DataToTable(prefix + "_" + field.Name, field.FieldType, field.GetValue(data));
    //        }
    //    }
    //}

}
