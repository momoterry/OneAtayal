using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGroup : MonoBehaviour
{
    public EnemyInfo[] enemyInfos;
    public int width = 4;
    public int height = 3;
    public float spwanDistance = 15.0f;
    public float alertDistance = 10.0f;

    [System.Serializable]
    public class EnemyInfo
    {
        public GameObject enemyRef;
        public int num;
    }

    protected enum PHASE
    {
        NONE,
        SLEEP,
        WAIT,
        BATTLE,
        FINISH,
    }
    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nexPhase = PHASE.NONE;
    protected float stateTime = 0;

    protected float gridWidth = 1.0f; protected float gridHeight = 1.0f;

    //陣型
    List<GameObject> enemyList = new List<GameObject>();
    List<Transform> slotList = new List<Transform>();

    void Start()
    {
        nexPhase = PHASE.SLEEP;
    }

    void Update()
    {
        if (nexPhase != currPhase)
        {
            currPhase = nexPhase;
        }
        else
        {
            stateTime += Time.deltaTime;
            switch (currPhase)
            {
                case PHASE.SLEEP:
                    UpdateSleep();
                    break;
                case PHASE.WAIT:
                    UpdateWait();
                    break;
                case PHASE.BATTLE:
                    UpdateBattle();
                    break;
            }
        }
    }

    protected void SpawnEnemies()
    {
        int[,] oGrid = new int[width+1, height+1];
        float xShift = -(float)width * 0.5f * gridWidth;
        float yShift = -(float)height * 0.5f * gridHeight;
        foreach (EnemyInfo enemyInfo in enemyInfos)
        {
            List<Vector2Int> slots = GetConnectedCells(enemyInfo.num, width + 1, height + 1, oGrid);
            foreach (Vector2Int slot in slots) 
            {
                oGrid[slot.x, slot.y] = 1;
                Vector3 localPos = new Vector3(slot.x * gridWidth + xShift, 0, slot.y * gridHeight + yShift);
                enemyList.Add(BattleSystem.SpawnGameObj(enemyInfo.enemyRef, transform.position + localPos));
                GameObject o = new GameObject("Slot" + slotList.Count);
                o.transform.position = transform.position + localPos;
                o.transform.parent = transform;
                slotList.Add(o.transform);
            }

        }
    }

    private List<Vector2Int> GetConnectedCells(int numberOfCells, int width, int height, int[,] oGrid)
    {
        int failureCount = 0;
        int maxFailures = 100;
        int x = Random.Range(0, width);
        int y = Random.Range(0, height);

        List<Vector2Int> slots = new List<Vector2Int>();
        int[,] grid = new int[width, height];       //裡面預設值為 0

        for (int i = 0; i < numberOfCells;)
        {
            slots.Add(new Vector2Int(x, y));
            grid[x, y] = 1;

            int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
            int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };
            List<int> validDirections = new List<int>();

            for (int dir = 0; dir < 8; dir++)
            {
                int newX = x + dx[dir];
                int newY = y + dy[dir];

                if (newX >= 0 && newX < width && newY >= 0 && newY < height && grid[newX, newY] == 0 && oGrid[newX, newY] == 0)
                {
                    validDirections.Add(dir);
                }
            }

            if (validDirections.Count == 0)
            {
                failureCount++;
                if (failureCount >= maxFailures)
                {
                    print("SpawnEnemies 超級失敗的");
                    return slots;
                }
            }
            else
            {
                int randomDirection = validDirections[Random.Range(0, validDirections.Count)];
                x += dx[randomDirection];
                y += dy[randomDirection];
                i++;
            }
        }

        return slots;
    }

    protected void UpdateSleep()
    {
        if (stateTime > 0.2f)
        {
            if (GetPlayerDistance() < spwanDistance)
            {
                SpawnEnemies();
                nexPhase = PHASE.WAIT;
            }
        }
    }

    protected void UpdateWait()
    {
        if (stateTime > 0.2f)
        {
            if (GetPlayerDistance() < alertDistance)
            {
                nexPhase = PHASE.BATTLE;
            }
        }
    }

    protected float speed = 4.0f;
    protected float closeDistance = 4.0f;
    protected void UpdateBattle()
    {
        if (!BattleSystem.GetPC())
        {
            nexPhase = PHASE.FINISH;
        }
        if (Vector3.Distance(transform.position, BattleSystem.GetPC().transform.position) > closeDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, BattleSystem.GetPC().transform.position, Time.deltaTime * speed);
        }
    }

    protected float GetPlayerDistance()
    {
        if (BattleSystem.GetPC() == null)
            return Mathf.Infinity;
        Vector3 playerPos = BattleSystem.GetPC().transform.position;
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, playerPos, NavMesh.AllAreas, path))
        {
            float pathLength = 0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            return pathLength;
        }
        return Mathf.Infinity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(width * gridWidth, 2.0f, height * gridHeight));
    }

    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currPhase.ToString());

    //}
}
