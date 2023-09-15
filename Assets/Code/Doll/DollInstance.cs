using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================================
//���F��y������G�A�۷����y�᪺���G
//DollInstance ����w Doll �����A�P�ɳs���@�� DollBuff
//�����s���� Doll ������W
//============================================================



public class DollInstance : MonoBehaviour
{
    public string fullName;
    public Doll theDoll;
    protected List<DollBuffBase> buffList = new List<DollBuffBase>();

    public List<DollBuffBase> GetBuffList() { return buffList; }

    public void Init( string _name, Doll _doll)
    {
        fullName = _name;
        theDoll = _doll;
    }

    public void AddBuff( DollBuffBase buff)
    {
        buffList.Add(buff);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
