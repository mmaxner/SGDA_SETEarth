using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseInterfacer {

	public void SaveWorld(string name, int size_factor, TerrainTile[,] tiles, List<Herbivore> herbivores, List<Carnivore> carnivores)
    {
        CreateSchema();
        int game_id = 0;
        using (var conn = new SqliteConnection(StaticData.dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;

                cmd.CommandText =
                    "INSERT INTO saved_games (name, size_factor) " +
                    "VALUES ('" + name + "', " + size_factor.ToString() + ");";
                var result = cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT id FROM saved_games WHERE name=@Name;";
                cmd.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "Name",
                    Value = name
                });

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    game_id = reader.GetInt32(0);
                }
            }
        }
        SaveTiles(tiles, game_id);
        SaveHerbivores(herbivores, game_id);
        SaveCarnivores(carnivores, game_id);
    }

    public void LoadWorld()
    {
        TerrainTile[,] worldTiles;
        List<Herbivore> herbivores;
        List<Carnivore> carnivores;
    }

    private string GameSchema =
        "CREATE TABLE IF NOT EXISTS 'saved_games' ( " +
        "  'id' INTEGER PRIMARY KEY, " +
        "  'name' TEXT NOT NULL, " +
        "  'size_factor' INTEGER NOT NULL" +
        ");";

    private string TileSchema =
        "CREATE TABLE IF NOT EXISTS 'terrain_tiles' ( " +
        "  'id' INTEGER PRIMARY KEY, " +
        "  'game_id' INTEGER, " +
        "  'x' INTEGER, " + 
        "  'y' INTEGER, " +
        "  'type' INTEGER, " +
        "  'height' REAL, " +
        "  'temperature' REAL, " +
        "  'nutrition' REAL, " + 
        "  'growth_rate' REAL, " +
        "  'regrowth_left' INTEGER" +
        ");";

    private string HerbivoreSchema =
        "CREATE TABLE IF NOT EXISTS 'herbivores' ( " +
        "  'game_id' INTEGER, " +
        "  'x' INTEGER, " +
        "  'y' INTEGER, " +
        "  'movement' REAL, " +
        "  'perception' REAL, " +
        "  'population' INTEGER, " +
        "  'starvation_threshold' REAL, " +
        "  'fertility' REAL, " +
        "  'food_efficiency' REAL, " +
        "  'size' REAL, " +
        "  'max_growth_factor' REAL " +
        ");";

    private string CarnivoreSchema =
        "CREATE TABLE IF NOT EXISTS 'carnivores' ( " +
        "  'game_id' INTEGER, " +
        "  'x' INTEGER, " +
        "  'y' INTEGER, " +
        "  'movement' REAL, " +
        "  'perception' REAL, " +
        "  'population' INTEGER, " +
        "  'starvation_threshold' REAL, " +
        "  'fertility' REAL, " +
        "  'last_meal_size' REAL " +
        ");";

    public void CreateSchema()
    {
        using (var conn = new SqliteConnection(StaticData.dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = GameSchema;
                var result = cmd.ExecuteNonQuery();
                cmd.CommandText = TileSchema;
                result = cmd.ExecuteNonQuery();
                cmd.CommandText = HerbivoreSchema;
                result = cmd.ExecuteNonQuery();
                cmd.CommandText = CarnivoreSchema;
                result = cmd.ExecuteNonQuery();
            }
        }
    }

    public List<string> ListSavedGames()
    {
        CreateSchema();
        List<string> games = new List<string>();
        using (var conn = new SqliteConnection(StaticData.dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT name FROM saved_games ORDER BY name ASC;";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    games.Add(reader.GetString(0));
                }
            }
        }
        return games;
    }

    public void SaveTiles(TerrainTile[,] tiles, int game_id)
    {
        using (var conn = new SqliteConnection(StaticData.dbPath))
        {
            conn.Open();
            // clear old tiles from a previous save of this game_id
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM terrain_tiles WHERE game_id=" + game_id.ToString();
                var result = cmd.ExecuteNonQuery();
            }
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText =
                            "INSERT INTO terrain_tiles (game_id, x, y, type, height, temperature, nutrition, growth_rate, regrowth_left) " +
                            "VALUES (" + game_id + ", " + x + "," + y + ", " + ((int)tiles[x, y].type).ToString() + ", " + tiles[x, y].height.ToString() + ", " + tiles[x, y].temperature.ToString() + ", " + tiles[x, y].nutrition.ToString() + ", " + tiles[x, y].growth_rate.ToString() + ", " + tiles[x, y].regrowth_left.ToString() + ");";
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    public void SaveHerbivores(List<Herbivore> herbivores, int game_id)
    {
        using (var conn = new SqliteConnection(StaticData.dbPath))
        {
            conn.Open();
            // clear old tiles from a previous save of this game_id
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM herbivores WHERE game_id=" + game_id.ToString();
                var result = cmd.ExecuteNonQuery();
            }
            for (int x = 0; x < herbivores.Count; x++)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "INSERT INTO herbivores (game_id, x, y, movement, perception, population, starvation_threshold, fertility, food_efficiency, size, max_growth_factor) " +
                        "VALUES (" + game_id + ", " + herbivores[x].location.x + "," + herbivores[x].location.y + ", " + herbivores[x].movement.ToString() + ", " + herbivores[x].perception.ToString() + ", " + herbivores[x].population.ToString() + ", " + herbivores[x].starvation_threshold.ToString() + ", " + herbivores[x].fertility.ToString() + ", " + herbivores[x].food_efficiency.ToString() + ", " + herbivores[x].size.ToString() + ", " + herbivores[x].max_growth_factor.ToString() + ");";
                    var result = cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public void SaveCarnivores(List<Carnivore> carnivores, int game_id)
    {
        using (var conn = new SqliteConnection(StaticData.dbPath))
        {
            conn.Open();
            // clear old tiles from a previous save of this game_id
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM carnivores WHERE game_id=" + game_id.ToString();
                var result = cmd.ExecuteNonQuery();
            }
            for (int x = 0; x < carnivores.Count; x++)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "INSERT INTO carnivores (game_id, x, y, movement, perception, population, starvation_threshold, fertility, last_meal_size) " +
                        "VALUES (" + game_id + ", " + carnivores[x].location.x + "," + carnivores[x].location.y + ", " + carnivores[x].movement.ToString() + ", " + carnivores[x].perception.ToString() + ", " + carnivores[x].population.ToString() + ", " + carnivores[x].starvation_threshold.ToString() + ", " + carnivores[x].fertility.ToString() + ", " + carnivores[x].last_meal_size.ToString() + ");";
                    var result = cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public void InsertScore(string highScoreName, int score)
    {
        using (var conn = new SqliteConnection(StaticData.dbPath))
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
        using (var conn = new SqliteConnection(StaticData.dbPath))
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

    public void LoadGame(string name, WorldManager manager)
    {
        CreateSchema();
        TerrainTile[,] world;
        List<Herbivore> herbivores = new List<Herbivore>();
        List<Carnivore> carnivores = new List<Carnivore>();
        int size = 0;
        int game_id = 0;

        using (var conn = new SqliteConnection(StaticData.dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT id, size_factor FROM saved_games WHERE name=@Name";
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "Name",
                    Value = name
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        game_id = reader.GetInt32(0);
                        size = reader.GetInt32(1);
                    }
                }
                int width = (int)System.Math.Pow(2, size + 1);
                int height = (int)System.Math.Pow(2, size);
                world = new TerrainTile[width, height];

                cmd.CommandText = "SELECT x, y, type, height, temperature, nutrition, growth_rate, regrowth_left FROM terrain_tiles WHERE game_id=@Game";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "Game",
                    Value = game_id
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int x = reader.GetInt32(0);
                        int y = reader.GetInt32(1);
                        TerrainTile.TerrainType type = (TerrainTile.TerrainType)reader.GetInt32(2);
                        float h = reader.GetFloat(3);
                        float t = reader.GetFloat(4);
                        float n = reader.GetFloat(5);
                        float g = reader.GetFloat(6);
                        int r = reader.GetInt32(7);
                        world[x, y] = new TerrainTile()
                        {
                            height = h,
                            temperature = t,
                            nutrition = n,
                            growth_rate = g,
                            regrowth_left = r,
                            type = type,
                            grazers = new List<Herbivore>(),
                            predators = new List<Carnivore>(),
                            location = new Vector2(x, y)
                        };

                    }
                }
                cmd.CommandText = "SELECT x, y, movement, perception, population, starvation_threshold, fertility, food_efficiency, size, max_growth_factor FROM herbivores WHERE game_id=@Game";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "Game",
                    Value = game_id
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int x = reader.GetInt32(0);
                        int y = reader.GetInt32(1);
                        float m = reader.GetFloat(2);
                        float per = reader.GetFloat(3);
                        int pop = reader.GetInt32(4);
                        float s = reader.GetFloat(5);
                        float f = reader.GetFloat(6);
                        float fe = reader.GetFloat(7);
                        float siz = reader.GetFloat(8);
                        float mgf = reader.GetFloat(9);

                        herbivores.Add(new Herbivore()
                        {
                            location = new Vector2(x, y),
                            movement = m,
                            perception = per,
                            population = pop,
                            starvation_threshold = s,
                            fertility = f,
                            food_efficiency = fe,
                            size = siz,
                            max_growth_factor = mgf
                        });
                    }
                }
                cmd.CommandText = "SELECT x, y, movement, perception, population, starvation_threshold, fertility, last_meal_size FROM carnivores WHERE game_id=@Game";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter()
                {
                    ParameterName = "Game",
                    Value = game_id
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int x = reader.GetInt32(0);
                        int y = reader.GetInt32(1);
                        float m = reader.GetFloat(2);
                        float per = reader.GetFloat(3);
                        int pop = reader.GetInt32(4);
                        float s = reader.GetFloat(5);
                        float f = reader.GetFloat(6);
                        float lms = reader.GetFloat(7);

                        carnivores.Add(new Carnivore()
                        {
                            location = new Vector2(x, y),
                            movement = m,
                            perception = per,
                            population = pop,
                            starvation_threshold = s,
                            fertility = f,
                            last_meal_size = lms

                        });
                    }
                }
            }
        }

        manager.LoadWorld(world, herbivores, carnivores);
    }
    /*
     * Game
     *  game id
     *  each stat
     *  =================== 
     * world tiles
     *  game id
     *  tile id
     *  x int
     *  y int
     *  type int
     *  height float
     *  temp float
     *  nutrition float
     *  growth_rate float
     *  regrowth_left int
     *  
     *  ===================
     * Herbivores
     *  game id
     *  herb id
     *  x int
     *  y int
     *  movement float
     *  perception float
     *  population int
     *  starvation_threshold float
     *  fertility float
     *  food_efficiency float
     *  size float
     *  max_growth_factor float
     *      
     *  ===================
     * Carnivores
     *  game id
     *  herb id
     *  x int
     *  y int
     *  movement float
     *  perception float
     *  population int
     *  starvation_threshold float
     *  fertility float
     *  last_meal_size float
     *  
     */
}
