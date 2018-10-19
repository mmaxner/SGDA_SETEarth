using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile {
    public enum TerrainType
    {
        LAND = 1,
        WATER = 2,
        GRASS = 4,
        SAND = 8,
        SHALLOW = 16,
        DEEP = 32,
        MAINLAND = LAND | GRASS,
        COAST = LAND | SAND,
        SHALLOWOCEAN = WATER | SHALLOW,
        OCEAN = WATER | DEEP
    }

    public float height;
    public float temperature;
    public float nutrition;
    public float growth_rate;
    public int regrowth_left;
    public TerrainType type;
    public List<Herbivore> grazers;
    public List<Carnivore> predators;
    public float available_meat;
    public Vector2 location;

    private static float min_viable_temperature = -5;
    private static float max_viable_temperature = 40;
    private static int regrowth_time = 5;
    private static float max_plants = 10000;

    public TerrainTile(float height_in, float temperature_in,TerrainType type_in, Vector2 location_in)
    {
        height = height_in;
        temperature = temperature_in;
        type = type_in;
        growth_rate = 0;
        if ((type & TerrainTile.TerrainType.LAND) > 0)
        {
            growth_rate += 0.1f;
        }
        if ((type & TerrainTile.TerrainType.GRASS) > 0)
        {
            growth_rate += 0.05f;
        }
        if (temperature > min_viable_temperature && temperature < max_viable_temperature)
        {
            growth_rate += 0.1f;
        }
        grazers = new List<Herbivore>();
        predators = new List<Carnivore>();
        available_meat = 0;
        nutrition = 0;
        regrowth_left = 0;
        location = location_in;
    }

    public void Reset()
    {
        available_meat = 0;
        grazers.Clear();
        predators.Clear();
    }

    public void GrowPlants()
    {
        if (regrowth_left > 1)
        {
            regrowth_left--;
        }
        else if (regrowth_left == 1)
        {
            regrowth_left--;
            nutrition = 100;
        }
        nutrition += nutrition * growth_rate;
        if (nutrition > max_plants)
        {
            nutrition = max_plants;
        }
    }

    public float EatPlants(float appetite)
    {
        if (nutrition == 0)
        {
            return 0;
        }
        if (appetite < nutrition)
        {
            nutrition -= appetite;
            return appetite;
        }
        else
        {
            float available = nutrition;
            nutrition = 0;
            regrowth_left = regrowth_time;
            return available;
        }
    }

    public float EatAnimal()
    {
        if (grazers.Count == 0)
        {
            return 0.0f;
        }

        int index = Random.Range(0, grazers.Count);
        Herbivore meal = grazers[index];
        float meat = meal.GetEaten();
        grazers.RemoveAt(index);
        return meat;
    }

    public List<Herbivore> HerbivoreSex()
    {
        List<Herbivore> babies = new List<Herbivore>();
        for (int i = 1;i < grazers.Count; i += 2)
        {
            Herbivore mateA = grazers[i - 1];
            Herbivore mateB = grazers[i];
            float fertility = (mateA.fertility + mateB.fertility) / 2;
            if (Random.value < fertility)
            {
                babies.Add(new Herbivore());
            }
        }

        return babies;
    }

    public List<Carnivore> CarnivoreSex()
    {
        List<Carnivore> babies = new List<Carnivore>();
        // match up pairs
        for (int i = 1; i < predators.Count; i += 2)
        {
            Carnivore mateA = predators[i - 1];
            Carnivore mateB = predators[i];
            float fertility = (mateA.CalculateFertility() + mateB.CalculateFertility()) / 2;      // use average fertility of the 2 mates
            if (Random.value < fertility)
            {
                babies.Add(new Carnivore());
            }
        }

        return babies;
    }
}
