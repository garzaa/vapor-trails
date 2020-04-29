using UnityEngine;

// gotta put this on everything instantiated at runtime into a GridLayout group
public class ScaleObjectToCanvas : MonoBehaviour {
    RectTransform rt;
    Vector2 originalSizeDelta;

    /* 
        for some ungodly reason, setting the size delta to itself (and then 
        scaling it to 1!) is the only thing that keeps the GridLayout from 
        shidding itself when the pixel-perfect canvas changes its scaling 
        factor, like in the event of a window resize
    */
    void Start() {
        rt = GetComponent<RectTransform>();
        originalSizeDelta = rt.sizeDelta;
        rt.sizeDelta = originalSizeDelta;
        rt.localScale = Vector3.one;
    }

}