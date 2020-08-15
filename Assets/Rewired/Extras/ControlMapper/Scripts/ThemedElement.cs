// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Rewired;

    [AddComponentMenu("")]
    public class ThemedElement : MonoBehaviour {

        [SerializeField]
        private ElementInfo[] _elements;
        
        void Start() {
            ControlMapper.ApplyTheme(_elements);
        }

        [System.Serializable]
        public class ElementInfo {
            [SerializeField]
            private string _themeClass;
            [SerializeField]
            private Component _component;

            public string themeClass { get { return _themeClass; } }
            public Component component { get { return _component; } }
        }
    }
}
