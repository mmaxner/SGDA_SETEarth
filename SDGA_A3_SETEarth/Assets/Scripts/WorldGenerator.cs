using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

public static class WorldGenerator  {
    public class TileAttributes
    {
        public float height;
        public bool dry;

        public TileAttributes(int Height = 0, bool Dry = false)
        {
            height = Height;
        }
    }

    public static TileAttributes[,] GenerateWorld(int width, int height, int seed, int flatness, float NCF)
    {
        TileAttributes[,] World = new TileAttributes[width,height];
        float[,] heightMap = Generate(width, height, seed, flatness, NCF);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                World[i, j] = new TileAttributes();
                World[i, j].height = heightMap[i, j];
            }
        }

        return World;
    }

    const int displacement = 4;
    const int perlin_octaves = 4;
    public static float[,] Generate(int width, int height, int seed, int flatness, float NCF)
    {
        // Create the module network
        ModuleBase moduleBase;

        moduleBase = new Voronoi(2, displacement, seed, false);
        Noise2D sound = new Noise2D(width, height, moduleBase);
        sound.GeneratePlanar(
                0,
                width,
                0,
                height, true);
        for (int i = 4; i <= 32; i *= 2)
        {
            seed++;
            moduleBase = new Voronoi(i, displacement, seed, false);
            LayerNoise(sound, moduleBase);
        }

        moduleBase = new RidgedMultifractal();
        LayerNoise(sound, moduleBase);
        moduleBase = new Perlin() { OctaveCount = perlin_octaves };
        LayerNoise(sound, moduleBase);

        Flatten(sound, flatness);

        return sound.GetNormalizedData();
    }

    private static void LayerNoise(Noise2D baseNoise, ModuleBase module)
    {
        Noise2D thisNoise = new Noise2D(baseNoise.Width, baseNoise.Height, module);
        thisNoise.GeneratePlanar(
            0,
            baseNoise.Width,
            0,
            baseNoise.Height, true);
        AddNoise(baseNoise, thisNoise);
    }

    private static void AddNoise(Noise2D baseNoise, Noise2D additionNoise)
    {
        for (int i = 0; i < baseNoise.Width; i++)
        {
            for (int j = 0; j < baseNoise.Height; j++)
            {
                baseNoise[i, j] += additionNoise[i, j];
            }
        }
    }

    private static void Flatten(Noise2D noise, int factor)
    {
        float[,] original = noise.GetNormalizedData();
        for (int i = 0; i < noise.Width; i++)
        {
            for (int j = 0; j < noise.Height; j++)
            {
                float total = 0;
                int elements = 0;
                for (int x = i + (factor * -1); x <= i + factor; x++)
                {
                    for (int y = j + (factor * -1); y <= j + factor; y++)
                    {
                        if (x >= 0 && x < noise.Width && y >= 0 && y < noise.Height && !(x == 0 && y == 0))
                        {
                            total += original[x, y];
                            elements++;
                        }
                    }
                }
                float average = original[i, j];
                if (elements > 0)
                {
                    average = total / (float)elements;
                }
                noise[i, j] = average;
            }
        }
    }
}
