using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BarUI : MonoBehaviour {
    [SerializeField] Image indicator;
    [SerializeField] Image container;
    [SerializeField] Image background;
    [SerializeField] float pixelsPerUnit;

    [SerializeField] int _max;
    [SerializeField] int _current;

    public int max {
        get { return _max; }
        set {
            _max = value;
            RedrawUI(0);
        }
    }
    public int current {
        get { return _current; }
        set {
            _current = value;
            RedrawUI(value - _current);
        }
    }

    void RedrawUI(int delta) {
        ScaleImage(background, max);
        ScaleImage(container, max, mod:1);
        ScaleImage(indicator, current);
    }

    void ScaleImage(Image i, int val, int mod=0) {
        i.rectTransform.sizeDelta = new Vector2((val*pixelsPerUnit)+mod, i.rectTransform.sizeDelta.y);
    }
}