using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbivore : Animal {

    public float food_efficiency; // 0.05-0.15 for 5-15% of nutrition into meat
    public float size;  // how much meat the animal has
    public float max_growth_factor; // how much size to increase by max eating nutrtiion

    public Herbivore CreateBasicHerb(Vector2 where, Sprite appearance)
    {
        Herbivore herby = new Herbivore()
        {
            movement = 1.0f,
            perception = 1.0f,
            fertility = 0.35f,
            food_efficiency = 0.1f,
            starvation_threshold = 10,
            size = 1.0f,
            max_growth_factor = 2.0f,
            location = where,
            sprite = GameObject.Instantiate(appearance)
        };
        return herby;
    }

    public float GetAppetite()
    {
        return ((size * (1.0f - max_growth_factor)) / food_efficiency);
    }

    public void Feed(float nutrition)
    {
        if (nutrition > starvation_threshold)
        {
            size += nutrition * food_efficiency;
        }
    }

    public float GetEaten()
    {
        float meat = size;
        size = 0;
        return meat;
        // :(
    }
}
