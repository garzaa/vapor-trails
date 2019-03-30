using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LocalSubwayController : AnimationInterface {
    List<Animator> doors;

    void Start() {
        doors = GetComponentsInChildren<Animator>().ToList();
    }

    public void OpenDoors() {
        foreach (Animator a in doors) {
            a.SetBool("Open", true);
        }
    }

    public void CloseDoors() {
        foreach (Animator a in doors) {
            a.SetBool("Open", false);
        }
    }
}