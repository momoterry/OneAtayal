using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneBase
{
    public string ID;
    public Vector2Int worldIndex;       //�H�@�ɤ��߬� (0,0) �� Zone �y��
    public Vector2 worldPos;            //�H�@�ɤ��߬� (0,0,0) �� World �y��
    public float width;
    public float height;
    public bool N;          //�W��O�_�s��
    public bool S;          //�U��O�_�s��
    public bool W;          //����O�_�s��
    public bool E;          //�k��O�_�s��

}

[System.Serializable]
public class ZonePF: ZoneBase           //�H Perlin Filed ���򩳪� Zone
{
    public string scene;
    public float perlinShiftX;
    public float perlinShiftY;
    public int cellSize;
    public int edgeWidth;
}

