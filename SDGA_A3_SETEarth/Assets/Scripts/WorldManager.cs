using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public TerrainTile[,] world;
    public List<Herbivore> herbies = new List<Herbivore>();
    public List<Carnivore> carnies = new List<Carnivore>();

    public void RunRound()
    {
        int i_max = world.GetLength(0);
        int j_max = world.GetLength(1);
        for (int i = 0; i < i_max; i++)
        {
            for (int j = 0; j < j_max; j++)
            {
                world[i, j].Reset();
                world[i, j].GrowPlants();
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
            world[(int)herb.location.x, (int)herb.location.y].EatPlants(herb.GetAppetite());
        }

        for(int i = 0; i < carnies.Count; i++)
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


        for (int i = 0; i < occupiedTiles.Count; i++)
        {
            herbies.AddRange(occupiedTiles[i].HerbivoreSex());
            carnies.AddRange(occupiedTiles[i].CarnivoreSex());
        }


    }
}
