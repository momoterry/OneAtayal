using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�޲z�ݭn�s��q�L�h�����d�ӧ������԰��A��p�s��a��
//�ݭn�O�����n�����d�ѼơA�� MapGenerate ��z�L�Ѽ��ܤƨӲ��ͤ��P�a��A�@�� Scene �N�i�H���ͤ��P�����d

public class LevelGenInfoBase { }

public class ContinuousBattleManager : MonoBehaviour
{
    static ContinuousBattleManager instance;
    protected void Awake()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� ContinuousBattleManager �s�b: ");
        instance = this;
    }

}
