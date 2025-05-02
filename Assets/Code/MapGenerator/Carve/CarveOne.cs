using System;
using System.Collections.Generic;
using UnityEngine;

public class CarveOne : MonoBehaviour
{
    public int width = 60;
    public int height = 120;
    public int initRoomWidth = 8;
    public int initRoomHeight = 8;
    public int bufferWidth = 1;     //房間之間的最小間隔 = bufferWidth x 2
    public int bufferHeight = 2;     //房間之間的最小間隔 = bufferHeight x 2

    [System.Serializable]
    public class RoomSequenceInfo
    {
        public enum TYPE
        {
            MAIN_ADD,       //主線和線追加 (只有一條)
            BRANCH_NEW,     //新支線
            BRANCH_ADD,     //支線追加 (從上一個支線)
        }
        [Header("基本設定")]
        [Tooltip("路徑追加方式")]
        public TYPE type = TYPE.MAIN_ADD;
        //public bool isMainPath = true;
        [Tooltip("要生成的房間數量")]
        [Min(1)]
        public int roomNum = 2;
        [Space(10)]
        [Header("房間設定")]
        public int roomWidthMin = 16;
        public int roomWidthMax = 20;
        public int roomHeightMin = 20;
        public int roomHeightMax = 26;
        [Space(10)]
        [Header("通道設定")]
        public int corridorWidth = 4;
        public int corridorLengthMin = 12;
        public int corridorLengthMax = 20;
    }
    public RoomSequenceInfo[] paths;

    public class Corridor
    {
        public int x, y, w, h;
    }

    //房間定義
    public class Room
    {
        public int x, y, w, h;
        public bool[] isPath = new bool[4];
        public Corridor corridorFrom = null;    //來到這個房間的通道
        public bool IsPath(DIRECTION dir) { return isPath[(int)dir]; }
        public void SetIsPath(DIRECTION dir, bool _isPath) { isPath[(int)dir] = _isPath; }
        public Room(int x, int y, int w, int h)
        {
            this.x = x; this.y = y; this.w = w; this.h = h;
        }
    }

    //支線定義
    public class Branch
    {
        public int mainDepth;       //從主線的第幾個房間分出來
        public List<Room> rooms = new List<Room>();
    }

    protected int roomWidthMin = 16;
    protected int roomWidthMax = 20;
    protected int roomHeightMin = 20;
    protected int roomHeightMax = 26;
    protected int corridorWidth = 4;
    protected int corridorLengthMin = 12;
    protected int corridorLengthMax = 20;

    protected int[,] map;
    protected List<Room> mainPathRooms = new List<Room>();              //主線上的房間
    //protected List<Room> branchPathRooms = new List<Room>();      //支線上的房間
    protected List<Branch> branchs = new List<Branch>();            //所有支線定義
    protected System.Random rand = new System.Random();

    public List<Room> GetMainPathRooms()
    {
        return mainPathRooms;
    }

    public List<Branch> GetAllBranches()
    {
        return branchs;
    }

    virtual public int[,] CreateCarveMap()
    {

        int buildCount = 0;
        bool buildFinish = false;
        while (buildCount < 100 && !buildFinish)
        {
            InitDungeon(initRoomWidth, initRoomWidth);

            buildFinish = true;
            buildCount++;

            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                //int num = GenerateDungeonPath(path.roomWidthMin, path.roomWidthMax, path.roomHeightMin, path.roomHeightMax, path.roomNum, path.isMainPath);
                int num = GenerateDungeonPath(path);

                if (num < path.roomNum)
                {
                    buildFinish = false;
                    break;
                }
            }
        }
        
        if (buildFinish)
        {
            One.LOG("路徑創建第 " + buildCount + " 次成功");
        }
        else
        {
            One.ERROR("路徑創建失敗");
        }

