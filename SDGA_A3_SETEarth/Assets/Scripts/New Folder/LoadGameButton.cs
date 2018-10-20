using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameButton : MonoBehaviour {

    public WorldManager manager;
    public Transform button_repo;
    public Transform button_start;
    public GameObject button;
    public CanvasController canvas;

    public void ClickityClackety()
    {
        List<string> games = new DatabaseInterfacer().ListSavedGames();
        for (int i = 0; i < games.Count; i++)
        {
            GameObject butt = GameObject.Instantiate(button);
            RectTransform transform = butt.GetComponent<RectTransform>();
            transform.parent = button_start;
            transform.localPosition = new Vector3(0, -(i * 30), 0);

            foreach (Transform child in transform)
            {
                Text text = child.GetComponent<Text>();
                if (text != null)
                {
                    text.text = games[i];
                }
            }

            LoadButton loader = butt.GetComponent<LoadButton>();
            loader.canvas = canvas;
            loader.manager = manager;
            loader.world_name = games[i];
        }
        
    }
}
