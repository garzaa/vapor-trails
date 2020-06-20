using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabUI : MonoBehaviour {
    bool firstEnable = true;
    List<string> subscreenNames = new List<string>();
    List<GameObject> subscreens = new List<GameObject>();

    [SerializeField]    
    Transform subscreenNameContainer;
    [SerializeField]
    GameObject subscreenNamePrefab;

    int currentTab = 0;

    void OnEnable() {
        if (firstEnable) {
            firstEnable = false;
            return;
        }
        foreach (Transform child in transform) {
            GameObject g = child.gameObject;
            if (g.activeSelf) {
                subscreenNames.Add(g.name);
                subscreens.Add(g);
            }
        }
        // so it persists across openings
        ShowTab(currentTab);
    }

    void Start() {
        // call this once every game object is loaded
        OnEnable();
        gameObject.SetActive(false);
    }

    public void NextTab() {
        ShowTab(currentTab++);
    }

    public void PreviousTab() {
        ShowTab(currentTab--);
    }

    void ShowTab(int tabNumber) {
        currentTab = tabNumber % subscreens.Count;
        HideAll();
        
    }

    void HideAll() {
        foreach (GameObject g in subscreens) {
            g.SetActive(false);
        }
    }

}