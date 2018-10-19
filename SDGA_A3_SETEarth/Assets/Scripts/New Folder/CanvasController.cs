using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour {

    public GameObject GameUI;
    public GameObject WorldGenerationUI;
    public GameObject MainMenu;

    public void StartNewGame()
    {
        WorldGenerationUI.SetActive(true);
        MainMenu.SetActive(false);

    }
}
