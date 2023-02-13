using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//======================================================
// 為迷宮小遊戲的大小設定跨關卡記錄器
//======================================================
public class MazeSizeRecorder : MonoBehaviour
{
    int mazeSize = -1;      // <0 表示沒有設定

    static MazeSizeRecorder instance;

    public MazeSizeRecorder() : base()
    {
        if (instance != null)
            print("ERROR !! 超過一份 MazeSizeRecorder 存在 ");
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
