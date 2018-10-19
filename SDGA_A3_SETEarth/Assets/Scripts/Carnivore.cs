using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carnivore : Animal {
    public float last_meal_size;
    public static float hunt_chance = 0.9f;
    public static float meal_bonus_increment = 5;
    public static float meal_bonus = 0.9f;
    public float size;
    //public float 

    public static Carnivore CreateBasicCarnivore(Vector2 where, Vector2 tile_offset, GameObject apperance, Transform parent)
    {
        Carnivore carny = new Carnivore()
        {
            movement = 3.0f,
            perception = 3.0f,
            fertility = 0.05f,
            starvation_threshold = 1,
            location = where,
            sprite = GameObject.Instantiate(apperance),
            last_meal_size = 0,
            isAlive = true,
            size = 1,
            population = 2
        };

        carny.sprite.transform.SetParent(parent);
        carny.sprite.transform.localPosition = new Vector3((where.x - tile_offset.x) * StaticData.size_increment, (where.y - tile_offset.y) * StaticData.size_increment);
        return carny;
    }

    public float CalculateFertility()
    {
        float calculated_fertility = fertility;
        while (meal_bonus_increment < last_meal_size)
        {
            last_meal_size -= meal_bonus_increment;
            calculated_fertility += fertility * meal_bonus;
        }
        return calculated_fertility;
    }

    public bool SucessfulHunt()
    {
        return Random.value < hunt_chance;
    }

    public void Eat(float meat)
    {
        if (meat >= starvation_threshold)
        {
            last_meal_size = meat;
        }
        else
        {
            isAlive = false;
        }
    }

    public Vector2 Search(List<TerrainTile> area)
    {
        TerrainTile best_place = null;
        float total_grazers = 0;
        List<MovementCandidate> candidates = new List<MovementCandidate>();
        for (int i = 0; i < area.Count; i++)
        {
            TerrainTile thisTile = area[i];
            if (thisTile.grazers.Count > 0)
            {
                candidates.Add(new MovementCandidate()
                {
                    candidate = area[i],
                    weight = thisTile.grazers.Count,
                    location = thisTile.location
                });
                total_grazers += thisTile.grazers.Count;
            }
        }

        float random_number = Random.Range(0, total_grazers);
        int a;
        for (a = 0; a < candidates.Count; a++)
        {
            if (random_number <= candidates[a].weight)
            {
                best_place = candidates[a].candidate;
                break;
            }
            else
            {
                random_number -= candidates[a].weight;
            }
        }

        return best_place != null ? best_place.location : location;
    }

    public int Reproduce()
    {
        if (population >= 2)
        {
            float value = Random.value;
            if (value < fertility)
            {
                return 1;
            }
        }
        return 0;
    }

    public Carnivore CreateBaby(Transform world, Vector2 tile_offset)
    {
        return CreateBasicCarnivore(location, tile_offset, sprite, world);
    }
}
