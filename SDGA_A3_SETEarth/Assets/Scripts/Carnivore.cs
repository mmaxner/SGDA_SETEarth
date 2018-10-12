using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carnivore : Animal {
    public float last_meal_size;
    public static float hunt_chance = 0.5f;
    public static float meal_bonus_increment = 5;
    public static float meal_bonus = 0.9f;

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
        if (meat > starvation_threshold)
        {
            last_meal_size = meat;
        }
        else
        {
            // RIP
        }
    }
}
