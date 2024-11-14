using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MG_Anthill : MG_MazeOneBase
{
    public Vector2Int[] roomSize;
    //public Vector2Int[] sizes = new Vector2Int[3]; // �ݭn��m���T?�x�Ϊ��ؤo

    protected List<RectInt> placedRooms = new List<RectInt>();

    protected override void CreatMazeMap()
    {
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                puzzleMap[i][j].value = CELL.INVALID;
            }
        }
        puzzleMap[puzzleStart.x][puzzleEnd.y].value = CELL.NORMAL;
        puzzleMap[puzzleEnd.x][puzzleEnd.y].value = CELL.NORMAL;

        map = new RectInt(0, 0, puzzleWidth, puzzleHeight);
        GenerateRooms();

        //List<RectInt> placedRooms = new List<RectInt>();
        foreach (RectInt ro in placedRects)
        {
            placedRooms.Add(new RectInt(ro.xMin+1, ro.yMin+1, ro.width-2, ro.height-2));
        }

        foreach (RectInt ro in placedRooms)
        {
            print("���ͪ��ж�: " + ro + "xMax " + ro.xMax + "yMax " + ro.yMax);
            for (int i = ro.xMin; i < ro.xMax; i++)
            {
                for (int j = ro.yMin; j < ro.yMax; j++)
                {
                    puzzleMap[i][j].value = CELL.NORMAL;
                }
            }
        }

        // ?��??�x�Ϊ���ɬ?����?
        List<Vector2Int> path = GetShortestPath(placedRooms[0], placedRooms[1]);

        // ���L��?
        foreach (Vector2Int p in path)
        {
            //Debug.Log($"Path Point: {p}");
            puzzleMap[p.x][p.y].value = CELL.NORMAL;
            puzzleMap[p.x][p.y].isPath = true;
        }
    }

    // ==========================  AI ���͡A�۰ʲ��ͤ����|�x���t��k

    protected RectInt map; // �a??��

    private List<RectInt> placedRects = new List<RectInt>();

    void GenerateRooms()
    {
        Vector2Int[] sizes = new Vector2Int[roomSize.Length]; // �ݭn��m���x�ΡA���e�U�[ 2 �H�T�O�����j�}

        for (int i=0; i<sizes.Length; i++)
        {
            sizes[i].x = roomSize[i].x + 2;
            sizes[i].y = roomSize[i].y + 2;
        }

        // ??�b�a?�W��m�x��
        for (int i = 0; i < sizes.Length; i++)
        {
            RectInt newRect = PlaceRect(sizes[i]);
            placedRects.Add(newRect);
            //Debug.Log($"Placed Rect {i + 1}: {newRect}");
        }
    }

    RectInt PlaceRect(Vector2Int size)
    {
        RectInt rect;
        int attempts = 0;

        do
        {
            // �b�a???��ͦ��x�Φ�m
            int x = Random.Range(map.xMin, map.xMax - size.x + 1);
            int y = Random.Range(map.yMin, map.yMax - size.y + 1);
            rect = new RectInt(x, y, size.x, size.y);
            attempts++;

            // ���?��?�A�קK���`?
            if (attempts > 1000)
            {
                Debug.LogWarning("Failed to place a rectangle after 1000 attempts.");
                break;
            }
        }
        while (!IsRectValid(rect));

        return rect;
    }

    bool IsRectValid(RectInt rect)
    {
        // ?�d�x�άO�_�b�a??�B�O�w��m���x��?����?
        if (!map.Contains(new Vector2Int(rect.xMin, rect.yMin)) ||
            !map.Contains(new Vector2Int(rect.xMax - 1, rect.yMax - 1)))
        {
            return false;
        }

        foreach (RectInt placedRect in placedRects)
        {
            if (rect.Overlaps(placedRect))
            {
                return false;
            }
        }

        return true;
    }

    // ==========================  AI ���͡A�۰ʧ�̵u�s���I�t��k

    List<Vector2Int> GetShortestPath(RectInt rect1, RectInt rect2)
    {
        List<Vector2Int> rect1Edges = GetEdgePoints(rect1);
        List<Vector2Int> rect2Edges = GetEdgePoints(rect2);
        List<Vector2Int> bestPath = null;
        int minTurns = int.MaxValue;

        // �M?�C??��?
        foreach (Vector2Int start in rect1Edges)
        {
            foreach (Vector2Int end in rect2Edges)
            {
                List<Vector2Int> currentPath = CalculatePath(start, end);

                // ?�d��?�O�_�I����?��
                if (!PathIntersectsObstacles(currentPath))
                {
                    int turns = CalculateTurns(currentPath);

                    // ????�̤֪���?
                    if (turns < minTurns)
                    {
                        minTurns = turns;
                        bestPath = currentPath;
                    }
                    else if (turns == minTurns && currentPath.Count < bestPath.Count)
                    {
                        bestPath = currentPath;
                    }
                }
            }
        }
        return bestPath;
    }

    List<Vector2Int> GetEdgePoints(RectInt rect)
    {
        List<Vector2Int> edges = new List<Vector2Int>();

        // �K�[�W???
        for (int x = rect.xMin; x < rect.xMax; x++)
            edges.Add(new Vector2Int(x, rect.yMax));

        // �K�[�U???
        for (int x = rect.xMin; x < rect.xMax; x++)
            edges.Add(new Vector2Int(x, rect.yMin));

        // �K�[��???
        for (int y = rect.yMin; y < rect.yMax; y++)
            edges.Add(new Vector2Int(rect.xMin, y));

        // �K�[�k???
        for (int y = rect.yMin; y < rect.yMax; y++)
            edges.Add(new Vector2Int(rect.xMax, y));

        return edges;
    }

    List<Vector2Int> CalculatePath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;

        // ������V��?
        while (current.x != end.x)
        {
            current.x += current.x < end.x ? 1 : -1;
            path.Add(current);
        }

        // ������V��?
        while (current.y != end.y)
        {
            current.y += current.y < end.y ? 1 : -1;
            path.Add(current);
        }

        return path;
    }

    bool PathIntersectsObstacles(List<Vector2Int> path)
    {
        foreach (RectInt obstacle in placedRooms)
        {
            foreach (Vector2Int point in path)
            {
                if (obstacle.Contains(point))
                {
                    return true;
                }
            }
        }
        return false;
    }

    int CalculateTurns(List<Vector2Int> path)
    {
        int turns = 0;
        Vector2Int? prevDirection = null;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int direction = path[i] - path[i - 1];

            // �p�G��V?��?�ơA��ܤ@��??
            if (prevDirection != null && direction != prevDirection)
            {
                turns++;
            }

            prevDirection = direction;
        }

        return turns;
    }
}
