// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Demos {

    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections;

    [AddComponentMenu("")]
    public class ControlMapperDemoMessage : MonoBehaviour {

        public Rewired.UI.ControlMapper.ControlMapper controlMapper;

        public UnityEngine.UI.Selectable defaultSelectable;

        void Awake() {
            if(controlMapper != null) {
                controlMapper.ScreenClosedEvent += OnControlMapperClosed;
                controlMapper.ScreenOpenedEvent += OnControlMapperOpened;
            }
        }

        void Start() {
            SelectDefault();
        }

        void OnControlMapperClosed() {
            this.gameObject.SetActive(true);
            StartCoroutine(SelectDefaultDeferred());
        }

        void OnControlMapperOpened() {
            this.gameObject.SetActive(false);
        }

        void SelectDefault() {
            if(EventSystem.current == null) return;
            if(defaultSelectable != null) EventSystem.current.SetSelectedGameObject(defaultSelectable.gameObject);
        }

        IEnumerator SelectDefaultDeferred() {
            yield return null;
            SelectDefault();
        }
    }
}