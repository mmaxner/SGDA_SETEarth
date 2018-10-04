using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldGenerator  {

    private static int seed = 1992;
    
    public class TileAttributes
    {
        public float height;
        public bool dry;

        public TileAttributes(int Height = 0, bool Dry = false)
        {
            height = Height;
            dry = Dry;
        }
    }

    public static TileAttributes[,] GenerateWorld(int width, int height)
    {
        Random.InitState(seed);
        TileAttributes[,] World = new TileAttributes[width,height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                World[i, j] = new TileAttributes();
                switch (i)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        World[i, j].height = -2;
                        World[i, j].dry = false;
                        break;
                    case 4:
                    case 5:
                        World[i, j].height = -1;
                        World[i, j].dry = false;
                        break;
                    case 6:
                    case 12:
                    case 13:
                    case 14:
                        World[i, j].height = 1;
                        World[i, j].dry = true;
                        break;
                    default:
                        World[i, j].height = 2;
                        World[i, j].dry = true;
                        break;
                }
            }
        }

        return World;
    }

}
