using UnityEngine;
using System.Collections.Generic;

public class ChildCycler : Activatable {
    List<GameObject> children = new List<GameObject>();
    int currentIndex;

    public bool savePositionOnDeactivate;

    void Start() {
        foreach (Transform child in transform) {
            children.Add(child.gameObject);
        }
    }

    override public void ActivateSwitch(bool b) {
        if (b) {
            currentIndex++;
            if (currentIndex == children.Count) {
                currentIndex = 0;
            }
        } else {
            currentIndex--;
            if (currentIndex < 0) {
                currentIndex = children.Count-1;
            }
        }
        DeactivateAllChildren();
        children[currentIndex].SetActive(true);
    }

    void DeactivateAllChildren() {
        foreach (GameObject child in children) {
            child.SetActive(false);
        }
    }

    void OnDisable() {
        if (!savePositionOnDeactivate) currentIndex = 0;
    }
}