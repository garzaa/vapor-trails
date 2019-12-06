using UnityEngine;
using System.Collections;

public class FlashingLaser : MonoBehaviour {
    public GameObject charge;
    public GameObject fire;

    public float delay = 0f;
    public float chargeTime = 1f;
    public float fireTime = 1f;
    public float restTime = 1f;

    void Start() {
        charge.SetActive(false);
        fire.SetActive(false);
        StartCoroutine(InitRoutine());
    }

    IEnumerator InitRoutine() {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine() {
        yield return new WaitForSeconds(restTime);
        charge.SetActive(true);
        yield return new WaitForSeconds(chargeTime);
        charge.SetActive(false);
        fire.SetActive(true);
        yield return new WaitForSeconds(fireTime);
        fire.SetActive(false);
        StartCoroutine(FireRoutine());
    }
}