using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDonut.Core;

namespace ProjectDonut.ProceduralGeneration.World.Generators
{
    public class RiverGenerator
    {
        private WorldMapSettings settings;
        private FastNoiseLite _noise;

        private float heightCutOff = -0.9f;

        public RiverGenerator(WorldMapSettings settings)
        {
            this.settings = settings;

            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _noise.SetSeed(new Random().Next(int.MinValue, int.MaxValue));
            _noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            _noise.SetFrequency(0.05f);
            _noise.SetCellularJitter(1.1f);
            _noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Sub);
            _noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.BasicGrid);
            _noise.SetDomainWarpAmp(27);
            _noise.SetFractalGain(0.5f);
            _noise.SetFractalType(FastNoiseLite.FractalType.DomainWarpIndependent);
            _noise.SetFractalOctaves(1);
            _noise.SetFractalLacunarity(2.0f);
        }

        // TODO: Use noise for this (OpenSimplex2 + Fractal Type = Ridged/Ping-Pong)
        public void GenerateRivers(WorldChunk chunk)
        {
            //for (int i = 0; i < chunk.Width; i++)
            //{
            //    for (int j = 0; j < chunk.Height; j++)
            //    {
            //        var biome = (Biome)chunk.BiomeData[i, j];
            //        if (biome != Biome.Wetlands)
            //        {
            //            continue;
            //        }

            //        var height = chunk.HeightData[i, j];
            //        if (height < settings.GroundHeightMin || height > settings.GroundHeightMax)
            //        {
            //            continue;
            //        }

            //        var x = chunk.ChunkCoordX * settings.Width + i;
            //        var y = chunk.ChunkCoordY * settings.Height + j;
            //        float sampleValue = _noise.GetNoise(x, y);

            //        if (sampleValue < heightCutOff)
            //        {
            //            chunk.HeightData[i, j] = settings.WaterHeightMin;
            //        }
            //    }
            //}
        }
    }
}
