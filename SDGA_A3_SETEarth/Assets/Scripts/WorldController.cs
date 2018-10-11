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
    private float[] thresh;
    private GameObject[] desh;
    private WorldGenerator.TileAttributes[,] baseWorld;

    private static int size_factor = 6;
    private int width = (int)System.Math.Pow(2, size_factor+1);
    private int height = (int)System.Math.Pow(2, size_factor);

    private int buffer = 0;
    private int halfbuffer = 0;

    private string dbPath;

    int flatness = 0;
    int maxflat = 6; 
    float nonContinentFacctor = 0.75f;
    float minNCF = 0.75f;

    struct genproc
    {
        public int seed;
        public int flatness;
        public int voronoi_iterations;
        public string name;
        internal int voronoi_start;
    }

    List<genproc> gens;

    // Use this for initialization
    void Start () {
        thresh = new float[] { 0.35f, 0.6f, 0.7f, 1.0f };
        desh = new GameObject[] { deep, shallow, sand, grass };
        tiles = new GameObject[width, height];

        gens = new List<genproc>();
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
        for (int s = 0; s < 1; s++)
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
        }
        /*dbPath = "URI=file:" + Application.persistentDataPath + "/exampleDatabase.db";
        Debug.Log(dbPath);
        CreateSchema();
        InsertScore("GG Meade", 3701);
        InsertScore("US Grant", 4242);
        InsertScore("GB McClellan", 107);
        GetHighScores(10);*/

    }

    private void LateUpdate()
    {
        if (gens.Count > 0)
        {
            DeleteOld();
            genproc thisgen = gens[gens.Count-1];
            Randomize(thisgen.seed, thisgen.flatness, thisgen.voronoi_iterations, thisgen.voronoi_start);
            ScreenCapture.CaptureScreenshot(thisgen.name);
            gens.RemoveAt(gens.Count-1);
        }
    }

    private void DeleteOld()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject.Destroy(tiles[i, j]);
            }
        }
    }

    struct TileObjet
    {
        public int x;
        public int y;
        public float height;
    }

    private void Randomize(int seed, int flatness, int voronoi_iterations, int voronoi_start)
    {
        baseWorld = WorldGenerator.GenerateWorld(width + buffer, height + buffer, seed, flatness, voronoi_iterations, voronoi_start);
        List<TileObjet> sorter = new List<TileObjet>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                sorter.Add(new TileObjet()
                {
                    x = i,
                    y = j,
                    height = baseWorld[i + halfbuffer, j + halfbuffer].height
                });
            }
        }
        sorter.Sort((x, y) => x.height.CompareTo(y.height));

        int tileIndex = 0;
        for (int i = 0; i < desh.Length; i++)
        {
            do
            {
                TileObjet thisTile = sorter[tileIndex];
                tiles[thisTile.x, thisTile.y] = GameObject.Instantiate(desh[i]);

                tiles[thisTile.x, thisTile.y].transform.SetParent(this.transform);
                tiles[thisTile.x, thisTile.y].transform.localPosition = new Vector3((thisTile.x - (width / 2)) * StaticData.size_increment, (thisTile.y - (height / 2)) * StaticData.size_increment, 0);
                
                float dir = (int)Mathf.Round(Random.Range(1.0f, 4.0f));
                //tiles[thisTile.x, thisTile.y].transform.Rotate(new Vector3(0, 0, dir * 90));

                tileIndex++;
            } while (tileIndex < sorter.Count && tileIndex < (thresh[i] * sorter.Count));
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
}
