using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame : MonoBehaviour {

    public WorldManager manager;
    public InputField input;

	public void Save()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Text>().text = "Saving...";
        }
        string name = input.text.Length > 0 ? input.text : "default";
        new DatabaseInterfacer().SaveWorld(name, manager.size_factor, manager.world, manager.herbies, manager.carnies);
        foreach (Transform child in transform)
        {
            child.GetComponent<Text>().text = "Game Saved";
        }
    }
}
