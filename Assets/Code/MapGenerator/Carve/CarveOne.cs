using System.Collections.Generic;
using UnityEngine;

public class CarveOne : MonoBehaviour
{
    public int width = 60;
    public int height = 120;
    public int roomWidthMin = 16;
    public int roomWidthMax = 20;
    public int roomHeightMin = 20;
    public int roomHeightMax = 26;

    public int initRoomWidth = 8;
    public int initRoomHeight = 8;

    public int corridorWidth = 4;
    public int corridorLengthMin = 12;
    public int corridorLengthMax = 20;

    public SpriteRenderer sr;
    protected int[,] map;
    protected List<Room> rooms = new List<Room>();
    protected System.Random rand = new System.Random();

    virtual public int[,] CreateCarveMap()
    {
        InitDungeon(initRoomWidth, initRoomWidth);
        //GenerateDungeon();
        //GenerateDungeonPath(roomWidthMin, roomWidthMax, roomHeightMin, roomHeightMax, 2, true);
        GenerateDungeonPath(8, 8, 12, 12, 4, true);
        GenerateDungeonPath(12, 12, 16, 16, 2, true);
        return map;
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
        rooms.Add(startRoom);
    }
    protected void GenerateDungeonPath(int _roomWidthMin, int _roomWidthMax, int _roomHeightMin, int _roomHeightMax, int num, bool isMainPath)
    {
        roomWidthMin = _roomWidthMin;
        roomWidthMax = _roomWidthMax;
        roomHeightMin = _roomHeightMin;
        roomHeightMax = _roomHeightMax;

        // 生成房間與通道
        List<Direction> directionsAll = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        Room prevRoom = rooms[rooms.Count - 1];
        for (int i = 0; i < num; i++)
        {
            List<Direction> directions = new();
            foreach (Direction d in directionsAll)
            {
                if (!prevRoom.IsPath(d))
                    directions.Add(d);
            }
            bool roomPlaced = false;

            while (directions.Count > 0 && !roomPlaced)
            {
                Direction dir = directions[rand.Next(directions.Count)];
                directions.Remove(dir);

                //Room lastRoom = rooms[rooms.Count - 1];
                if (TryPlaceCorridorAndRoom(prevRoom, dir, out Room newRoom))
                {
                    prevRoom = newRoom;
                    rooms.Add(newRoom);
                    roomPlaced = true;
                }
            }
        }

    }

    protected bool TryPlaceCorridorAndRoom(Room fromRoom, Direction dir, out Room newRoom)
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
            case Direction.Up:
                cx = fromRoom.x + fromRoom.w / 2 - 1;
                cy = fromRoom.y + fromRoom.h;
                break;
            case Direction.Down:
                cx = fromRoom.x + fromRoom.w / 2 - 1;
                cy = fromRoom.y - 1;
                break;
            case Direction.Left:
                cx = fromRoom.x - 1;
                cy = fromRoom.y + fromRoom.h / 2 - 1;
                break;
            case Direction.Right:
                cx = fromRoom.x + fromRoom.w;
                cy = fromRoom.y + fromRoom.h / 2 - 1;
                break;
            default:
                return false;
        }

        int newRoomW = RandomEven(roomWidthMin, roomWidthMax);
        int newRoomH = RandomEven(roomHeightMin, roomHeightMax);
        int rx = (dir == Direction.Left) ? cx - newRoomW - corridorLength + 1 : (dir == Direction.Right) ? cx + corridorLength : cx - newRoomW / 2 + 1;
        int ry = (dir == Direction.Down) ? cy - newRoomH - corridorLength + 1 : (dir == Direction.Up) ? cy + corridorLength : cy - newRoomH / 2 + 1;

        Room candidateRoom = new Room(rx, ry, newRoomW, newRoomH);
        if (!IsValidPlacement(candidateRoom) || !IsValidCorridor(cx, cy, corridorLength, corridorWidth, dir))
        {
            return false;
        }
        PlaceCorridor(cx, cy, corridorLength, corridorWidth, dir);
        PlaceRoom(candidateRoom);
        newRoom = candidateRoom;
        fromRoom.SetIsPath(dir, true);
        return true;
    }

    protected bool IsValidPlacement(Room room)
    {
        if (room.x - 1 < 0 || room.y - 1 < 0 || room.x + room.w >= width || room.y + room.h >= height)
            return false;

        for (int x = room.x - 1; x <= room.x + room.w; x++)
        {
            for (int y = room.y - 1; y <= room.y + room.h; y++)
            {
                if (map[x, y] != 0)
                    return false;
            }
        }
        return true;
    }

    protected bool IsValidCorridor(int x, int y, int length, int w, Direction dir)
    {
        //for (int i = 0; i < length; i++)
        //{
        //    int cx = (dir == Direction.Left) ? x - i : (dir == Direction.Right) ? x + i : x;
        //    int cy = (dir == Direction.Up) ? y + i : (dir == Direction.Down) ? y - i : y;

        //    if (cx < 0 || cy < 0 || cx + w >= width || cy >= height || map[cx, cy] != 0)
        //        return false;
        //}
        //return true;

        Vector2Int dv = Vector2Int.zero;
        switch (dir)
        {
            case Direction.Up:
            case Direction.Down:
                dv = new Vector2Int(1, 0);
                break;
            case Direction.Left:
            case Direction.Right:
                dv = new Vector2Int(0, 1);
                break;
        }
        for (int i = 0; i < length; i++)
        {
            int cx = (dir == Direction.Left) ? x - i : (dir == Direction.Right) ? x + i : x;
            int cy = (dir == Direction.Up) ? y + i : (dir == Direction.Down) ? y - i : y;

            for (int j = -w / 2 + 1; j <= w / 2; j++)
            {
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

    protected void PlaceCorridor(int x, int y, int length, int w, Direction dir)
    {
        Vector2Int dv = Vector2Int.zero;
        switch (dir)
        {
            case Direction.Up:
            case Direction.Down:
                dv = new Vector2Int(1, 0);
                break;
            case Direction.Left:
            case Direction.Right:
                dv = new Vector2Int(0, 1);
                break;
        }
        for (int i = 0; i < length; i++)
        {
            int cx = (dir == Direction.Left) ? x - i : (dir == Direction.Right) ? x + i : x;
            int cy = (dir == Direction.Up) ? y + i : (dir == Direction.Down) ? y - i : y;

            for (int j = -w / 2 + 1; j <= w / 2; j++)
            {
                map[cx + dv.x * j, cy + j * dv.y] = 2;
            }
        }
    }

    protected int RandomEven(int min, int max) => min + rand.Next((max - min) / 2 + 1) * 2;
    protected enum Direction { Up, Down, Left, Right, NUM }
    protected class Room 
    { 
        public int x, y, w, h;
        public bool[] isPath = new bool[(int)Direction.NUM];
        public bool IsPath(Direction dir) { return isPath[(int)dir]; }
        public void SetIsPath(Direction dir, bool _isPath) { isPath[(int)dir] = _isPath; }
        public Room(int x, int y, int w, int h) 
        { 
            this.x = x; this.y = y; this.w = w; this.h = h;
        } }
}