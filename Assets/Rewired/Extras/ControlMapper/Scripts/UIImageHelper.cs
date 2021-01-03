// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using Rewired;

    [AddComponentMenu("")]
    [RequireComponent(typeof(Image))]
    public class UIImageHelper : MonoBehaviour {

        [SerializeField]
        private State enabledState;
        [SerializeField]
        private State disabledState;

        private bool currentState;

        public void SetEnabledState(bool newState) {
            currentState = newState;
            State state = newState ? enabledState : disabledState;
            if(state == null) return;
            Image image = gameObject.GetComponent<Image>();
            if(image == null) {
                Debug.LogError("Image is missing!");
                return;
            }

            state.Set(image);
        }

        public void SetEnabledStateColor(Color color) {
            enabledState.color = color;
        }

        public void SetDisabledStateColor(Color color) {
            disabledState.color = color;
        }

        public void Refresh() {
            State state = currentState ? enabledState : disabledState;
            Image image = gameObject.GetComponent<Image>();
            if(image == null) return;
            state.Set(image);
        }

        [System.Serializable]
        private class State {

            [SerializeField]
            public Color color;

            public void Set(Image image) {
                if(image == null) return;
                image.color = color;
            }

        }
    }
}
