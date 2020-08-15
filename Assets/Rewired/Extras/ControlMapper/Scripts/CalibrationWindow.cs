// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

//#define REWIRED_CONTROL_MAPPER_USE_TMPRO

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
    using Rewired.Utils;
    using Rewired.Integration.UnityUI;
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
    using Text = TMPro.TMP_Text;
#else
    using Text = UnityEngine.UI.Text;
#endif

    [AddComponentMenu("")]
    public class CalibrationWindow : Window {

        private const float minSensitivityOtherAxes = 0.1f; // used for non-menu axes, min value to prevent axis from becoming useless
        private const float maxDeadzone = 0.8f; // max dead zone value user is allowed to set to prevent full axis from becoming useless

        [SerializeField]
        private RectTransform rightContentContainer;
        [SerializeField]
        private RectTransform valueDisplayGroup;
        [SerializeField]
        private RectTransform calibratedValueMarker;
        [SerializeField]
        private RectTransform rawValueMarker;
        [SerializeField]
        private RectTransform calibratedZeroMarker;
        [SerializeField]
        private RectTransform deadzoneArea;
        [SerializeField]
        private Slider deadzoneSlider;
        [SerializeField]
        private Slider zeroSlider;
        [SerializeField]
        private Slider sensitivitySlider;
        [SerializeField]
        private Toggle invertToggle;
        [SerializeField]
        private RectTransform axisScrollAreaContent;
        [SerializeField]
        private Button doneButton;
        [SerializeField]
        private Button calibrateButton;
        [SerializeField]
        private Text doneButtonLabel;
        [SerializeField]
        private Text cancelButtonLabel;
        [SerializeField]
        private Text defaultButtonLabel;
        [SerializeField]
        private Text deadzoneSliderLabel;
        [SerializeField]
        private Text zeroSliderLabel;
        [SerializeField]
        private Text sensitivitySliderLabel;
        [SerializeField]
        private Text invertToggleLabel;
        [SerializeField]
        private Text calibrateButtonLabel;

        [SerializeField]
        private GameObject axisButtonPrefab;

        private Joystick joystick;

        private string origCalibrationData;
        private int selectedAxis = -1;
        private AxisCalibrationData origSelectedAxisCalibrationData;
        private float displayAreaWidth;
        private List<Button> axisButtons;

        private bool axisSelected {
            get {
                if (joystick == null) return false;
                if (selectedAxis < 0 || selectedAxis >= joystick.calibrationMap.axisCount) return false;
                return true;
            }
        }
        private AxisCalibration axisCalibration {
            get {
                if (!axisSelected) return null;
                return joystick.calibrationMap.GetAxis(selectedAxis);
            }
        }
        private Dictionary<int, System.Action<int>> buttonCallbacks;

        private int playerId;
        private RewiredStandaloneInputModule rewiredStandaloneInputModule;
        private int menuHorizActionId = -1;
        private int menuVertActionId = -1;

        /// <summary>
        /// This value prevents the user from lowering the axis sensitivity so far that the axis is unusable.
        /// This is very important for the axes used for menu navigation, otherwise the user could permanently
        /// disable their ability to navigate the menu on a console with no mouse/keyboard.
        /// This value will be drawn from the InputBehavior(s) assigned to the menu horizontal and vertical axes
        /// </summary>
        private float minSensitivity;


        public override void Initialize(int id, System.Func<int, bool> isFocusedCallback) {
            if (
                rightContentContainer == null ||
                valueDisplayGroup == null ||
                calibratedValueMarker == null ||
                rawValueMarker == null ||
                calibratedZeroMarker == null ||
                deadzoneArea == null ||
                deadzoneSlider == null ||
                sensitivitySlider == null ||
                zeroSlider == null ||
                invertToggle == null ||
                axisScrollAreaContent == null ||
                doneButton == null ||
                calibrateButton == null ||
                axisButtonPrefab == null ||
                doneButtonLabel == null ||
                cancelButtonLabel == null ||
                defaultButtonLabel == null ||
                deadzoneSliderLabel == null ||
                zeroSliderLabel == null ||
                sensitivitySliderLabel == null ||
                invertToggleLabel == null ||
                calibrateButtonLabel == null
            ) {
                Debug.LogError("Rewired Control Mapper: All inspector values must be assigned!");
                return;
            }

            axisButtons = new List<Button>();
            buttonCallbacks = new Dictionary<int, System.Action<int>>();

            // Set static element labels
            doneButtonLabel.text = ControlMapper.GetLanguage().done;
            cancelButtonLabel.text = ControlMapper.GetLanguage().cancel;
            defaultButtonLabel.text = ControlMapper.GetLanguage().default_;
            deadzoneSliderLabel.text = ControlMapper.GetLanguage().calibrateWindow_deadZoneSliderLabel;
            zeroSliderLabel.text = ControlMapper.GetLanguage().calibrateWindow_zeroSliderLabel;
            sensitivitySliderLabel.text = ControlMapper.GetLanguage().calibrateWindow_sensitivitySliderLabel;
            invertToggleLabel.text = ControlMapper.GetLanguage().calibrateWindow_invertToggleLabel;
            calibrateButtonLabel.text = ControlMapper.GetLanguage().calibrateWindow_calibrateButtonLabel;

            base.Initialize(id, isFocusedCallback);
        }

        public void SetJoystick(int playerId, Joystick joystick) {
            if (!initialized) return;

            this.playerId = playerId;
            this.joystick = joystick;

            if (joystick == null) {
                Debug.LogError("Rewired Control Mapper: Joystick cannot be null!");
                return;
            }

            // Create axis list
            float buttonHeight = 0.0f;
            for (int i = 0; i < joystick.axisCount; i++) {
                int index = i;
                GameObject instance = UITools.InstantiateGUIObject<Button>(axisButtonPrefab, axisScrollAreaContent, "Axis" + i);
                Button button = instance.GetComponent<Button>();
                button.onClick.AddListener(() => { OnAxisSelected(index, button); });
                Text text = UnityTools.GetComponentInSelfOrChildren<Text>(instance);
                if (text != null) text.text = ControlMapper.GetLanguage().GetElementIdentifierName(joystick, joystick.AxisElementIdentifiers[i].id, AxisRange.Full);
                if (buttonHeight == 0.0f) buttonHeight = UnityTools.GetComponentInSelfOrChildren<LayoutElement>(instance).minHeight;
                axisButtons.Add(button);
            }

            // set axis list height
            float vSpacing = axisScrollAreaContent.GetComponent<VerticalLayoutGroup>().spacing;
            axisScrollAreaContent.sizeDelta = new Vector2(axisScrollAreaContent.sizeDelta.x, Mathf.Max((joystick.axisCount * (buttonHeight + vSpacing) - vSpacing), axisScrollAreaContent.sizeDelta.y));

            // Store the original calibration data so we can revert
            origCalibrationData = joystick.calibrationMap.ToXmlString();

            // Record info
            displayAreaWidth = rightContentContainer.sizeDelta.x;

            // Try to get the UI control axis deadzone from the RewiredStandaloneInputModule if it exists in the hierarchy
            // This is used to prevent users from rendering menu navigation axes unusable by changing the axis sensitivity
            rewiredStandaloneInputModule = gameObject.transform.root.GetComponentInChildren<RewiredStandaloneInputModule>();
            if (rewiredStandaloneInputModule != null) {
                menuHorizActionId = ReInput.mapping.GetActionId(rewiredStandaloneInputModule.horizontalAxis);
                menuVertActionId = ReInput.mapping.GetActionId(rewiredStandaloneInputModule.verticalAxis);
            }

            // Select first axis
            if (joystick.axisCount > 0) {
                SelectAxis(0);
            }

            // Set default UI element
            defaultUIElement = doneButton.gameObject;

            // Draw window
            RefreshControls();
            Redraw();
        }

        public void SetButtonCallback(ButtonIdentifier buttonIdentifier, System.Action<int> callback) {
            if (!initialized) return;
            if (callback == null) return;
            if (buttonCallbacks.ContainsKey((int)buttonIdentifier)) buttonCallbacks[(int)buttonIdentifier] = callback;
            else buttonCallbacks.Add((int)buttonIdentifier, callback);
        }

        public override void Cancel() {
            if (!initialized) return;
            // don't call on base
            if (joystick != null) joystick.ImportCalibrationMapFromXmlString(origCalibrationData); // restore old data
            System.Action<int> callback;
            if (!buttonCallbacks.TryGetValue((int)ButtonIdentifier.Cancel, out callback)) {
                if (cancelCallback != null) cancelCallback();
                return;
            }
            callback(id);
        }

        protected override void Update() {
            if (!initialized) return;

            base.Update();

            UpdateDisplay(); // update the real-time display
        }

        #region Control Event Handlers

        public void OnDone() {
            if (!initialized) return;
            System.Action<int> callback;
            if (!buttonCallbacks.TryGetValue((int)ButtonIdentifier.Done, out callback)) return;
            callback(id);
        }

        public void OnCancel() {
            Cancel();
        }

        public void OnRestoreDefault() {
            if (!initialized) return;
            if (joystick == null) return;
            joystick.calibrationMap.Reset();
            RefreshControls();
            Redraw();
        }

        public void OnCalibrate() {
            if (!initialized) return;
            System.Action<int> callback;
            if (!buttonCallbacks.TryGetValue((int)ButtonIdentifier.Calibrate, out callback)) return;
            callback(selectedAxis);
        }

        public void OnInvert(bool state) {
            if (!initialized) return;
            if (!axisSelected) return;
            axisCalibration.invert = state;
        }

        public void OnZeroValueChange(float value) {
            if (!initialized) return;
            if (!axisSelected) return;
            axisCalibration.calibratedZero = value;
            RedrawCalibratedZero();
        }

        public void OnZeroCancel() {
            if (!initialized) return;
            if (!axisSelected) return;
            axisCalibration.calibratedZero = origSelectedAxisCalibrationData.zero;
            RedrawCalibratedZero();
            RefreshControls();
        }

        public void OnDeadzoneValueChange(float value) {
            if (!initialized) return;
            if (!axisSelected) return;
            // Enforce a max dead zone to prevent axis from becoming useless
            axisCalibration.deadZone = Mathf.Clamp(value, 0.0f, maxDeadzone);
            if (value > maxDeadzone) deadzoneSlider.value = maxDeadzone; // prevent control from going outside range
            RedrawDeadzone();
        }

        public void OnDeadzoneCancel() {
            if (!initialized) return;
            if (!axisSelected) return;
            axisCalibration.deadZone = origSelectedAxisCalibrationData.deadZone;
            RedrawDeadzone();
            RefreshControls();
        }

        public void OnSensitivityValueChange(float value) {
            if (!initialized) return;
            if (!axisSelected) return;
            SetSensitivity(axisCalibration, value);
        }

        public void OnSensitivityCancel(float value) {
            if (!initialized) return;
            if (!axisSelected) return;
            axisCalibration.sensitivity = origSelectedAxisCalibrationData.sensitivity;
            RefreshControls();
        }

        public void OnAxisScrollRectScroll(Vector2 pos) {
            if (!initialized) return;
        }

        private void OnAxisSelected(int axisIndex, Button button) {
            if (!initialized) return;
            if (joystick == null) return;
            SelectAxis(axisIndex);
            RefreshControls();
            Redraw();
        }

        #endregion

        private void UpdateDisplay() {
            RedrawValueMarkers();
        }

        private void Redraw() {
            RedrawCalibratedZero(); // also updates deadzone
            RedrawValueMarkers();
        }

        private void RefreshControls() {
            if (!axisSelected) {
                // Deadzone slider
                deadzoneSlider.value = 0;

                // Zero slider
                zeroSlider.value = 0;

                // Sensitivity slider
                sensitivitySlider.value = 0;

                // Invert toggle
                invertToggle.isOn = false;
            } else {
                // Deadzone slider
                deadzoneSlider.value = axisCalibration.deadZone;

                // Zero slider
                zeroSlider.value = axisCalibration.calibratedZero;

                // Sensitivity slider
                sensitivitySlider.value = GetSliderSensitivity(axisCalibration);

                // Invert toggle
                invertToggle.isOn = axisCalibration.invert;
            }
        }

        private void RedrawDeadzone() {
            if (!axisSelected) return;
            float width = displayAreaWidth * axisCalibration.deadZone;
            deadzoneArea.sizeDelta = new Vector2(width, deadzoneArea.sizeDelta.y);
            deadzoneArea.anchoredPosition = new Vector2(axisCalibration.calibratedZero * -deadzoneArea.parent.localPosition.x, deadzoneArea.anchoredPosition.y);
        }

        private void RedrawCalibratedZero() {
            if (!axisSelected) return;
            calibratedZeroMarker.anchoredPosition = new Vector2(axisCalibration.calibratedZero * -deadzoneArea.parent.localPosition.x, calibratedZeroMarker.anchoredPosition.y);
            RedrawDeadzone();
        }

        private void RedrawValueMarkers() {
            if (!axisSelected) {
                calibratedValueMarker.anchoredPosition = new Vector2(0, calibratedValueMarker.anchoredPosition.y);
                rawValueMarker.anchoredPosition = new Vector2(0, rawValueMarker.anchoredPosition.y);
                return;
            }

            float value = joystick.GetAxis(selectedAxis);
            float rawValue = Mathf.Clamp(joystick.GetAxisRaw(selectedAxis), -1.0f, 1.0f);

            calibratedValueMarker.anchoredPosition = new Vector2(displayAreaWidth * 0.5f * value, calibratedValueMarker.anchoredPosition.y);
            rawValueMarker.anchoredPosition = new Vector2(displayAreaWidth * 0.5f * rawValue, rawValueMarker.anchoredPosition.y);
        }

        private void SelectAxis(int index) {
            if (index < 0 || index >= axisButtons.Count) return;
            if (axisButtons[index] == null) return;
            axisButtons[index].interactable = false; // disable this axis
#if UNITY_5_3_PLUS
            // Unity changed the system so when interactible is set to false,
            // the Selectable is immediately deselected.
            axisButtons[index].Select(); // force select after Unity deselects it
#endif
            // Enable other axes
            for (int i = 0; i < axisButtons.Count; i++) {
                if (i == index) continue;
                axisButtons[i].interactable = true;
            }
            selectedAxis = index;
            origSelectedAxisCalibrationData = axisCalibration.GetData();
            SetMinSensitivity();
        }

        public override void TakeInputFocus() {
            base.TakeInputFocus();
            if (selectedAxis >= 0) SelectAxis(selectedAxis); // refresh the axis selection so button interactivity matches
            RefreshControls();
            Redraw();
        }

        private void SetMinSensitivity() {
            if (!axisSelected) return;

            minSensitivity = minSensitivityOtherAxes;

            // Set the minimum sensitivity for this axis
            if (rewiredStandaloneInputModule != null) {
                if (IsMenuAxis(menuHorizActionId, selectedAxis)) {
                    GetAxisButtonDeadZone(playerId, menuHorizActionId, ref minSensitivity);
                } else if (IsMenuAxis(menuVertActionId, selectedAxis)) {
                    GetAxisButtonDeadZone(playerId, menuVertActionId, ref minSensitivity);
                }
            }
        }

        private bool IsMenuAxis(int actionId, int axisIndex) {
            if (rewiredStandaloneInputModule == null) return false;

            // Determine if menu action is mapped to this axis on any player
            IList<Player> players = ReInput.players.AllPlayers;
            int playerCount = players.Count;
            for (int i = 0; i < playerCount; i++) {
                IList<JoystickMap> maps = players[i].controllers.maps.GetMaps<JoystickMap>(joystick.id);
                if (maps == null) continue;
                int mapCount = maps.Count;
                for (int j = 0; j < mapCount; j++) {
                    IList<ActionElementMap> aems = maps[j].AxisMaps;
                    if (aems == null) continue;
                    int aemCount = aems.Count;
                    for (int k = 0; k < aemCount; k++) {
                        ActionElementMap aem = aems[k];
                        if (aem.actionId == actionId && aem.elementIndex == axisIndex) return true;
                    }
                }
            }
            return false;
        }

        private void GetAxisButtonDeadZone(int playerId, int actionId, ref float value) {
            InputAction action = ReInput.mapping.GetAction(actionId);
            if (action == null) return;

            int behaviorId = action.behaviorId;
            InputBehavior inputBehavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
            if (inputBehavior == null) return;

            value = inputBehavior.buttonDeadZone + 0.1f; // add a small amount so it never reaches the deadzone
        }

        private float GetSliderSensitivity(AxisCalibration axisCalibration) {
            if (axisCalibration.sensitivityType == AxisSensitivityType.Multiplier) {
                return axisCalibration.sensitivity;
            } else if (axisCalibration.sensitivityType == AxisSensitivityType.Power) {
                return ProcessPowerValue(axisCalibration.sensitivity, 0f, sensitivitySlider.maxValue);
            } else {
                return axisCalibration.sensitivity;
            }
        }

        public void SetSensitivity(AxisCalibration axisCalibration, float sliderValue) {
            if (axisCalibration.sensitivityType == AxisSensitivityType.Multiplier) {
                // Enforce a min sensitivity to prevent axis from becoming useless
                axisCalibration.sensitivity = Mathf.Clamp(sliderValue, minSensitivity, Mathf.Infinity);
                if (sliderValue < minSensitivity) sensitivitySlider.value = minSensitivity; // prevent control from going outside range
            } else if (axisCalibration.sensitivityType == AxisSensitivityType.Power) {
                axisCalibration.sensitivity = ProcessPowerValue(sliderValue, 0f, sensitivitySlider.maxValue);
            } else {
                axisCalibration.sensitivity = sliderValue;
            }
        }

        private static float ProcessPowerValue(float value, float minValue, float maxValue) {
            value = Mathf.Clamp(value, minValue, maxValue);
            if (value > 1f) {
                // Remap values > 1 to 0-1 because Power > 1 == less sensitive
                value = MathTools.ValueInNewRange(
                    value,
                    1f,
                    maxValue,
                    1f, // invert
                    0f
                );
            } else if (value < 1f) {
                // Remap values < 1 to 1-MAX because Power < 1 == more sensitive
                value = MathTools.ValueInNewRange(
                    value,
                    0f,
                    1f,
                    maxValue, // invert
                    1f
                );
            }
            return value;
        }

        public enum ButtonIdentifier {
            Done,
            Cancel,
            Default,
            Calibrate
        }
    }
}
