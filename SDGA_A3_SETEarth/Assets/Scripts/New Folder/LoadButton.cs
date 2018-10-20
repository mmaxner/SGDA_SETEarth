using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour {

    public CanvasController canvas;
    public WorldManager manager;
    public string world_name;

    public void Load()
    {
        new DatabaseInterfacer().LoadGame(world_name, manager);
        canvas.ShowGameViewMenu();
    }

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(Load);
    }
}
