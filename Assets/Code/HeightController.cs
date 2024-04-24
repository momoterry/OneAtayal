using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ױ��
//���F���� 3D �����סA��]�w�����׸�T (�z�פW�O Y �b�ഫ�� Z �b���ץh����)

public class HeightController : MonoBehaviour
{
    public GameObject mainBody;

    protected float cuuuHeight = 0;     //�Ҧ��b�a���W�w�]���O 0
    protected Vector3 mainBodyOffset;


    void Awake()
    {
        if (mainBody)
        {
            mainBodyOffset = mainBody.transform.position - transform.position;
        }    
    }

    virtual public void SetHeight(float height)
    {
        cuuuHeight = height;
        mainBody.transform.position = transform.position + mainBodyOffset + Vector3.forward * height;
    }
}
