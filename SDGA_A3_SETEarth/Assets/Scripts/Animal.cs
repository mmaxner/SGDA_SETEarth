using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal {
    public Vector2 location;
    public float movement;  // tiles per turn
    public float perception;    // tiles perceived when moving
    public int population;  // they live in a society
    public float starvation_threshold;  // 10 for needs to eat 10 food per turn to survive
    public float fertility;   // percent chance of reproducing
    public GameObject sprite;   // its a sprite 
    public bool isAlive;
    public void MoveTo(Vector2 new_location, Vector2 new_sprite_location)
    {
        sprite.transform.localPosition = new_sprite_location;
        location = new_location;
    }

    public struct MovementCandidate
    {
        public TerrainTile candidate;
        public float weight;
        public Vector2 location;
    }

    public List<Vector2> GetPerceptionRange()
    {
        List<Vector2> range = new List<Vector2>();
        for (int i = (int)location.x - (int)perception; i <= (int)location.x + (int)perception; i++)
        {
            for (int j = (int)location.y - (int)perception; j <= (int)location.y + (int)perception; j++)
            {
                range.Add(new Vector2(i, j));
            }
        }

        return range;
    }
}
