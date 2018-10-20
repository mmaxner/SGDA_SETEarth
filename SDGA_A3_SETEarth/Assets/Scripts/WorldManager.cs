using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public TerrainTile[,] world;
    public List<Herbivore> herbies = new List<Herbivore>();
    public List<Carnivore> carnies = new List<Carnivore>();

    public GameObject HerbSprite;
    public GameObject CarnSprite;

    public GameObject OceanSprite;
    public GameObject CoastSprite;
    public GameObject ShallowSprite;
    public GameObject LandSprite;

    public MaxValueReader max_plants;
    public MaxValueReader max_herbivores;
    public MaxValueReader max_carnivores;

    public CurrentValueReader current_plants;
    public CurrentValueReader current_herbivores;
    public CurrentValueReader current_carnivores;

    public CurrentValueReader current_minimum_plants;
    Vector2 sprite_offset;

    public int size_factor;

    public void SetWorld(TerrainTile[,] terrainTiles, List<Vector2> initial_herbivores, List<Vector2> initial_carnivores, int size_factor_in)
    {
        size_factor = size_factor_in;
        world = terrainTiles;
        sprite_offset = new Vector2(world.GetLength(0) / 2, world.GetLength(1) / 2);
        for (int i = 0; i < initial_herbivores.Count; i++)
        {
            herbies.Add(Herbivore.CreateBasicHerb(initial_herbivores[i], sprite_offset, HerbSprite, this.transform));
        }

        for (int i = 0; i < initial_carnivores.Count; i++)
        {
            carnies.Add(Carnivore.CreateBasicCarnivore(initial_carnivores[i], sprite_offset, CarnSprite, this.transform));
        }
    }

    public void LoadWorld(TerrainTile[,] tiles, List<Herbivore> herbivores, List<Carnivore> carnivores)
    {
        world = tiles;
        sprite_offset = new Vector2(world.GetLength(0) / 2, world.GetLength(1) / 2);
        herbies = herbivores;
        carnies = carnivores;

        for (int x = 0; x < world.GetLength(0); x++)
        {
            for (int y = 0; y < world.GetLength(1); y++)
            {
                GameObject sprite = null;
                switch (world[x,y].type)
                {
                    case TerrainTile.TerrainType.MAINLAND:
                        sprite = LandSprite;
                        break;
                    case TerrainTile.TerrainType.COAST:
                        sprite = CoastSprite;
                        break;
                    case TerrainTile.TerrainType.SHALLOWOCEAN:
                        sprite = ShallowSprite;
                        break;
                    case TerrainTile.TerrainType.OCEAN:
                        sprite = OceanSprite;
                        break;
                    default:
                        sprite = LandSprite;
                        break;
                }
                GameObject tile = GameObject.Instantiate(sprite);
                tile.transform.parent = this.transform;
                tile.transform.localPosition = new Vector3((x - sprite_offset.x) * StaticData.size_increment, (y - sprite_offset.y) * StaticData.size_increment, 1);

                float dir = (int)Mathf.Round(Random.Range(1.0f, 4.0f));
                tile.transform.Rotate(new Vector3(0, 0, dir * 90));
            }
        }

        for (int i = 0; i < herbies.Count; i++)
        {
            herbies[i].isAlive = true;
            herbies[i].sprite = GameObject.Instantiate(HerbSprite);
            herbies[i].sprite.transform.parent = this.transform;
            herbies[i].MoveTo(herbies[i].location, new Vector2((herbies[i].location.x - sprite_offset.x) * StaticData.size_increment, (herbies[i].location.y - sprite_offset.y) * StaticData.size_increment));
        }

        for (int i = 0; i < carnies.Count; i++)
        {
            carnies[i].isAlive = true;
            carnies[i].sprite = GameObject.Instantiate(CarnSprite);
            carnies[i].sprite.transform.parent = this.transform;
            carnies[i].MoveTo(carnies[i].location, new Vector2((carnies[i].location.x - sprite_offset.x) * StaticData.size_increment, (carnies[i].location.y - sprite_offset.y) * StaticData.size_increment));
        }
    }

    private int round_speed = 1;
    private int round_count = 0;
    private int counter = 0;
    private int seed_round = 5;
    private bool is_playing = false;

    public void PlayRounds()
    {
        is_playing = true;
    }

    public void PauseRounds()
    {
        is_playing = false;
    }

    private void FixedUpdate()
    {
        if (is_playing)
        {
            if (counter == 0)
            {
                counter = round_speed;
                RunRound();
            }
            else
            {
                counter--;
            }
        }
    }

    public void RunRound()
    {
        round_count++;

        Debug.Log(herbies.Count);

        float current_total_nutrition = 0;
        float current_minimium_nutrition = 100000;
        int i_max = world.GetLength(0);
        int j_max = world.GetLength(1);
        for (int i = 0; i < i_max; i++)
        {
            for (int j = 0; j < j_max; j++)
            {
                world[i, j].Reset();
                if (round_count % seed_round == seed_round - 1)
                {
                    SpreadPlants(new Vector2(i, j));
                }
                world[i, j].GrowPlants();
                current_total_nutrition += world[i, j].nutrition;
                if (world[i, j].nutrition > 0 && world[i, j].nutrition < current_minimium_nutrition)
                {
                    current_minimium_nutrition = world[i, j].nutrition;
                }
            }
        }

        for (int i = 0; i < herbies.Count; i++)
        {
            if (herbies[i].isAlive)
            {
                Vector2 best_spot = herbies[i].Search(SliceOfWorld(herbies[i].GetPerceptionRange()));
                Vector2 new_sprite_location = new Vector2((best_spot.x - world.GetLength(0) / 2) * StaticData.size_increment, (best_spot.y - world.GetLength(1) / 2) * StaticData.size_increment);
                herbies[i].MoveTo(best_spot, new_sprite_location);
            }
            else
            {
                GameObject.Destroy(herbies[i].sprite);
                herbies.RemoveAt(i);
                i--;
            }
        }

        List<TerrainTile> occupiedTiles = new List<TerrainTile>();
        for(int i = 0; i < herbies.Count; i++)
        {
            Herbivore herb = herbies[i];
            if (!occupiedTiles.Contains(world[(int)herb.location.x, (int)herb.location.y]))
            {
                occupiedTiles.Add(world[(int)herb.location.x, (int)herb.location.y]);
            }
            herb.Feed(world[(int)herb.location.x, (int)herb.location.y].EatPlants(herb.GetAppetite()));
            world[(int)herb.location.x, (int)herb.location.y].grazers.Add(herbies[i]);
        }

        for (int i = 0; i < carnies.Count; i++)
        {
            if (carnies[i].isAlive)
            {
                Vector2 best_spot = carnies[i].Search(SliceOfWorld(carnies[i].GetPerceptionRange()));
                Vector2 new_sprite_location = new Vector2((best_spot.x - world.GetLength(0) / 2) * StaticData.size_increment, (best_spot.y - world.GetLength(1) / 2) * StaticData.size_increment);
                carnies[i].MoveTo(best_spot, new_sprite_location);
            }
            else
            {
                GameObject.Destroy(carnies[i].sprite);
                carnies.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < carnies.Count; i++)
        {
            Carnivore carn = carnies[i];
            if (!occupiedTiles.Contains(world[(int)carn.location.x, (int)carn.location.y]))
            {
                occupiedTiles.Add(world[(int)carn.location.x, (int)carn.location.y]);
            }
            float meal_size = 0;
            if (carn.SucessfulHunt())
            {
                meal_size = world[(int)carn.location.x, (int)carn.location.y].EatAnimal();
            }
            carn.Eat(meal_size);
        }

        for (int i = 0; i < herbies.Count; i++)
        {
            int babies = herbies[i].Reproduce();
            for (int b = 0; b < babies; b++)
            {
                herbies.Add(herbies[i].CreateBaby(transform, sprite_offset));
            }
        }

        for (int i = 0; i < carnies.Count; i++)
        {
            int babies = carnies[i].Reproduce();
            for (int b = 0; b < babies; b++)
            {
                carnies.Add(carnies[i].CreateBaby(transform, sprite_offset));
            }
        }

        max_carnivores.FeedValue(carnies.Count);
        current_carnivores.FeedValue(carnies.Count);
        max_plants.FeedValue(current_total_nutrition);
        current_plants.FeedValue(current_total_nutrition);
        max_herbivores.FeedValue(herbies.Count);
        current_herbivores.FeedValue(herbies.Count);
    }

    List<TerrainTile> SliceOfWorld(List<Vector2> slices_required)
    {
        List<TerrainTile> return_thing = new List<TerrainTile>();
        for (int i = 0; i < slices_required.Count; i++)
        {
            if ((int)slices_required[i].x >= 0 && (int)slices_required[i].x < world.GetLength(0) && (int)slices_required[i].y >= 0 && (int)slices_required[i].y < world.GetLength(1))
            {
                return_thing.Add(world[(int)slices_required[i].x, (int)slices_required[i].y]);
            }
        }
        return return_thing;
    }

    void SpreadPlants(Vector2 at)
    {
        if (world[(int)at.x,(int)at.y].nutrition > 0)
        {
            float seed_nutrition = world[(int)at.x, (int)at.y].nutrition * 0.1f;
            if (at.x - 1 >= 0)
            {
                TerrainTile spredable = world[(int)at.x - 1, (int)at.y];
                if ((spredable.type & TerrainTile.TerrainType.LAND) > 0)
                {
                    spredable.nutrition += seed_nutrition;
                    world[(int)at.x, (int)at.y].nutrition -= seed_nutrition;
                }
            }
            if (at.x + 1 < world.GetLength(0))
            {
                TerrainTile spredable = world[(int)at.x + 1, (int)at.y];
                if ((spredable.type & TerrainTile.TerrainType.LAND) > 0)
                {
                    spredable.nutrition += seed_nutrition;
                    world[(int)at.x, (int)at.y].nutrition -= seed_nutrition;
                }
            }
            if (at.y - 1 >= 0)
            {
                TerrainTile spredable = world[(int)at.x, (int)at.y - 1];
                if ((spredable.type & TerrainTile.TerrainType.LAND) > 0)
                {
                    spredable.nutrition += seed_nutrition;
                    world[(int)at.x, (int)at.y].nutrition -= seed_nutrition;
                }
            }
            if (at.y + 1 < world.GetLength(1))
            {
                TerrainTile spredable = world[(int)at.x, (int)at.y + 1];
                if ((spredable.type & TerrainTile.TerrainType.LAND) > 0)
                {
                    spredable.nutrition += seed_nutrition;
                    world[(int)at.x, (int)at.y].nutrition -= seed_nutrition;
                }
            }
        }
    }
}
