using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData {

    public const float size_increment = 0.64f;
    public const int world_size = 30;

    public static int seed;
    public static int flatness;
    public static int voronoi_iterations;
    public static int voronoi_start;

    public static string dbPath = "URI=file:" + Application.persistentDataPath + "/exampleDatabase.db";
}
