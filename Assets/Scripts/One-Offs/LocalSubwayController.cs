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

    bool holdingPlayer = false;
    bool arrivingForPlayer = false;

    GameObject subwayCars;

    void PlayDoorCloseSound() {
        PlaySound(1);
    }

    void Start() {
        doors = GetComponentsInChildren<Animator>().ToList();
        animator = GetComponent<Animator>();
        subwayCars = transform.Find("Cars").gameObject;
    }

    public void FinishDeparting() {
        if (holdingPlayer) SubwayManager.DepartWithPlayer();
    }

    public void CheckPlayerEnter() {
        if (!holdingPlayer) {
            animator.SetTrigger("Arrive");
            arrivingForPlayer = true;
        }
    }

    public void FinishArriving() {
        OpenDoors();
    }

    public void OpenDoors() {
        doorsOpening.PlayOneShot(doorsOpening.clip);
        foreach (Animator a in doors) {
            a.SetBool("Open", true);
        }
        Invoke("FinishOpeningDoors", 1f);
    }

    public void FinishOpeningDoors() {
        if (holdingPlayer && !arrivingForPlayer) {
            ShowPlayer();
            LeaveWithoutPlayer();
        }
    }

    // called from the end of the doors opening if it's not holding or arriving for the player
    void LeaveWithoutPlayer() {
        CloseDoors();
        Invoke("FinishClosingDoors", 1f);
    }

    // called from the interaction
    public void BoardPlayer() {
        HidePlayer();
        Invoke("FinishClosingDoors", 1f);
        animator.SetTrigger("BoardPlayer");
    }

    public void CloseDoors() {
        Invoke("PlayDoorCloseSound", .7f);
        foreach (Animator a in doors) {
            a.SetBool("Open", false);
        }
    }

    public void FinishClosingDoors() {
        if (holdingPlayer) {
            SubwayManager.SetPlayerOffset(playerFollower.transform.position - this.transform.position);
            SubwayManager.OpenMapUI(this);
        } else {
            // skip the map, this will be called when the map closes
            ShowPlayer();
            animator.SetTrigger("Depart");
        }
    }

    public void OffsetPlayerFollower(Vector3 offset) {
        subwayCars = subwayCars ?? transform.Find("Cars").gameObject;
        playerFollower.transform.position = subwayCars.transform.position + offset;
    }

    override public void HidePlayer() {
        // if called from OnSceneLoaded
        holdingPlayer = true;
        animator = animator ?? GetComponent<Animator>();
        animator.SetBool("PlayerHidden", true);
        base.HidePlayer();
    }

    override public void ShowPlayer() {
        base.ShowPlayer();
        holdingPlayer = false;
        animator.SetBool("PlayerHidden", false);
    }
}