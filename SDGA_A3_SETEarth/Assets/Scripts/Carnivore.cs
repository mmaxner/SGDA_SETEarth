using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carnivore {

    public float Population;
    public float Consumption;
    public float ReproductionRate;
    public GameObject Sprite;

    public override string ToString()
    {
        return "Carnivore: \n" +
            "\tPopulation: " + Population.ToString() +
            "\tTotalConsumption: " + (Consumption * Population).ToString();
    }
}