        return map;
    }

    protected void SetPathInfo( RoomSequenceInfo pathInfo)
    {
        roomWidthMin = pathInfo.roomWidthMin;
        roomWidthMax = pathInfo.roomWidthMax;
        roomHeightMin = pathInfo.roomHeightMin;
        roomHeightMax = pathInfo.roomHeightMax;
        corridorWidth = pathInfo.corridorWidth;
        corridorLengthMin = pathInfo.corridorLengthMin;
        corridorLengthMax = pathInfo.corridorLengthMax;
    }

    protected void InitDungeon(int _initRoomWidth, int _initRoomHeight)
    {
        map = new int[width, height];

        // 初始房間
        initRoomWidth = _initRoomWidth;
        initRoomHeight = _initRoomHeight;
        int startX = width / 2 - initRoomWidth / 2;
        int startY = 0;
        Room startRoom = new Room(startX, startY, initRoomWidth, initRoomHeight);
        PlaceRoom(startRoom);
        mainPathRooms.Clear();          //確保 InitDungeon 可以重複使用
        mainPathRooms.Add(startRoom);
        //branchPathRooms.Clear();
        branchs.Clear();
    }

    int mainIndexForBranch = 0;
    protected int GenerateDungeonPath( RoomSequenceInfo pathInfo)
    {
        SetPathInfo(pathInfo);

        List<DIRECTION> directionsAll = new List<DIRECTION> { DIRECTION.U, DIRECTION.D, DIRECTION.L, DIRECTION.R };

        //如果是主線，從最後房間出發，如果是支線，從最後房間以外的房間隨機挑選
        //Room prevRoom = pathInfo.type == RoomSequenceInfo.TYPE.MAIN_ADD ? mainPathRooms[mainPathRooms.Count - 1] : mainPathRooms[rand.Next(mainPathRooms.Count - 1)];
        Room prevRoom = null;
        switch (pathInfo.type)
        {
            case RoomSequenceInfo.TYPE.MAIN_ADD:
                prevRoom = mainPathRooms[mainPathRooms.Count - 1];
                break;
            case RoomSequenceInfo.TYPE.BRANCH_NEW:
                Branch newBranch = new();
                newBranch.mainDepth = rand.Next(mainPathRooms.Count - 1);
                branchs.Add(newBranch);
                prevRoom = mainPathRooms[newBranch.mainDepth];
                break;
            case RoomSequenceInfo.TYPE.BRANCH_ADD:
                //if (branchPathRooms.Count > 0)
                //    prevRoom = branchPathRooms[branchPathRooms.Count - 1];
                //else
                //    prevRoom = mainPathRooms[rand.Next(mainPathRooms.Count - 1)];
                if (branchs.Count > 0)
                {
                    List<Room> lastBranch = branchs[branchs.Count - 1].rooms;
                    prevRoom = lastBranch[lastBranch.Count - 1];
                }
                else
                {
                    One.ERROR("CarveOne : BRANCH_ADD 但沒有任何支線");
                    return 0;
                }
                break;
        }
        
        int roomPlacedNum = 0;

        // 生成房間與通道
        for (int i = 0; i < pathInfo.roomNum; i++)
        {
            List<DIRECTION> directions = new();
            foreach (DIRECTION d in directionsAll)
            {
                if (!prevRoom.IsPath(d))
                    directions.Add(d);
            }
            bool roomPlaced = false;

            while (directions.Count > 0 && !roomPlaced)
            {
                DIRECTION dir = directions[rand.Next(directions.Count)];
                directions.Remove(dir);

                //Room lastRoom = rooms[rooms.Count - 1];
                if (TryPlaceCorridorAndRoom(prevRoom, dir, out Room newRoom))
                {
                    prevRoom = newRoom;
                    switch (pathInfo.type)
                    {
                        case RoomSequenceInfo.TYPE.MAIN_ADD:
                            //如果是主線，加到主線列表中
                            mainPathRooms.Add(newRoom);
                            break;
                        case RoomSequenceInfo.TYPE.BRANCH_NEW:
                        case RoomSequenceInfo.TYPE.BRANCH_ADD:
                            //如果是支線，加到最後一個支線列表中
                            //branchPathRooms.Add(newRoom);
                            branchs[branchs.Count - 1].rooms.Add(newRoom);
                            break;
                    }
                    //if (pathInfo.isMainPath)
                    //{
                    //    //如果是主線，加到主線列表中
                    //    mainPathRooms.Add(newRoom);
                    //}
                    //else
                    //{
                    //    //如果是支線，加到支線列表中
                    //    branchPathRooms.Add(newRoom);
                    //}
                    roomPlaced = true;
                    roomPlacedNum++;
                }
            }

            if (!roomPlaced)
            {
                //print("房間創建失敗 .....");
                break;
            }
        }

        return roomPlacedNum;
    }
    protected bool TryPlaceCorridorAndRoom(Room fromRoom, DIRECTION dir, out Room newRoom)
    {
        newRoom = null;
        if (fromRoom.IsPath(dir))
        {
            print("已經不能再挖的方向，離開");
            return false;
        }

        //cx,cy 起點
        int cx, cy, corridorLength = RandomEven(corridorLengthMin, corridorLengthMax);

        switch (dir)
        {
            case DIRECTION.U:
                cx = fromRoom.x + fromRoom.w / 2 - 1;
                cy = fromRoom.y + fromRoom.h;
                break;
            case DIRECTION.D:
                cx = fromRoom.x + fromRoom.w / 2 - 1;
                cy = fromRoom.y - 1;
                break;
            case DIRECTION.L:
                cx = fromRoom.x - 1;
                cy = fromRoom.y + fromRoom.h / 2 - 1;
                break;
            case DIRECTION.R:
                cx = fromRoom.x + fromRoom.w;
                cy = fromRoom.y + fromRoom.h / 2 - 1;
                break;
            default:
                return false;
        }

        int newRoomW = RandomEven(roomWidthMin, roomWidthMax);
        int newRoomH = RandomEven(roomHeightMin, roomHeightMax);
        int rx = (dir == DIRECTION.L) ? cx - newRoomW - corridorLength + 1 : (dir == DIRECTION.R) ? cx + corridorLength : cx - newRoomW / 2 + 1;
        int ry = (dir == DIRECTION.D) ? cy - newRoomH - corridorLength + 1 : (dir == DIRECTION.U) ? cy + corridorLength : cy - newRoomH / 2 + 1;

        Room candidateRoom = new Room(rx, ry, newRoomW, newRoomH);
        if (!IsValidPlacement(candidateRoom) || !IsValidCorridor(cx, cy, corridorLength, corridorWidth, dir))
        {
            return false;
        }
        Corridor newCorridor = PlaceCorridor(cx, cy, corridorLength, corridorWidth, dir);
        PlaceRoom(candidateRoom);
        newRoom = candidateRoom;
        fromRoom.SetIsPath(dir, true);
        newRoom.SetIsPath(GetInverseDirection(dir),true);
        newRoom.corridorFrom = newCorridor;
        return true;
    }

    protected bool IsValidPlacement(Room room)
    {
        if (room.x - bufferWidth < 0 || room.y - bufferHeight < 0 || room.x + room.w + bufferWidth > width || room.y + room.h + bufferHeight > height)
            return false;

        int xMin = Mathf.Max(0, room.x - bufferWidth * 2);
        int xMax = Mathf.Min(width, room.x + room.w + bufferWidth * 2);
        int yMin = Mathf.Max(0, room.y - bufferHeight * 2);
        int yMax = Mathf.Min(height, room.y + room.h + bufferHeight * 2);
        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                if (map[x, y] != 0)
                    return false;
            }
        }
        return true;
    }

    protected bool IsValidCorridor(int x, int y, int length, int w, DIRECTION dir)
    {
        Vector2Int dv = Vector2Int.zero;
        int hW = w / 2; //檢查範圍的「半徑」
        switch (dir)
        {
            case DIRECTION.U:
            case DIRECTION.D:
                dv = new Vector2Int(1, 0);
                hW += bufferWidth;
                break;
            case DIRECTION.L:
            case DIRECTION.R:
                dv = new Vector2Int(0, 1);
                hW += bufferHeight;
                break;
        }
        for (int i = 0; i < length; i++)
        {
            int cx = (dir == DIRECTION.L) ? x - i : (dir == DIRECTION.R) ? x + i : x;
            int cy = (dir == DIRECTION.U) ? y + i : (dir == DIRECTION.D) ? y - i : y;

            for (int j = -hW + 1; j <= hW; j++)
            {
                if (cx + dv.x * j < 0 || cx + dv.x * j >= width || cy + j * dv.y < 0 || cy + j * dv.y >= height)
                    continue;
                if (map[cx + dv.x * j, cy + j * dv.y] != 0)
                    return false;
            }
        }

        return true;
    }

    protected void PlaceRoom(Room room)
    {
        for (int x = room.x; x < room.x + room.w; x++)
            for (int y = room.y; y < room.y + room.h; y++)
                map[x, y] = 1;
    }

    protected Corridor PlaceCorridor(int x, int y, int length, int w, DIRECTION dir)
    {
        int x2 = x, x1 = x, y1 = y, y2 = y;     //x1, y1, x2, y2 都是「包含」
        switch (dir)
        {
            case DIRECTION.U:
                y2 += length - 1;
                x1 -= w / 2 - 1;
                x2 = x1 + w - 1;
                break;
            case DIRECTION.D:
                y1 -= length + 1;
                x1 -= w / 2 - 1;
                x2 = x1 + w - 1;
                break;
            case DIRECTION.R:
                x2 += length - 1;
                y1 -= w / 2 - 1;
                y2 = y1 + w - 1;
                break;
            case DIRECTION.L:
                x1 -= length;
                y1 -= w / 2 - 1;
                y2 = y1 + w - 1;
                break;

        }

        for (int i=x1; i<=x2; i++)
        {
            for (int j=y1; j<=y2; j++)
            {
                map[i, j] = 2;
            }
        }

        //Vector2Int dv = Vector2Int.zero;
        //switch (dir)
        //{
        //    case DIRECTION.U:
        //    case DIRECTION.D:
        //        dv = new Vector2Int(1, 0);
        //        break;
        //    case DIRECTION.L:
        //    case DIRECTION.R:
        //        dv = new Vector2Int(0, 1);
        //        break;
        //}
        //for (int i = 0; i < length; i++)
        //{
        //    int cx = (dir == DIRECTION.L) ? x - i : (dir == DIRECTION.R) ? x + i : x;
        //    int cy = (dir == DIRECTION.U) ? y + i : (dir == DIRECTION.D) ? y - i : y;

        //    for (int j = -w / 2 + 1; j <= w / 2; j++)
        //    {
        //        map[cx + dv.x * j, cy + j * dv.y] = 2;
        //    }
        //}

        return new Corridor { x = x1, y = y1, w = x2 - x1 + 1, h = y2 - y1 + 1 };
    }

    protected DIRECTION GetInverseDirection(DIRECTION dir)
    {
        switch (dir)
        {
            case DIRECTION.U:
                return DIRECTION.D;
            case DIRECTION.D:
                return DIRECTION.U;
            case DIRECTION.L:
                return DIRECTION.R;
            case DIRECTION.R:
                return DIRECTION.L;
        }
        return DIRECTION.NONE;
    }

    protected int RandomEven(int min, int max) => min + rand.Next((max - min) / 2 + 1) * 2;

}