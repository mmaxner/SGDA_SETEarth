using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerationPanel : MonoBehaviour {

    List<GameObject> controls = new List<GameObject>();

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
            controls.Add(child.gameObject);
        }
	}
	
	public void DisableChildren()
    {
        foreach (GameObject child in controls)
        {
            child.SetActive(false);
        }
    }
}
