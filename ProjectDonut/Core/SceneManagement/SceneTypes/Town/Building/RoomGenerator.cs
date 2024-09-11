using Microsoft.Xna.Framework;
using ProjectDonut.ProceduralGeneration.BSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public static class RoomGenerator
    {
        public static List<Rectangle> GenerateRooms(Rectangle houseBounds, Vector2 roomMinSize)
        {
            var rooms = new List<Rectangle>();

            var allRoomsNotValidForVerticalSplit = false;
            var allRoomsNotValidForHorizontalSplit = false;

            do
            {
                var validVertical = IsRoomPartitionableVertically(houseBounds, roomMinSize);
                var validHorizontal = IsRoomPartitionableHorizontally(houseBounds, roomMinSize);

                if (validVertical && validHorizontal)
                {
                    var random = new Random();
                    var vertical = random.Next(0, 2) == 0;

                    if (vertical)
                    {
                        rooms = SplitVertically(houseBounds, roomMinSize);
                    }
                    else
                    {
                        rooms = SplitHorizontally(houseBounds, roomMinSize);
                    }
                }
                else if (validVertical)
                {
                    rooms = SplitVertically(houseBounds, roomMinSize);
                }
                else if (validHorizontal)
                {
                    rooms = SplitHorizontally(houseBounds, roomMinSize);
                }
                else
                {
                    rooms.Add(houseBounds);
                }

                allRoomsNotValidForVerticalSplit = false;
                allRoomsNotValidForHorizontalSplit = false;
                foreach (var room in rooms)
                {
                    if (IsRoomPartitionableHorizontally(room, roomMinSize))
                    {
                        allRoomsNotValidForHorizontalSplit = true;
                        break;
                    }
                }

                foreach (var room in rooms)
                {
                    if (IsRoomPartitionableVertically(room, roomMinSize))
                    {
                        allRoomsNotValidForHorizontalSplit = true;
                        break;
                    }
                }

                //allRoomsNotValidForVerticalSplit = rooms.All(r => !IsRoomPartitionableVertically(r, roomMinSize));
                //allRoomsNotValidForHorizontalSplit = rooms.All(r => !IsRoomPartitionableHorizontally(r, roomMinSize));
            }
            while (allRoomsNotValidForHorizontalSplit && allRoomsNotValidForVerticalSplit);

            return rooms;
        }

        private static List<Rectangle> SplitVertically(Rectangle room, Vector2 roomMinSize)
        {
            var random = new Random();
            var minValue = (int)roomMinSize.Y;
            var maxValue = room.Height - minValue; // Ensure space for both rooms

            if (maxValue <= minValue)
            {
                return new List<Rectangle>() { room };
            }

            var split = random.Next(minValue, maxValue);

            var roomA = new Rectangle(room.X, room.Y, room.Width, split);
            var roomB = new Rectangle(room.X, room.Y + split, room.Width, room.Height - split);

            return new List<Rectangle>() { roomA, roomB };
        }

        private static List<Rectangle> SplitHorizontally(Rectangle room, Vector2 roomMinSize)
        {
            var random = new Random();
            var minValue = (int)roomMinSize.X;
            var maxValue = room.Width - minValue; // Ensure space for both rooms

            if (maxValue <= minValue)
            {
                return new List<Rectangle>() { room };
            }

            var split = random.Next(minValue, maxValue);

            var roomA = new Rectangle(room.X, room.Y, split, room.Height);
            var roomB = new Rectangle(room.X + split, room.Y, room.Width - split, room.Height);

            return new List<Rectangle>() { roomA, roomB };
        }

        private static bool IsRoomPartitionableHorizontally(Rectangle room, Vector2 roomMinSize)
        {
            var horizontalSizeAppropriate = room.Width > roomMinSize.X * 2;

            if (horizontalSizeAppropriate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsRoomPartitionableVertically(Rectangle room, Vector2 roomMinSize)
        {
            var verticalSizeAppropriate = room.Height > roomMinSize.Y * 2;

            if (verticalSizeAppropriate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
