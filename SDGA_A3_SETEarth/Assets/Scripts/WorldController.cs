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
    private WorldGenerator.TileAttributes[,] baseWorld;

    private int width = 128;
    private int height = 64;

    private int buffer = 32;
    private int halfbuffer = 16;

    private string dbPath;

    int seed = 8;
    int flatness = 0;
    int maxflat = 6; 
    float nonContinentFacctor = 0.75f;
    float minNCF = 0.75f;

    struct genproc
    {
        public int seed;
        public int flatness;
        public float NCF;
        public string name;
    }

    List<genproc> gens;

    // Use this for initialization
    void Start () {
        tiles = new GameObject[width, height];
        gens = new List<genproc>();
        for (int s = 0; s < 1; s++)
        {
            for (int f = 0; f <= 6; f++)
            {
                for (int n = 0; n < 1; n++)
                {
                    gens.Add(new genproc()
                    {
                        seed = s,
                        flatness = f,
                        NCF = 0.75f + 0.5f * n,
                        name = @"C:\Users\mmaxn\Desktop\tg\terrain-gen-" + seed.ToString() + f.ToString() + n.ToString() + ".png"
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
            genproc thisgen = gens[0];
            Randomize(thisgen.seed, thisgen.flatness, thisgen.NCF);
            ScreenCapture.CaptureScreenshot(thisgen.name);
            gens.RemoveAt(0);
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

    private void Randomize(int seed, int flatness, float NCF)
    {
        baseWorld = WorldGenerator.GenerateWorld(width + buffer, height + buffer, seed, flatness, NCF);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (baseWorld[i + halfbuffer, j + halfbuffer].height >= 0.6f)
                {
                    tiles[i, j] = (GameObject)Instantiate(deep);
                }
                else if (baseWorld[i + halfbuffer, j + halfbuffer].height >= 0.2f)
                {
                    tiles[i, j] = (GameObject)Instantiate(shallow);
                }
                else if (baseWorld[i + halfbuffer, j + halfbuffer].height >= 0.1f)
                {
                    tiles[i, j] = (GameObject)Instantiate(sand);
                }
                else
                {
                    tiles[i, j] = (GameObject)Instantiate(grass);
                }
                tiles[i, j].transform.SetParent(this.transform);
                tiles[i, j].transform.localPosition = new Vector3(0, 0, 0);
                tiles[i, j].transform.Translate(new Vector3((i - (width / 2)) * StaticData.size_increment, (j - (height / 2)) * StaticData.size_increment, 0));
                float dir = (int)Mathf.Round(Random.Range(1.0f, 4.0f));
                tiles[i, j].transform.Rotate(new Vector3(0, 0, dir * 90));
            }
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
