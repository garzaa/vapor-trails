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
    [RequireComponent(typeof(Selectable))]
    public class ScrollRectSelectableChild : MonoBehaviour, ISelectHandler {

        public bool useCustomEdgePadding = false;
        public float customEdgePadding = 50.0f;

        private ScrollRect parentScrollRect;
        private RectTransform parentScrollRectContentTransform { get { return parentScrollRect.content; } }

        private Selectable _selectable;
        private Selectable selectable { get { return _selectable ?? (_selectable = GetComponent<Selectable>()); } }

        private RectTransform rectTransform { get { return transform as RectTransform; } }

        void Start() {
            parentScrollRect = transform.GetComponentInParent<ScrollRect>();
            if(parentScrollRect == null) {
                Debug.LogError("Rewired Control Mapper: No ScrollRect found! This component must be a child of a ScrollRect!");
                return;
            }
        }

        #region ISelectHandlder Implementation

        public void OnSelect(BaseEventData eventData) {
            if(parentScrollRect == null) return;

            // Check if this is a keyboard/joystick move event -- mouse click events are excluded otherwise clicks will miss when scroll rect is moved
            AxisEventData aed = eventData as AxisEventData;
            if(aed == null) return;

            // Evaluate object size and position and determine if visible within the scroll rect
            
            RectTransform parentScrollRectTransform = parentScrollRect.transform as RectTransform;

            // Get the selectable rect relative to the view rect
            Rect relSelectableRect = Rewired.Utils.MathTools.TransformRect(rectTransform.rect, rectTransform, parentScrollRectTransform);
            
            // Get the view rect
            Rect viewRect = parentScrollRectTransform.rect;

            // Reduce the size of the visible rect so we stay a bit away from the edge and still scroll to the next item
            Rect paddedViewRect = parentScrollRectTransform.rect;
            float yPad;
            // float xPad;
            if(useCustomEdgePadding) {
                yPad = customEdgePadding;
            } else {
                yPad = relSelectableRect.height; // scroll when we are 1 selectable unit from edge
                //xPad = relSelectableRect.width; // scroll when we are 1 selectable unit from edge
            }
            paddedViewRect.yMax -= yPad;
            paddedViewRect.yMin += yPad;
            // Do not do the horizontal padding because this causes problems with 1-column scroll views. No need for it in this usage.
            //paddedViewRect.xMax -= xPad;
            //paddedViewRect.xMin += xPad;

            // Check if selectable is not fully inside the padded visible rect area
            if(Rewired.Utils.MathTools.RectContains(paddedViewRect, relSelectableRect)) { // rect is visible
                return;
            }

            // Offset scroll rect to fit selectable
            Vector2 offset;
            if(!Rewired.Utils.MathTools.GetOffsetToContainRect(paddedViewRect, relSelectableRect, out offset)) { // get the view area offet required to fit this element
                return; // selectable cannot fit within view rect
            }

            // Offset the view rect to be within valid area
            Vector2 newPos = parentScrollRectContentTransform.anchoredPosition;
            newPos.x = Mathf.Clamp(newPos.x + offset.x, 0.0f, Mathf.Abs(viewRect.width - parentScrollRectContentTransform.sizeDelta.x)); // prevent overscroll
            newPos.y = Mathf.Clamp(newPos.y + offset.y, 0.0f, Mathf.Abs(viewRect.height - parentScrollRectContentTransform.sizeDelta.y)); // prevent overscroll

            // Set the new scroll rect position
            parentScrollRectContentTransform.anchoredPosition = newPos;
        }

        #endregion

    }
}
