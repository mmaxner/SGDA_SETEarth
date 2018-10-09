using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {

    private float Altitude;
    private bool Land;
    private List<Plant> Plants;
    private List<Herbivore> Herbivores;
    private List<Carnivore> Carnivores;

    private float total_nutrition;
    private float total_meat;

    private int round = 0;
    private int herbivore_round = 10;
    private int carnivore_round = 20;

    private void Start()
    {
        Plants = new List<Plant>();
        Plants.Add(new Plant()
        {
            Nutrition = 0,
            Population = 1,
            GrowthRate = 1.2135f,
            RegrowthTime = 4,
            RegrowthTimeLeft = 0,
            Sprite = null
        });
        Herbivores = new List<Herbivore>();
        Carnivores = new List<Carnivore>();
    }

    private void FixedUpdate()
    {
        if (Herbivores.Count == 0 || Herbivores[0].Meat > 0)
        {
            round++;
            if (herbivore_round == round)
            {
                Herbivores.Add(new Herbivore()
                {
                    Population = 2,
                    EdibleMass = 1,
                    Consumption = 1,
                    Meat = 0,
                    ReproductionRate = 1.15f,
                    Sprite = null
                });
            }

            if (carnivore_round == round)
            {
                Carnivores.Add(new Carnivore()
                {
                    Population = 2,
                    Consumption = 1,
                    ReproductionRate = 1.1f,
                    Sprite = null
                });
            }

            Round();
            Debug.Log("Round: " + round.ToString());
            Debug.Log(Plants[0].ToString());
            if (Herbivores.Count > 0)
            {
                Debug.Log(Herbivores[0]);
            }
            if (Carnivores.Count > 0)
            {
                Debug.Log(Carnivores[0].ToString());
            }
        }
    }

    public void Round()
    {
        Produce();
        Consume();
        Reproduce();
    }

	public void Produce()
    {
        total_nutrition = 0;
        foreach (Plant plant in Plants)
        {
            if (plant.RegrowthTimeLeft == 0)
            {
                plant.Nutrition += plant.GrowthRate * plant.Population;
                total_nutrition += plant.Nutrition;
            }
            else
            {
                plant.RegrowthTimeLeft--;
            }
        }

        total_meat = 0;
        foreach (Herbivore herbivore in Herbivores)
        {
            herbivore.Meat = herbivore.EdibleMass * herbivore.Population;
            total_meat += herbivore.Meat;
        }
    }

    public void Consume()
    {
        // Calculate consumption amounts
        float total_plant_consumption = 0;
        foreach (Herbivore herbivore in Herbivores)
        {
            total_plant_consumption += herbivore.Consumption * herbivore.Population;
        }

        float total_meat_consumption = 0;
        foreach (Carnivore carnivore in Carnivores)
        {
            total_meat_consumption += carnivore.Consumption * carnivore.Population;
        }

        if (total_plant_consumption > 0)
        {
            // Consume plants
            if (total_plant_consumption > total_nutrition)
            {
                foreach (Plant plant in Plants)
                {
                    // regrowing plants weren't actually eatens
                    if (plant.RegrowthTimeLeft == 0)
                    {
                        plant.RegrowthTimeLeft = plant.RegrowthTime;
                        plant.Population = 1;
                        plant.Nutrition = 0;
                    }
                }
                float percent_herbivores_fed = total_nutrition / total_plant_consumption;
                foreach (Herbivore herbivore in Herbivores)
                {
                    herbivore.Population *= percent_herbivores_fed;
                }
                total_nutrition = 0;
            }
            else
            {
                float percent_plants_not_eaten = 1 - (total_plant_consumption / total_nutrition);
                total_nutrition -= total_plant_consumption;
                foreach (Plant plant in Plants)
                {
                    plant.Nutrition *= percent_plants_not_eaten;
                    plant.Population *= percent_plants_not_eaten;
                }
            }
        }

        if (total_meat_consumption > 0)
        {
            // Consume meat
            if (total_meat_consumption > total_meat)
            {
                float percent_carnivores_fed = total_meat / total_meat_consumption;
                foreach (Carnivore carnivore in Carnivores)
                {
                    carnivore.Population *= percent_carnivores_fed;
                }
                foreach (Herbivore herbivore in Herbivores)
                {
                    // uh oh
                    herbivore.Population = 0;
                }
                total_meat = 0;
            }
            else
            {
                float percent_herbivores_eaten = 1 - total_meat_consumption / total_meat;
                foreach (Herbivore herbivore in Herbivores)
                {
                    herbivore.Meat -= herbivore.Meat * percent_herbivores_eaten;
                    herbivore.Population *= percent_herbivores_eaten;
                }
                total_meat -= total_meat_consumption;
            }
        }
    }

    public void Reproduce()
    {
        foreach (Plant plant in Plants)
        {
            plant.Population *= plant.GrowthRate;
        }

        foreach (Herbivore herbivore in Herbivores)
        {
            herbivore.Population *= herbivore.ReproductionRate;
        }

        foreach (Carnivore carnivore in Carnivores)
        {
            carnivore.Population *= carnivore.ReproductionRate; 
        }
    }
}
