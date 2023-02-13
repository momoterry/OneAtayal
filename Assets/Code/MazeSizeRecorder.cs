using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//======================================================
// ���g�c�p�C�����j�p�]�w�����d�O����
//======================================================
public class MazeSizeRecorder : MonoBehaviour
{
    int mazeSize = -1;      // <0 ��ܨS���]�w

    static MazeSizeRecorder instance;

    public MazeSizeRecorder() : base()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� MazeSizeRecorder �s�b ");
        instance = this;
    }
    static public int GetMazeSize()
    {
        if (instance)
            return instance.mazeSize;
        return -1;
    }

    static public void SetMazeSize(int _size)
    {
        if (instance)
        {
            instance.mazeSize = _size;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
