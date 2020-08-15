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
    public class InputBehaviorWindow : Window {

        private const float minSensitivity = 0.1f;

        [SerializeField]
        private RectTransform spawnTransform;
        [SerializeField]
        private Button doneButton;
        [SerializeField]
        private Button cancelButton;
        [SerializeField]
        private Button defaultButton;
        [SerializeField]
        private Text doneButtonLabel;
        [SerializeField]
        private Text cancelButtonLabel;
        [SerializeField]
        private Text defaultButtonLabel;

        [SerializeField]
        private GameObject uiControlSetPrefab;
        [SerializeField]
        private GameObject uiSliderControlPrefab;

        private List<InputBehaviorInfo> inputBehaviorInfo;

        private Dictionary<int, System.Action<int>> buttonCallbacks;

        private int playerId;

        public override void Initialize(int id, System.Func<int, bool> isFocusedCallback) {
            if(
                spawnTransform == null ||
                doneButton == null ||
                cancelButton == null ||
                defaultButton == null ||
                uiControlSetPrefab == null ||
                uiSliderControlPrefab == null ||
                doneButtonLabel == null ||
                cancelButtonLabel == null ||
                defaultButtonLabel == null
            ) {
                Debug.LogError("Rewired Control Mapper: All inspector values must be assigned!");
                return;
            }

            inputBehaviorInfo = new List<InputBehaviorInfo>();
            buttonCallbacks = new Dictionary<int, System.Action<int>>();

            // Set static element labels
            doneButtonLabel.text = ControlMapper.GetLanguage().done;
            cancelButtonLabel.text = ControlMapper.GetLanguage().cancel;
            defaultButtonLabel.text = ControlMapper.GetLanguage().default_;

            base.Initialize(id, isFocusedCallback);
        }

        public void SetData(int playerId, ControlMapper.InputBehaviorSettings[] data) {
            if(!initialized) return;

            this.playerId = playerId;

            // Create control sets
            for(int i = 0; i < data.Length; i++) {
                var item = data[i];
                if(item == null || !item.isValid) continue;

                InputBehavior inputBehavior = GetInputBehavior(item.inputBehaviorId);
                if(inputBehavior == null) continue;

                UIControlSet set = CreateControlSet();
                Dictionary<int, PropertyType> idToProperty = new Dictionary<int, PropertyType>(); ;

                // Set the title
                string customTitle = ControlMapper.GetLanguage().GetCustomEntry(item.labelLanguageKey);
                if(!string.IsNullOrEmpty(customTitle)) set.SetTitle(customTitle);
                else set.SetTitle(inputBehavior.name);

                // Create sliders

                if(item.showJoystickAxisSensitivity) {
                    UISliderControl slider = CreateSlider(
                        set,
                        inputBehavior.id,
                        null,
                        ControlMapper.GetLanguage().GetCustomEntry(item.joystickAxisSensitivityLabelLanguageKey),
                        item.joystickAxisSensitivityIcon,
                        item.joystickAxisSensitivityMin,
                        item.joystickAxisSensitivityMax,
                        JoystickAxisSensitivityValueChanged,
                        JoystickAxisSensitivityCanceled
                    );
                    // Set initial value
                    slider.slider.value = Mathf.Clamp(inputBehavior.joystickAxisSensitivity, item.joystickAxisSensitivityMin, item.joystickAxisSensitivityMax);
                    // Store property type
                    idToProperty.Add(slider.id, PropertyType.JoystickAxisSensitivity);
                }

                if(item.showMouseXYAxisSensitivity) {
                    UISliderControl slider = CreateSlider(
                        set,
                        inputBehavior.id,
                        null,
                        ControlMapper.GetLanguage().GetCustomEntry(item.mouseXYAxisSensitivityLabelLanguageKey),
                        item.mouseXYAxisSensitivityIcon,
                        item.mouseXYAxisSensitivityMin,
                        item.mouseXYAxisSensitivityMax,
                        MouseXYAxisSensitivityValueChanged,
                        MouseXYAxisSensitivityCanceled
                    );
                    // Set initial value
                    slider.slider.value = Mathf.Clamp(inputBehavior.mouseXYAxisSensitivity, item.mouseXYAxisSensitivityMin, item.mouseXYAxisSensitivityMax);
                    // Store property type
                    idToProperty.Add(slider.id, PropertyType.MouseXYAxisSensitivity);
                }

                // mouseOtherAxisSensitivity not implemented

                // Add to the list
                inputBehaviorInfo.Add(new InputBehaviorInfo(inputBehavior, set, idToProperty));
            }

            // Set default UI element
            defaultUIElement = doneButton.gameObject;
        }

        public void SetButtonCallback(ButtonIdentifier buttonIdentifier, System.Action<int> callback) {
            if(!initialized) return;
            if(callback == null) return;
            if(buttonCallbacks.ContainsKey((int)buttonIdentifier)) buttonCallbacks[(int)buttonIdentifier] = callback;
            else buttonCallbacks.Add((int)buttonIdentifier, callback);
        }

        public override void Cancel() {
            if(!initialized) return;
            // don't call on base

            // Restore original data to input behaviors
            foreach(InputBehaviorInfo info in inputBehaviorInfo) {
                info.RestorePreviousData();
            }
        
            System.Action<int> callback;
            if(!buttonCallbacks.TryGetValue((int)ButtonIdentifier.Cancel, out callback)) {
                if(cancelCallback != null) cancelCallback();
                return;
            }
            callback(id);
        }

        #region Window Button Event Handlers

        public void OnDone() {
            if(!initialized) return;
            System.Action<int> callback;
            if(!buttonCallbacks.TryGetValue((int)ButtonIdentifier.Done, out callback)) return;
            callback(id);
        }

        public void OnCancel() {
            Cancel();
        }

        public void OnRestoreDefault() {
            if(!initialized) return;

            // Revert to default settings in each input behavior
            foreach(InputBehaviorInfo info in inputBehaviorInfo) {
                info.RestoreDefaultData();
            }
        }

        #endregion

        #region Slider Control Event Handlers

        private void JoystickAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value) {
            GetInputBehavior(inputBehaviorId).joystickAxisSensitivity = value;
        }

        private void MouseXYAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value) {
            GetInputBehavior(inputBehaviorId).mouseXYAxisSensitivity = value;
        }

        private void JoystickAxisSensitivityCanceled(int inputBehaviorId, int controlId) {
            InputBehaviorInfo info = GetInputBehaviorInfo(inputBehaviorId);
            if(info == null) return;
            info.RestoreData(PropertyType.JoystickAxisSensitivity, controlId);
        }

        private void MouseXYAxisSensitivityCanceled(int inputBehaviorId, int controlId) {
            InputBehaviorInfo info = GetInputBehaviorInfo(inputBehaviorId);
            if(info == null) return;
            info.RestoreData(PropertyType.MouseXYAxisSensitivity, controlId);
        }

        #endregion

        public override void TakeInputFocus() {
            base.TakeInputFocus();
        }

        private UIControlSet CreateControlSet() {
            GameObject instance = (GameObject)Object.Instantiate(uiControlSetPrefab);
            instance.transform.SetParent(spawnTransform, false);
            return instance.GetComponent<UIControlSet>();
        }

        private UISliderControl CreateSlider(UIControlSet set, int inputBehaviorId, string defaultTitle, string overrideTitle, Sprite icon, float minValue, float maxValue, System.Action<int, int, float> valueChangedCallback, System.Action<int, int> cancelCallback) {
            // Create slider control
            UISliderControl control = set.CreateSlider(
                uiSliderControlPrefab,
                icon,
                minValue,
                maxValue,
                (int cId, float value) => { valueChangedCallback(inputBehaviorId, cId, value); },
                (int cId) => { cancelCallback(inputBehaviorId, cId); }
            );

            // Title
            string title = string.IsNullOrEmpty(overrideTitle) ? defaultTitle : overrideTitle;
            if(!string.IsNullOrEmpty(title)) {
                control.showTitle = true;
                control.title.text = title;
            } else {
                control.showTitle = false;
            }

            // Icon
            control.showIcon = icon != null;

            return control;
        }

        private InputBehavior GetInputBehavior(int id) {
            return ReInput.mapping.GetInputBehavior(playerId, id);
        }

        private InputBehaviorInfo GetInputBehaviorInfo(int inputBehaviorId) {
            int count = inputBehaviorInfo.Count;
            for(int i = 0; i < count; i++) {
                if(inputBehaviorInfo[i].inputBehavior.id != inputBehaviorId) continue;
                return inputBehaviorInfo[i];
            }
            return null;
        }

        // Classes

        private class InputBehaviorInfo {
            private InputBehavior _inputBehavior; 
            private UIControlSet _controlSet;
            private Dictionary<int, PropertyType> idToProperty;

            public InputBehavior inputBehavior { get { return _inputBehavior; } }
            public UIControlSet controlSet { get { return _controlSet; } }

            private InputBehavior copyOfOriginal;

            public InputBehaviorInfo(InputBehavior inputBehavior, UIControlSet controlSet, Dictionary<int, PropertyType> idToProperty) {
                this._inputBehavior = inputBehavior;
                this._controlSet = controlSet;
                this.idToProperty = idToProperty;
                copyOfOriginal = new InputBehavior(inputBehavior);
            }

            public void RestorePreviousData() {
                _inputBehavior.ImportData(copyOfOriginal);
            }

            public void RestoreDefaultData() {
                _inputBehavior.Reset();
                RefreshControls();
            }

            public void RestoreData(PropertyType propertyType, int controlId) {
                
                switch(propertyType) {
                    case PropertyType.JoystickAxisSensitivity: {
                        float value = copyOfOriginal.joystickAxisSensitivity;
                        _inputBehavior.joystickAxisSensitivity = value;
                        UISliderControl control = _controlSet.GetControl<UISliderControl>(controlId);
                        if(control != null) control.slider.value = value; // update control
                        break;
                    } case PropertyType.MouseXYAxisSensitivity: {
                        float value = copyOfOriginal.mouseXYAxisSensitivity;
                        _inputBehavior.mouseXYAxisSensitivity = value;
                        UISliderControl control = _controlSet.GetControl<UISliderControl>(controlId);
                        if(control != null) control.slider.value = value; // update control
                        break;
                    }
                }
            }

            public void RefreshControls() {
                if(_controlSet == null) return;
                if(idToProperty == null) return;

                // Redraw the controls with the current values

                foreach(KeyValuePair<int, PropertyType> pair in idToProperty) {
                    UISliderControl control = _controlSet.GetControl<UISliderControl>(pair.Key);
                    if(control == null) continue;
                    switch(pair.Value) {
                        case PropertyType.JoystickAxisSensitivity:
                            control.slider.value = _inputBehavior.joystickAxisSensitivity;
                            break;
                        case PropertyType.MouseXYAxisSensitivity:
                            control.slider.value = _inputBehavior.mouseXYAxisSensitivity;
                            break;
                    }
                }
            }
        }

        // Enums

        public enum ButtonIdentifier {
            Done,
            Cancel,
            Default
        }

        private enum PropertyType {
            JoystickAxisSensitivity = 0,
            MouseXYAxisSensitivity = 1
        }
    }
}
