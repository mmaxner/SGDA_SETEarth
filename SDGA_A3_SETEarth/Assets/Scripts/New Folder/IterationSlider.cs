using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IterationSlider : MonoBehaviour {

    public WorldController world;
    private Slider slider;

    // Use this for initialization
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(world.SetIterations);
        slider.onValueChanged.AddListener(world.PreviewMapSettings);
        world.SetIterations(slider.value);
    }
}
