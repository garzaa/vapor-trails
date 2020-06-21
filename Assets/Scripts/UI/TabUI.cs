using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabUI : MonoBehaviour {
    bool firstEnable = true;

    [SerializeField] Transform tabContainer;
    [SerializeField] GameObject tabPrefab;
    [SerializeField] Transform screenContainer;

    List<GameObject> activeTabs;

    int currentTab = 0;
    UILerper lerper;

    // called whenever enabled
    void OnEnable() {
        if (firstEnable) {
            firstEnable = false;
            return;
        }
        InitializeUI();
    }
    
    void Start() {
        // called once! after every game object is loaded, after first OnEnable
        InitializeUI();
    }

    void InitializeUI() {
        ClearTabs();
        LinkSubscreens();
        ShowTab(currentTab);
    }

    void ClearTabs() {
        foreach (Transform child in tabContainer.transform) {
            Destroy(child.gameObject);
        }
    }

    void LinkSubscreens() {
        activeTabs = new List<GameObject>();
        int currentChild = 0;
        foreach (Transform child in screenContainer) {
            GameObject g = child.gameObject;
            if (g.activeSelf) {
                activeTabs.Add(g);
                AddTab(g.name, currentChild);
            }
            currentChild++;
        }
    }

    void AddTab(string tabName, int tabNum) {
        lerper = GetComponent<UILerper>();
        GameObject t = Instantiate(tabPrefab, Vector3.zero, Quaternion.identity, tabContainer);
        t.name = tabName;
        t.GetComponentInChildren<Text>().text = tabName;
        Button b = t.GetComponentInChildren<Button>();
        b.onClick.AddListener(delegate { ShowTab(tabNum); });
    }

    public void NextTab() {
        ShowTab(currentTab++);
    }

    public void PreviousTab() {
        ShowTab(currentTab--);
    }

    void ShowTab(int tabNumber) {
        currentTab = tabNumber % activeTabs.Count;
        HideAll();
        activeTabs[currentTab].gameObject.SetActive(true);
        GameObject currentTabObj = tabContainer.transform.GetChild(currentTab).gameObject;
        Button b = currentTabObj.GetComponent<Button>();
        b.Select();
        b.OnSelect(null);
    }

    void HideAll() {
        foreach (Transform t in screenContainer) {
            t.gameObject.SetActive(false);
        }
    }

}