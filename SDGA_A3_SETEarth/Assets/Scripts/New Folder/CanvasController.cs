using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour {
   

    public GameObject GenerationMenu;
    public GameObject GameViewMenu;
    public GameObject MainMenu;
    public GameObject AboutMenu;
    public GameObject HelpMenu;

    public void ShowGenerationMenu()
    {
        GenerationMenu.SetActive(true);
        GameViewMenu.SetActive(false);
        MainMenu.SetActive(false);
        AboutMenu.SetActive(false);
        HelpMenu.SetActive(false);
    }

    public void ShowMainMenu()
    {
        GenerationMenu.SetActive(false);
        GameViewMenu.SetActive(false);
        MainMenu.SetActive(true);
        AboutMenu.SetActive(false);
        HelpMenu.SetActive(false);
    }

    public void ShowGameViewMenu()
    {
        GenerationMenu.SetActive(false);
        GameViewMenu.SetActive(true);
        MainMenu.SetActive(false);
        AboutMenu.SetActive(false);
        HelpMenu.SetActive(false);
    }

    public void ShowAboutMenu()
    {
        GenerationMenu.SetActive(false);
        GameViewMenu.SetActive(false);
        MainMenu.SetActive(false);
        AboutMenu.SetActive(true);
        HelpMenu.SetActive(false);
    }

    public void ShowHelpMenu()
    {
        GenerationMenu.SetActive(false);
        GameViewMenu.SetActive(false);
        MainMenu.SetActive(false);
        AboutMenu.SetActive(false);
        HelpMenu.SetActive(true);
    }
}
