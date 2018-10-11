using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class WorldController : MonoBehaviour {
    public GameObject deep;
    public GameObject shallow;
    public GameObject sand;
    public GameObject grass;
    public GameObject[,] tiles;
    private float[] tileTypeAmountThreshold;
    private GameObject[] tileTypes;
    private float[,] baseWorld;

    public int seed;
    public int size_factor;
    public int flatness;
    public int voronoi_iterations;
    public int voronoi_start;

    public float water_weight;
    public float shallow_water_weight;
    public float coast_weight;
    public float land_weight;

    public bool large_preview = false;

    private int width;
    private int height;

    public CameraController camera;

    private string dbPath;

    struct genproc
    {
        public int seed;
        public int flatness;
        public int voronoi_iterations;
        public string name;
        public int voronoi_start;
    }

    List<genproc> gens;

    // Use this for initialization
    void Start () {
        tileTypes = new GameObject[] { deep, shallow, sand, grass };
        tileTypeAmountThreshold = new float[tileTypes.Length];

       // Randomize(seed, flatness, voronoi_iterations, voronoi_start);

        //gens = new List<genproc>();
        /*for (int s = 0; s < 32; s++)
        {
            gens.Add(new genproc()
            {
                seed = s,
                flatness = 3,
                voronoi_iterations = 9,
                name = @"C:\Users\mmaxn\Desktop\tg\terrain-gen-" + s.ToString() + "-PRIME.png"
            });
        }*/
        /*for (int s = 0; s < 1; s++)
        {
            for (int f = 0; f <= height / 3; f +=2 )
            {
                for (int x = 0; x < 16; x++)
                {
                    gens.Add(new genproc()
                    {
                        seed = s,
                        flatness = 3,
                        voronoi_iterations = x,
                        voronoi_start = f,
                        name = @"C:\Users\mmaxn\Desktop\tg\terrain-gen-" + s.ToString() + f.ToString() + x.ToString()+ ".png"
                    });
                }
            }
        }*/
        /*dbPath = "URI=file:" + Application.persistentDataPath + "/exampleDatabase.db";
        Debug.Log(dbPath);
        CreateSchema();
        InsertScore("GG Meade", 3701);
        InsertScore("US Grant", 4242);
        InsertScore("GB McClellan", 107);
        GetHighScores(10);*/

    }

   /*private void LateUpdate()
    {
        if (gens.Count > 0)
        {
            DeleteOld();
            genproc thisgen = gens[gens.Count-1];
            Randomize(thisgen.seed, thisgen.flatness, thisgen.voronoi_iterations, thisgen.voronoi_start);
            ScreenCapture.CaptureScreenshot(thisgen.name);
            gens.RemoveAt(gens.Count-1);
        }
    }*/

    private void DeleteOld()
    {
        try
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GameObject.Destroy(tiles[i, j]);
                }
            }
        }
        catch
        {

        }
    }

    struct TileObject
    {
        public int x;
        public int y;
        public float height;
    }

    public void Randomize()
    {
        DeleteOld();
        width = (int)System.Math.Pow(2, size_factor + 1);
        height = (int)System.Math.Pow(2, size_factor);

        camera.SizeTo(width, height);

        float weight_total = water_weight + shallow_water_weight + coast_weight + land_weight;
        tileTypeAmountThreshold[0] = water_weight / weight_total;
        tileTypeAmountThreshold[1] = tileTypeAmountThreshold[0] + shallow_water_weight / weight_total;
        tileTypeAmountThreshold[2] = tileTypeAmountThreshold[1] + coast_weight / weight_total;
        tileTypeAmountThreshold[3] = 1.0f;// (float)land_weight / (float)weight_total;

        Debug.Log("total: " + weight_total + ", water: " + tileTypeAmountThreshold[0] + ", shallow: " + tileTypeAmountThreshold[1] + ", coastal: " + tileTypeAmountThreshold[2] + ", land: " + tileTypeAmountThreshold[3]);

        tiles = new GameObject[width, height];

        Debug.Log("width: " + width + ", height: " + height + ", seed: " + seed + ", flatness: " + flatness + ", voronoi iterations: " + voronoi_iterations + ", voronoi start: " + voronoi_start);

        baseWorld = HeightMapGenerator.GenerateWorld(width, height, seed, flatness, voronoi_iterations, voronoi_start);
        List<TileObject> sorter = new List<TileObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                sorter.Add(new TileObject()
                {
                    x = i,
                    y = j,
                    height = baseWorld[i, j]
                });
            }
        }
        sorter.Sort((x, y) => x.height.CompareTo(y.height));

        int tileIndex = 0;
        for (int i = 0; i < tileTypes.Length && tileIndex < sorter.Count; i++)
        {
            do
            {
                TileObject thisTile = sorter[tileIndex];
                tiles[thisTile.x, thisTile.y] = GameObject.Instantiate(tileTypes[i]);

                tiles[thisTile.x, thisTile.y].transform.SetParent(this.transform);
                tiles[thisTile.x, thisTile.y].transform.localPosition = new Vector3((thisTile.x - (width / 2)) * StaticData.size_increment, (thisTile.y - (height / 2)) * StaticData.size_increment, 0);
                
                float dir = (int)Mathf.Round(Random.Range(1.0f, 4.0f));
                //tiles[thisTile.x, thisTile.y].transform.Rotate(new Vector3(0, 0, dir * 90));

                tileIndex++;
            } while (tileIndex < sorter.Count && tileIndex < (tileTypeAmountThreshold[i] * sorter.Count));
        }
    }

    public void CreateSchema()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS 'high_score' ( " +
                                    "  'id' INTEGER PRIMARY KEY, " +
                                    "  'name' TEXT NOT NULL, " +
                                    "  'score' INTEGER NOT NULL" +
                                    ");";

                var result = cmd.ExecuteNonQuery();
                Debug.Log("create schema: " + result);
            }
        }
    }

    public void InsertScore(string highScoreName, int score)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO high_score (name, score) " +
                                    "VALUES (@Name, @Score);";

                cmd.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "Name",
                    Value = highScoreName
                });

                cmd.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "Score",
                    Value = score
                });

                var result = cmd.ExecuteNonQuery();
                Debug.Log("insert score: " + result);
            }
        }
    }

    public void GetHighScores(int limit)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM high_score ORDER BY score DESC LIMIT @Count;";

                cmd.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "Count",
                    Value = limit
                });

                Debug.Log("scores (begin)");
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var highScoreName = reader.GetString(1);
                    var score = reader.GetInt32(2);
                    var text = string.Format("{0}: {1} [#{2}]", highScoreName, score, id);
                    Debug.Log(text);
                }
                Debug.Log("scores (end)");
            }
        }
    }

    public void SetSize(float value)
    {
        size_factor = (int)value;
    }

    public void SetFlatness(float value)
    {
        flatness = (int)value;
    }

    public void SetSeed(float value)
    {
        seed = (int)value;
    }

    public void SetIterations(float value)
    {
        voronoi_iterations = (int)value;
    }

    // watch out for this, maybe find a better UI label and make it exactly what the value is ??
    public void SetStart(float value)
    {
        voronoi_start = 10 - (int)value;
    }

    public void SetWaterWeight(float value)
    {
        water_weight = value;
    }

    public void SetShallowWeight(float value)
    {
        shallow_water_weight = value;
    }

    public void SetCoastWeight(float value)
    {
        coast_weight = value;
    }

    public void SetLandWeight(float value)
    {
        land_weight = value;
    }

    public void SetLargePreview(bool isLarge)
    {
        large_preview = isLarge;
    }

    public void PreviewMapSettings(float lol_jk = 0.0f)
    {
        int actual_size = size_factor;
        size_factor = large_preview ? 5 : 4;
        Randomize();
        size_factor = actual_size;
    }
}
