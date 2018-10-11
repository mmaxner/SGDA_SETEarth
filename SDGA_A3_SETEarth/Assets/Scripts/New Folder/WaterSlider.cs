using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterSlider : MonoBehaviour
{

    public WorldController world;
    private Slider slider;

    // Use this for initialization
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(world.SetWaterWeight);
        slider.onValueChanged.AddListener(world.PreviewMapSettings);
        world.SetWaterWeight(slider.value);
    }
}
