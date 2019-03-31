using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LocalSubwayController : AnimationInterface {
    List<Animator> doors;
    public AudioSource doorsOpening;

    void Start() {
        doors = GetComponentsInChildren<Animator>().ToList();
    }

    public void OpenDoors() {
        doorsOpening.PlayOneShot(doorsOpening.clip);
        foreach (Animator a in doors) {
            a.SetBool("Open", true);
        }
    }

    public void CloseDoors() {
        PlaySound(1);
        foreach (Animator a in doors) {
            a.SetBool("Open", false);
        }
    }

    public void OpenMapUI() {

    }

    public void CloseMapUI() {

    }

    public void ReactToStationSelect(SubwayStopButton stop) {

    }
}