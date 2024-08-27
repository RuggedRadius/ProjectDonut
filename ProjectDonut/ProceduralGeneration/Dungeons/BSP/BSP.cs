using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectDonut.ProceduralGeneration.Dungeons.BSP
{
    public class BSP
    {
        private Dictionary<int, List<Room>> _roomsGenerations = new Dictionary<int, List<Room>>();
        private List<Room> _rooms = new List<Room>();
        private Vector2 _roomMinSize = new Vector2(10, 10);
        private Vector2 _room2MinSize = new Vector2(5, 5);
        private Random _random = new Random();

        private int _roomCounter = 0;

        public List<Room> CreateRoomsWithinAreas(List<Room> rooms)
        {
            foreach (var room in rooms)
            {
                var b = room.Bounds;

                var newWidth = _random.Next((int)_room2MinSize.X, b.Width);
                var newHeight = _random.Next((int)_room2MinSize.Y, b.Height);

                var newX = _random.Next(b.Left, b.Right - newWidth);
                var newY = _random.Next(b.Top, b.Bottom - newHeight);

                room.Bounds = new Rectangle(newX, newY, newWidth, newHeight);
            }

            return rooms;
        }

        public List<Room> SquashRooms(List<Room> rooms, int width, int height)
        {
            int counter = 0;

            while (counter < 1000)
            {
                foreach (var room in rooms)
                {
                    if (room.Bounds.X >= 0)
                    {
                        var tempRect = new Rectangle(room.Bounds.X - 1, room.Bounds.Y, room.Bounds.Width, room.Bounds.Height);
                        var intersects = false;

                        foreach (var otherRoom in rooms)
                        {
                            if (otherRoom == room)
                            {
                                continue;
                            }

                            if (otherRoom.Bounds.Intersects(tempRect))
                            {
                                intersects = true;
                                break;
                            }

                            if (tempRect.X < 0 || tempRect.X > width - 1 || tempRect.Y < 0 || tempRect.Y > height - 1)
                            {
                                intersects = true;
                            }
                        }

                        if (!intersects)
                        {
                            room.Bounds = tempRect;
                        }
                    }
                    else if (room.Bounds.X < 0)
                    {
                        var tempRect = new Rectangle(room.Bounds.X + 1, room.Bounds.Y, room.Bounds.Width, room.Bounds.Height);
                        var intersects = false;

                        foreach (var otherRoom in rooms)
                        {
                            if (otherRoom == room)
                            {
                                continue;
                            }

                            if (otherRoom.Bounds.Intersects(tempRect))
                            {
                                intersects = true;
                                break;
                            }

                            if (tempRect.X < 0 || tempRect.X > width - 1 || tempRect.Y < 0 || tempRect.Y > height - 1)
                            {
                                intersects = true;
                            }
                        }

                        if (!intersects)
                        {
                            room.Bounds = tempRect;
                        }
                    }

                    if (room.Bounds.Y >= 0)
                    {
                        var tempRect = new Rectangle(room.Bounds.X, room.Bounds.Y - 1, room.Bounds.Width, room.Bounds.Height);
                        var intersects = false;

                        foreach (var otherRoom in rooms)
                        {
                            if (otherRoom == room)
                            {
                                continue;
                            }

                            if (otherRoom.Bounds.Intersects(tempRect))
                            {
                                intersects = true;
                                break;
                            }

                            if (tempRect.X < 0 || tempRect.X > width - 1 || tempRect.Y < 0 || tempRect.Y > height - 1)
                            {
                                intersects = true;
                            }
                        }

                        if (!intersects)
                        {
                            room.Bounds = tempRect;
                        }
                    }
                    else if (room.Bounds.Y < 0)
                    {
                        var tempRect = new Rectangle(room.Bounds.X, room.Bounds.Y + 1, room.Bounds.Width, room.Bounds.Height);
                        var intersects = false;

                        foreach (var otherRoom in rooms)
                        {
                            if (otherRoom == room)
                            {
                                continue;
                            }

                            if (otherRoom.Bounds.Intersects(tempRect))
                            {
                                intersects = true;
                                break;
                            }

                            if (tempRect.X < 0 || tempRect.X > width - 1 || tempRect.Y < 0 || tempRect.Y > height - 1)
                            {
                                intersects = true;
                            }
                        }

                        if (!intersects)
                        {
                            room.Bounds = tempRect;
                        }
                    }
                }

                counter++;
            }

            return rooms;
        }

        public Dictionary<int, List<Room>> GenerateRooms(int xTileCount, int yTileCount)
        {
            _roomsGenerations = new Dictionary<int, List<Room>>();
            _roomCounter = 0;

            var startBounds = new Rectangle(0, 0, xTileCount, yTileCount);
            _roomsGenerations.Add(0, new List<Room>() { new Room() { Bounds = startBounds, Sibling = null, RoomID = _roomCounter++ } });

            _rooms = _roomsGenerations[0];
            var genCounter = 1;
            var partitionableRooms = new List<Room>();
            do
            {
                _rooms = PartitionRooms(_roomsGenerations[genCounter - 1]);
                _roomsGenerations.Add(genCounter, _rooms);
                genCounter++;

                partitionableRooms.Clear();
                foreach (var room in _rooms)
                {
                    room.CanSplitVertical = IsRoomPartitionableVertically(room);
                    room.CanSplitHorizontal = IsRoomPartitionableHorizontally(room);

                    if (room.CanSplitHorizontal || room.CanSplitVertical)
                    {
                        partitionableRooms.Add(room);
                    }
                }
            }
            while (partitionableRooms.Count > 0);

            return _roomsGenerations;
        }

        private bool IsRoomPartitionableHorizontally(Room room)
        {
            var horizontalSizeAppropriate = room.Bounds.Width > (_roomMinSize.X * 2);

            if (horizontalSizeAppropriate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsRoomPartitionableVertically(Room room)
        {
            var verticalSizeAppropriate = room.Bounds.Height > (_roomMinSize.Y * 2);

            if (verticalSizeAppropriate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<Room> PartitionRooms(List<Room> rooms)
        {
            var roomsList = new List<Room>();

            foreach (var room in rooms)
            {
                if (!room.CanSplitHorizontal && !room.CanSplitVertical)
                {
                    roomsList.Add(room);
                    continue;
                }

                var originalCount = roomsList.Count;

                var coinFlip = _random.Next(0, 2);

                if (coinFlip == 0)
                {
                    if (room.CanSplitHorizontal)
                    {
                        roomsList.AddRange(SplitHorizontally(room));
                    }
                    else
                    {
                        roomsList.AddRange(SplitVertically(room));
                    }
                }
                else
                {
                    if (room.CanSplitVertical)
                    {
                        roomsList.AddRange(SplitVertically(room));
                    }
                    else
                    {
                        roomsList.AddRange(SplitHorizontally(room));
                    }
                }

                // If no rooms were added, it means the room couldn't be partitioned.
                if (roomsList.Count == originalCount)
                {
                    roomsList.Add(room);
                }
            }

            return roomsList;
        }

        private List<Room> SplitVertically(Room room)
        {
            var minValue = (int)_roomMinSize.Y;
            var maxValue = room.Bounds.Height - minValue; // Ensure space for both rooms

            if (maxValue <= minValue)
            {
                return new List<Room>() { room };
            }

            var split = _random.Next(minValue, maxValue);

            var roomA = new Room()
            {
                RoomID = _roomCounter++,
                Bounds = new Rectangle(room.Bounds.X, room.Bounds.Y, room.Bounds.Width, split),
            };

            var roomB = new Room()
            {
                RoomID = _roomCounter++,
                Bounds = new Rectangle(room.Bounds.X, room.Bounds.Y + split, room.Bounds.Width, room.Bounds.Height - split),
            };

            roomA.Sibling = roomB;
            roomB.Sibling = roomA;

            return new List<Room>() { roomA, roomB };
        }

        private List<Room> SplitHorizontally(Room room)
        {
            var minValue = (int)_roomMinSize.X;
            var maxValue = room.Bounds.Width - minValue; // Ensure space for both rooms

            if (maxValue <= minValue)
            {
                return new List<Room>() { room };
            }

            var split = _random.Next(minValue, maxValue);

            var roomA = new Room()
            {
                RoomID = _roomCounter++,
                Bounds = new Rectangle(room.Bounds.X, room.Bounds.Y, split, room.Bounds.Height),
            };

            var roomB = new Room()
            {
                RoomID = _roomCounter++,
                Bounds = new Rectangle(room.Bounds.X + split, room.Bounds.Y, room.Bounds.Width - split, room.Bounds.Height),
            };

            roomA.Sibling = roomB;
            roomB.Sibling = roomA;

            return new List<Room>() { roomA, roomB };
        }


        public int[,] CreateDataMap(List<Room> rooms, int width, int height)
        {
            // Initialise canvas
            int[,] canvas = new int[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    canvas[x, y] = 0;
                }
            }

            // Populate with floors and walls
            foreach (var room in rooms)
            {
                for (int y = room.Bounds.Top; y < room.Bounds.Bottom; y++)
                {
                    for (int x = room.Bounds.Left; x < room.Bounds.Right; x++)
                    {
                        // Check if the current position is on the edge of the room
                        if (x == room.Bounds.Left || 
                            x == room.Bounds.Right - 1 ||
                            y == room.Bounds.Top || 
                            y == room.Bounds.Bottom - 1)
                        {
                            if (x >= 0 && x < width && y >= 0 && y < height)
                            {
                                canvas[x, y] = 1;
                            }
                        }
                        else
                        {
                            canvas[x, y] = 2;
                        }
                    }
                }
            }

            return canvas;
        }
    
        public int[,] LinkAllRooms(List<Room> rooms, int width, int height)
        {
            var canvas = new int[width, height];

            foreach (var room in rooms)
            {
                if (room.CorridorDrawn)
                {
                    continue;
                }

                var curRoomLink = LinkRooms(room, room.Sibling, width, height);
                canvas = MergeArrays(canvas, curRoomLink); 
            }

            return canvas;
        }

        public int[,] LinkAllRooms2(List<(Rectangle, Rectangle)> rooms, int width, int height)
        {
            var canvas = new int[width, height];

            foreach (var roomPair in rooms)
            {
                var curRoomLink = LinkRooms2(roomPair, width, height);
                canvas = MergeArrays(canvas, curRoomLink);
            }

            return canvas;
        }

        private int[,] LinkRooms2((Rectangle, Rectangle) rooms, int width, int height)
        {
            var canvas = new int[width, height];

            var aX = _random.Next(rooms.Item1.X + 1, rooms.Item1.X + rooms.Item1.Width - 1);
            var aY = _random.Next(rooms.Item1.Y + 1, rooms.Item1.Y + rooms.Item1.Height - 1);
            var bX = _random.Next(rooms.Item2.X + 1, rooms.Item2.X + rooms.Item2.Width - 1);
            var bY = _random.Next(rooms.Item2.Y + 1, rooms.Item2.Y + rooms.Item2.Height - 1);

            var deltaX = bX - aX;
            var deltaY = bY - aY;

            // Draw horizontal corridor
            if (deltaX != 0)
            {
                int stepX = deltaX > 0 ? 1 : -1;
                for (int x = 0; x != deltaX; x += stepX)
                {
                    if (rooms.Item1.Contains(aX + x, aY) || rooms.Item2.Contains(aX + x, aY))
                    {
                        continue;
                    }

                    canvas[aX + x, aY] = 4;
                }
            }

            // Draw vertical corridor
            if (deltaY != 0)
            {
                int stepY = deltaY > 0 ? 1 : -1;
                for (int y = 0; y != deltaY; y += stepY)
                {
                    if (rooms.Item1.Contains(aX + deltaX, aY + y) || rooms.Item2.Contains(aX + deltaX, aY + y))
                    {
                        continue;
                    }

                    canvas[aX + deltaX, aY + y] = 4;
                }
            }

            // Set the final position of bX, bY
            //canvas[bX, bY] = 1;

            return canvas;
        }


        private int[,] LinkRooms(Room roomA, Room roomB, int width, int height)
        {
            var canvas = new int[width, height];

            var aX = _random.Next(roomA.Bounds.X + 1, roomA.Bounds.X + roomA.Bounds.Width - 1);
            var aY = _random.Next(roomA.Bounds.Y + 1, roomA.Bounds.Y + roomA.Bounds.Height - 1);
            var bX = _random.Next(roomB.Bounds.X + 1, roomB.Bounds.X + roomB.Bounds.Width - 1);
            var bY = _random.Next(roomB.Bounds.Y + 1, roomB.Bounds.Y + roomB.Bounds.Height - 1);

            var deltaX = bX - aX;
            var deltaY = bY - aY;

            // Draw horizontal corridor
            if (deltaX != 0)
            {
                int stepX = deltaX > 0 ? 1 : -1;
                for (int x = 0; x != deltaX; x += stepX)
                {
                    if(roomA.Bounds.Contains(aX + x, aY) || roomB.Bounds.Contains(aX + x, aY))
                    {
                        continue;
                    }

                    canvas[aX + x, aY] = 4;
                }
            }

            // Draw vertical corridor
            if (deltaY != 0)
            {
                int stepY = deltaY > 0 ? 1 : -1;
                for (int y = 0; y != deltaY; y += stepY)
                {
                    if (roomA.Bounds.Contains(aX + deltaX, aY + y) || roomB.Bounds.Contains(aX + deltaX, aY + y))
                    {
                        continue;
                    }

                    canvas[aX + deltaX, aY + y] = 4;
                }
            }

            // Set the final position of bX, bY
            canvas[bX, bY] = 1;

            roomA.CorridorDrawn = true;
            roomB.CorridorDrawn = true;

            return canvas;
        }

        public static int[,] MergeArrays(int[,] array1, int[,] array2)
        {
            int rows = array1.GetLength(0);
            int cols = array1.GetLength(1);

            if (rows != array2.GetLength(0) || cols != array2.GetLength(1))
            {
                throw new ArgumentException("Both arrays must have the same dimensions.");
            }

            int[,] result = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = array2[i, j] != 0 ? array2[i, j] : array1[i, j];
                }
            }

            return result;
        }


    }
}
