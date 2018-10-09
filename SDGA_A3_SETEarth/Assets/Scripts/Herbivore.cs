using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbivore {

    public float Population;
    public float EdibleMass;
    public float Consumption;
    public float Meat;
    public float ReproductionRate;
    public GameObject Sprite;

    public override string ToString()
    {
        return "Herbivore: \n\tMeat: " + Meat.ToString() +
            "\tPopulation: " + Population.ToString() +
            "\tTotalConsumption: " + (Consumption * Population).ToString();
    }
}
