// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

//#define REWIRED_CONTROL_MAPPER_USE_TMPRO

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper
{

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Rewired;
    using Rewired.Utils;
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
    using Text = TMPro.TMP_Text;
#else
    using Text = UnityEngine.UI.Text;
#endif

    public partial class ControlMapper
    {

        #region GUI Elements

        private abstract class GUIElement
        {
            public readonly GameObject gameObject;
            protected readonly Text text;
            public readonly Selectable selectable;
            protected readonly UIElementInfo uiElementInfo;
            protected bool permanentStateSet;
            protected readonly List<GUIElement> children;

            public RectTransform rectTransform { get; private set; }

            public GUIElement(GameObject gameObject) {
                if(gameObject == null) {
                    Debug.LogError("Rewired Control Mapper: gameObject is null!");
                    return;
                }
                this.selectable = gameObject.GetComponent<Selectable>();
                if(selectable == null) {
                    Debug.LogError("Rewired Control Mapper: Selectable is null!");
                    return;
                }
                this.gameObject = gameObject;
                this.rectTransform = gameObject.GetComponent<RectTransform>();
                text = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
                this.uiElementInfo = gameObject.GetComponent<UIElementInfo>();
                children = new List<GUIElement>();
            }
            public GUIElement(Selectable selectable, Text label) {
                if(selectable == null) {
                    Debug.LogError("Rewired Control Mapper: Selectable is null!");
                    return;
                }
                this.selectable = selectable;
                this.gameObject = selectable.gameObject;
                this.rectTransform = gameObject.GetComponent<RectTransform>();
                this.text = label;
                this.uiElementInfo = gameObject.GetComponent<UIElementInfo>();
                children = new List<GUIElement>();
            }

            public virtual void SetInteractible(bool state, bool playTransition) {
                SetInteractible(state, playTransition, false);
            }
            public virtual void SetInteractible(bool state, bool playTransition, bool permanent) {
                for(int i = 0; i < children.Count; i++) {
                    if(children[i] == null) continue;
                    children[i].SetInteractible(state, playTransition, permanent);
                }
                if(permanentStateSet) return;
                if(selectable == null) return;
                if(permanent) permanentStateSet = true;
                if(selectable.interactable == state) return;
                UI.ControlMapper.UITools.SetInteractable(selectable, state, playTransition);
            }

            public virtual void SetTextWidth(int value) {
                if(text == null) return;
                LayoutElement e = text.GetComponent<LayoutElement>();
                if(e == null) e = text.gameObject.AddComponent<LayoutElement>();
                e.preferredWidth = value;
            }

            public virtual void SetFirstChildObjectWidth(LayoutElementSizeType type, int value) {
                if(rectTransform.childCount == 0) return;
                Transform child = rectTransform.GetChild(0);
                LayoutElement e = child.GetComponent<LayoutElement>();
                if(e == null) e = child.gameObject.AddComponent<LayoutElement>();

                if(type == LayoutElementSizeType.MinSize) e.minWidth = value;
                else if(type == LayoutElementSizeType.PreferredSize) e.preferredWidth = value;
                else throw new System.NotImplementedException();
            }

            public virtual void SetLabel(string label) {
                if(text == null) return;
                text.text = label;
            }

            public virtual string GetLabel() {
                if(text == null) return string.Empty;
                return text.text;
            }

            public virtual void AddChild(GUIElement child) {
                children.Add(child);
            }

            public void SetElementInfoData(string identifier, int intData) {
                if(uiElementInfo == null) return;
                uiElementInfo.identifier = identifier;
                uiElementInfo.intData = intData;
            }

            public virtual void SetActive(bool state) {
                if(gameObject == null) return;
                gameObject.SetActive(state);
            }

            protected virtual bool Init() {
                bool result = true;
                for(int i = 0; i < children.Count; i++) {
                    if(children[i] == null) continue;
                    if(!children[i].Init()) result = false;
                }
                if(selectable == null) {
                    Debug.LogError("Rewired Control Mapper: UI Element is missing Selectable component!");
                    result = false;
                }
                if(rectTransform == null) {
                    Debug.LogError("Rewired Control Mapper: UI Element is missing RectTransform component!");
                    result = false;
                }
                if(uiElementInfo == null) {
                    Debug.LogError("Rewired Control Mapper: UI Element is missing UIElementInfo component!");
                    result = false;
                }
                return result;
            }
        }

        private class GUIButton : GUIElement
        {

            protected Button button { get { return selectable as Button; } }
            public ButtonInfo buttonInfo { get { return uiElementInfo as ButtonInfo; } }

            public GUIButton(GameObject gameObject)
                : base(gameObject) {
                if(!Init()) return;
            }
            public GUIButton(Button button, Text label)
                : base(button, label) {
                if(!Init()) return;
            }

            public void SetButtonInfoData(string identifier, int intData) {
                base.SetElementInfoData(identifier, intData);
            }

            public void SetOnClickCallback(System.Action<ButtonInfo> callback) {
                if(button == null) return;
                button.onClick.AddListener(() => { callback(buttonInfo); });
            }
        }

        private class GUIInputField : GUIElement
        {

            protected Button button { get { return selectable as Button; } }
            public InputFieldInfo fieldInfo { get { return uiElementInfo as InputFieldInfo; } }
            public bool hasToggle { get { return toggle != null; } }
            public GUIToggle toggle { get; private set; }

            public int actionElementMapId {
                get {
                    if(fieldInfo == null) return -1;
                    return fieldInfo.actionElementMapId;
                }
                set {
                    if(fieldInfo == null) return;
                    fieldInfo.actionElementMapId = value;
                }
            }
            public int controllerId {
                get {
                    if(fieldInfo == null) return -1;
                    return fieldInfo.controllerId;
                }
                set {
                    if(fieldInfo == null) return;
                    fieldInfo.controllerId = value;
                }
            }

            public GUIInputField(GameObject gameObject)
                : base(gameObject) {
                if(!Init()) return;
            }
            public GUIInputField(Button button, Text label)
                : base(button, label) {
                if(!Init()) return;
            }

            public void SetFieldInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData) {
                base.SetElementInfoData(string.Empty, intData);
                if(fieldInfo == null) return;
                fieldInfo.actionId = actionId;
                fieldInfo.axisRange = axisRange;
                fieldInfo.controllerType = controllerType;
            }

            public void SetOnClickCallback(System.Action<InputFieldInfo> callback) {
                if(button == null) return;
                button.onClick.AddListener(() => { callback(fieldInfo as InputFieldInfo); });
            }

            public virtual void SetInteractable(bool state, bool playTransition, bool permanent) {
                if(permanentStateSet) return;
                if(hasToggle && !state) toggle.SetInteractible(state, playTransition, permanent); // disable toggle if main element disabled
                base.SetInteractible(state, playTransition, permanent);
            }

            public void AddToggle(GUIToggle toggle) {
                if(toggle == null) return;
                this.toggle = toggle;
            }
        }

        private class GUIToggle : GUIElement
        {

            protected Toggle toggle { get { return selectable as Toggle; } }
            public ToggleInfo toggleInfo { get { return uiElementInfo as ToggleInfo; } }

            public int actionElementMapId {
                get {
                    if(toggleInfo == null) return -1;
                    return toggleInfo.actionElementMapId;
                }
                set {
                    if(toggleInfo == null) return;
                    toggleInfo.actionElementMapId = value;
                }
            }

            public GUIToggle(GameObject gameObject)
                : base(gameObject) {
                if(!Init()) return;
            }
            public GUIToggle(Toggle toggle, Text label)
                : base(toggle, label) {
                if(!Init()) return;
            }

            public void SetToggleInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData) {
                base.SetElementInfoData(string.Empty, intData);
                if(toggleInfo == null) return;
                toggleInfo.actionId = actionId;
                toggleInfo.axisRange = axisRange;
                toggleInfo.controllerType = controllerType;
            }

            public void SetOnSubmitCallback(System.Action<ToggleInfo, bool> callback) {
                if(toggle == null) return;

                EventTrigger trigger = toggle.GetComponent<EventTrigger>();
                if(trigger == null) trigger = toggle.gameObject.AddComponent<EventTrigger>();

                EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
                triggerEvent.AddListener((BaseEventData data) => {
                    PointerEventData p = data as PointerEventData;
                    if(p != null && p.button != 0) return; // ignore mouse clicks for all buttons except LMB
                    callback(toggleInfo, toggle.isOn);
                });

                // Joystick/keyboard submit event
                EventTrigger.Entry entry = new EventTrigger.Entry() {
                    callback = triggerEvent,
                    eventID = EventTriggerType.Submit
                };

                // Mouse click submit event
                EventTrigger.Entry entry2 = new EventTrigger.Entry() {
                    callback = triggerEvent,
                    eventID = EventTriggerType.PointerClick
                };

#if (UNITY_5_0_0 || UNITY_5_0_1 || UNITY_5_0_2 || UNITY_5_0_3 || UNITY_5_0_4) || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
                if(trigger.delegates != null) trigger.delegates.Clear();
                else trigger.delegates = new List<EventTrigger.Entry>();
                trigger.delegates.Add(entry);
                trigger.delegates.Add(entry2);
#else
                if(trigger.triggers != null) trigger.triggers.Clear();
                else trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(entry);
                trigger.triggers.Add(entry2);
#endif
            }

            public void SetToggleState(bool state) {
                if(toggle == null) return;
                toggle.isOn = state;
            }
        }

        private class GUILabel
        {

            public GameObject gameObject { get; private set; }
            private Text text { get; set; }
            public RectTransform rectTransform { get; private set; }

            public GUILabel(GameObject gameObject) {
                if(gameObject == null) {
                    Debug.LogError("Rewired Control Mapper: gameObject is null!");
                    return;
                }
                text = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
                Check();
            }
            public GUILabel(Text label) {
                this.text = label;
                if(!Check()) return;
            }

            public void SetSize(int width, int height) {
                if(text == null) return;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }

            public void SetWidth(int width) {
                if(text == null) return;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }

            public void SetHeight(int height) {
                if(text == null) return;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }

            public void SetLabel(string label) {
                if(text == null) return;
                text.text = label;
            }

#if REWIRED_CONTROL_MAPPER_USE_TMPRO
            public void SetFontStyle(TMPro.FontStyles style) {
                if(text == null) return;
                text.fontStyle = style;
            }

            public void SetTextAlignment(TMPro.TextAlignmentOptions alignment) {
                if(text == null) return;
                text.alignment = alignment;
            }
#else
            public void SetFontStyle(FontStyle style) {
                if(text == null) return;
                text.fontStyle = style;
            }

            public void SetTextAlignment(TextAnchor alignment) {
                if(text == null) return;
                text.alignment = alignment;
            }
#endif

            public void SetActive(bool state) {
                if(gameObject == null) return;
                gameObject.SetActive(state);
            }

            private bool Check() {
                bool result = true;
                if(text == null) {
                    Debug.LogError("Rewired Control Mapper: Button is missing Text child component!");
                    result = false;
                }
                gameObject = text.gameObject;
                rectTransform = text.GetComponent<RectTransform>();
                return result;
            }
        }

        #endregion

        #region Serialized Data

        [System.Serializable]
        public class MappingSet
        {

            [SerializeField]
            [Tooltip("The Map Category that will be displayed to the user for remapping.")]
            private int _mapCategoryId;
            [SerializeField]
            [Tooltip("Choose whether you want to list Actions to display for this Map Category by individual Action or by all the Actions in an Action Category.")]
            private ActionListMode _actionListMode;
            [SerializeField]
            private int[] _actionCategoryIds;
            [SerializeField]
            private int[] _actionIds;

            // Runtime vars
            private IList<int> _actionCategoryIdsReadOnly;
            private IList<int> _actionIdsReadOnly;

            public int mapCategoryId { get { return _mapCategoryId; } }
            public ActionListMode actionListMode { get { return _actionListMode; } }
            public IList<int> actionCategoryIds {
                get {
                    if(_actionCategoryIds == null) return null;
                    if(_actionCategoryIdsReadOnly == null) _actionCategoryIdsReadOnly = new System.Collections.ObjectModel.ReadOnlyCollection<int>(_actionCategoryIds);
                    return _actionCategoryIdsReadOnly;
                }
            }
            public IList<int> actionIds {
                get {
                    if(_actionIds == null) return null;
                    if(_actionIdsReadOnly == null) _actionIdsReadOnly = new System.Collections.ObjectModel.ReadOnlyCollection<int>(_actionIds);
                    return _actionIds;
                }
            }
            public bool isValid {
                get {
                    if(_mapCategoryId < 0 || ReInput.mapping.GetMapCategory(_mapCategoryId) == null) return false;
                    return true;
                }
            }


            public MappingSet() {
                this._mapCategoryId = -1;
                this._actionCategoryIds = new int[0];
                this._actionIds = new int[0];
                this._actionListMode = ActionListMode.ActionCategory;
            }

            private MappingSet(int mapCategoryId, ActionListMode actionListMode, int[] actionCategoryIds, int[] actionIds) {
                this._mapCategoryId = mapCategoryId;
                this._actionListMode = actionListMode;
                this._actionCategoryIds = actionCategoryIds;
                this._actionIds = actionIds;
            }

            // Static

            public static MappingSet Default {
                get {
                    return new MappingSet(
                        0, // Default
                        ActionListMode.ActionCategory,
                        new int[1] { 0 }, // Default
                        new int[0] // No individual actions
                    );
                }
            }

            public enum ActionListMode
            {
                ActionCategory = 0,
                Action = 1
            }
        }

        [System.Serializable]
        public class InputBehaviorSettings
        {

            // Info
            [SerializeField]
            [Tooltip("The Input Behavior that will be displayed to the user for modification.")]
            private int _inputBehaviorId = -1;

            // Display options
            [SerializeField]
            [Tooltip("If checked, a slider will be displayed so the user can change this value.")]
            private bool _showJoystickAxisSensitivity = true;
            [SerializeField]
            [Tooltip("If checked, a slider will be displayed so the user can change this value.")]
            private bool _showMouseXYAxisSensitivity = true;
            //[SerializeField]
            //private bool _showMouseOtherAxisSensitivity = true;

            // Name overrides
            [SerializeField]
            [Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed as the title for the Input Behavior control set. Otherwise, the name field of the InputBehavior will be used.")]
            private string _labelLanguageKey = string.Empty;
            [SerializeField]
            [Tooltip("If set to a non-blank value, this name will be displayed above the individual slider control. Otherwise, no name will be displayed.")]
            private string _joystickAxisSensitivityLabelLanguageKey = string.Empty;
            [SerializeField]
            [Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed above the individual slider control. Otherwise, no name will be displayed.")]
            private string _mouseXYAxisSensitivityLabelLanguageKey = string.Empty;
            //[SerializeField]
            //private string _mouseOtherAxisSensitivityDisplayName= string.Empty;

            // Icons
            [SerializeField]
            [Tooltip("The icon to display next to the slider. Set to none for no icon.")]
            private Sprite _joystickAxisSensitivityIcon;
            [SerializeField]
            [Tooltip("The icon to display next to the slider. Set to none for no icon.")]
            private Sprite _mouseXYAxisSensitivityIcon;
            //[SerializeField]
            //private Sprite _mouseOtherAxisSensitivityIcon;

            // Values
            [SerializeField]
            [Tooltip("Minimum value the user is allowed to set for this property.")]
            private float _joystickAxisSensitivityMin = 0f;
            [SerializeField]
            [Tooltip("Maximum value the user is allowed to set for this property.")]
            private float _joystickAxisSensitivityMax = 2.0f;
            [SerializeField]
            [Tooltip("Minimum value the user is allowed to set for this property.")]
            private float _mouseXYAxisSensitivityMin = 0f;
            [SerializeField]
            [Tooltip("Maximum value the user is allowed to set for this property.")]
            private float _mouseXYAxisSensitivityMax = 2.0f;

            public int inputBehaviorId { get { return _inputBehaviorId; } }
            public bool showJoystickAxisSensitivity { get { return _showJoystickAxisSensitivity; } }
            public bool showMouseXYAxisSensitivity { get { return _showMouseXYAxisSensitivity; } }
            public string labelLanguageKey { get { return _labelLanguageKey; } }
            public string joystickAxisSensitivityLabelLanguageKey { get { return _joystickAxisSensitivityLabelLanguageKey; } }
            public string mouseXYAxisSensitivityLabelLanguageKey { get { return _mouseXYAxisSensitivityLabelLanguageKey; } }
            public Sprite joystickAxisSensitivityIcon { get { return _joystickAxisSensitivityIcon; } }
            public Sprite mouseXYAxisSensitivityIcon { get { return _mouseXYAxisSensitivityIcon; } }
            public float joystickAxisSensitivityMin { get { return _joystickAxisSensitivityMin; } }
            public float joystickAxisSensitivityMax { get { return _joystickAxisSensitivityMax; } }
            public float mouseXYAxisSensitivityMin { get { return _mouseXYAxisSensitivityMin; } }
            public float mouseXYAxisSensitivityMax { get { return _mouseXYAxisSensitivityMax; } }

            public bool isValid { get { return _inputBehaviorId >= 0 && (_showJoystickAxisSensitivity || _showMouseXYAxisSensitivity); } } // || _showMouseOtherAxisSensitivity); } }


            public InputBehaviorSettings() {
            }

        }

        [System.Serializable]
        private class Prefabs
        {

            [SerializeField]
            private GameObject _button;
            [SerializeField]
            private GameObject _fitButton;
            [SerializeField]
            private GameObject _inputGridLabel;
            [SerializeField]
            private GameObject _inputGridHeaderLabel;
            [SerializeField]
            private GameObject _inputGridFieldButton;
            [SerializeField]
            private GameObject _inputGridFieldInvertToggle;
            [SerializeField]
            private GameObject _window;
            [SerializeField]
            private GameObject _windowTitleText;
            [SerializeField]
            private GameObject _windowContentText;
            [SerializeField]
            private GameObject _fader;
            [SerializeField]
            private GameObject _calibrationWindow;
            [SerializeField]
            private GameObject _inputBehaviorsWindow;

            // Optional
            [SerializeField]
            private GameObject _centerStickGraphic;
            [SerializeField]
            private GameObject _moveStickGraphic;

            public GameObject button { get { return _button; } }
            public GameObject fitButton { get { return _fitButton; } }
            public GameObject inputGridLabel { get { return _inputGridLabel; } }
            public GameObject inputGridHeaderLabel { get { return _inputGridHeaderLabel; } }
            public GameObject inputGridFieldButton { get { return _inputGridFieldButton; } }
            public GameObject inputGridFieldInvertToggle { get { return _inputGridFieldInvertToggle; } }
            public GameObject window { get { return _window; } }
            public GameObject windowTitleText { get { return _windowTitleText; } }
            public GameObject windowContentText { get { return _windowContentText; } }
            public GameObject fader { get { return _fader; } }
            public GameObject calibrationWindow { get { return _calibrationWindow; } }
            public GameObject inputBehaviorsWindow { get { return _inputBehaviorsWindow; } }

            public GameObject centerStickGraphic { get { return _centerStickGraphic; } }
            public GameObject moveStickGraphic { get { return _moveStickGraphic; } }

            public bool Check() {
                if(
                    _button == null ||
                    _fitButton == null ||
                    _inputGridLabel == null ||
                    _inputGridHeaderLabel == null ||
                    _inputGridFieldButton == null ||
                    _inputGridFieldInvertToggle == null ||
                    _window == null ||
                    _windowTitleText == null ||
                    _windowContentText == null ||
                    _fader == null ||
                    _calibrationWindow == null ||
                    _inputBehaviorsWindow == null
                ) return false;
                return true;
            }

        }

        [System.Serializable]
        private class References
        {

            [SerializeField]
            private Canvas _canvas;
            [SerializeField]
            private CanvasGroup _mainCanvasGroup;
            [SerializeField]
            private Transform _mainContent;
            [SerializeField]
            private Transform _mainContentInner;
            [SerializeField]
            private UIGroup _playersGroup;
            [SerializeField]
            private Transform _controllerGroup;
            [SerializeField]
            private Transform _controllerGroupLabelGroup;
            [SerializeField]
            private UIGroup _controllerSettingsGroup;
            [SerializeField]
            private UIGroup _assignedControllersGroup;
            [SerializeField]
            private Transform _settingsAndMapCategoriesGroup;
            [SerializeField]
            private UIGroup _settingsGroup;
            [SerializeField]
            private UIGroup _mapCategoriesGroup;
            [SerializeField]
            private Transform _inputGridGroup;
            [SerializeField]
            private Transform _inputGridContainer;
            [SerializeField]
            private Transform _inputGridHeadersGroup;
            [SerializeField]
            private Scrollbar _inputGridVScrollbar;
            [SerializeField]
            private ScrollRect _inputGridScrollRect;
            [SerializeField]
            private Transform _inputGridInnerGroup;
            [SerializeField]
            private Text _controllerNameLabel;
            [SerializeField]
            private Button _removeControllerButton;
            [SerializeField]
            private Button _assignControllerButton;
            [SerializeField]
            private Button _calibrateControllerButton;
            [SerializeField]
            private Button _doneButton;
            [SerializeField]
            private Button _restoreDefaultsButton;
            [SerializeField]
            private Selectable _defaultSelection;
            [SerializeField]
            private GameObject[] _fixedSelectableUIElements;

            // OPTIONAL
            [SerializeField]
            private Image _mainBackgroundImage;

            public Canvas canvas { get { return _canvas; } }
            public CanvasGroup mainCanvasGroup { get { return _mainCanvasGroup; } }
            public Transform mainContent { get { return _mainContent; } }
            public Transform mainContentInner { get { return _mainContentInner; } }
            public UIGroup playersGroup { get { return _playersGroup; } }
            public Transform controllerGroup { get { return _controllerGroup; } }
            public Transform controllerGroupLabelGroup { get { return _controllerGroupLabelGroup; } }
            public UIGroup controllerSettingsGroup { get { return _controllerSettingsGroup; } }
            public UIGroup assignedControllersGroup { get { return _assignedControllersGroup; } }
            public Transform settingsAndMapCategoriesGroup { get { return _settingsAndMapCategoriesGroup; } }
            public UIGroup settingsGroup { get { return _settingsGroup; } }
            public UIGroup mapCategoriesGroup { get { return _mapCategoriesGroup; } }
            public Transform inputGridGroup { get { return _inputGridGroup; } }
            public Transform inputGridContainer { get { return _inputGridContainer; } }
            public Transform inputGridHeadersGroup { get { return _inputGridHeadersGroup; } }
            public Scrollbar inputGridVScrollbar { get { return _inputGridVScrollbar; } }
            public ScrollRect inputGridScrollRect { get { return _inputGridScrollRect; } }
            public Transform inputGridInnerGroup { get { return _inputGridInnerGroup; } }
            public Text controllerNameLabel { get { return _controllerNameLabel; } }
            public Button removeControllerButton { get { return _removeControllerButton; } }
            public Button assignControllerButton { get { return _assignControllerButton; } }
            public Button calibrateControllerButton { get { return _calibrateControllerButton; } }
            public Button doneButton { get { return _doneButton; } }
            public Button restoreDefaultsButton { get { return _restoreDefaultsButton; } }
            public Selectable defaultSelection { get { return _defaultSelection; } }
            public GameObject[] fixedSelectableUIElements { get { return _fixedSelectableUIElements; } }

            public Image mainBackgroundImage { get { return _mainBackgroundImage; } }

            // Runtime references

            public LayoutElement inputGridLayoutElement { get; set; }
            public Transform inputGridActionColumn { get; set; }
            public Transform inputGridKeyboardColumn { get; set; }
            public Transform inputGridMouseColumn { get; set; }
            public Transform inputGridControllerColumn { get; set; }

            public Transform inputGridHeader1 { get; set; }
            public Transform inputGridHeader2 { get; set; }
            public Transform inputGridHeader3 { get; set; }
            public Transform inputGridHeader4 { get; set; }

            public bool Check() {
                if(_canvas == null ||
                    _mainCanvasGroup == null ||
                    _mainContent == null ||
                    _mainContentInner == null ||
                    _playersGroup == null ||
                    _controllerGroup == null ||
                    _controllerGroupLabelGroup == null ||
                    _controllerSettingsGroup == null ||
                    _assignedControllersGroup == null ||
                    _settingsAndMapCategoriesGroup == null ||
                    _settingsGroup == null ||
                    _mapCategoriesGroup == null ||
                    _inputGridGroup == null ||
                    _inputGridContainer == null ||
                    _inputGridHeadersGroup == null ||
                    _inputGridVScrollbar == null ||
                    _inputGridScrollRect == null ||
                    _inputGridInnerGroup == null ||
                    _controllerNameLabel == null ||
                    _removeControllerButton == null ||
                    _assignControllerButton == null ||
                    _calibrateControllerButton == null ||
                    _doneButton == null ||
                    _restoreDefaultsButton == null ||
                    _defaultSelection == null
                ) return false;
                return true;
            }

        }

        #endregion

        private class InputActionSet
        {
            private int _actionId;
            private AxisRange _axisRange;

            public int actionId { get { return _actionId; } }
            public AxisRange axisRange { get { return _axisRange; } }

            public InputActionSet(int actionId, AxisRange axisRange) {
                this._actionId = actionId;
                this._axisRange = axisRange;
            }
        }

        private class InputMapping
        {
            public string actionName { get; private set; }
            public InputFieldInfo fieldInfo { get; private set; }
            public ControllerMap map { get; private set; }
            public ActionElementMap aem { get; private set; }
            public ControllerType controllerType { get; private set; }
            public int controllerId { get; private set; }

            public ControllerPollingInfo pollingInfo { get; set; }
            public ModifierKeyFlags modifierKeyFlags { get; set; }

            public AxisRange axisRange {
                get {
                    AxisRange axisRange = AxisRange.Positive;
                    if(pollingInfo.elementType == ControllerElementType.Axis) {
                        if(fieldInfo.axisRange == AxisRange.Full) axisRange = AxisRange.Full;
                        else axisRange = pollingInfo.axisPole == Pole.Positive ? AxisRange.Positive : AxisRange.Negative;
                    }
                    return axisRange;
                }
            }
            public string elementName {
                get {
                    if(controllerType == ControllerType.Keyboard) {
                        return ControlMapper.GetLanguage().GetElementIdentifierName(pollingInfo.keyboardKey, modifierKeyFlags);
                    } else {
                        return ControlMapper.GetLanguage().GetElementIdentifierName(pollingInfo.controller, pollingInfo.elementIdentifierId, pollingInfo.axisPole == Pole.Positive ? AxisRange.Positive : AxisRange.Negative);
                    }
                }
            }

            public InputMapping(string actionName, InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, ControllerType controllerType, int controllerId) {
                this.actionName = actionName;
                this.fieldInfo = fieldInfo;
                this.map = map;
                this.aem = aem;
                this.controllerType = controllerType;
                this.controllerId = controllerId;
            }

            public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo) {
                this.pollingInfo = pollingInfo;
                return ToElementAssignment();
            }
            public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo, ModifierKeyFlags modifierKeyFlags) {
                this.pollingInfo = pollingInfo;
                this.modifierKeyFlags = modifierKeyFlags;
                return ToElementAssignment();
            }
            public ElementAssignment ToElementAssignment() {
                return new ElementAssignment(
                    controllerType,
                    pollingInfo.elementType,
                    pollingInfo.elementIdentifierId,
                    axisRange,
                    pollingInfo.keyboardKey,
                    modifierKeyFlags,
                    fieldInfo.actionId,
                    fieldInfo.axisRange == AxisRange.Negative ? Pole.Negative : Pole.Positive,
                    false,
                    aem != null ? aem.id : -1
                );
            }
        }

        private class AxisCalibrator
        {

            public AxisCalibrationData data;
            public readonly Joystick joystick;
            public readonly int axisIndex;

            private Controller.Axis axis;
            private bool firstRun;

            public bool isValid { get { return axis != null; } }

            public AxisCalibrator(Joystick joystick, int axisIndex) {
                this.data = new AxisCalibrationData();
                this.joystick = joystick;
                this.axisIndex = axisIndex;
                if(joystick != null && axisIndex >= 0 && joystick.axisCount > axisIndex) {
                    axis = joystick.Axes[axisIndex];
                    data = joystick.calibrationMap.GetAxis(axisIndex).GetData();
                }
                firstRun = true;
            }

            public void RecordMinMax() {
                if(axis == null) return;

                float valueRaw = axis.valueRaw;
                if(firstRun || valueRaw < data.min) data.min = valueRaw;
                if(firstRun || valueRaw > data.max) data.max = valueRaw;

                firstRun = false;
            }

            public void RecordZero() {
                if(axis == null) return;
                data.zero = axis.valueRaw;
            }

            public void Commit() {
                if(axis == null) return;
                AxisCalibration calibration = joystick.calibrationMap.GetAxis(axisIndex);
                if(calibration == null) return;

                // Make sure min/max isn't the same or joystick cannot move
                if(Mathf.Abs(data.max - data.min) < 0.1) return; // too close, joystick would be useless

                calibration.SetData(data);
            }
        }

        private class IndexedDictionary<TKey, TValue>
        {

            private List<Entry> list;

            public int Count {
                get {
                    return list.Count;
                }
            }

            public IndexedDictionary() {
                list = new List<Entry>();
            }

            public TValue this[int index] {
                get {
                    return list[index].value;
                }
            }

            public TValue Get(TKey key) {
                int index = IndexOfKey(key);
                if(index < 0) throw new System.Exception("Key does not exist!");
                return list[index].value;
            }

            public bool TryGet(TKey key, out TValue value) {
                value = default(TValue);
                int index = IndexOfKey(key);
                if(index < 0) return false;
                value = list[index].value;
                return true;
            }

            public void Add(TKey key, TValue value) {
                if(ContainsKey(key)) throw new System.Exception("Key " + key.ToString() + " is already in use!");
                list.Add(new Entry(key, value));
            }

            public int IndexOfKey(TKey key) {
                int count = list.Count;
                for(int i = 0; i < count; i++) {
                    if(EqualityComparer<TKey>.Default.Equals(list[i].key, key)) return i;
                }
                return -1;
            }

            public bool ContainsKey(TKey key) {
                int count = list.Count;
                for(int i = 0; i < count; i++) {
                    if(EqualityComparer<TKey>.Default.Equals(list[i].key, key)) return true;
                }
                return false;
            }

            public void Clear() {
                list.Clear();
            }

            private class Entry
            {
                public TKey key;
                public TValue value;

                public Entry(TKey key, TValue value) {
                    this.key = key;
                    this.value = value;
                }
            }
        }

    }
}
