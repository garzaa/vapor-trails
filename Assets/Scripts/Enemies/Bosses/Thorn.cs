using UnityEngine;
using System.Collections;

public class Thorn : Boss {

    public GameObject orbitingSwords;
    public GameObject blurredSwords;
    public AudioClip swordSpinNoise;
    public AudioClip ringBreakSound;

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

    [ContextMenu("Space Swords")]
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
        if (currentSwords <= 1) {
            RingBreak();
            return;
        }
        LoseSword();
        anim.SetTrigger("RingHit");
        rb2d.velocity = new Vector2(
            rb2d.velocity.x - ForwardScalar(),
            rb2d.velocity.y
        );
    }

    public void LoseSword() {
        orbitingSwords.transform.GetChild(currentSwords-1).gameObject.SetActive(false);
        // rotate forwards and back
        targetSwordAngle.z += (currentSwords % 2 == 0) ? 170 : -170;
        targetSwordAngle.z = targetSwordAngle.z % 360;
        targetingRotation = true;
        SoundManager.WorldSound(swordSpinNoise);
        SpaceSwordRotation();
    }

    protected override void Update() {
        base.Update();
        currentSwords = GetActiveSwords();
        anim.SetInteger("CurrentSwords", currentSwords);

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
    }

    void RingBreak() {
        // todo: disable all of them i guess
        SoundManager.WorldSound(ringBreakSound);
        orbitingSwords.transform.GetChild(currentSwords-1).gameObject.SetActive(false);
        anim.SetTrigger("RingBreak");
    }

    public void ResetSwords() {
        currentSwords = maxSwords;
        targetSwordAngle.z = initialSwordRingAngle;
        StartCoroutine(ShowSwords());
    }

    IEnumerator ShowSwords() {
        // int idx=0;
        foreach (Transform s in orbitingSwords.transform) {
            // AlerterText.AlertImmediate("reset sword "+idx++);
            s.gameObject.SetActive(true);
            SpaceSwordRotation();
        }
        yield return new WaitForSeconds(0f);
    }

    int GetActiveSwords() {
        int c = 0;
        foreach (Transform t in orbitingSwords.transform) {
            if (t.gameObject.activeSelf) c++;
        }
        return c;
    }
}