// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

//#define REWIRED_CONTROL_MAPPER_USE_TMPRO

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using Rewired;
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
    using Text = TMPro.TMP_Text;
#else
    using Text = UnityEngine.UI.Text;
#endif

    [AddComponentMenu("")]
    public abstract class UIElementInfo : MonoBehaviour, ISelectHandler {

        public string identifier;
        public int intData;
        public Text text;

        public event System.Action<GameObject> OnSelectedEvent;

        #region ISelectHandler Implementation

        public void OnSelect(BaseEventData eventData) {
            if(OnSelectedEvent != null) OnSelectedEvent(this.gameObject);
        }

        #endregion
    }
}
