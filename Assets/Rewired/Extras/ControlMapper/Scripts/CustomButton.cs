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

    /// <summary>
    /// Overrides auto-navigation in Selectable because it's inadequate for selectables inside a scroll rect
    /// Also enables selection of disabled controls for better navigation experience.
    /// </summary>
    [AddComponentMenu("")]
    public class CustomButton : Button, ICustomSelectable {

        [SerializeField]
        private Sprite _disabledHighlightedSprite;
        [SerializeField]
        private Color _disabledHighlightedColor;
        [SerializeField]
        private string _disabledHighlightedTrigger;

        [SerializeField]
        private bool _autoNavUp = true;
        [SerializeField]
        private bool _autoNavDown = true;
        [SerializeField]
        private bool _autoNavLeft = true;
        [SerializeField]
        private bool _autoNavRight = true;

        public Sprite disabledHighlightedSprite { get { return _disabledHighlightedSprite; } set { _disabledHighlightedSprite = value; } }
        public Color disabledHighlightedColor { get { return _disabledHighlightedColor; } set { _disabledHighlightedColor = value; } }
        public string disabledHighlightedTrigger { get { return _disabledHighlightedTrigger; } set { _disabledHighlightedTrigger = value; } }

        public bool autoNavUp { get { return _autoNavUp; } set { _autoNavUp = value; } }
        public bool autoNavDown { get { return _autoNavDown; } set { _autoNavDown = value; } }
        public bool autoNavLeft { get { return _autoNavLeft; } set { _autoNavLeft = value; } }
        public bool autoNavRight { get { return _autoNavRight; } set { _autoNavRight = value; } }

        private bool isDisabled { get { return !IsInteractable(); } }
        private bool isHighlightDisabled;

        // Events

        private event UnityAction _CancelEvent;
        public event UnityAction CancelEvent { add { _CancelEvent += value; } remove { _CancelEvent -= value; } }

        #region Selectable Overrides

        public override Selectable FindSelectableOnLeft() {
            if((navigation.mode & Navigation.Mode.Horizontal) != 0 || _autoNavLeft) {
                return UISelectionUtility.FindNextSelectable(this, transform, Vector3.left);
            }
            return base.FindSelectableOnLeft();
        }

        public override Selectable FindSelectableOnRight() {
            if((navigation.mode & Navigation.Mode.Horizontal) != 0 || _autoNavRight) {
                return UISelectionUtility.FindNextSelectable(this, transform, Vector3.right);
            }
            return base.FindSelectableOnRight();
        }

        public override Selectable FindSelectableOnUp() {
            if((navigation.mode & Navigation.Mode.Vertical) != 0 || _autoNavUp) {
                return UISelectionUtility.FindNextSelectable(this, transform, Vector3.up);
            }
            return base.FindSelectableOnUp();
        }

        public override Selectable FindSelectableOnDown() {
            if((navigation.mode & Navigation.Mode.Vertical) != 0 || _autoNavDown) {
                return UISelectionUtility.FindNextSelectable(this, transform, Vector3.down);
            }
            return base.FindSelectableOnDown();
        }

        protected override void OnCanvasGroupChanged() {
            base.OnCanvasGroupChanged();

            if(EventSystem.current == null) return;

            // Handle highlight-disabled state transition
            EvaluateHightlightDisabled(EventSystem.current.currentSelectedGameObject == gameObject);
        }

        protected override void DoStateTransition(SelectionState state, bool instant) {

            if(isHighlightDisabled) {

                Color tintColor = _disabledHighlightedColor;
                Sprite transitionSprite = _disabledHighlightedSprite;
                string triggerName = _disabledHighlightedTrigger;

                if(gameObject.activeInHierarchy) {
                    switch(this.transition) {
                        case Transition.ColorTint:
                            StartColorTween(tintColor * colors.colorMultiplier, instant);
                            break;
                        case Transition.SpriteSwap:
                            DoSpriteSwap(transitionSprite);
                            break;
                        case Transition.Animation:
                            TriggerAnimation(triggerName);
                            break;
                    }
                }

            } else {
                base.DoStateTransition(state, instant);
            }
        }

        void StartColorTween(Color targetColor, bool instant) {
            if(targetGraphic == null)
                return;

            targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
        }

        void DoSpriteSwap(Sprite newSprite) {
            if(image == null)
                return;

            image.overrideSprite = newSprite;
        }

        void TriggerAnimation(string triggername) {
#if UNITY_4_6 && (UNITY_4_6_0 || UNITY_4_6_1 || UNITY_4_6_2)
            if(animator == null || !animator.enabled || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
                return;
#else
            if(animator == null || !animator.enabled || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
                return;
#endif

            animator.ResetTrigger(_disabledHighlightedTrigger);
            animator.SetTrigger(triggername);
        }

        public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);

            // Handle highlight-disabled state transition
            EvaluateHightlightDisabled(true);
        }

        public override void OnDeselect(BaseEventData eventData) {
            base.OnDeselect(eventData);

            // Handle highlight-disabled state transition
            EvaluateHightlightDisabled(false);
        }

        #endregion

        #region Button Overrides

        private void Press() {
            if(!IsActive() || !IsInteractable())
                return;

            onClick.Invoke();
        }

        // Trigger all registered callbacks.
        public override void OnPointerClick(PointerEventData eventData) {
            if(!IsActive() || !IsInteractable()) return; // ignore click entirely if button is already disabled

            if(eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();

            // Transition to highlight-disabled state if disabled
            if(!IsActive() || !IsInteractable()) {
                isHighlightDisabled = true;
                DoStateTransition(SelectionState.Disabled, false);
            }
        }

        public override void OnSubmit(BaseEventData eventData) {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if(!IsActive() || !IsInteractable()) {
                // Transition to highlight-disabled state
                isHighlightDisabled = true;
                DoStateTransition(SelectionState.Disabled, false);
                return;
            }

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit() {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while(elapsedTime < fadeTime) {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        #endregion

        private void EvaluateHightlightDisabled(bool isSelected) {
            
            if(!isSelected) { // Deselection
                
                if(isHighlightDisabled) {
                    isHighlightDisabled = false;
                    SelectionState state = isDisabled ? SelectionState.Disabled : currentSelectionState;
                    DoStateTransition(state, false);
                }

            } else { // Selection
                
                if(!isDisabled) return;
                isHighlightDisabled = true;
                DoStateTransition(SelectionState.Disabled, false);
            }
        }

        #region ICancelHandler Implementation

        public void OnCancel(BaseEventData eventData) {
            if(_CancelEvent != null) _CancelEvent();
        }

        #endregion
    }
}
