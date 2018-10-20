using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPanelSwitcher : MonoBehaviour {

    public GameObject Settings;
    public GameObject Generation;
    public GameObject Herbivores;
    public GameObject Carnivores;
    public GameObject Plants;

    public void Set()
    {
        Settings.SetActive(true);
        Generation.SetActive(false);
        Herbivores.SetActive(false);
        Carnivores.SetActive(false);
        Plants.SetActive(false);
    }
    public void Gen()
    {
        Settings.SetActive(false);
        Generation.SetActive(true);
        Herbivores.SetActive(false);
        Carnivores.SetActive(false);
        Plants.SetActive(false);
    }
    public void Herb()
    {
        Settings.SetActive(false);
        Generation.SetActive(false);
        Herbivores.SetActive(true);
        Carnivores.SetActive(false);
        Plants.SetActive(false);
    }
    public void Carn()
    {
        Settings.SetActive(false);
        Generation.SetActive(false);
        Herbivores.SetActive(false);
        Carnivores.SetActive(true);
        Plants.SetActive(false);
    }
    public void Plant()
    {
        Settings.SetActive(false);
        Generation.SetActive(false);
        Herbivores.SetActive(false);
        Carnivores.SetActive(false);
        Plants.SetActive(true);
    }
}
