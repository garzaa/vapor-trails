// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

//#define REWIRED_CONTROL_MAPPER_USE_TMPRO

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Rewired;
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
    using Text = TMPro.TMP_Text;
#else
    using Text = UnityEngine.UI.Text;
#endif

    [AddComponentMenu("")]
    public class InputRow : MonoBehaviour {
        
        public Text label;
        public ButtonInfo[] buttons { get; private set; }

        private int rowIndex;
        private System.Action<int, ButtonInfo> inputFieldActivatedCallback;

        public void Initialize(int rowIndex, string label, System.Action<int, ButtonInfo> inputFieldActivatedCallback) {
            this.rowIndex = rowIndex;
            this.label.text = label;
            this.inputFieldActivatedCallback = inputFieldActivatedCallback;
            buttons = transform.GetComponentsInChildren<ButtonInfo>(true);
        }

        public void OnButtonActivated(ButtonInfo buttonInfo) {
            if(inputFieldActivatedCallback == null) return;
            inputFieldActivatedCallback(rowIndex, buttonInfo);
        }
    }
}
