using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EnemyGroup : MonoBehaviour
{
    public EnemyInfo[] enemyInfos;
    public bool isRandomEnemyTotal = false;
    public int randomEnemyTotal = 0;
    public int width = 4;
    public int height = 3;
    public float spwanDistance = 15.0f;
    public float alertDistance = 10.0f;
    public float stopDistance = 20.0f;
    public float speed = 4.0f;
    public float closeDistance = 4.0f;

    public GameObject[] triggerTargetWhenAllKilled;

    public float difficulty = 1.0f;     //1.0f 為正常值

    protected bool finishWhenEngaged = false;

    [System.Serializable]
    public class EnemyInfo
    {
        public GameObject enemyRef;
        public string enemyID = "";      //如果有設定 ID，則 enemyRef 無效
        public int LV = 1;
        public int num;
    }

    protected enum PHASE
    {
        NONE,
        SLEEP,
        WAIT,
        BATTLE,
        TRACE_ONLY,
        FINISH,
    }
    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nextPhase = PHASE.NONE;
    protected float stateTime = 0;

    protected float gridWidth = 1.0f; protected float gridHeight = 1.0f;

    //陣型
    //List<GameObject> enemies = new List<GameObject>();
    List<Enemy> enemyList = new List<Enemy>();
    List<Transform> slots = new List<Transform>();

    //計算用的中心點
    protected Transform centerT;
    protected float newWidth = 0;
    protected float newHeight = 0;


    public void SetFinishWhenEngaged(bool _finishWhenEngaged)
    {
        finishWhenEngaged = _finishWhenEngaged;
    }

    void Awake()
    {
        centerT = transform;
        newWidth = width;
        newHeight = height;
    }

    void Start()
    {
        if (isRandomEnemyTotal)
        {
            int[] rds = new int[enemyInfos.Length - 1];
            for (int i = 0; i < enemyInfos.Length - 1; i++)
            {
                rds[i] = Random.Range(0, randomEnemyTotal + 1);
            }
            System.Array.Sort(rds);
            int prev = 0;
            for (int i = 0; i < enemyInfos.Length - 1; i++)
            {
                enemyInfos[i].num = rds[i] - prev;
                prev = rds[i];
            }
            enemyInfos[enemyInfos.Length - 1].num = randomEnemyTotal - prev;

            //print("----");
            //for (int i = 0; i < enemyInfos.Length; i++)
            //{
            //    print("RandomEnemyNum: " + enemyInfos[i].num + "  << " + randomEnemyTotal);
            //}
        }

        nextPhase = PHASE.SLEEP;
    }

    void Update()
    {
        if (nextPhase != currPhase)
        {
            if (nextPhase == PHASE.TRACE_ONLY)
            {
                //print("清除所有 Enemy 的 Slot 狀態 !!");
                foreach (Enemy e in enemyList)
                {
                    e.SetSlot(null);
                }
            }
            if (nextPhase == PHASE.FINISH && triggerTargetWhenAllKilled != null)
            {
                foreach (GameObject o in triggerTargetWhenAllKilled)
                {
                    o.SendMessage("OnTG", gameObject, SendMessageOptions.DontRequireReceiver);
                }
            }
            currPhase = nextPhase;
            stateTime = 0;
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
                case PHASE.TRACE_ONLY:
                    UpdateTraceEnemies();
                    break;
                case PHASE.FINISH:
                    Destroy(gameObject);
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
            List<Vector2Int> iSlots = GetConnectedCells(enemyInfo.num, width + 1, height + 1, oGrid);
            foreach (Vector2Int slot in iSlots) 
            {
                oGrid[slot.x, slot.y] = 1;
                Vector3 localPos = new Vector3(slot.x * gridWidth + xShift, 0, slot.y * gridHeight + yShift);
                GameObject eo;
                if (enemyInfo.enemyID != "")
                {
                    if (enemyInfo.LV > 1)
                        eo = EnemyManager.GetInstance().SpawnEnemyByID(enemyInfo.enemyID, transform.position + localPos, enemyInfo.LV);
                    else
                        eo = EnemyManager.GetInstance().SpawnEnemyByID(enemyInfo.enemyID, transform.position + localPos);
                }
                else
                {
                    //eo = BattleSystem.SpawnGameObj(enemyInfo.enemyRef, transform.position + localPos);
                    eo = EnemyManager.GetInstance().SpawnEnemyByRef(enemyInfo.enemyRef, transform.position + localPos, enemyInfo.LV);
                }
                //enemies.Add(eo);
                GameObject o = new GameObject("Slot" + slots.Count);
                o.transform.position = transform.position + localPos;
                o.transform.parent = transform;
                slots.Add(o.transform);

                Enemy e = eo.GetComponent<Enemy>();
                if (e)
                {
                    e.SetSlot(o.transform);
                    //e.Attack = e.Attack * difficulty;
                    //e.MaxHP = e.MaxHP * difficulty;
                    e.SetDiffcult(difficulty);
                    enemyList.Add(e);
                }
                else
                {
                    print("ERROR!!!! EnemyGroup 指定了沒有 Enemy 的物件!!");
                }
            }

        }

        //重新尋找中心點
        if (slots.Count == 0)
        {
            centerT = transform;
            newWidth = 0.5f;
            newHeight = 0.5f;
            return;
        }
        float xMin = Mathf.Infinity;
        float xMax = -Mathf.Infinity;
        float yMin = Mathf.Infinity;
        float yMax = -Mathf.Infinity;

        foreach (Transform t in slots)
        {
            float x = t.position.x;
            float y = t.position.z;

            if (x < xMin) xMin = x;
            if (x > xMax) xMax = x;
            if (y < yMin) yMin = y;
            if (y > yMax) yMax = y;
        }

        GameObject centerObj = new("GroupCenter");
        centerObj.transform.position = new Vector3((xMin + xMax) * 0.5f, 0, (yMin + yMax) * 0.5f);
        centerObj.transform.parent = transform;
        centerT = centerObj.transform;
        newWidth = xMax - xMin + 1.0f;
        newHeight = yMax - yMin + 1.0f;
    }

    private List<Vector2Int> GetConnectedCells(int numberOfCells, int width, int height, int[,] oGrid)
    {
        //int failureCount = 0;
        //int maxFailures = 100;

        int x = 0, y = 0;

        List<Vector2Int> slots = new List<Vector2Int>();
        int[,] grid = new int[width, height];       //裡面預設值為 0

        List<Vector2Int> validStart = new List<Vector2Int>();
        for (int i=0; i<width; i++)
        {
            for (int j=0; j<height; j++)
            {
                if (oGrid[i,j] == 0)
                {
                    validStart.Add(new Vector2Int(i, j));
                }
            }
        }
        if (validStart.Count == 0)
            return slots;

        for (int i = 0; i < numberOfCells;)
        {
            bool isSuccessStep = false;
            //slots.Add(new Vector2Int(x, y));
            //grid[x, y] = 1;
            if (i == 0)
            {
                Vector2Int randomStart = validStart[Random.Range(0, validStart.Count)];
                x = randomStart.x; y = randomStart.y;
                isSuccessStep = true;
            }
            else
            {
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
                    //failureCount++;
                    //if (failureCount >= maxFailures)
                    //{
                    //    print("SpawnEnemies 超級失敗的");
                    //    break;
                    //}
                    ////這一步沒有下一步可走，退回上一步，還原到上一步
                    //print("回頭再試一次 !! " + i + "失敗次數: " + failureCount);
                    //i--;
                    //slots.RemoveAt(i);
                    //grid[x, y] = 0;
                    print("沒路可走了，先結束.... ");
                    break;
                }
                else
                {
                    int randomDirection = validDirections[Random.Range(0, validDirections.Count)];
                    x += dx[randomDirection];
                    y += dy[randomDirection];
                    isSuccessStep = true;
                }
            }
            if (isSuccessStep)
            {
                i++;
                slots.Add(new Vector2Int(x, y));
                grid[x, y] = 1;
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
                nextPhase = PHASE.WAIT;
            }
        }
    }

    protected void UpdateWait()
    {
        if (stateTime > 0.2f)
        {
            if (GetPlayerDistance() < alertDistance)
            {
                nextPhase = PHASE.BATTLE;
            }
        }
    }

    protected void UpdateBattle()
    {
        if (!BattleSystem.GetPC())
        {
            nextPhase = PHASE.FINISH;
        }
        //if (Vector3.Distance(transform.position, BattleSystem.GetPC().transform.position) > closeDistance)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, BattleSystem.GetPC().transform.position, Time.deltaTime * speed);
        //}
        UpdateBattleMove(); //TODO: 不要每個 Frame 算方向
        //if (stateTime > 0.2f)
        //{
        //    stateTime = 0;
        //    for (int i=0; i<enemies.Count; i++)
        //    {
        //        if (enemies[i] == null)
        //        {
        //            enemies.RemoveAt(i);
        //            //if (enemies.Count == 0)
        //            //{
        //            //    nextPhase = PHASE.FINISH;
        //            //}
        //            break;  //避免連續刪除
        //        }
        //    }
        //    if (enemies.Count == 0)     //放到這邊確保一開始就沒產生敵人時可以自然結束
        //    {
        //        nextPhase = PHASE.FINISH;
        //    }
        //}
        UpdateTraceEnemies();
    }


    protected void UpdateTraceEnemies()
    {
        if (stateTime > 0.2f)
        {
            stateTime = 0;
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i] == null)
                {
                    enemyList.RemoveAt(i);
                    break;  //避免連續刪除
                }
            }
            if (enemyList.Count == 0)     //放到這邊確保一開始就沒產生敵人時可以自然結束
            {
                nextPhase = PHASE.FINISH;
            }
        }
    }


    protected Vector3 moveDirection = Vector3.zero;
    protected float moveCheckTime = 0;
    protected void UpdateBattleMove()
    {
        moveCheckTime -= Time.deltaTime;
        if (moveCheckTime <= 0)
        {
            moveCheckTime = 0.2f;
            if (BattleSystem.GetPC() == null)
                return;
            Vector3 playerPos = BattleSystem.GetPC().transform.position;
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(centerT.position, playerPos, NavMesh.AllAreas, path))
            {
                float pathLength = 0f;
                Vector3 movdD = Vector3.zero;
                for (int i = 1; i < path.corners.Length; i++)
                {
                    pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                    if (i == 1)
                    {
                        movdD = path.corners[i] - path.corners[i - 1];
                    }
                }
                if (pathLength > closeDistance)
                {
                    if (pathLength > stopDistance)  //Group 停止追擊 (TODO: 跑回去?)
                    {
                        nextPhase = PHASE.WAIT;
                        return;
                    }
                    moveDirection = movdD;
                }
                else
                {
                    //已經抓到目標
                    moveDirection = Vector3.zero;
                    if (finishWhenEngaged)
                        nextPhase = PHASE.TRACE_ONLY;
                }
            }
            else
                moveDirection = Vector3.zero;
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + moveDirection, Time.deltaTime * speed);
    }


    protected float GetPlayerDistance()
    {
        if (BattleSystem.GetPC() == null)
            return Mathf.Infinity;
        Vector3 playerPos = BattleSystem.GetPC().transform.position;
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(centerT.position, playerPos, NavMesh.AllAreas, path))
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
        if (centerT && currPhase != PHASE.SLEEP)
            Gizmos.DrawWireCube(centerT.position, new Vector3(newWidth * gridWidth, 2.0f, newHeight * gridHeight));
        else
            Gizmos.DrawWireCube(transform.position, new Vector3(width * gridWidth, 2.0f, height * gridHeight));
    }

    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currPhase.ToString());

    //}
}
