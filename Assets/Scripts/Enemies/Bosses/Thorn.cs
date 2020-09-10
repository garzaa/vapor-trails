using UnityEngine;
using System.Collections;

public class Thorn : Boss {

    public GameObject orbitingSwords;
    public GameObject blurredSwords;
    public AudioClip swordSpinNoise;
    public float blurCutoff;

    public int maxSwords;
    int currentSwords;

    public Vector3 targetSwordAngle = Vector3.zero;
    Vector3 swordRingVelocity = Vector3.zero;
    float initialSwordRingAngle = 0;

    public float smoothAmount = 10f;
    public float maxSpinSpeed = 10f;

    public Spinner swordSpinner;
    public bool targetingRotation = true;

    override protected void Start() {
        base.Start();
        maxSwords = orbitingSwords.transform.childCount;
        currentSwords = maxSwords;
        targetSwordAngle = orbitingSwords.transform.localRotation.eulerAngles;
        initialSwordRingAngle = targetSwordAngle.z;
        SpaceSwordRotation();
    }

    void SpaceSwordRotation() {
        currentSwords = GetActiveSwords();
        for (int i=0; i<currentSwords; i++) {
            orbitingSwords.transform.GetChild(i).gameObject.transform.localRotation = Quaternion.Euler(
                0,
                0,
                i * (360f / currentSwords)
            );
        }
    }

    public void OnSwordRingHit() {
        currentSwords = GetActiveSwords();
        orbitingSwords.transform.GetChild(currentSwords-1).gameObject.SetActive(false);
        if (currentSwords <= 1) {
            RingBreak();
            return;
        }
        targetSwordAngle.z += 180; // (360f/((float) maxSwords)) * (maxSwords-currentSwords);
        targetSwordAngle.z = targetSwordAngle.z % 360;
        targetingRotation = true;
        SpaceSwordRotation();
        SoundManager.WorldSound(swordSpinNoise);
        anim.SetTrigger("RingHit");
    }

    protected override void Update() {
        base.Update();

        if (targetingRotation && Mathf.Abs(orbitingSwords.transform.localRotation.eulerAngles.z - targetSwordAngle.z) < 1f) {
            swordSpinner.enabled = true;
            targetingRotation = false;
        } else if (targetingRotation) {
            swordSpinner.enabled = false;
            
            orbitingSwords.transform.localRotation = Quaternion.Euler(Vector3.SmoothDamp(
                orbitingSwords.transform.localRotation.eulerAngles,
                targetSwordAngle,
                ref swordRingVelocity,
                smoothAmount * Time.unscaledDeltaTime
            ));
        }

        blurredSwords.SetActive(Mathf.Abs(swordRingVelocity.z) > blurCutoff);
    }

    void RingBreak() {
        AlerterText.Alert("<color=red>RING BREAK</color>");
        anim.SetTrigger("RingBreak");
    }

    public void ResetSwords() {
        currentSwords = maxSwords;
        targetSwordAngle.z = initialSwordRingAngle;
        StartCoroutine(ShowSwords());
    }

    IEnumerator ShowSwords() {
        int idx=0;
        foreach (Transform s in orbitingSwords.transform) {
            yield return new WaitForSeconds(0.1f);
            AlerterText.AlertImmediate("reset sword "+idx++);
            s.gameObject.SetActive(true);
            SpaceSwordRotation();
        }
    }

    int GetActiveSwords() {
        int c = 0;
        foreach (Transform t in orbitingSwords.transform) {
            if (t.gameObject.activeSelf) c++;
        }
        return c;
    }
}