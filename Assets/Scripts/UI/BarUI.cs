using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BarUI : MonoBehaviour {
    public Image indicator;
    public Image container;
    public Image background;
    public Image deltaIndicator;
    public float pixelsPerUnit;

    int _max;
    int _current;
    
    readonly float deltaDelay = 0.5f;
    readonly float deltaMoveSpeed = 20f;
    readonly float deltaTolerance = 1f;
    float currentDelta;
    float changeTime;

    void OnEnable() {
        currentDelta = 0;
    }

    public int max {
        get { return _max; }
        set {
            if (_max == value) return;

            _max = value;
            if (deltaIndicator != null ) {
                ScaleImage(deltaIndicator, max);
                currentDelta=max;
            }
            RedrawUI();
        }
    }
    public int current {
        get { return _current; }
        set {
            if (_current == value) return;

            _current = value;
            changeTime = Time.time;
            RedrawUI();
        }
    }

    public void SetBarColor(Color color) {
        indicator.color = color;
    }

    void RedrawUI() {
        ScaleImage(background, max);
        ScaleImage(container, max, mod:1);
        ScaleImage(indicator, current);
    }

    void ScaleImage(Image i, float val, int mod=0) {
        i.rectTransform.sizeDelta = new Vector2((val*pixelsPerUnit)+mod, i.rectTransform.sizeDelta.y);
    }

    void Update() {
        if (deltaIndicator == null || currentDelta==current) {
            return;
        }
        
        if (Mathf.Abs(currentDelta-current) < deltaTolerance) {
            currentDelta=current;
        }
        else if (Time.time > changeTime + deltaDelay) {
            float dir = Mathf.Sign(current - currentDelta);
            currentDelta += (deltaMoveSpeed*Time.deltaTime*dir);
        }

        ScaleImage(deltaIndicator, currentDelta);
    }
}
