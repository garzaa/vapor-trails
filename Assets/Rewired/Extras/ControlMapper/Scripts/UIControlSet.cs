// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

//#define REWIRED_CONTROL_MAPPER_USE_TMPRO

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System.Collections;
    using Rewired;
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
    using Text = TMPro.TMP_Text;
#else
    using Text = UnityEngine.UI.Text;
#endif

    [AddComponentMenu("")]
    public class UIControlSet : MonoBehaviour {

        [SerializeField]
        private Text title;

        private Dictionary<int, UIControl> _controls;
        private Dictionary<int, UIControl> controls { get { return _controls ?? (_controls = new Dictionary<int, UIControl>()); } }

        public void SetTitle(string text) {
            if(title == null) return;
            title.text = text;
        }

        public T GetControl<T>(int uniqueId) where T : UIControl {
            UIControl control;
            controls.TryGetValue(uniqueId, out control);
            return control as T;
        }

        public UISliderControl CreateSlider(GameObject prefab, Sprite icon, float minValue, float maxValue, System.Action<int, float> valueChangedCallback, System.Action<int> cancelCallback) {
            GameObject instance = (GameObject)Object.Instantiate(prefab);
            UISliderControl control = instance.GetComponent<UISliderControl>();
            if(control == null) {
                Object.Destroy(instance);
                Debug.LogError("Prefab missing UISliderControl component!");
                return null;
            }
                
            instance.transform.SetParent(this.transform, false);
            if(control.iconImage != null) {
                control.iconImage.sprite = icon;
            }
            if(control.slider != null) {
                control.slider.minValue = minValue;
                control.slider.maxValue = maxValue;
                if(valueChangedCallback != null) {
                    control.slider.onValueChanged.AddListener((float value) => { valueChangedCallback(control.id, value); });
                }
                if(cancelCallback != null) {
                    control.SetCancelCallback(() => { cancelCallback(control.id); });
                }
            }
            controls.Add(control.id, control);
            return control;
        }

    }
}
