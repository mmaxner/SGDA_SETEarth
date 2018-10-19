using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentValueReader : MonoBehaviour {
    
    private Text text;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
    }

    public void FeedValue(float value)
    {
        text.text = value.ToString("#####");
    }
}
