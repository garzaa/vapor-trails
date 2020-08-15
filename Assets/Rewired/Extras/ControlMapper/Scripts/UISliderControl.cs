// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
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

    [AddComponentMenu("")]
    public class UISliderControl : UIControl {
        
        public Image iconImage;
        public Slider slider;
        
        private bool _showIcon;
        private bool _showSlider;
        
        public bool showIcon {
            get { return _showIcon; }
            set {
                if(iconImage == null) return;
                iconImage.gameObject.SetActive(value);
                _showIcon = value;
            }
        }
        public bool showSlider {
            get { return _showSlider; }
            set {
                if(slider == null) return;
                slider.gameObject.SetActive(value);
                _showSlider = value;
            }
        }

        public override void SetCancelCallback(System.Action cancelCallback) {
            base.SetCancelCallback(cancelCallback);
            if(cancelCallback == null || slider == null) return;

            if(slider as ICustomSelectable != null) { // it's a custom selectable, we can use our simpler system
                (slider as ICustomSelectable).CancelEvent += () => { cancelCallback(); };
            } else { // it's a normal selectable so use Unity's Event Trigger system
                EventTrigger trigger = slider.GetComponent<EventTrigger>();
                if(trigger == null) trigger = slider.gameObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.callback = new EventTrigger.TriggerEvent();
                entry.eventID = EventTriggerType.Cancel;
                entry.callback.AddListener((BaseEventData data) => { cancelCallback(); });
#if (UNITY_5_0_0 || UNITY_5_0_1 || UNITY_5_0_2 || UNITY_5_0_3 || UNITY_5_0_4) || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
                if(trigger.delegates == null) trigger.delegates = new List<EventTrigger.Entry>();
                trigger.delegates.Add(entry);
#else
                if(trigger.triggers == null) trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(entry);
#endif
            }
        }

    }
}
