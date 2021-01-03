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
    public class UIControl : MonoBehaviour {

        public Text title;

        private int _id;
        private bool _showTitle;

        public int id { get { return _id; } }


        void Awake() {
            _id = GetNextUid(); // assign a unique id to this control
        }

        public bool showTitle {
            get { return _showTitle; }
            set {
                if(title == null) return;
                title.gameObject.SetActive(value);
                _showTitle = value;
            }
        }

        public virtual void SetCancelCallback(System.Action cancelCallback) {
        }

        #region Static Members

        private static int _uidCounter;
        private static int GetNextUid() {
            if(_uidCounter == System.Int32.MaxValue) _uidCounter = 0;
            int current = _uidCounter;
            _uidCounter += 1;
            return current;
        }

        #endregion
    }
}
