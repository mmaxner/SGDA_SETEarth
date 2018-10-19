using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandSlider : MonoBehaviour
{

    public WorldController world;
    private Slider slider;

    // Use this for initialization
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(world.SetLandWeight);
        world.SetLandWeight(slider.value);
    }
}
