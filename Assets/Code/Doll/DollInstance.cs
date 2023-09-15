using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================================
//巫靈鍛造機制的成果，相當於鍛造後的結果
//DollInstance 能指定 Doll 種類，同時連結一串 DollBuff
//直接連結到 Doll 的物件上
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
