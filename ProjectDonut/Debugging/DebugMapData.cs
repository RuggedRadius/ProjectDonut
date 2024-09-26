using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using ProjectDonut.Core;
using ProjectDonut.ProceduralGeneration.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Debugging
{
    public class DebugMapData
    {
        private WorldMapSettings settings;

        public DebugMapData(WorldMapSettings settings)
        {
            this.settings = settings;
        }

        public static void WriteMapData(int[,] data, string identifier)
        {
            //var filePath = $@"C:\Users\benro\Documents\{identifier}_{DateTime.Now.ToString("hh-mm-ss.ffftt")}.txt";
            var filePath = $@"C:\Users\benro\Documents\DEBUG\{identifier}.txt";
            var lines = new List<string>();

            var width = data.GetLength(0);
            var height = data.GetLength(1);

            for (int j = 0; j < height; j++)
            {
                var line = string.Empty;
                for (int i = width - 1; i >= 0; i--)
                {
                    if (data[i, j] == 0)
                    {
                        line += "░░░";
                    }
                    else if (data[i, j] == 2)
                    {
                        line += "###";
                    }
                    else
                    {
                        line += "▓▓▓";
                    }
                }

                lines.Add(ReverseLine(line));
            }

            //lines.Reverse(); // Add this line to horizontally flip the output

            System.IO.File.WriteAllLines(filePath, lines);
        }

        private static string ReverseLine(string line)
        {
            var result = string.Empty;

            for (int i = line.Length - 1; i >= 0; i--)
            {
                result += line[i];
            }

            return result;
        }

        private char GetCellCharacter(int value)
        {
            if (value == -1)
            {
                return '#';
            }
            if (value >= settings.MountainHeightMin)
            {
                return 'M';
            }
            if (value >= settings.GroundHeightMin)
            {
                return 'G';
            }
            else
            {
                return 'W';
            }
        }

        public static void SaveMapThumbnail(int chunkX, int chunkY, Texture2D thumbnail)
        {
            var filePath = $@"C:\Users\benro\Documents\DEBUG\MapThumbnails\[{chunkX}][{chunkY}].png";
            using (var stream = System.IO.File.Create(filePath))
            {
                thumbnail.SaveAsPng(stream, thumbnail.Width, thumbnail.Height);
            }
        }

        //private void DrawMapToFile(int[,] data, Tilemap tilemap)
        //{
        //    var width = settings.Width * settings.TileSize;
        //    var height = settings.Height * settings.TileSize;

        //    var colData = new Color[width * height];
        //    var targetRect = new Rectangle(0, 0, settings.TileSize, settings.TileSize);

        //    for (int x = 0; x < settings.Width; x++)
        //    {
        //        for (int y = 0; y < settings.Height; y++)
        //        {
        //            var tile = tilemap.GetTile(x, y);
        //            var heightValue = data[x, y];

        //            targetRect.X = x * settings.TileSize;
        //            targetRect.Y = y * settings.TileSize;

        //            var tileData = GetTileColourData(tile.Texture);

        //            for (int i = 0; i < settings.TileSize; i++)
        //            {
        //                for (int j = 0; j < settings.TileSize; j++)
        //                {
        //                    colData[(targetRect.X + i) + (targetRect.Y + j) * width] = tileData[i * j];
        //                }
        //            }
        //        }
        //    }
        //}

        //private Color[] GetTileColourData(Texture2D tile)
        //{
        //    var result = new Color[settings.TileSize, settings.TileSize];
        //    var rect = new Rectangle(0, 0, settings.TileSize, settings.TileSize);

        //    tile.GetData(0, rect, result, 0, settings.TileSize * settings.TileSize);

        //    return result;
        //}
    }
}
