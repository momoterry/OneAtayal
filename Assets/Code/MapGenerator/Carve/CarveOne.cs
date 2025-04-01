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

    public int intRoomWidth = 8;
    public int intRoomHeight = 8;

    public int corridorWidth = 4;
    public int corridorLengthMin = 12;
    public int corridorLengthMax = 20;

    public SpriteRenderer sr;
    protected int[,] map;
    protected List<Room> rooms = new List<Room>();
    protected System.Random rand = new System.Random();

    virtual public int[,] CreateCarveMap()
    {
        GenerateDungeon();
        return map;
    }

    protected void GenerateDungeon()
    {
        map = new int[width, height];

        // 初始房間
        int roomW = intRoomWidth;
        int roomH = intRoomHeight;
        int startX = width / 2 - roomW / 2;
        int startY = 0;
        Room startRoom = new Room(startX, startY, roomW, roomH);
        PlaceRoom(startRoom);
        rooms.Add(startRoom);

        // 生成額外房間與通道
        for (int i = 0; i < 3; i++)
        {
            List<Direction> directions = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            bool roomPlaced = false;

            while (directions.Count > 0 && !roomPlaced)
            {
                Direction dir = directions[rand.Next(directions.Count)];
                directions.Remove(dir);

                Room lastRoom = rooms[rooms.Count - 1];
                if (TryPlaceCorridorAndRoom(lastRoom, dir, out Room newRoom))
                {
                    rooms.Add(newRoom);
                    roomPlaced = true;
                }
            }
        }
    }

    protected bool TryPlaceCorridorAndRoom(Room fromRoom, Direction dir, out Room newRoom)
    {
        int cx, cy, corridorLength = RandomEven(corridorLengthMin, corridorLengthMax);
        //int corridorWidth = corriDerWidth;

        switch (dir)
        {
            case Direction.Up:
                cx = fromRoom.x + fromRoom.w / 2 - 1;
                cy = fromRoom.y + fromRoom.h;
                break;
            case Direction.Down:
                cx = fromRoom.x + fromRoom.w / 2 - 1;
                cy = fromRoom.y - corridorLength;
                break;
            case Direction.Left:
                cx = fromRoom.x - corridorLength;
                cy = fromRoom.y + fromRoom.h / 2 - 1;
                break;
            case Direction.Right:
                cx = fromRoom.x + fromRoom.w;
                cy = fromRoom.y + fromRoom.h / 2 - 1;
                break;
            default:
                newRoom = null;
                return false;
        }

        int newRoomW = RandomEven(roomWidthMin, roomWidthMax);
        int newRoomH = RandomEven(roomHeightMin, roomHeightMax);
        int rx = (dir == Direction.Left) ? cx - newRoomW - corridorLength : (dir == Direction.Right) ? cx + corridorLength : cx - newRoomW / 2;
        int ry = (dir == Direction.Down) ? cy - newRoomH - corridorLength : (dir == Direction.Up) ? cy + corridorLength : cy - newRoomH / 2;

        Room candidateRoom = new Room(rx, ry, newRoomW, newRoomH);
        if (!IsValidPlacement(candidateRoom) || !IsValidCorridor(cx, cy, corridorLength, corridorWidth, dir))
        {
            newRoom = null;
            return false;
        }

        PlaceCorridor(cx, cy, corridorLength, corridorWidth, dir);
        PlaceRoom(candidateRoom);
        newRoom = candidateRoom;
        return true;
    }

    protected bool IsValidPlacement(Room room)
    {
        if (room.x < 0 || room.y < 0 || room.x + room.w >= width || room.y + room.h >= height)
            return false;

        for (int x = room.x - 1; x <= room.x + room.w; x++)
            for (int y = room.y - 1; y <= room.y + room.h; y++)
                if (x >= 0 && y >= 0 && x < width && y < height && map[x, y] != 0)
                    return false;
        return true;
    }

    protected bool IsValidCorridor(int x, int y, int length, int w, Direction dir)
    {
        for (int i = 0; i < length; i++)
        {
            int cx = (dir == Direction.Left) ? x - i : (dir == Direction.Right) ? x + i : x;
            int cy = (dir == Direction.Up) ? y + i : (dir == Direction.Down) ? y - i : y;

            if (cx < 0 || cy < 0 || cx + w >= width || cy >= height || map[cx, cy] != 0)
                return false;
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

            //for (int j = 0; j < w; j++)
            //    map[cx, cy + j] = 2;
            for (int j = -w / 2 + 1; j <= w / 2; j++)
            {
                map[cx + dv.x * j, cy + j * dv.y] = 2;
            }
        }
    }

    void ApplyTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                texture.SetPixel(x, y, map[x, y] == 0 ? Color.black : map[x, y] == 1 ? Color.white : Color.gray);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.zero, 16);
    }

    protected int RandomEven(int min, int max) => min + rand.Next((max - min) / 2 + 1) * 2;
    protected enum Direction { Up, Down, Left, Right }
    protected class Room { public int x, y, w, h; public Room(int x, int y, int w, int h) { this.x = x; this.y = y; this.w = w; this.h = h; } }
}