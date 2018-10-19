using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbivore : Animal {

    public float food_efficiency; // 0.05-0.15 for 5-15% of nutrition into meat
    public float size;  // how much meat the animal has
    public float max_growth_factor; // how much size to increase by max eating nutrtiion

    public static Herbivore CreateBasicHerb(Vector2 where, Vector2 tile_offset, GameObject appearance, Transform parent)
    {
        Herbivore herby = new Herbivore()
        {
            movement = 1.0f,
            perception = 1.0f,
            fertility = 0.15f,
            food_efficiency = 0.1f,
            starvation_threshold = 10,
            size = 1.0f,
            max_growth_factor = 0.5f,
            location = where,
            sprite = GameObject.Instantiate(appearance),
            isAlive = true,
            population = 2
        };
        herby.sprite.transform.SetParent(parent);
        herby.sprite.transform.localPosition = new Vector3((where.x - tile_offset.x) * StaticData.size_increment, (where.y - tile_offset.y) * StaticData.size_increment, -1);

        return herby;
    }
    
    public float GetAppetite()
    {
        return (population * starvation_threshold) + (size * max_growth_factor) / food_efficiency;
    }

    public void Feed(float nutrition)
    {
        if (nutrition >= population * starvation_threshold)
        {
            nutrition -= starvation_threshold;
            size += nutrition * food_efficiency;
        }
        else
        {
            int surviving_population = (int)nutrition / (int)starvation_threshold;
            isAlive = false;
        }
    }

    public float GetEaten()
    {
        float meat = size;
        size = 0;
        isAlive = false;
        return meat;
    }

    public Vector2 Search(List<TerrainTile> area)
    {
        Vector2 best_place = location;
        float total_nutrtition = 0;
        List<MovementCandidate> candidates = new List<MovementCandidate>();
        for (int i = 0; i < area.Count; i++)
        {
            TerrainTile thisTile = area[i];
            if (thisTile.nutrition > starvation_threshold)
            {
                candidates.Add(new MovementCandidate()
                {
                    weight = thisTile.nutrition,
                    location = thisTile.location
                });
                total_nutrtition += thisTile.nutrition;
            }
        }

        float random_number = Random.Range(0, total_nutrtition);
        for (int i = 0; i < candidates.Count; i++)
        {
            if (random_number <= candidates[i].weight)
            {
                best_place = candidates[i].location;
                break;
            }
            else
            {
                random_number -= candidates[i].weight;
            }
        }

        return best_place;
    }

    public int Reproduce()
    {
        int baby_count = 0;
        // for eaach pair of animals in this population
        /*for (int i = 2; i < size; i+=2)
        {
            float value = Random.value;
            if (value < fertility)
            {
                baby_count++;
                size -= 0.65f;
            }
        }*/
        if (size >= 2)
        {
            float value = Random.value;
            if (value < fertility)
            {
                baby_count++;
                size -= 0.65f;
            }
        }

        return baby_count;
    }

    public Herbivore CreateBaby(Transform world, Vector2 tile_offset)
    {
        return CreateBasicHerb(location, tile_offset, sprite, world);
    }
}
