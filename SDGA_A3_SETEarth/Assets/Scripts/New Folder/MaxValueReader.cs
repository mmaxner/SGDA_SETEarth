using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxValueReader : MonoBehaviour {

    private float max = 0;
    private Text text;
	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}

    public void FeedValue(float value)
    {
        if (value > max)
        {
            max = value;
        }
        text.text = max.ToString("#####");
    }
}
