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

    private string dbPath;

    // Use this for initialization
    void Start () {


        baseWorld = WorldGenerator.GenerateWorld(30, 20);
        tiles = new GameObject[StaticData.world_size, StaticData.world_size];
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 20; j++)
            {
               if (baseWorld[i,j].height == -2)
                {
                    tiles[i, j] = (GameObject)Instantiate(deep); 
                }
               else if (baseWorld[i,j].height == -1)
                {
                    tiles[i, j] = (GameObject)Instantiate(shallow);
                }
               else if (baseWorld[i,j].height == 1)
                {
                    tiles[i, j] = (GameObject)Instantiate(sand);
                }
               else
                {
                    tiles[i, j] = (GameObject)Instantiate(grass);
                }
                tiles[i, j].transform.Translate(new Vector3 ((i - (StaticData.world_size/2)) * StaticData.size_increment, (j - (StaticData.world_size / 2)) * StaticData.size_increment, 0));
            }
        }
        dbPath = "URI=file:" + Application.persistentDataPath + "/exampleDatabase.db";
        Debug.Log(dbPath);
        CreateSchema();
        InsertScore("GG Meade", 3701);
        InsertScore("US Grant", 4242);
        InsertScore("GB McClellan", 107);
        GetHighScores(10);

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
