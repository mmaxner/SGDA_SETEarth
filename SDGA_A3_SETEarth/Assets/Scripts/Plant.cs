using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant {

    public float Nutrition;
    public float Population;
    public float GrowthRate;
    public int RegrowthTime;
    public int RegrowthTimeLeft;
    public GameObject Sprite;

    public override string ToString()
    {
        return "Plant: \n\tNutrition: " + Nutrition.ToString() +
            "\tPopulation: " + Population.ToString() +
            "\tReGrowthTimeLeft: " + RegrowthTimeLeft.ToString();
    }
}
