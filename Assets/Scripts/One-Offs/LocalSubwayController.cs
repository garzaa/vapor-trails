using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LocalSubwayController : AnimationInterface {
    List<Animator> doors;
    Animator animator;
    public AudioSource doorsOpening;
    public SubwayStop stop;
    public PlayerFollower playerFollower;

    public bool playerArriving = false;

    void PlayDoorCloseSound() {
        PlaySound(1);
    }

    void Start() {
        doors = GetComponentsInChildren<Animator>().ToList();
        animator = GetComponent<Animator>();
    }

    public void OpenDoors() {
        doorsOpening.PlayOneShot(doorsOpening.clip);
        foreach (Animator a in doors) {
            a.SetBool("Open", true);
        }
    }

    public void CloseDoors() {
        Invoke("PlayDoorCloseSound", .7f);
        foreach (Animator a in doors) {
            a.SetBool("Open", false);
        }
    }

    public void FinishClosingDoors() {
        SubwayManager.SetPlayerOffset(this.transform.position - playerFollower.transform.position);
        SubwayManager.OpenMapUI(this);
    }

    public void FinishDeparting() {
        SubwayManager.DepartWithPlayer();
    }

    public void CheckPlayerEnter() {
        if (!playerArriving) {
            animator.SetTrigger("Arrive");
        }
    }

    public void FinishArriving() {
        playerArriving = false;
        animator.SetBool("PlayerHidden", false);
        ShowPlayer();
        DepartWithoutPlayer();
    }

    void DepartWithoutPlayer() {
        CloseDoors();
        animator.SetTrigger("Depart");
    }
}