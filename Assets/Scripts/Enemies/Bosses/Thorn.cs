using UnityEngine;

public class Thorn : Boss {

    public GameObject orbitingSwords;

    int maxSwords;
    int currentSwords;

    override protected void Start() {
        base.Start();
        maxSwords = orbitingSwords.transform.childCount;
    }

    public void OnSwordRingHit() {
        currentSwords--;
        if (currentSwords <= 0) {
            RingBreak();
            return;
        }

        float degreeDifference = 360f/((float) maxSwords);
        Vector3 e = orbitingSwords.transform.eulerAngles;
        orbitingSwords.transform.rotation = Quaternion.Euler(
            e.x,
            e.y,
            e.z + ((currentSwords % 2 == 0) ? degreeDifference : -degreeDifference)
        );
    }

    public void RingBreak() {

    }
}