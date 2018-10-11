using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System.IO;
using System;

public static class HeightMapGenerator  {
    public static float[,] GenerateWorld(int width, int height, int seed, int flatness, int voronoi_iterations, int voronoi_start)
    {
        float[,] heightMap = Generate(width, height, seed, flatness, voronoi_iterations, voronoi_start);

        return heightMap;
    }

    const int displacement = 4;
    const int perlinOctaves = 4;
    public static float[,] Generate(int width, int height, int seed, int flatness, int voronoi_iterations, int voronoi_start)
    {
        // Create the module network
        ModuleBase moduleBase;

        moduleBase = new RidgedMultifractal();
        Noise2D sound = new Noise2D(width, height, moduleBase);
        sound.GeneratePlanar(
                -1,
                1,
                -1,
                1, true);
        
        for (int i = 0; i <= voronoi_iterations; i++)
        {
            seed++;
            ModuleBase tempBase = new Voronoi(voronoi_start + i, displacement, seed, false);
            Noise2D temp = new Noise2D(width, height, tempBase);
            temp.GeneratePlanar(
                -1,
                1,
                -1,
                1, true);
            LayerNoise(sound, tempBase);
            ModuleBase pBase = new Perlin() { OctaveCount = perlinOctaves };
            LayerNoise(sound, pBase);
        }
        Flatten(sound, flatness);

        return sound.GetData();
    }

    private static void LayerNoise(Noise2D baseNoise, ModuleBase module)
    {
        Noise2D thisNoise = new Noise2D(baseNoise.Width, baseNoise.Height, module);
        thisNoise.GeneratePlanar(
            -1,
            1,
            -1,
            1, true);
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
                        if (x >= 0 && x < noise.Width && y >= 0 && y < noise.Height && (x != 0 && y != 0))
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
