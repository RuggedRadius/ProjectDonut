using Microsoft.Xna.Framework;
using ProjectDonut.ProceduralGeneration.BSP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Core.SceneManagement.SceneTypes.Town.Building
{
    public static class RoomGenerator
    {
        public static List<Rectangle> GenerateRooms(Rectangle houseBounds, Vector2 roomMinSize)
        {
            var rooms = new List<Rectangle>()
            {
                houseBounds
            };

            var aRoomCanBeSplitVertically = false;
            var aRoomCanBeSplitHorizontally = false;

            do
            {
                var iterationRooms = new List<Rectangle>();
                foreach (var room in rooms)
                {
                    var validVertical = IsRoomPartitionableVertically(room, roomMinSize);
                    var validHorizontal = IsRoomPartitionableHorizontally(room, roomMinSize);

                    if (validVertical && validHorizontal)
                    {
                        var random = new Random();
                        var vertical = random.Next(0, 2) == 0;

                        if (vertical)
                        {
                            iterationRooms.AddRange(SplitVertically(room, roomMinSize));
                        }
                        else
                        {
                            iterationRooms.AddRange(SplitHorizontally(room, roomMinSize));
                        }
                    }
                    else if (validVertical)
                    {
                        iterationRooms.AddRange(SplitVertically(room, roomMinSize));
                    }
                    else if (validHorizontal)
                    {
                        iterationRooms.AddRange(SplitHorizontally(room, roomMinSize));
                    }
                    else
                    {
                        iterationRooms.Add(room);
                    }
                }

                rooms.Clear(); 
                rooms.AddRange(iterationRooms);

                aRoomCanBeSplitVertically = false;
                aRoomCanBeSplitHorizontally = false;
                foreach (var room in rooms)
                {
                    if (IsRoomPartitionableHorizontally(room, roomMinSize))
                    {
                        aRoomCanBeSplitHorizontally = true;
                        break;
                    }

                    if (IsRoomPartitionableVertically(room, roomMinSize))
                    {
                        aRoomCanBeSplitVertically = true;
                        break;
                    }
                }

                //OutputRectanglesToFile(rooms, "C:/Users/benro/Documents/rooms.txt");
            }
            while ( aRoomCanBeSplitHorizontally == true || 
                    aRoomCanBeSplitVertically == true);

            return rooms;
        }

        public static void OutputRectanglesToFile(List<Rectangle> rectangles, string filePath)
        {
            // Find the bounds of the entire grid by calculating the max width and height
            int maxWidth = 0;
            int maxHeight = 0;

            foreach (var rect in rectangles)
            {
                maxWidth = Math.Max(maxWidth, rect.Right);
                maxHeight = Math.Max(maxHeight, rect.Bottom);
            }

            // Create a 2D array (grid) large enough to contain all rectangles
            char[,] grid = new char[maxHeight, maxWidth];

            // Initialize the grid with '0' (empty space)
            for (int y = 0; y < maxHeight; y++)
            {
                for (int x = 0; x < maxWidth; x++)
                {
                    grid[y, x] = '0';
                }
            }

            // Assign a unique character for each rectangle
            char currentChar = '1'; // Start with '1'
            foreach (var rect in rectangles)
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        grid[y, x] = currentChar;
                    }
                }

                // Move to the next character (either '1' -> '9' or 'a' -> 'z')
                currentChar = NextChar(currentChar);
            }

            // Write the grid to a file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    for (int x = 0; x < maxWidth; x++)
                    {
                        writer.Write(grid[y, x]);
                    }
                    writer.WriteLine(); // Newline after each row
                }
            }

            Console.WriteLine("Rectangles have been written to the file: " + filePath);
        }

        // Function to get the next character (from '1' to '9', then 'a' to 'z')
        private static char NextChar(char currentChar)
        {
            if (currentChar == '9')
                return 'a';
            else if (currentChar == 'z')
                return '1';
            else
                return (char)(currentChar + 1);
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
