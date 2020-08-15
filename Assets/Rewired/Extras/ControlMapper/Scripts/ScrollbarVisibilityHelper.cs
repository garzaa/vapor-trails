// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_2024 || UNITY_2025
#define UNITY_2020_PLUS
#endif

#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif

#if UNITY_2018 || UNITY_2019_PLUS
#define UNITY_2018_PLUS
#endif

#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif

#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif

#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_1_PLUS
#endif

#if UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_2_PLUS
#endif

#if UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_3_PLUS
#endif

#if UNITY_5_4_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_4_PLUS
#endif

#if UNITY_5_5_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_5_PLUS
#endif

#if UNITY_5_6_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_6_PLUS
#endif

#if UNITY_5_7_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_7_PLUS
#endif

#if UNITY_5_8_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_8_PLUS
#endif

#if UNITY_5_9_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_9_PLUS
#endif

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_PLUS
#define SUPPORTS_UNITY_UI
#endif

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using Rewired;

    /// <summary>
    /// Hides ScrollRect scrollbars based on the dimensions of the content.
    /// This must be placed on the ScrollRect.content GameObject.
    /// This component is no longer necessary in Unity 5.2+ because ScrollRect now handles ScrollBar visibility.
    /// </summary>
    [AddComponentMenu("")]
    public class ScrollbarVisibilityHelper : MonoBehaviour {

        public ScrollRect scrollRect;

#if !UNITY_5_2_PLUS

        private Scrollbar hScrollBar { get { return scrollRect != null ? scrollRect.horizontalScrollbar : null; } }
        private Scrollbar vScrollBar { get { return scrollRect != null ? scrollRect.verticalScrollbar : null; } }

        // Used by component on ScrollRect that is used just for sending messages when its size changes
        private bool onlySendMessage;
        private ScrollbarVisibilityHelper target;

        void Awake() {
            // Add component on parent ScrollRect so we know when its dimensions change too
            if(scrollRect != null) {
                target = scrollRect.gameObject.AddComponent<ScrollbarVisibilityHelper>();
                target.onlySendMessage = true;
                target.target = this;
            }
        }

        void OnRectTransformDimensionsChange() {
            if(onlySendMessage) { // this is a parent ScrollRect, just send a message to target
                if(target != null) target.ScrollRectTransformDimensionsChanged();
            } else { // this is the component on the content game object, evaluate the scroll bars
                EvaluateScrollbar();
            }
        }

        private void ScrollRectTransformDimensionsChanged() {
            OnRectTransformDimensionsChange();
        }

        private void EvaluateScrollbar() {
            if(scrollRect == null) return;
            if(vScrollBar == null && hScrollBar == null) return;
            if(!gameObject.activeInHierarchy) return; // exit if not enabled or coroutine will fail

            Rect contentRect = scrollRect.content.rect;
            Rect visibleRect = (scrollRect.transform as RectTransform).rect;

            if(vScrollBar != null) {
                bool newState = contentRect.height <= visibleRect.height ? false : true;
                SetActiveDeferred(vScrollBar.gameObject, newState);
            }

            if(hScrollBar != null) {
                bool newState = contentRect.width <= visibleRect.width ? false : true;
                SetActiveDeferred(hScrollBar.gameObject, newState);
            }
        }

        private void SetActiveDeferred(GameObject obj, bool value) {
            StopAllCoroutines(); // clear all coroutines in case any were still pending -- this gets around an issue where the UI is being built and the enabled state runs several times in a row and coroutines playback in opposite order

            // Sets the active state after 1 frame because UI is being rebuilt when OnRectTransformDimensionsChange takes place and not allowed to set active at this time.
            StartCoroutine(SetActiveCoroutine(obj, value));
        }

        private IEnumerator SetActiveCoroutine(GameObject obj, bool value) {
            yield return null;
            if(obj != null) {
                obj.SetActive(value);
            }
        }
#endif
    }
}
