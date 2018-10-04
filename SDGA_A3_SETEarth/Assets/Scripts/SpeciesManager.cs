using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpeciesManager {

    private static List<Sprite> PlantSprites;

	public static bool AreSameSpecies(Plant a, Plant b)
    {
        if (Mathf.Abs(a.GrowthRate - b.GrowthRate) < 0.15)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Plant AssignNewSpeciesProgenitor(Plant Progenitor)
    {
        return Progenitor;
    }
}
