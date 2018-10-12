using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal {
    public Vector2 location;
    public float movement;  // tiles per turn
    public float perception;    // tiles perceived when moving
    //public int population;  // they live in a society
    public float starvation_threshold;  // 10 for needs to eat 10 food per turn to survive
    public float fertility;   // percent chance of reproducing
    public Sprite sprite;   // its a sprite 
}
