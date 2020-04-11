using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BarUI : MonoBehaviour {
    [SerializeField] Image indicator;
    [SerializeField] Image container;
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
            if (_current > _max) _max = current;
            RedrawUI(value - _current);
        }
    }

    void RedrawUI(float delta) {
        container.rectTransform.sizeDelta = new Vector2((max*pixelsPerUnit)+1,  container.rectTransform.sizeDelta.y);
        indicator.rectTransform.sizeDelta = new Vector2(current*pixelsPerUnit, indicator.rectTransform.sizeDelta.y);
    }
}