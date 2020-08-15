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

namespace Rewired.UI.ControlMapper
{

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using UnityEngine.Serialization;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.EventSystems;
    using Rewired;
    using Rewired.Utils;
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
    using Text = TMPro.TMP_Text;
#else
    using Text = UnityEngine.UI.Text;
#endif

    [AddComponentMenu("")]
    public partial class ControlMapper : MonoBehaviour
    {

        #region Consts

        public const int versionMajor = 1;
        public const int versionMinor = 1;
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
        public const bool usesTMPro = true;
#else
        public const bool usesTMPro = false;
#endif

        private const float blockInputOnFocusTimeout = 0.1f; // a small delay after main screen receives focus to filter out button down events during new assignments

        private const string buttonIdentifier_playerSelection = "PlayerSelection";
        private const string buttonIdentifier_removeController = "RemoveController";
        private const string buttonIdentifier_assignController = "AssignController";
        private const string buttonIdentifier_calibrateController = "CalibrateController";
        private const string buttonIdentifier_editInputBehaviors = "EditInputBehaviors";
        private const string buttonIdentifier_mapCategorySelection = "MapCategorySelection";
        private const string buttonIdentifier_assignedControllerSelection = "AssignedControllerSelection";
        private const string buttonIdentifier_done = "Done";
        private const string buttonIdentifier_restoreDefaults = "RestoreDefaults";

        #endregion

        #region Inspector Vars

        [SerializeField]
        [Tooltip("Must be assigned a Rewired Input Manager scene object or prefab.")]
        private InputManager _rewiredInputManager;

        [SerializeField]
        [Tooltip("Set to True to prevent the Game Object from being destroyed when a new scene is loaded.\n\nNOTE: Changing this value from True to False at runtime will have no effect because Object.DontDestroyOnLoad cannot be undone once set.")]
        private bool _dontDestroyOnLoad;

        [SerializeField]
        [Tooltip("Open the control mapping screen immediately on start. Mainly used for testing.")]
        private bool _openOnStart = false;

        [SerializeField]
        [Tooltip("The Layout of the Keyboard Maps to be displayed.")]
        private int _keyboardMapDefaultLayout = 0;
        [SerializeField]
        [Tooltip("The Layout of the Mouse Maps to be displayed.")]
        private int _mouseMapDefaultLayout = 0;
        [SerializeField]
        [Tooltip("The Layout of the Mouse Maps to be displayed.")]
        private int _joystickMapDefaultLayout = 0;

        [SerializeField]
        private MappingSet[] _mappingSets = new MappingSet[1] { MappingSet.Default };

        [SerializeField]
        [Tooltip("Display a selectable list of Players. If your game only supports 1 player, you can disable this.")]
        private bool _showPlayers = true;
        [SerializeField]
        [Tooltip("Display the Controller column for input mapping.")]
        private bool _showControllers = true;
        [SerializeField]
        [Tooltip("Display the Keyboard column for input mapping.")]
        private bool _showKeyboard = true;
        [SerializeField]
        [Tooltip("Display the Mouse column for input mapping.")]
        private bool _showMouse = true;

        [SerializeField]
        [Tooltip(
            "The maximum number of controllers allowed to be assigned to a Player. " +
            "If set to any value other than 1, a selectable list of currently-assigned controller will be displayed to the user. " +
            "[0 = infinite]"
        )]
        private int _maxControllersPerPlayer = 1;

        [SerializeField]
        [Tooltip("Display section labels for each Action Category in the input field grid. Only applies if Action Categories are used to display the Action list.")]
        private bool _showActionCategoryLabels = false;

        [SerializeField]
        [Tooltip("The number of input fields to display for the keyboard. If you want to support alternate mappings on the same device, set this to 2 or more.")]
        private int _keyboardInputFieldCount = 2;
        [SerializeField]
        [Tooltip("The number of input fields to display for the mouse. If you want to support alternate mappings on the same device, set this to 2 or more.")]
        private int _mouseInputFieldCount = 1;
        [SerializeField]
        [Tooltip("The number of input fields to display for joysticks. If you want to support alternate mappings on the same device, set this to 2 or more.")]
        private int _controllerInputFieldCount = 1;

        [SerializeField]
        [Tooltip(
            "Display a full-axis input assignment field for every axis-type Action in the input field grid. Also displays an invert toggle for the user " +
            " to invert the full-axis assignment direction." +
            "\n\n*IMPORTANT*: This field is required if you have made any full-axis assignments in the Rewired Input Manager or in saved XML user data. " +
            "Disabling this field when you have full-axis assignments will result in the inability for the user to view, remove, or modify these " +
            "full-axis assignments. In addition, these assignments may cause conflicts when trying to remap the same axes to Actions."
        )]
        private bool _showFullAxisInputFields = true;
        [SerializeField]
        [Tooltip(
            "Display a positive and negative input assignment field for every axis-type Action in the input field grid." +
            "\n\n*IMPORTANT*: These fields are required to assign buttons, keyboard keys, and hat or D-Pad directions to axis-type Actions. " +
            "If you have made any split-axis assignments or button/key/D-pad assignments to axis-type Actions in the Rewired Input Manager or " +
            "in saved XML user data, disabling these fields will result in the inability for the user to view, remove, or modify these assignments. " +
            "In addition, these assignments may cause conflicts when trying to remap the same elements to Actions."
        )]
        private bool _showSplitAxisInputFields = true;

        [SerializeField]
        [Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to make the conflicting assignment anyway.")]
        private bool _allowElementAssignmentConflicts = false;
        [SerializeField]
        [Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to swap conflicting assignments. This only applies to the first conflicting assignment found. This option will not be displayed if allowElementAssignmentConflicts is true.")]
        private bool _allowElementAssignmentSwap = false;

        [SerializeField]
        [Tooltip("The width in relative pixels of the Action label column.")]
        private int _actionLabelWidth = 200;
        [SerializeField]
        [Tooltip("The width in relative pixels of the Keyboard column.")]
        private int _keyboardColMaxWidth = 360;
        [SerializeField]
        [Tooltip("The width in relative pixels of the Mouse column.")]
        private int _mouseColMaxWidth = 200;
        [SerializeField]
        [Tooltip("The width in relative pixels of the Controller column.")]
        private int _controllerColMaxWidth = 200;

        [SerializeField]
        [Tooltip("The height in relative pixels of the input grid button rows.")]
        private int _inputRowHeight = 40;
        [SerializeField]
        [Tooltip("The padding of the input grid button rows.")]
        private RectOffset _inputRowPadding = new RectOffset();
        [SerializeField]
        [Tooltip("The width in relative pixels of spacing between input fields in a single column.")]
        private int _inputRowFieldSpacing = 0;
        [SerializeField]
        [Tooltip("The width in relative pixels of spacing between columns.")]
        private int _inputColumnSpacing = 40;
        [SerializeField]
        [Tooltip("The height in relative pixels of the space between Action Category sections. Only applicable if Show Action Category Labels is checked.")]
        private int _inputRowCategorySpacing = 20;
        [SerializeField]
        [Tooltip("The width in relative pixels of the invert toggle buttons.")]
        private int _invertToggleWidth = 40;

        [SerializeField]
        [Tooltip("The width in relative pixels of generated popup windows.")]
        private int _defaultWindowWidth = 500;
        [SerializeField]
        [Tooltip("The height in relative pixels of generated popup windows.")]
        private int _defaultWindowHeight = 400;

        [SerializeField]
        [Tooltip("The time in seconds the user has to press an element on a controller when assigning a controller to a Player. If this time elapses with no user input a controller, the assignment will be canceled.")]
        private float _controllerAssignmentTimeout = 5.0f;
        [SerializeField]
        [Tooltip("The time in seconds the user has to press an element on a controller while waiting for axes to be centered before assigning input.")]
        private float _preInputAssignmentTimeout = 5.0f;
        [SerializeField]
        [Tooltip("The time in seconds the user has to press an element on a controller when assigning input. If this time elapses with no user input on the target controller, the assignment will be canceled.")]
        private float _inputAssignmentTimeout = 5.0f;
        [SerializeField]
        [Tooltip("The time in seconds the user has to press an element on a controller during calibration.")]
        private float _axisCalibrationTimeout = 5.0f;

        [SerializeField]
        [Tooltip("If checked, mouse X-axis movement will always be ignored during input assignment. Check this if you don't want the horizontal mouse axis to be user-assignable to any Actions.")]
        private bool _ignoreMouseXAxisAssignment = true;
        [SerializeField]
        [Tooltip("If checked, mouse Y-axis movement will always be ignored during input assignment. Check this if you don't want the vertical mouse axis to be user-assignable to any Actions.")]
        private bool _ignoreMouseYAxisAssignment = true;

        [SerializeField]
        [Tooltip("An Action that when activated will alternately close or open the main screen as long as no popup windows are open.")]
        private int _screenToggleAction = -1;
        [SerializeField]
        [Tooltip("An Action that when activated will open the main screen if it is closed.")]
        private int _screenOpenAction = -1;
        [SerializeField]
        [Tooltip("An Action that when activated will close the main screen as long as no popup windows are open.")]
        private int _screenCloseAction = -1;
        [SerializeField]
        [Tooltip("An Action that when activated will cancel and close any open popup window. Use with care because the element assigned to this Action can never be mapped by the user (because it would just cancel his assignment).")]
        private int _universalCancelAction = -1;
        [SerializeField]
        [Tooltip("If enabled, Universal Cancel will also close the main screen if pressed when no windows are open.")]
        private bool _universalCancelClosesScreen = true;

        [SerializeField]
        [Tooltip("If checked, controls will be displayed which will allow the user to customize certain Input Behavior settings.")]
        private bool _showInputBehaviorSettings;

        [SerializeField]
        [Tooltip("Customizable settings for user-modifiable Input Behaviors. This can be used for settings like Mouse Look Sensitivity.")]
        private InputBehaviorSettings[] _inputBehaviorSettings;

        [SerializeField]
        [Tooltip("If enabled, UI elements will be themed based on the settings in Theme Settings.")]
        private bool _useThemeSettings = true;
        [SerializeField]
        [Tooltip("Must be assigned a ThemeSettings object. Used to theme UI elements.")]
        private ThemeSettings _themeSettings;

        [SerializeField]
        [Tooltip("Must be assigned a LanguageData object. Used to retrieve language entries for UI elements.")]
        private UI.ControlMapper.LanguageDataBase _language;

        [SerializeField]
        [Tooltip("A list of prefabs. You should not have to modify this.")]
        private Prefabs prefabs;
        [SerializeField]
        [Tooltip("A list of references to elements in the hierarchy. You should not have to modify this.")]
        private References references;

        [SerializeField]
        [Tooltip("Show the label for the Players button group?")]
        private bool _showPlayersGroupLabel = true;
        [SerializeField]
        [Tooltip("Show the label for the Controller button group?")]
        private bool _showControllerGroupLabel = true;
        [SerializeField]
        [Tooltip("Show the label for the Assigned Controllers button group?")]
        private bool _showAssignedControllersGroupLabel = true;
        [SerializeField]
        [Tooltip("Show the label for the Settings button group?")]
        private bool _showSettingsGroupLabel = true;
        [SerializeField]
        [Tooltip("Show the label for the Map Categories button group?")]
        private bool _showMapCategoriesGroupLabel = true;

        [SerializeField]
        [Tooltip("Show the label for the current controller name?")]
        private bool _showControllerNameLabel = true;
        [SerializeField]
        [Tooltip("Show the Assigned Controllers group? If joystick auto-assignment is enabled in the Rewired Input Manager and the max joysticks per player is set to any value other than 1, the Assigned Controllers group will always be displayed.")]
        private bool _showAssignedControllers = true;

        #endregion

        #region Events

        // .NET events

        private System.Action _ScreenClosedEvent;
        private System.Action _ScreenOpenedEvent;
        private System.Action _PopupWindowOpenedEvent;
        private System.Action _PopupWindowClosedEvent;
        private System.Action _InputPollingStartedEvent;
        private System.Action _InputPollingEndedEvent;

        /// <summary>
        /// Event sent when the UI is closed.
        /// This is a .NET event. If you are using Unity Events,
        /// use the onScreenClosed event.
        /// </summary>
        public event System.Action ScreenClosedEvent {
            add {
                _ScreenClosedEvent += value;
            }
            remove {
                _ScreenClosedEvent -= value;
            }
        }

        /// <summary>
        /// Event sent when the UI is opened.
        /// This is a .NET event. If you are using Unity Events,
        /// use the onScreenOpened event.
        /// </summary>
        public event System.Action ScreenOpenedEvent {
            add {
                _ScreenOpenedEvent += value;
            }
            remove {
                _ScreenOpenedEvent -= value;
            }
        }

        /// <summary>
        /// Event sent when a popup window is closed.
        /// This is a .NET event. If you are using Unity Events,
        /// use the onPopupWindowClosed event.
        /// </summary>
        public event System.Action PopupWindowClosedEvent {
            add {
                _PopupWindowClosedEvent += value;
            }
            remove {
                _PopupWindowClosedEvent -= value;
            }
        }

        /// <summary>
        /// Event sent when a popup window is opened.
        /// This is a .NET event. If you are using Unity Events,
        /// use the onPopupWindowOpened event.
        /// </summary>
        public event System.Action PopupWindowOpenedEvent {
            add {
                _PopupWindowOpenedEvent += value;
            }
            remove {
                _PopupWindowOpenedEvent -= value;
            }
        }

        /// <summary>
        /// Event sent when polling for input has started.
        /// This is a .NET event. If you are using Unity Events,
        /// use the onInputPollingStarted event.
        /// </summary>
        public event System.Action InputPollingStartedEvent {
            add {
                _InputPollingStartedEvent += value;
            }
            remove {
                _InputPollingStartedEvent -= value;
            }
        }

        /// <summary>
        /// Event sent when polling for input has ended.
        /// This is a .NET event. If you are using Unity Events,
        /// use the onInputPollingStopped event.
        /// </summary>
        public event System.Action InputPollingEndedEvent {
            add {
                _InputPollingEndedEvent += value;
            }
            remove {
                _InputPollingEndedEvent -= value;
            }
        }

        // Unity events

        [SerializeField]
        [Tooltip("Event sent when the UI is closed.")]
        private UnityEvent _onScreenClosed;

        [SerializeField]
        [Tooltip("Event sent when the UI is opened.")]
        private UnityEvent _onScreenOpened;

        [SerializeField]
        [Tooltip("Event sent when a popup window is closed.")]
        private UnityEvent _onPopupWindowClosed;

        [SerializeField]
        [Tooltip("Event sent when a popup window is opened.")]
        private UnityEvent _onPopupWindowOpened;

        [SerializeField]
        [Tooltip("Event sent when polling for input has started.")]
        private UnityEvent _onInputPollingStarted;

        [SerializeField]
        [Tooltip("Event sent when polling for input has ended.")]
        private UnityEvent _onInputPollingEnded;

        /// <summary>
        /// Unity event sent when the UI is closed.
        /// This is a Unity event. For the .NET event,
        /// use ScreenClosedEvent.
        /// </summary>
        public event UnityAction onScreenClosed {
            add {
                _onScreenClosed.AddListener(value);
            }
            remove {
                _onScreenClosed.RemoveListener(value);
            }
        }

        /// <summary>
        /// Unity event sent when the UI is opened.
        /// This is a Unity event. For the .NET event,
        /// use ScreenOpenedEvent.
        /// </summary>
        public event UnityAction onScreenOpened {
            add {
                _onScreenOpened.AddListener(value);
            }
            remove {
                _onScreenOpened.RemoveListener(value);
            }
        }

        /// <summary>
        /// Unity event sent when a popup window is closed.
        /// This is a Unity event. For the .NET event,
        /// use PopupWindowClosedEvent.
        /// </summary>
        public event UnityAction onPopupWindowClosed {
            add {
                _onPopupWindowClosed.AddListener(value);
            }
            remove {
                _onPopupWindowClosed.RemoveListener(value);
            }
        }

        /// <summary>
        /// Unity event sent when a popup window is opened.
        /// This is a Unity event. For the .NET event,
        /// use PopupWindowOpenedEvent.
        /// </summary>
        public event UnityAction onPopupWindowOpened {
            add {
                _onPopupWindowOpened.AddListener(value);
            }
            remove {
                _onPopupWindowOpened.RemoveListener(value);
            }
        }

        /// <summary>
        /// Unity event sent when polling for input has started.
        /// This is a Unity event. For the .NET event,
        /// use InputPollingStartedEvent.
        /// </summary>
        public event UnityAction onInputPollingStarted {
            add {
                _onInputPollingStarted.AddListener(value);
            }
            remove {
                _onInputPollingStarted.RemoveListener(value);
            }
        }

        /// <summary>
        /// Unity event sent when polling for input has ended.
        /// This is a Unity event. For the .NET event,
        /// use InputPollingEndedEvent.
        /// </summary>
        public event UnityAction onInputPollingEnded {
            add {
                _onInputPollingEnded.AddListener(value);
            }
            remove {
                _onInputPollingEnded.RemoveListener(value);
            }
        }

        #endregion

        #region Working vars

        private static ControlMapper Instance;

        private bool initialized;
        private int playerCount;

        private InputGrid inputGrid;
        private WindowManager windowManager;
        private int currentPlayerId;
        private int currentMapCategoryId;
        private List<GUIButton> playerButtons;
        private List<GUIButton> mapCategoryButtons;
        private List<GUIButton> assignedControllerButtons;
        private GUIButton assignedControllerButtonsPlaceholder;
        private List<GameObject> miscInstantiatedObjects;
        private GameObject canvas;
        private GameObject lastUISelection;
        private int currentJoystickId = -1;
        private float blockInputOnFocusEndTime;
        private bool isPollingForInput;

        private InputMapping pendingInputMapping;
        private AxisCalibrator pendingAxisCalibration;

        private System.Action<InputFieldInfo> inputFieldActivatedDelegate;
        private System.Action<ToggleInfo, bool> inputFieldInvertToggleStateChangedDelegate;

        private System.Action _restoreDefaultsDelegate;

        #endregion

        #region Properties

        #region Inspector Properties

        // Public access to certain settings which can be changed at runtime before initialization or after initialization provided Reset() is called afterwards.
        // This is useful if you want to show only certain settings based on the current platform.
        // Not all properties are runtime modifiable.

        public InputManager rewiredInputManager { get { return _rewiredInputManager; } set { _rewiredInputManager = value; InspectorPropertyChanged(true); } }
        public bool dontDestroyOnLoad { get { return _dontDestroyOnLoad; } set { if(value != _dontDestroyOnLoad) { if(value) DontDestroyOnLoad(transform.gameObject); } _dontDestroyOnLoad = value; } }
        public int keyboardMapDefaultLayout { get { return _keyboardMapDefaultLayout; } set { _keyboardMapDefaultLayout = value; InspectorPropertyChanged(true); } }
        public int mouseMapDefaultLayout { get { return _mouseMapDefaultLayout; } set { _mouseMapDefaultLayout = value; InspectorPropertyChanged(true); } }
        public int joystickMapDefaultLayout { get { return _joystickMapDefaultLayout; } set { _joystickMapDefaultLayout = value; InspectorPropertyChanged(true); } }
        //public MappingSet[] mappingSets { get { return _mappingSets; } set { _mappingSets = value; InspectorPropertyChanged(); } }
        public bool showPlayers { get { return _showPlayers && ReInput.players.playerCount > 1; } set { _showPlayers = value; InspectorPropertyChanged(true); } }
        public bool showControllers { get { return _showControllers; } set { _showControllers = value; InspectorPropertyChanged(true); } }
        public bool showKeyboard { get { return _showKeyboard; } set { _showKeyboard = value; InspectorPropertyChanged(true); } }
        public bool showMouse { get { return _showMouse; } set { _showMouse = value; InspectorPropertyChanged(true); } }
        public int maxControllersPerPlayer { get { return _maxControllersPerPlayer; } set { _maxControllersPerPlayer = value; InspectorPropertyChanged(true); } }
        public bool showActionCategoryLabels { get { return _showActionCategoryLabels; } set { _showActionCategoryLabels = value; InspectorPropertyChanged(true); } }
        public int keyboardInputFieldCount { get { return _keyboardInputFieldCount; } set { _keyboardInputFieldCount = value; InspectorPropertyChanged(true); } }
        public int mouseInputFieldCount { get { return _mouseInputFieldCount; } set { _mouseInputFieldCount = value; InspectorPropertyChanged(true); } }
        public int controllerInputFieldCount { get { return _controllerInputFieldCount; } set { _controllerInputFieldCount = value; InspectorPropertyChanged(true); } }
        public bool showFullAxisInputFields { get { return _showFullAxisInputFields; } set { _showFullAxisInputFields = value; InspectorPropertyChanged(true); } }
        public bool showSplitAxisInputFields { get { return _showSplitAxisInputFields; } set { _showSplitAxisInputFields = value; InspectorPropertyChanged(true); } }
        public bool allowElementAssignmentConflicts { get { return _allowElementAssignmentConflicts; } set { _allowElementAssignmentConflicts = value; InspectorPropertyChanged(); } }
        public bool allowElementAssignmentSwap { get { return _allowElementAssignmentSwap; } set { _allowElementAssignmentSwap = value; InspectorPropertyChanged(); } }
        public int actionLabelWidth { get { return _actionLabelWidth; } set { _actionLabelWidth = value; InspectorPropertyChanged(true); } }
        public int keyboardColMaxWidth { get { return _keyboardColMaxWidth; } set { _keyboardColMaxWidth = value; InspectorPropertyChanged(true); } }
        public int mouseColMaxWidth { get { return _mouseColMaxWidth; } set { _mouseColMaxWidth = value; InspectorPropertyChanged(true); } }
        public int controllerColMaxWidth { get { return _controllerColMaxWidth; } set { _controllerColMaxWidth = value; InspectorPropertyChanged(true); } }
        public int inputRowHeight { get { return _inputRowHeight; } set { _inputRowHeight = value; InspectorPropertyChanged(true); } }
        public int inputColumnSpacing { get { return _inputColumnSpacing; } set { _inputColumnSpacing = value; InspectorPropertyChanged(true); } }
        public int inputRowCategorySpacing { get { return _inputRowCategorySpacing; } set { _inputRowCategorySpacing = value; InspectorPropertyChanged(true); } }
        public int invertToggleWidth { get { return _invertToggleWidth; } set { _invertToggleWidth = value; InspectorPropertyChanged(true); } }
        public int defaultWindowWidth { get { return _defaultWindowWidth; } set { _defaultWindowWidth = value; InspectorPropertyChanged(true); } }
        public int defaultWindowHeight { get { return _defaultWindowHeight; } set { _defaultWindowHeight = value; InspectorPropertyChanged(true); } }
        public float controllerAssignmentTimeout { get { return _controllerAssignmentTimeout; } set { _controllerAssignmentTimeout = value; InspectorPropertyChanged(); } }
        public float preInputAssignmentTimeout { get { return _preInputAssignmentTimeout; } set { _preInputAssignmentTimeout = value; InspectorPropertyChanged(); } }
        public float inputAssignmentTimeout { get { return _inputAssignmentTimeout; } set { _inputAssignmentTimeout = value; InspectorPropertyChanged(); } }
        public float axisCalibrationTimeout { get { return _axisCalibrationTimeout; } set { _axisCalibrationTimeout = value; InspectorPropertyChanged(); } }
        public bool ignoreMouseXAxisAssignment { get { return _ignoreMouseXAxisAssignment; } set { _ignoreMouseXAxisAssignment = value; InspectorPropertyChanged(); } }
        public bool ignoreMouseYAxisAssignment { get { return _ignoreMouseYAxisAssignment; } set { _ignoreMouseYAxisAssignment = value; InspectorPropertyChanged(); } }
        //public int screenToggleAction { get { return _screenToggleAction; } set { _screenToggleAction = value; InspectorPropertyChanged(); } }
        //public int screenOpenAction { get { return _screenOpenAction; } set { _screenOpenAction = value; InspectorPropertyChanged(); } }
        //public int screenCloseAction { get { return _screenCloseAction; } set { _screenCloseAction = value; InspectorPropertyChanged(); } }
        //public int universalCancelAction { get { return _universalCancelAction; } set { _universalCancelAction = value; InspectorPropertyChanged(); } }
        public bool universalCancelClosesScreen { get { return _universalCancelClosesScreen; } set { _universalCancelClosesScreen = value; InspectorPropertyChanged(); } }
        public bool showInputBehaviorSettings { get { return _showInputBehaviorSettings; } set { _showInputBehaviorSettings = value; InspectorPropertyChanged(true); } }
        public bool useThemeSettings { get { return _useThemeSettings; } set { _useThemeSettings = value; InspectorPropertyChanged(true); } }
        public LanguageDataBase language { get { return _language; } set { _language = value; if(_language != null) _language.Initialize(); InspectorPropertyChanged(true); } }

        public bool showPlayersGroupLabel { get { return _showPlayersGroupLabel; } set { _showPlayersGroupLabel = value; InspectorPropertyChanged(true); } }
        public bool showControllerGroupLabel { get { return _showControllerGroupLabel; } set { _showControllerGroupLabel = value; InspectorPropertyChanged(true); } }
        public bool showAssignedControllersGroupLabel { get { return _showAssignedControllersGroupLabel; } set { _showAssignedControllersGroupLabel = value; InspectorPropertyChanged(true); } }
        public bool showSettingsGroupLabel { get { return _showSettingsGroupLabel; } set { _showSettingsGroupLabel = value; InspectorPropertyChanged(true); } }
        public bool showMapCategoriesGroupLabel { get { return _showMapCategoriesGroupLabel; } set { _showMapCategoriesGroupLabel = value; InspectorPropertyChanged(true); } }
        public bool showControllerNameLabel { get { return _showControllerNameLabel; } set { _showControllerNameLabel = value; InspectorPropertyChanged(true); } }
        public bool showAssignedControllers { get { return _showAssignedControllers; } set { _showAssignedControllers = value; InspectorPropertyChanged(true); } }

        public System.Action restoreDefaultsDelegate { get { return _restoreDefaultsDelegate; } set { _restoreDefaultsDelegate = value; } }

        #endregion

        public bool isOpen {
            get {
                if(!initialized) return references.canvas != null ? references.canvas.gameObject.activeInHierarchy : false;
                return canvas.activeInHierarchy;
            }
        }

        private bool isFocused {
            get {
                if(!initialized) return false;
                return !windowManager.isWindowOpen;
            }
        }

        private bool inputAllowed {
            get {
                if(blockInputOnFocusEndTime > Time.unscaledTime) return false;
                return true;
            }
        }

        private int inputGridColumnCount {
            get {
                int count = 1; // action label is always displayed
                if(_showKeyboard) count++;
                if(_showMouse) count++;
                if(_showControllers) count++;
                return count;
            }
        }

        private int inputGridWidth {
            get {
                return _actionLabelWidth + (_showKeyboard ? _keyboardColMaxWidth : 0) + (_showMouse ? _mouseColMaxWidth : 0) + (_showControllers ? _controllerColMaxWidth : 0) + ((inputGridColumnCount - 1) * _inputColumnSpacing);
            }
        }

        private Player currentPlayer {
            get {
                return ReInput.players.GetPlayer(currentPlayerId);
            }
        }

        private InputCategory currentMapCategory {
            get {
                return ReInput.mapping.GetMapCategory(currentMapCategoryId);
            }
        }

        private MappingSet currentMappingSet {
            get {
                if(currentMapCategoryId < 0) return null;
                for(int i = 0; i < _mappingSets.Length; i++) {
                    if(_mappingSets[i].mapCategoryId == currentMapCategoryId) return _mappingSets[i];
                }
                return null;
            }
        }

        private Joystick currentJoystick { get { return ReInput.controllers.GetJoystick(currentJoystickId); } }

        private bool isJoystickSelected { get { return currentJoystickId >= 0; } }

        private GameObject currentUISelection { get { return EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null; } }

        private bool showSettings {
            get {
                return _showInputBehaviorSettings && _inputBehaviorSettings.Length > 0;
            }
        }

        private bool showMapCategories {
            get {
                if(_mappingSets == null) return false;
                if(_mappingSets.Length <= 1) return false;
                return true;
            }
        }

        #endregion

        #region MonoBehaviour Events

        void Awake() {
            if(_dontDestroyOnLoad) {
                DontDestroyOnLoad(transform.gameObject);
            }

            PreInitialize();

            // Open immediately if instantiated with canvas active
            if(isOpen) {
                Initialize();
                Open(true);
            }
        }

        void Start() {
            if(_openOnStart) Open(false);
        }

        void Update() {
#if UNITY_EDITOR
            CheckEditorRecompile();
#endif

            if(!isOpen) return;
            if(!initialized) return;

            // Make sure the selection is never totally lost for joystick only users
            CheckUISelection();
        }

        void OnDestroy() {
            // Remove all events
            ReInput.ControllerConnectedEvent -= OnJoystickConnected;
            ReInput.ControllerDisconnectedEvent -= OnJoystickDisconnected;
            ReInput.ControllerPreDisconnectEvent -= OnJoystickPreDisconnect;
#if UNITY_EDITOR
            ReInput.EditorRecompileEvent -= OnEditorRecompile;
#endif
            UnsubscribeMenuControlInputEvents();
        }

        #endregion

        #region Initialization

        private void PreInitialize() {
            if(!ReInput.isReady) {
                Debug.LogError("Rewired Control Mapper: Rewired has not been initialized! Are you missing a Rewired Input Manager in your scene?");
                return;
            }

            // Set up menu control input events
            SubscribeMenuControlInputEvents();
        }

        private void Initialize() {
            if(initialized) return;
            if(!ReInput.isReady) return;

            if(_rewiredInputManager == null) {
                _rewiredInputManager = Object.FindObjectOfType<Rewired.InputManager>();
                if(_rewiredInputManager == null) {
                    Debug.LogError("Rewired Control Mapper: A Rewired Input Manager was not assigned in the inspector or found in the current scene! Control Mapper will not function.");
                    return;
                }
            }

            // Set up singleton
            if(ControlMapper.Instance != null) {
                Debug.LogError("Rewired Control Mapper: Only one ControlMapper can exist at one time!");
                return;
            }
            ControlMapper.Instance = this;

            // Check inspector vars
            if(prefabs == null || !prefabs.Check()) {
                Debug.LogError("Rewired Control Mapper: All prefabs must be assigned in the inspector!");
                return;
            }
            if(references == null || !references.Check()) {
                Debug.LogError("Rewired Control Mapper: All references must be assigned in the inspector!");
                return;
            }
            references.inputGridLayoutElement = references.inputGridContainer.GetComponent<LayoutElement>();
            if(references.inputGridLayoutElement == null) {
                Debug.LogError("Rewired Control Mapper: InputGridContainer is missing LayoutElement component!");
                return;
            }

            if(_showKeyboard && _keyboardInputFieldCount < 1) {
                Debug.LogWarning("Rewired Control Mapper: Keyboard Input Fields must be at least 1!");
                _keyboardInputFieldCount = 1;
            }
            if(_showMouse && _mouseInputFieldCount < 1) {
                Debug.LogWarning("Rewired Control Mapper: Mouse Input Fields must be at least 1!");
                _mouseInputFieldCount = 1;
            }
            if(_showControllers && _controllerInputFieldCount < 1) {
                Debug.LogWarning("Rewired Control Mapper: Controller Input Fields must be at least 1!");
                _controllerInputFieldCount = 1;
            }
            if(_maxControllersPerPlayer < 0) {
                Debug.LogWarning("Rewired Control Mapper: Max Controllers Per Player must be at least 0 (no limit)!");
                _maxControllersPerPlayer = 0;
            }
            if(_useThemeSettings && _themeSettings == null) {
                Debug.LogWarning("Rewired Control Mapper: To use theming, Theme Settings must be set in the inspector! Theming has been disabled.");
                _useThemeSettings = false;
            }
            if(_language == null) {
                Debug.LogError("Rawired UI: Language must be set in the inspector!");
                return;
            }

            // Initialize objects
            _language.Initialize();

            // Create delegates
            inputFieldActivatedDelegate = OnInputFieldActivated;
            inputFieldInvertToggleStateChangedDelegate = OnInputFieldInvertToggleStateChanged;

            // Register for events
            ReInput.ControllerConnectedEvent += OnJoystickConnected;
            ReInput.ControllerDisconnectedEvent += OnJoystickDisconnected;
            ReInput.ControllerPreDisconnectEvent += OnJoystickPreDisconnect;
#if UNITY_EDITOR
            ReInput.EditorRecompileEvent += OnEditorRecompile;
#endif

            // Get the number of players
            playerCount = ReInput.players.playerCount;

            // Store references
            canvas = references.canvas.gameObject;

            // Create objects
            windowManager = new WindowManager(prefabs.window, prefabs.fader, references.canvas.transform);
            playerButtons = new List<GUIButton>();
            mapCategoryButtons = new List<GUIButton>();
            assignedControllerButtons = new List<GUIButton>();
            miscInstantiatedObjects = new List<GameObject>();

            // Set default values
            currentMapCategoryId = _mappingSets[0].mapCategoryId;

            // Draw generated UI items
            Draw();

            // Create input grid
            CreateInputGrid();

            // Create the layout
            CreateLayout();

            // Set up events in fixed UI elements
            SubscribeFixedUISelectionEvents();

            initialized = true;
        }

        #endregion

        #region Controller Connect / Disconnect Event Handlers

        private void OnJoystickConnected(ControllerStatusChangedEventArgs args) {
            if(!initialized) return;
            if(!_showControllers) return;
            ClearVarsOnJoystickChange();
            ForceRefresh();
        }

        private void OnJoystickDisconnected(ControllerStatusChangedEventArgs args) {
            if(!initialized) return;
            if(!_showControllers) return;
            ClearVarsOnJoystickChange();
            ForceRefresh();
        }

        private void OnJoystickPreDisconnect(ControllerStatusChangedEventArgs args) {
            if(!initialized) return;
            if(!_showControllers) return;
        }

        #endregion

        #region Main Window Event Handlers

        // Public - UI Buttons reference and call these directly

        public void OnButtonActivated(ButtonInfo buttonInfo) {
            if(!initialized) return;
            if(!inputAllowed) return;

            switch(buttonInfo.identifier) {
                case buttonIdentifier_playerSelection: // player selection
                    OnPlayerSelected(buttonInfo.intData, true);
                    break;
                case buttonIdentifier_assignedControllerSelection:
                    OnControllerSelected(buttonInfo.intData);
                    break;
                case buttonIdentifier_removeController:
                    OnRemoveCurrentController();
                    break;
                case buttonIdentifier_assignController:
                    ShowAssignControllerWindow();
                    break;
                case buttonIdentifier_calibrateController:
                    ShowCalibrateControllerWindow();
                    break;
                case buttonIdentifier_editInputBehaviors:
                    ShowEditInputBehaviorsWindow();
                    break;
                case buttonIdentifier_mapCategorySelection:
                    OnMapCategorySelected(buttonInfo.intData, true);
                    break;
                case buttonIdentifier_done:
                    Close(true);
                    break;
                case buttonIdentifier_restoreDefaults:
                    OnRestoreDefaults();
                    break;
            }
        }

        public void OnInputFieldActivated(InputFieldInfo fieldInfo) {
            if(!initialized) return;
            if(!inputAllowed) return;
            if(currentPlayer == null) return;

            InputAction action = ReInput.mapping.GetAction(fieldInfo.actionId);
            if(action == null) return;

            AxisRange range = action.type == InputActionType.Axis ? fieldInfo.axisRange : AxisRange.Full;
            string actionName = _language.GetActionName(action.id, range);

            ControllerMap map = GetControllerMap(fieldInfo.controllerType);
            if(map == null) return;

            ActionElementMap aem = fieldInfo.actionElementMapId >= 0 ? map.GetElementMap(fieldInfo.actionElementMapId) : null;

            if(aem != null) { // element replacement dialog
                ShowBeginElementAssignmentReplacementWindow(fieldInfo, action, map, aem, actionName);
            } else { // create new element dialog
                ShowCreateNewElementAssignmentWindow(fieldInfo, action, map, actionName);
            }
        }

        public void OnInputFieldInvertToggleStateChanged(ToggleInfo toggleInfo, bool newState) {
            if(!initialized) return;
            if(!inputAllowed) return;

            SetActionAxisInverted(newState, toggleInfo.controllerType, toggleInfo.actionElementMapId);
        }

        // Private

        private void OnPlayerSelected(int playerId, bool redraw) {
            if(!initialized) return;
            currentPlayerId = playerId;
            ClearVarsOnPlayerChange();
            if(redraw) Redraw(true, true);
        }

        private void OnControllerSelected(int joystickId) {
            if(!initialized) return;

            currentJoystickId = joystickId;
            Redraw(true, true);
        }

        private void OnRemoveCurrentController() {
            if(currentPlayer == null) return;
            if(currentJoystickId < 0) return;
            RemoveController(currentPlayer, currentJoystickId);
            ClearVarsOnJoystickChange();
            Redraw(false, false);
        }

        private void OnMapCategorySelected(int id, bool redraw) {
            if(!initialized) return;
            currentMapCategoryId = id;
            if(redraw) Redraw(true, true);
        }

        private void OnRestoreDefaults() {
            if(!initialized) return;
            ShowRestoreDefaultsWindow();
        }

        private void OnScreenToggleActionPressed(InputActionEventData data) {
            if(!isOpen) { // mapper is not open, open it
                Open();
                return;
            }

            if(!initialized) return;

            if(!isFocused) return; // a window is open, so do nothing
            Close(true); // no window is open, close the mapper
        }

        private void OnScreenOpenActionPressed(InputActionEventData data) {
            Open();
        }

        private void OnScreenCloseActionPressed(InputActionEventData data) {
            if(!initialized) return;
            if(!isOpen) return;
            if(!isFocused) return; // a window is open, so do nothing
            Close(true); // no window is open, close the mapper
        }

        private void OnUniversalCancelActionPressed(InputActionEventData data) {
            if(!initialized) return;
            if(!isOpen) return;
            if(_universalCancelClosesScreen) {
                if(isFocused) {
                    Close(true); // close the main screen
                    return;
                }
            } else if(isFocused) { // no window is open, so do nothing
                return;
            }
            CloseAllWindows();
        }

        #endregion

        #region Popup Window Button Callbacks

        private void OnWindowCancel(int windowId) {
            if(!initialized) return;
            if(windowId < 0) return;
            CloseWindow(windowId);
        }

        private void OnRemoveElementAssignment(int windowId, ControllerMap map, ActionElementMap aem) {
            if(map == null || aem == null) return;
            map.DeleteElementMap(aem.id);
            CloseWindow(windowId);
        }

        private void OnBeginElementAssignment(InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, string actionName) {
            if(fieldInfo == null || map == null) return;

            // Store the mapping information
            pendingInputMapping = new InputMapping(actionName, fieldInfo, map, aem, fieldInfo.controllerType, fieldInfo.controllerId);

            switch(fieldInfo.controllerType) {
                case ControllerType.Joystick:
                    ShowElementAssignmentPrePollingWindow();
                    break;
                case ControllerType.Keyboard:
                    ShowElementAssignmentPollingWindow();
                    break;
                case ControllerType.Mouse:
                    ShowElementAssignmentPollingWindow();
                    break;
                default: throw new System.NotImplementedException();
            }
        }

        private void OnControllerAssignmentConfirmed(int windowId, Player player, int controllerId) {
            if(windowId < 0 || player == null || controllerId < 0) return;

            // Assign the joystick
            AssignController(player, controllerId);

            // Close the window
            CloseWindow(windowId);
        }

        private void OnMouseAssignmentConfirmed(int windowId, Player player) {
            if(windowId < 0 || player == null) return;

            // Remove mouse from all normal Players
            IList<Player> playes = ReInput.players.Players;
            for(int i = 0; i < playes.Count; i++) {
                if(playes[i] == player) continue; // skip self
                playes[i].controllers.hasMouse = false;
            }

            // Assign the controller
            player.controllers.hasMouse = true;
            CloseWindow(windowId);
        }

        private void OnElementAssignmentConflictReplaceConfirmed(int windowId, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers, bool allowSwap) {
            if(currentPlayer == null || mapping == null) return;
            ElementAssignmentConflictCheck conflictCheck;
            if(!CreateConflictCheck(mapping, assignment, out conflictCheck)) {
                Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
                CloseWindow(windowId);
                return;
            }

            ElementAssignmentConflictInfo firstConflict = new ElementAssignmentConflictInfo();
            ActionElementMap firstConflictAEM = null;
            ActionElementMap origAemToReplaceCopy = null;
            bool swap = false;

            // Check for conflicts for swapping
            if(allowSwap && mapping.aem != null) { // cannot swap if this is not a replacement mapping
                // Find the first conflict
                if(GetFirstElementAssignmentConflict(conflictCheck, out firstConflict, skipOtherPlayers)) {
                    swap = true;
                    origAemToReplaceCopy = new ActionElementMap(mapping.aem); // create a copy of the mapping because the original will be modified before we can use it
                    firstConflictAEM = new ActionElementMap(firstConflict.elementMap); // store this because it will no longer be accessible once the AEM is removed
                }
            }

            // Remove conflicting mappings
            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) {
                Player player = allPlayers[i];
                if(skipOtherPlayers && player != currentPlayer && player != ReInput.players.SystemPlayer) continue; // skip other players, only remove from system and self
                player.controllers.conflictChecking.RemoveElementAssignmentConflicts(conflictCheck);
            }

            // Create the new mapping or replace existing
            mapping.map.ReplaceOrCreateElementMap(assignment);

            // Handle assignment swapping
            if(allowSwap && swap) {

                // Take the Action and some properties from the conflict
                int swapActionId = firstConflictAEM.actionId;
                Pole swapAxisContribution = firstConflictAEM.axisContribution;
                bool swapInvert = firstConflictAEM.invert;
                // Take properties from the original
                AxisRange swapAxisRange = origAemToReplaceCopy.axisRange;
                ControllerElementType swapElementType = origAemToReplaceCopy.elementType;
                int swapElementIdentifierId = origAemToReplaceCopy.elementIdentifierId;
                KeyCode swapKeyCode = origAemToReplaceCopy.keyCode;
                ModifierKeyFlags swapModifierKeyFlags = origAemToReplaceCopy.modifierKeyFlags;

                if(swapElementType == firstConflictAEM.elementType && swapElementType == ControllerElementType.Axis) {
                    if(swapAxisRange != firstConflictAEM.axisRange) {
                        if(swapAxisRange == AxisRange.Full) { // swapping full-axis into a split-axis mapping
                            swapAxisRange = AxisRange.Positive; // just make it positive. Triggers would be a problem if trying to match the field pole.
                        } else if(firstConflictAEM.axisRange == AxisRange.Full) { // swapping split-axis into a full-axis mapping
                            // do nothing, making this a full mapping would too easily create new conflicts.
                            // the existing split axis will work if it fits
                        }
                    }

                } else if(swapElementType == ControllerElementType.Axis) {

                    if(firstConflictAEM.elementType == ControllerElementType.Button || (firstConflictAEM.elementType == ControllerElementType.Axis && firstConflictAEM.axisRange != AxisRange.Full)) {
                        // Make sure Axis is split and only one side of it is bound to the Action
                        if(swapAxisRange == AxisRange.Full) {
                            swapAxisRange = AxisRange.Positive; // just bind the positive side of the Axis
                        }
                    }
                }

                if(swapElementType != ControllerElementType.Axis || swapAxisRange != AxisRange.Full) swapInvert = false;

                // Determine if there is space for the new swapped assignment
                // Prevent swap from creating a mapping that the user cannot see because the input fields
                // are already full. This can happen with a swap between a full-axis mapping and a button mapping.
                int usedFieldCount = 0;

                // Count how many mappings already exist in the same mapping range
                foreach(var aem in firstConflict.controllerMap.ElementMapsWithAction(swapActionId)) {
                    if(
                        SwapIsSameInputRange(
                            swapElementType,
                            swapAxisRange,
                            swapAxisContribution,
                            aem.elementType,
                            aem.axisRange,
                            aem.axisContribution
                        )
                    ) {
                        usedFieldCount++;
                    }
                }

                if(usedFieldCount < GetControllerInputFieldCount(mapping.controllerType)) { // there are unused fields, create it
                    // Create the new swap assignment
                    firstConflict.controllerMap.ReplaceOrCreateElementMap(
                        ElementAssignment.CompleteAssignment(
                            mapping.controllerType,
                            swapElementType,
                            swapElementIdentifierId,
                            swapAxisRange,
                            swapKeyCode,
                            swapModifierKeyFlags,
                            swapActionId,
                            swapAxisContribution,
                            swapInvert
                        )
                    );
                } // otherwise, the swap mapping will be dropped
            }

            CloseWindow(windowId);
        }

        private void OnElementAssignmentAddConfirmed(int windowId, InputMapping mapping, ElementAssignment assignment) {
            if(currentPlayer == null || mapping == null) return;

            // Create the new mapping or replace existing
            mapping.map.ReplaceOrCreateElementMap(assignment);
            CloseWindow(windowId);
        }

        private void OnRestoreDefaultsConfirmed(int windowId) {
            if(_restoreDefaultsDelegate == null) {
                IList<Player> players = ReInput.players.Players;
                for(int i = 0; i < players.Count; i++) {
                    Player player = players[i];
                    if(_showControllers) player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
                    if(_showKeyboard) player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
                    if(_showMouse) player.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);
                }
            }
            CloseWindow(windowId);
            if(_restoreDefaultsDelegate != null) {
                _restoreDefaultsDelegate();
            }
        }

        #endregion

        #region Popup Window Update Callbacks

        private void OnAssignControllerWindowUpdate(int windowId) {
            if(currentPlayer == null) return;

            Window window = windowManager.GetWindow(windowId);
            if(windowId < 0) return;

            InputPollingStarted();

            // Check the close window timer
            if(window.timer.finished) { // timer expired
                InputPollingStopped();
                CloseWindow(windowId); // close the window
                return;
            }

            // Poll for controller element down
            ControllerPollingInfo info = ReInput.controllers.polling.PollAllControllersOfTypeForFirstElementDown(ControllerType.Joystick);
            if(info.success) {

                InputPollingStopped();

                // Check if another Player has this controller already
                if(ReInput.controllers.IsControllerAssigned(ControllerType.Joystick, info.controllerId) &&
                    !currentPlayer.controllers.ContainsController(ControllerType.Joystick, info.controllerId)) { // another player has the controller
                    ShowControllerAssignmentConflictWindow(info.controllerId);
                    return;
                }

                // Assign the controller to this Player
                OnControllerAssignmentConfirmed(windowId, currentPlayer, info.controllerId);
                return;
            }

            // Show the window close timer
            window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
        }

        private void OnElementAssignmentPrePollingWindowUpdate(int windowId) {
            if(currentPlayer == null) return;

            Window window = windowManager.GetWindow(windowId);
            if(windowId < 0) return;

            if(pendingInputMapping == null) return;

            InputPollingStarted();

            // Check the close window timer
            if(window.timer.finished) { // timer expired
                goto Success; // when timer expires, go to next step to support no-button controllers like pedals
            }

            // Show the window close timer
            window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);

            ControllerPollingInfo pollingInfo;

            switch(pendingInputMapping.controllerType) {
                case ControllerType.Joystick:
                    if(currentPlayer.controllers.joystickCount == 0) return;
                    pollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(pendingInputMapping.controllerType, currentJoystick.id);
                    break;
                case ControllerType.Keyboard:
                case ControllerType.Mouse:
                    pollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(pendingInputMapping.controllerType, 0);
                    break;
                default: throw new System.NotImplementedException();
            }

            if(!pollingInfo.success) return;

            Success:
            ShowElementAssignmentPollingWindow();
        }

        private void OnJoystickElementAssignmentPollingWindowUpdate(int windowId) {
            if(currentPlayer == null) return;

            Window window = windowManager.GetWindow(windowId);
            if(windowId < 0) return;

            if(pendingInputMapping == null) return;

            InputPollingStarted();

            // Check the close window timer
            if(window.timer.finished) { // timer expired
                InputPollingStopped();
                CloseWindow(windowId); // close the window
                return;
            }

            // Show the window close timer
            window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);

            if(currentPlayer.controllers.joystickCount == 0) return;

            ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
            if(!pollingInfo.success) return;

            // Verify that this assignment is allowed
            if(!IsAllowedAssignment(pendingInputMapping, pollingInfo)) return;

            ElementAssignment assignment = pendingInputMapping.ToElementAssignment(pollingInfo);

            if(!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, assignment, false)) {
                pendingInputMapping.map.ReplaceOrCreateElementMap(assignment);
                InputPollingStopped();
                CloseWindow(windowId);
            } else {
                InputPollingStopped();
                ShowElementAssignmentConflictWindow(assignment, false);
            }
        }

        private void OnKeyboardElementAssignmentPollingWindowUpdate(int windowId) {
            if(currentPlayer == null) return;

            Window window = windowManager.GetWindow(windowId);
            if(windowId < 0) return;

            if(pendingInputMapping == null) return;

            InputPollingStarted();

            // Check the close window timer
            if(window.timer.finished) { // timer expired
                InputPollingStopped();
                CloseWindow(windowId); // close the window
                return;
            }

            ControllerPollingInfo pollingInfo;
            ModifierKeyFlags modifierFlags;
            bool modifierKeyPressed;
            string label;

            PollKeyboardForAssignment(out pollingInfo, out modifierKeyPressed, out modifierFlags, out label);

            if(modifierKeyPressed) window.timer.Start(_inputAssignmentTimeout); // reset close timer if a modifier key is pressed

            // Show the window close timer
            window.SetContentText(modifierKeyPressed ? string.Empty : Mathf.CeilToInt(window.timer.remaining).ToString(), 2);

            // Show the modifier key label
            window.SetContentText(label, 1);

            if(!pollingInfo.success) return;

            // Verify that this assignment is allowed
            if(!IsAllowedAssignment(pendingInputMapping, pollingInfo)) return;

            ElementAssignment assignment = pendingInputMapping.ToElementAssignment(pollingInfo, modifierFlags);

            if(!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, assignment, false)) {
                pendingInputMapping.map.ReplaceOrCreateElementMap(assignment);
                InputPollingStopped();
                CloseWindow(windowId);
            } else {
                InputPollingStopped();
                ShowElementAssignmentConflictWindow(assignment, false);
            }
        }

        private void OnMouseElementAssignmentPollingWindowUpdate(int windowId) {
            if(currentPlayer == null) return;

            Window window = windowManager.GetWindow(windowId);
            if(windowId < 0) return;

            if(pendingInputMapping == null) return;

            InputPollingStarted();

            // Check the close window timer
            if(window.timer.finished) { // timer expired
                InputPollingStopped();
                CloseWindow(windowId); // close the window
                return;
            }

            // Show the window close timer
            window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);

            ControllerPollingInfo pollingInfo;

            if(_ignoreMouseXAxisAssignment || _ignoreMouseYAxisAssignment) {
                pollingInfo = new ControllerPollingInfo();
                foreach(ControllerPollingInfo p in ReInput.controllers.polling.PollControllerForAllElementsDown(ControllerType.Mouse, 0)) {
                    if(p.elementType == ControllerElementType.Axis) {
                        if(_ignoreMouseXAxisAssignment && p.elementIndex == 0) continue; // skip X
                        else if(_ignoreMouseYAxisAssignment && p.elementIndex == 1) continue; // skip Y
                    }
                    pollingInfo = p;
                    break;
                }
            } else {
                pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Mouse, 0);
            }
            if(!pollingInfo.success) return;

            // Verify that the assignment is allowed
            if(!IsAllowedAssignment(pendingInputMapping, pollingInfo)) return;

            ElementAssignment assignment = pendingInputMapping.ToElementAssignment(pollingInfo);

            if(!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, assignment, true)) {
                pendingInputMapping.map.ReplaceOrCreateElementMap(assignment);
                InputPollingStopped();
                CloseWindow(windowId);
            } else {
                InputPollingStopped();
                ShowElementAssignmentConflictWindow(assignment, true);
            }
        }

        private void OnCalibrateAxisStep1WindowUpdate(int windowId) {
            if(currentPlayer == null) return;

            Window window = windowManager.GetWindow(windowId);
            if(windowId < 0) return;

            if(pendingAxisCalibration == null || !pendingAxisCalibration.isValid) return;

            InputPollingStarted();

            // Check the close window timer
            if(window.timer.finished) { // timer expired
                goto Success; // when timer expires, go to next step to support no-button controllers like pedals
            }

            // Show the window close timer
            window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);

            // Poll for button press for confirmation
            if(currentPlayer.controllers.joystickCount == 0) return;
            ControllerPollingInfo pollingInfo = pendingAxisCalibration.joystick.PollForFirstButtonDown();

            if(!pollingInfo.success) return;

            Success:
            pendingAxisCalibration.RecordZero(); // record zero value
            CloseWindow(windowId); // close this window
            ShowCalibrateAxisStep2Window();
        }

        private void OnCalibrateAxisStep2WindowUpdate(int windowId) {
            if(currentPlayer == null) return;

            Window window = windowManager.GetWindow(windowId);
            if(windowId < 0) return;

            if(pendingAxisCalibration == null || !pendingAxisCalibration.isValid) return;

            // Check the close window timer
            if(window.timer.finished) { // timer expired
                goto Success; // when timer expires, go to next step to support no-button controllers like pedals
            }

            // Show the window close timer
            window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);

            // Record min/max values as user moves the axis
            pendingAxisCalibration.RecordMinMax();

            // Poll for button press for confirmation
            if(currentPlayer.controllers.joystickCount == 0) return;
            ControllerPollingInfo pollingInfo = pendingAxisCalibration.joystick.PollForFirstButtonDown();

            if(!pollingInfo.success) return;

            Success:
            EndAxisCalibration(); // commit the calibration
            InputPollingStopped();
            CloseWindow(windowId); // close this window
        }

        #endregion

        #region Show Specific Popup Windows

        private void ShowAssignControllerWindow() {
            if(currentPlayer == null) return;
            if(ReInput.controllers.joystickCount == 0) return;

            Window window = OpenWindow(true);
            if(window == null) return;

            window.SetUpdateCallback(OnAssignControllerWindowUpdate);
            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.assignControllerWindowTitle);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), _language.assignControllerWindowMessage);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomHStretch, Vector2.zero, "");
            window.timer.Start(_controllerAssignmentTimeout);
            windowManager.Focus(window);
        }

        private void ShowControllerAssignmentConflictWindow(int controllerId) {
            if(currentPlayer == null) return;
            if(ReInput.controllers.joystickCount == 0) return;

            Window window = OpenWindow(true);
            if(window == null) return;

            string otherPlayer = string.Empty;
            IList<Player> players = ReInput.players.Players;
            for(int i = 0; i < players.Count; i++) {
                if(players[i] == currentPlayer) continue; // skip self
                if(!players[i].controllers.ContainsController(ControllerType.Joystick, controllerId)) continue;
                otherPlayer = _language.GetPlayerName(players[i].id);
                break;
            }

            Joystick joystick = ReInput.controllers.GetJoystick(controllerId);

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.controllerAssignmentConflictWindowTitle);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), _language.GetControllerAssignmentConflictWindowMessage(_language.GetControllerName(joystick), otherPlayer, _language.GetPlayerName(currentPlayer.id)));
            UnityAction cancelCallback = () => { OnWindowCancel(window.id); };
            window.cancelCallback = cancelCallback;
            window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomLeft, UI.UIAnchor.BottomLeft, Vector2.zero, _language.yes, () => { OnControllerAssignmentConfirmed(window.id, currentPlayer, controllerId); }, cancelCallback, true);
            window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomRight, UI.UIAnchor.BottomRight, Vector2.zero, _language.no, cancelCallback, cancelCallback, false);
            windowManager.Focus(window);
        }

        private void ShowBeginElementAssignmentReplacementWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, ActionElementMap aem, string actionName) {
            GUIInputField field = inputGrid.GetGUIInputField(currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData);
            if(field == null) return;

            // Open window and set properties
            Window window = OpenWindow(true);
            if(window == null) return;

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, actionName);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), field.GetLabel());
            UnityAction cancelCallback = () => { OnWindowCancel(window.id); };
            window.cancelCallback = cancelCallback;
            window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomLeft, UI.UIAnchor.BottomLeft, Vector2.zero, _language.replace, () => { OnBeginElementAssignment(fieldInfo, map, aem, actionName); }, cancelCallback, true);
            window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomCenter, Vector2.zero, _language.remove, () => { OnRemoveElementAssignment(window.id, map, aem); }, cancelCallback, false);
            window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomRight, UI.UIAnchor.BottomRight, Vector2.zero, _language.cancel, cancelCallback, cancelCallback, false);
            windowManager.Focus(window);
        }

        private void ShowCreateNewElementAssignmentWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, string actionName) {
            GUIInputField field = inputGrid.GetGUIInputField(currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData);
            if(field == null) return;
            OnBeginElementAssignment(fieldInfo, map, null, actionName);
        }

        private void ShowElementAssignmentPrePollingWindow() {
            if(pendingInputMapping == null) return;

            // Open window and set properties
            Window window = OpenWindow(true);
            if(window == null) return;

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), _language.elementAssignmentPrePollingWindowMessage);
            if(prefabs.centerStickGraphic != null) window.AddContentImage(prefabs.centerStickGraphic, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomCenter, new Vector2(0, 40));
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomHStretch, Vector2.zero, "");
            window.SetUpdateCallback(OnElementAssignmentPrePollingWindowUpdate);
            window.timer.Start(_preInputAssignmentTimeout);
            windowManager.Focus(window);
        }

        private void ShowElementAssignmentPollingWindow() {
            if(pendingInputMapping == null) return;

            switch(pendingInputMapping.controllerType) {
                case ControllerType.Joystick:
                    ShowJoystickElementAssignmentPollingWindow();
                    break;
                case ControllerType.Keyboard:
                    ShowKeyboardElementAssignmentPollingWindow();
                    break;
                case ControllerType.Mouse:
                    if(currentPlayer.controllers.hasMouse) ShowMouseElementAssignmentPollingWindow();
                    else ShowMouseAssignmentConflictWindow();
                    break;
                default: throw new System.NotImplementedException();
            }
        }

        private void ShowJoystickElementAssignmentPollingWindow() {
            if(pendingInputMapping == null) return;

            // Open window and set properties
            Window window = OpenWindow(true);
            if(window == null) return;

            string message = pendingInputMapping.axisRange == AxisRange.Full && _showFullAxisInputFields && !_showSplitAxisInputFields ?
                _language.GetJoystickElementAssignmentPollingWindowMessage_FullAxisFieldOnly(pendingInputMapping.actionName) :
                _language.GetJoystickElementAssignmentPollingWindowMessage(pendingInputMapping.actionName);

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), message);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomHStretch, Vector2.zero, "");
            window.SetUpdateCallback(OnJoystickElementAssignmentPollingWindowUpdate);
            window.timer.Start(_inputAssignmentTimeout);
            windowManager.Focus(window);
        }

        private void ShowKeyboardElementAssignmentPollingWindow() {
            if(pendingInputMapping == null) return;

            // Open window and set properties
            Window window = OpenWindow(true);
            if(window == null) return;

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), _language.GetKeyboardElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -(window.GetContentTextHeight(0) + 50.0f)), "");
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomHStretch, Vector2.zero, "");
            window.SetUpdateCallback(OnKeyboardElementAssignmentPollingWindowUpdate);
            window.timer.Start(_inputAssignmentTimeout);
            windowManager.Focus(window);
        }

        private void ShowMouseElementAssignmentPollingWindow() {
            if(pendingInputMapping == null) return;

            // Open window and set properties
            Window window = OpenWindow(true);
            if(window == null) return;

            string message = pendingInputMapping.axisRange == AxisRange.Full && _showFullAxisInputFields && !_showSplitAxisInputFields ?
                _language.GetMouseElementAssignmentPollingWindowMessage_FullAxisFieldOnly(pendingInputMapping.actionName) :
                _language.GetMouseElementAssignmentPollingWindowMessage(pendingInputMapping.actionName);

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), message);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomHStretch, Vector2.zero, "");
            window.SetUpdateCallback(OnMouseElementAssignmentPollingWindowUpdate);
            window.timer.Start(_inputAssignmentTimeout);
            windowManager.Focus(window);
        }

        private void ShowElementAssignmentConflictWindow(ElementAssignment assignment, bool skipOtherPlayers) {
            if(pendingInputMapping == null) return;

            // Determine what kind of conflict we had
            bool blocked = IsBlockingAssignmentConflict(pendingInputMapping, assignment, skipOtherPlayers);

            string message = blocked ?
                _language.GetElementAlreadyInUseBlocked(pendingInputMapping.elementName) :
                _language.GetElementAlreadyInUseCanReplace(pendingInputMapping.elementName, _allowElementAssignmentConflicts);

            // Open window and set properties
            Window window = OpenWindow(true);
            if(window == null) return;

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.elementAssignmentConflictWindowMessage);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), message);

            UnityAction cancelCallback = () => { OnWindowCancel(window.id); };
            window.cancelCallback = cancelCallback;

            if(blocked) {
                window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomCenter, Vector2.zero, _language.okay, cancelCallback, cancelCallback, true);
            } else {
                window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomLeft, UI.UIAnchor.BottomLeft, Vector2.zero, _language.replace, () => { OnElementAssignmentConflictReplaceConfirmed(window.id, pendingInputMapping, assignment, skipOtherPlayers, false); }, cancelCallback, true);

                if(_allowElementAssignmentConflicts) {
                    window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomCenter, Vector2.zero, _language.add, () => { OnElementAssignmentAddConfirmed(window.id, pendingInputMapping, assignment); }, cancelCallback, false);
                } else {
                    if(ShowSwapButton(window.id, pendingInputMapping, assignment, skipOtherPlayers)) {
                        window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomCenter, Vector2.zero, _language.swap, () => { OnElementAssignmentConflictReplaceConfirmed(window.id, pendingInputMapping, assignment, skipOtherPlayers, true); }, cancelCallback, false);
                    }
                }

                window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomRight, UI.UIAnchor.BottomRight, Vector2.zero, _language.cancel, cancelCallback, cancelCallback, false);
            }
            windowManager.Focus(window);
        }

        private void ShowMouseAssignmentConflictWindow() {
            if(currentPlayer == null) return;

            Window window = OpenWindow(true);
            if(window == null) return;

            string otherPlayer = string.Empty;
            IList<Player> players = ReInput.players.Players;
            for(int i = 0; i < players.Count; i++) {
                if(players[i] == currentPlayer) continue; // skip self
                if(!players[i].controllers.hasMouse) continue;
                otherPlayer = _language.GetPlayerName(players[i].id);
                break;
            }

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.mouseAssignmentConflictWindowTitle);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), _language.GetMouseAssignmentConflictWindowMessage(otherPlayer, _language.GetPlayerName(currentPlayer.id)));
            UnityAction cancelCallback = () => { OnWindowCancel(window.id); };
            window.cancelCallback = cancelCallback;
            window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomLeft, UI.UIAnchor.BottomLeft, Vector2.zero, _language.yes, () => { OnMouseAssignmentConfirmed(window.id, currentPlayer); }, cancelCallback, true);
            window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomRight, UI.UIAnchor.BottomRight, Vector2.zero, _language.no, cancelCallback, cancelCallback, false);
            windowManager.Focus(window);
        }

        private void ShowCalibrateControllerWindow() {
            if(currentPlayer == null) return;
            if(currentPlayer.controllers.joystickCount == 0) return;

            CalibrationWindow window = OpenWindow(prefabs.calibrationWindow, "CalibrationWindow", true) as CalibrationWindow;
            if(window == null) return;

            Joystick joystick = currentJoystick;

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateControllerWindowTitle);
            window.SetJoystick(currentPlayer.id, joystick);
            window.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Done, CloseWindow);
            window.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Calibrate, StartAxisCalibration);
            window.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Cancel, CloseWindow);
            windowManager.Focus(window);
        }

        private void ShowCalibrateAxisStep1Window() {
            if(currentPlayer == null) return;

            Window window = OpenWindow(false);
            if(window == null) return;

            if(pendingAxisCalibration == null) return;

            Joystick joystick = pendingAxisCalibration.joystick;
            if(joystick.axisCount == 0) return;

            int axisIndex = pendingAxisCalibration.axisIndex;
            if(axisIndex < 0 || axisIndex >= joystick.axisCount) return;

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateAxisStep1WindowTitle);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), _language.GetCalibrateAxisStep1WindowMessage(_language.GetElementIdentifierName(joystick, joystick.AxisElementIdentifiers[axisIndex].id, AxisRange.Full)));
            if(prefabs.centerStickGraphic != null) window.AddContentImage(prefabs.centerStickGraphic, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomCenter, new Vector2(0, 40));
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomHStretch, Vector2.zero, "");
            window.SetUpdateCallback(OnCalibrateAxisStep1WindowUpdate);
            window.timer.Start(_axisCalibrationTimeout);
            windowManager.Focus(window);
        }

        private void ShowCalibrateAxisStep2Window() {
            if(currentPlayer == null) return;

            Window window = OpenWindow(false);
            if(window == null) return;

            if(pendingAxisCalibration == null) return;

            Joystick joystick = pendingAxisCalibration.joystick;
            if(joystick.axisCount == 0) return;

            int axisIndex = pendingAxisCalibration.axisIndex;
            if(axisIndex < 0 || axisIndex >= joystick.axisCount) return;

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateAxisStep2WindowTitle);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), _language.GetCalibrateAxisStep2WindowMessage(_language.GetElementIdentifierName(joystick, joystick.AxisElementIdentifiers[axisIndex].id, AxisRange.Full)));
            if(prefabs.moveStickGraphic != null) window.AddContentImage(prefabs.moveStickGraphic, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomCenter, new Vector2(0, 40));
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.BottomCenter, UI.UIAnchor.BottomHStretch, Vector2.zero, "");
            window.SetUpdateCallback(OnCalibrateAxisStep2WindowUpdate);
            window.timer.Start(_axisCalibrationTimeout);
            windowManager.Focus(window);
        }

        private void ShowEditInputBehaviorsWindow() {
            if(currentPlayer == null) return;
            if(_inputBehaviorSettings == null) return;

            InputBehaviorWindow window = OpenWindow(prefabs.inputBehaviorsWindow, "EditInputBehaviorsWindow", true) as InputBehaviorWindow;
            if(window == null) return;

            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.inputBehaviorSettingsWindowTitle);
            window.SetData(currentPlayer.id, _inputBehaviorSettings);
            window.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Done, CloseWindow);
            window.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Cancel, CloseWindow);
            windowManager.Focus(window);
        }

        private void ShowRestoreDefaultsWindow() {
            if(currentPlayer == null) return;
            OpenModal(
                _language.restoreDefaultsWindowTitle,
                _language.restoreDefaultsWindowMessage,
                _language.yes,
                OnRestoreDefaultsConfirmed,
                _language.no,
                OnWindowCancel,
                true
            );
        }

        #endregion

        #region Input Field Grid

        private void CreateInputGrid() {
            InitializeInputGrid();

            // Create the column groups
            CreateHeaderLabels();
            CreateActionLabelColumn();
            CreateKeyboardInputFieldColumn();
            CreateMouseInputFieldColumn();
            CreateControllerInputFieldColumn();

            // Create GUI Elements
            CreateInputActionLabels();
            CreateInputFields();

            // Disable all GUI elements initially
            inputGrid.HideAll();

            // Fix Unity scroll bar bug
            ResetInputGridScrollBar();
        }

        private void InitializeInputGrid() {
            if(inputGrid == null) inputGrid = new InputGrid();
            else inputGrid.ClearAll();

            // Add all user-assignable actions to the grid
            for(int i = 0; i < _mappingSets.Length; i++) {
                MappingSet set = _mappingSets[i];
                if(set == null || !set.isValid) continue;

                InputMapCategory mapCat = ReInput.mapping.GetMapCategory(set.mapCategoryId);
                if(mapCat == null) continue;
                if(!mapCat.userAssignable) continue;

                inputGrid.AddMapCategory(set.mapCategoryId);

                if(set.actionListMode == MappingSet.ActionListMode.ActionCategory) { // list actions in action categories

                    IList<int> actionCategoryIds = set.actionCategoryIds;
                    for(int j = 0; j < actionCategoryIds.Count; j++) {
                        int actionCatId = actionCategoryIds[j];
                        InputCategory actionCat = ReInput.mapping.GetActionCategory(actionCatId);
                        if(actionCat == null) continue;
                        if(!actionCat.userAssignable) continue;

                        inputGrid.AddActionCategory(set.mapCategoryId, actionCatId); // add the action category

                        foreach(InputAction action in ReInput.mapping.UserAssignableActionsInCategory(actionCatId)) {
                            if(action.type == InputActionType.Axis) {
                                if(_showFullAxisInputFields) inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Full);
                                if(_showSplitAxisInputFields) {
                                    inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Positive);
                                    inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Negative);
                                }
                            } else if(action.type == InputActionType.Button) {
                                inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Positive);
                            }
                        }

                    }

                } else { // list individual user-assigned actions

                    IList<int> actionIds = set.actionIds;
                    for(int j = 0; j < actionIds.Count; j++) {
                        InputAction action = ReInput.mapping.GetAction(actionIds[j]);
                        if(action == null) continue;

                        if(action.type == InputActionType.Axis) {
                            if(_showFullAxisInputFields) inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Full);
                            if(_showSplitAxisInputFields) {
                                inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Positive);
                                inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Negative);
                            }
                        } else if(action.type == InputActionType.Button) {
                            inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Positive);
                        }
                    }
                }
            }

            // Set column H-spacing
            references.inputGridInnerGroup.GetComponent<HorizontalLayoutGroup>().spacing = _inputColumnSpacing;

            // Set grid layout width
            references.inputGridLayoutElement.flexibleWidth = 0; // disable flexible width
            references.inputGridLayoutElement.preferredWidth = inputGridWidth;
        }

        private void RefreshInputGridStructure() {
            if(currentMappingSet == null) return;
            inputGrid.HideAll();
            inputGrid.Show(currentMappingSet.mapCategoryId);
            // Resize the main container to fit the elements
            references.inputGridInnerGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inputGrid.GetColumnHeight(currentMappingSet.mapCategoryId));
        }

        // Layout

        private void CreateHeaderLabels() {
            GUILabel label;

            // Actions column header
            references.inputGridHeader1 = CreateNewColumnGroup("ActionsHeader", references.inputGridHeadersGroup, _actionLabelWidth).transform;
            label = CreateLabel(prefabs.inputGridHeaderLabel, _language.actionColumnLabel, references.inputGridHeader1, Vector2.zero);

            // Keyboard column header
            if(_showKeyboard) {
                references.inputGridHeader2 = CreateNewColumnGroup("KeybordHeader", references.inputGridHeadersGroup, _keyboardColMaxWidth).transform;
                label = CreateLabel(prefabs.inputGridHeaderLabel, _language.keyboardColumnLabel, references.inputGridHeader2, Vector2.zero);
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
                label.SetTextAlignment(TMPro.TextAlignmentOptions.Center);
#else
                label.SetTextAlignment(TextAnchor.MiddleCenter);
#endif
            }

            // Mouse column header
            if(_showMouse) {
                references.inputGridHeader3 = CreateNewColumnGroup("MouseHeader", references.inputGridHeadersGroup, _mouseColMaxWidth).transform;
                label = CreateLabel(prefabs.inputGridHeaderLabel, _language.mouseColumnLabel, references.inputGridHeader3, Vector2.zero);
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
                label.SetTextAlignment(TMPro.TextAlignmentOptions.Center);
#else
                label.SetTextAlignment(TextAnchor.MiddleCenter);
#endif
            }

            // Controller column header
            if(_showControllers) {
                references.inputGridHeader4 = CreateNewColumnGroup("ControllerHeader", references.inputGridHeadersGroup, _controllerColMaxWidth).transform;
                label = CreateLabel(prefabs.inputGridHeaderLabel, _language.controllerColumnLabel, references.inputGridHeader4, Vector2.zero);
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
                label.SetTextAlignment(TMPro.TextAlignmentOptions.Center);
#else
                label.SetTextAlignment(TextAnchor.MiddleCenter);
#endif
            }
        }

        private void CreateActionLabelColumn() {
            Transform columnXform = CreateNewColumnGroup("ActionLabelColumn", references.inputGridInnerGroup, _actionLabelWidth).transform;
            references.inputGridActionColumn = columnXform;
        }

        private void CreateKeyboardInputFieldColumn() {
            if(!_showKeyboard) return;
            CreateInputFieldColumn("KeyboardColumn", ControllerType.Keyboard, _keyboardColMaxWidth, _keyboardInputFieldCount, true);
        }

        private void CreateMouseInputFieldColumn() {
            if(!_showMouse) return;
            CreateInputFieldColumn("MouseColumn", ControllerType.Mouse, _mouseColMaxWidth, _mouseInputFieldCount, false);
        }

        private void CreateControllerInputFieldColumn() {
            if(!_showControllers) return;
            CreateInputFieldColumn("ControllerColumn", ControllerType.Joystick, _controllerColMaxWidth, _controllerInputFieldCount, false);
        }

        private void CreateInputFieldColumn(string name, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis) {
            Transform columnXform = CreateNewColumnGroup(name, references.inputGridInnerGroup, maxWidth).transform;
            switch(controllerType) {
                case ControllerType.Joystick:
                    references.inputGridControllerColumn = columnXform;
                    break;
                case ControllerType.Keyboard:
                    references.inputGridKeyboardColumn = columnXform;
                    break;
                case ControllerType.Mouse:
                    references.inputGridMouseColumn = columnXform;
                    break;
                default: throw new System.NotImplementedException();
            }
        }

        // Create the individual elements

        private void CreateInputActionLabels() {

            Transform columnXform = references.inputGridActionColumn;

            for(int mappingSetIndex = 0; mappingSetIndex < _mappingSets.Length; mappingSetIndex++) {
                MappingSet set = _mappingSets[mappingSetIndex];
                if(set == null || !set.isValid) continue;

                int yPos = 0;

                // Create the labels
                if(set.actionListMode == MappingSet.ActionListMode.ActionCategory) { // list all actions in categories

                    int categoryCount = 0;
                    IList<int> actionCategoryIds = set.actionCategoryIds;

                    for(int i = 0; i < actionCategoryIds.Count; i++) {
                        InputCategory category = ReInput.mapping.GetActionCategory(actionCategoryIds[i]);
                        if(category == null) continue;
                        if(!category.userAssignable) continue; // action category isn't user assignable

                        // Count actions first
                        int count = CountIEnumerable<InputAction>(ReInput.mapping.UserAssignableActionsInCategory(category.id));
                        if(count == 0) continue; // no actions so skip category

                        // Draw category label
                        if(_showActionCategoryLabels) {
                            if(categoryCount > 0) yPos -= _inputRowCategorySpacing; // extra space above category
                            GUILabel label = CreateLabel(_language.GetActionCategoryName(category.id), columnXform, new Vector2(0, yPos));
#if REWIRED_CONTROL_MAPPER_USE_TMPRO
                            label.SetFontStyle(TMPro.FontStyles.Bold);
#else
                            label.SetFontStyle(FontStyle.Bold);
#endif
                            label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
                            inputGrid.AddActionCategoryLabel(set.mapCategoryId, category.id, label);
                            yPos -= _inputRowHeight;
                        }

                        // Draw Action labels
                        foreach(InputAction action in ReInput.mapping.UserAssignableActionsInCategory(category.id, true)) {

                            GUILabel label;
                            if(action.type == InputActionType.Axis) {

                                if(_showFullAxisInputFields) {
                                    label = CreateLabel(_language.GetActionName(action.id, AxisRange.Full), columnXform, new Vector2(0, yPos));
                                    label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
                                    inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Full, label);
                                    yPos -= _inputRowHeight;
                                }

                                if(_showSplitAxisInputFields) {
                                    string positiveDescriptiveName = _language.GetActionName(action.id, AxisRange.Positive);
                                    label = CreateLabel(positiveDescriptiveName, columnXform, new Vector2(0, yPos));
                                    label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
                                    inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Positive, label);
                                    yPos -= _inputRowHeight;

                                    string negativeDescriptiveName = _language.GetActionName(action.id, AxisRange.Negative);
                                    label = CreateLabel(negativeDescriptiveName, columnXform, new Vector2(0, yPos));
                                    label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
                                    inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Negative, label);
                                    yPos -= _inputRowHeight;
                                }

                            } else if(action.type == InputActionType.Button) {
                                label = CreateLabel(_language.GetActionName(action.id), columnXform, new Vector2(0, yPos));
                                label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
                                inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Positive, label);
                                yPos -= _inputRowHeight;
                            }
                        }
                        categoryCount++;
                    }

                } else { // list only individual actions defined by the user

                    IList<int> actionIds = set.actionIds;

                    for(int i = 0; i < actionIds.Count; i++) {
                        InputAction action = ReInput.mapping.GetAction(actionIds[i]);
                        if(action == null) continue;
                        if(!action.userAssignable) continue; // action isn't user assignable

                        InputCategory category = ReInput.mapping.GetActionCategory(action.categoryId);
                        if(category == null) continue;
                        if(!category.userAssignable) continue; // action category isn't user assignable

                        // Draw Action labels
                        GUILabel label;
                        if(action.type == InputActionType.Axis) {

                            if(_showFullAxisInputFields) {
                                label = CreateLabel(_language.GetActionName(action.id, AxisRange.Full), columnXform, new Vector2(0, yPos));
                                label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
                                inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Full, label);
                                yPos -= _inputRowHeight;
                            }

                            if(_showSplitAxisInputFields) {
                                label = CreateLabel(_language.GetActionName(action.id, AxisRange.Positive), columnXform, new Vector2(0, yPos));
                                label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
                                inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Positive, label);
                                yPos -= _inputRowHeight;

                                label = CreateLabel(_language.GetActionName(action.id, AxisRange.Negative), columnXform, new Vector2(0, yPos));
                                label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
                                inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Negative, label);
                                yPos -= _inputRowHeight;
                            }

                        } else if(action.type == InputActionType.Button) {
                            label = CreateLabel(_language.GetActionName(action.id), columnXform, new Vector2(0, yPos));
                            label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
                            inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Positive, label);
                            yPos -= _inputRowHeight;
                        }
                    }
                }

                inputGrid.SetColumnHeight(set.mapCategoryId, -yPos); // store the column height for this map category
            }
        }

        private void CreateInputFields() {
            if(_showControllers) CreateInputFields(references.inputGridControllerColumn, ControllerType.Joystick, _controllerColMaxWidth, _controllerInputFieldCount, false);
            if(_showKeyboard) CreateInputFields(references.inputGridKeyboardColumn, ControllerType.Keyboard, _keyboardColMaxWidth, _keyboardInputFieldCount, true);
            if(_showMouse) CreateInputFields(references.inputGridMouseColumn, ControllerType.Mouse, _mouseColMaxWidth, _mouseInputFieldCount, false);
        }

        private void CreateInputFields(Transform columnXform, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis) {
            for(int mappingSetIndex = 0; mappingSetIndex < _mappingSets.Length; mappingSetIndex++) {
                MappingSet set = _mappingSets[mappingSetIndex];
                if(set == null || !set.isValid) continue;

                // Create input field buttons
                int fieldWidth = maxWidth / cols;
                int yPos = 0;
                int categoryCount = 0;

                // Create the labels
                if(set.actionListMode == MappingSet.ActionListMode.ActionCategory) { // list all actions in categories

                    IList<int> actionCategoryIds = set.actionCategoryIds;
                    for(int i = 0; i < actionCategoryIds.Count; i++) {
                        InputCategory category = ReInput.mapping.GetActionCategory(actionCategoryIds[i]);
                        if(category == null) continue;
                        if(!category.userAssignable) continue; // action category isn't user assignable

                        // Count actions first
                        int count = CountIEnumerable<InputAction>(ReInput.mapping.UserAssignableActionsInCategory(category.id));
                        if(count == 0) continue; // no actions so skip category

                        // Add spaces for category labels
                        if(_showActionCategoryLabels) {
                            yPos -= categoryCount > 0 ? _inputRowHeight + _inputRowCategorySpacing : _inputRowHeight;
                        }

                        // Draw input field buttons
                        foreach(InputAction action in ReInput.mapping.UserAssignableActionsInCategory(category.id, true)) {

                            if(action.type == InputActionType.Axis) {
                                if(_showFullAxisInputFields) CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Full, controllerType, cols, fieldWidth, ref yPos, disableFullAxis);
                                if(_showSplitAxisInputFields) {
                                    CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, false);
                                    CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Negative, controllerType, cols, fieldWidth, ref yPos, false);
                                }
                            } else if(action.type == InputActionType.Button) {
                                CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, false);
                            }

                            categoryCount++;
                        }
                    }

                } else { // list only individual actins defined by the user

                    IList<int> actionIds = set.actionIds;
                    for(int i = 0; i < actionIds.Count; i++) {
                        InputAction action = ReInput.mapping.GetAction(actionIds[i]);
                        if(action == null) continue;
                        if(!action.userAssignable) continue;

                        InputCategory category = ReInput.mapping.GetActionCategory(action.categoryId);
                        if(category == null) continue;
                        if(!category.userAssignable) continue; // action category isn't user assignable

                        // Draw input field buttons
                        if(action.type == InputActionType.Axis) {
                            if(_showFullAxisInputFields) CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Full, controllerType, cols, fieldWidth, ref yPos, disableFullAxis);
                            if(_showSplitAxisInputFields) {
                                CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, false);
                                CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Negative, controllerType, cols, fieldWidth, ref yPos, false);
                            }
                        } else if(action.type == InputActionType.Button) {
                            CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, false);
                        }
                    }
                }
            }
        }

        private void CreateInputFieldSet(Transform parent, int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int cols, int fieldWidth, ref int yPos, bool disableFullAxis) {
            // Create horizontal layout group to hold fields
            GameObject layoutGroup = CreateNewGUIObject("FieldLayoutGroup", parent, new Vector2(0, yPos));
            HorizontalLayoutGroup hLayoutGroup = layoutGroup.AddComponent<HorizontalLayoutGroup>();
            hLayoutGroup.padding = _inputRowPadding;
            hLayoutGroup.spacing = _inputRowFieldSpacing;
            RectTransform rt = layoutGroup.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = Vector2.zero;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);

            // Create the field set
            inputGrid.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, layoutGroup);

            // Create the fields
            for(int fieldIndex = 0; fieldIndex < cols; fieldIndex++) {
                int toggleWidth = axisRange == AxisRange.Full ? _invertToggleWidth : 0;

                GUIInputField field = CreateInputField(hLayoutGroup.transform, Vector2.zero, "", action.id, axisRange, controllerType, fieldIndex);
                field.SetFirstChildObjectWidth(LayoutElementSizeType.PreferredSize, fieldWidth - toggleWidth); // set max width so buttons will stay equal widths
                inputGrid.AddInputField(mapCategoryId, action, axisRange, controllerType, fieldIndex, field); // add to the system

                if(axisRange == AxisRange.Full) {
                    // Create invert toggle field
                    if(!disableFullAxis) {
                        GUIToggle toggle = CreateToggle(prefabs.inputGridFieldInvertToggle, hLayoutGroup.transform, Vector2.zero, "", action.id, axisRange, controllerType, fieldIndex);
                        toggle.SetFirstChildObjectWidth(LayoutElementSizeType.MinSize, toggleWidth); // set max width so buttons will stay equal widths
                        field.AddToggle(toggle);
                    } else {
                        field.SetInteractible(false, false, true);
                    }
                }
            }

            yPos -= _inputRowHeight;
        }

        private void PopulateInputFields() {

            // Clear
            inputGrid.InitializeFields(currentMapCategoryId);

            // Get the player
            if(currentPlayer == null) return;

            inputGrid.SetFieldsActive(currentMapCategoryId, true);

            foreach(InputActionSet actionSet in inputGrid.GetActionSets(currentMapCategoryId)) {

                ControllerType controllerType;
                int controllerId;
                int layoutId;
                int fieldCount;

                // Draw keyboard and mouse fields
                if(_showKeyboard) {
                    controllerType = ControllerType.Keyboard;
                    controllerId = 0;
                    layoutId = _keyboardMapDefaultLayout;
                    fieldCount = _keyboardInputFieldCount;
                    ControllerMap map = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
                    PopulateInputFieldGroup(actionSet, map, controllerType, controllerId, fieldCount);
                }

                if(_showMouse) {
                    controllerType = ControllerType.Mouse;
                    controllerId = 0;
                    layoutId = _mouseMapDefaultLayout;
                    fieldCount = _mouseInputFieldCount;
                    ControllerMap map = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
                    if(currentPlayer.controllers.hasMouse) PopulateInputFieldGroup(actionSet, map, controllerType, controllerId, fieldCount); // only populate if mouse is assigned, hide maps if not
                    // Do not disable fields if player doesn't have mouse so player can assign it by clicking on the fields
                }

                // Draw joystick fields
                if(isJoystickSelected && currentPlayer.controllers.joystickCount > 0) {
                    controllerType = ControllerType.Joystick;
                    controllerId = currentJoystick.id;
                    layoutId = _joystickMapDefaultLayout;
                    fieldCount = _controllerInputFieldCount;
                    ControllerMap map = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
                    PopulateInputFieldGroup(actionSet, map, controllerType, controllerId, fieldCount);
                } else { // no controller assigned to this player
                    DisableInputFieldGroup(actionSet, ControllerType.Joystick, _controllerInputFieldCount);
                }
            }
        }

        private void PopulateInputFieldGroup(InputActionSet actionSet, ControllerMap controllerMap, ControllerType controllerType, int controllerId, int maxFields) {
            if(controllerMap == null) return;

            int count = 0;

            // First populate fixed data in all fields including blanks
            inputGrid.SetFixedFieldData(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId);

            // Populate data in fields that have an ActionElementMap
            foreach(ActionElementMap aem in controllerMap.ElementMapsWithAction(actionSet.actionId)) {

                if(aem.elementType == ControllerElementType.Button) {

                    if(actionSet.axisRange == AxisRange.Full) {

                        // Not implemented
                        continue;

                    } else if(actionSet.axisRange == AxisRange.Positive) {

                        if(aem.axisContribution == Pole.Negative) continue;

                    } else if(actionSet.axisRange == AxisRange.Negative) {

                        if(aem.axisContribution == Pole.Positive) continue;

                    }

                    inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, count, aem.id, _language.GetElementIdentifierName(aem), false);

                } else if(aem.elementType == ControllerElementType.Axis) {

                    if(actionSet.axisRange == AxisRange.Full) {
                        if(aem.axisRange != AxisRange.Full) continue;

                        inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, count, aem.id, _language.GetElementIdentifierName(aem), aem.invert);

                    } else if(actionSet.axisRange == AxisRange.Positive) {
                        if(aem.axisRange == AxisRange.Full && ReInput.mapping.GetAction(actionSet.actionId).type != InputActionType.Button) continue;
                        if(aem.axisContribution == Pole.Negative) continue;

                        inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, count, aem.id, _language.GetElementIdentifierName(aem), false);

                    } else if(actionSet.axisRange == AxisRange.Negative) {
                        if(aem.axisRange == AxisRange.Full) continue;
                        if(aem.axisContribution == Pole.Positive) continue;

                        inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, count, aem.id, _language.GetElementIdentifierName(aem), false);
                    }
                }

                count++;
                if(count > maxFields) break; // no more room
            }
        }

        private void DisableInputFieldGroup(InputActionSet actionSet, ControllerType controllerType, int fieldCount) {
            // Disable the fields
            for(int i = 0; i < fieldCount; i++) {
                GUIInputField field = inputGrid.GetGUIInputField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, i);
                if(field == null) continue;
                field.SetInteractible(false, false);
            }
        }

        private void ResetInputGridScrollBar() {
            references.inputGridInnerGroup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // fix Unity scrollbar bug
            references.inputGridVScrollbar.value = 1.0f;
#if UNITY_5_2_PLUS
            // Unity 5.2 adds ScrollBar visibility support to ScrollRect which breaks the old visibility handling
            // Set it to AutoHide because it default sto Permanent
            references.inputGridScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
#endif
        }

        #endregion

        #region Drawing (UI Layout)

        // Draw the layout initially

        private void CreateLayout() {

            // Enable all groups that are used by the current settings.

            // Player section
            references.playersGroup.gameObject.SetActive(showPlayers);

            // Controller section
            references.controllerGroup.gameObject.SetActive(_showControllers);
            references.assignedControllersGroup.gameObject.SetActive(_showControllers && ShowAssignedControllers());

            // Settings and Map Categories section

            references.settingsAndMapCategoriesGroup.gameObject.SetActive(showSettings || showMapCategories);

            // Settings section
            references.settingsGroup.gameObject.SetActive(showSettings);

            // Map categories section
            references.mapCategoriesGroup.gameObject.SetActive(showMapCategories);
        }

        private void Draw() {
            DrawPlayersGroup();
            DrawControllersGroup();
            DrawSettingsGroup();
            DrawMapCategoriesGroup();
            DrawWindowButtonsGroup();
        }

        private void DrawPlayersGroup() {
            if(!showPlayers) return;

            // Set label
            references.playersGroup.labelText = _language.playersGroupLabel;
            references.playersGroup.SetLabelActive(_showPlayersGroupLabel);

            for(int i = 0; i < playerCount; i++) {
                Player player = ReInput.players.GetPlayer(i);
                if(player == null) continue;
                GameObject instance = UI.ControlMapper.UITools.InstantiateGUIObject<ButtonInfo>(prefabs.button, references.playersGroup.content, "Player" + i + "Button");
                GUIButton button = new GUIButton(instance);
                button.SetLabel(_language.GetPlayerName(player.id));
                button.SetButtonInfoData(buttonIdentifier_playerSelection, player.id);
                button.SetOnClickCallback(OnButtonActivated);
                button.buttonInfo.OnSelectedEvent += OnUIElementSelected;
                playerButtons.Add(button);
            }
        }

        private void DrawControllersGroup() {
            if(!_showControllers) return;

            // Set group labels
            references.controllerSettingsGroup.labelText = _language.controllerSettingsGroupLabel;
            references.controllerSettingsGroup.SetLabelActive(_showControllerGroupLabel);
            references.controllerNameLabel.gameObject.SetActive(_showControllerNameLabel);
            references.controllerGroupLabelGroup.gameObject.SetActive(_showControllerGroupLabel || _showControllerNameLabel);
            if(ShowAssignedControllers()) {
                references.assignedControllersGroup.labelText = _language.assignedControllersGroupLabel;
                references.assignedControllersGroup.SetLabelActive(_showAssignedControllersGroupLabel);
            }

            ButtonInfo buttonInfo;

            // Set up button labels in static buttons
            buttonInfo = references.removeControllerButton.GetComponent<ButtonInfo>();
            buttonInfo.text.text = _language.removeControllerButtonLabel;

            buttonInfo = references.calibrateControllerButton.GetComponent<ButtonInfo>();
            buttonInfo.text.text = _language.calibrateControllerButtonLabel;

            buttonInfo = references.assignControllerButton.GetComponent<ButtonInfo>();
            buttonInfo.text.text = _language.assignControllerButtonLabel;

            // Create None placeholder button
            GUIButton guiButton = CreateButton(_language.none, references.assignedControllersGroup.content, Vector2.zero);
            guiButton.SetInteractible(false, false, true);
            assignedControllerButtonsPlaceholder = guiButton;
        }

        private void DrawSettingsGroup() {
            if(!showSettings) return;

            // Set label on group
            references.settingsGroup.labelText = _language.settingsGroupLabel;
            references.settingsGroup.SetLabelActive(_showSettingsGroupLabel);

            // Create Input Behavior Settings button
            GUIButton button = CreateButton(_language.inputBehaviorSettingsButtonLabel, references.settingsGroup.content, Vector2.zero);
            miscInstantiatedObjects.Add(button.gameObject);
            button.buttonInfo.OnSelectedEvent += OnUIElementSelected;
            button.SetButtonInfoData(buttonIdentifier_editInputBehaviors, 0);
            button.SetOnClickCallback(OnButtonActivated);
        }

        private void DrawMapCategoriesGroup() {
            if(!showMapCategories) return;
            if(_mappingSets == null) return;

            // Set label on group
            references.mapCategoriesGroup.labelText = _language.mapCategoriesGroupLabel;
            references.mapCategoriesGroup.SetLabelActive(_showMapCategoriesGroupLabel);

            for(int i = 0; i < _mappingSets.Length; i++) {
                MappingSet set = _mappingSets[i];
                if(set == null) continue;
                InputMapCategory cat = ReInput.mapping.GetMapCategory(set.mapCategoryId);
                if(cat == null) continue; // invalid map category id

                GameObject instance = UI.ControlMapper.UITools.InstantiateGUIObject<ButtonInfo>(prefabs.button, references.mapCategoriesGroup.content, cat.name + "Button");
                GUIButton button = new GUIButton(instance);
                button.SetLabel(_language.GetMapCategoryName(cat.id));
                button.SetButtonInfoData(buttonIdentifier_mapCategorySelection, cat.id);
                button.SetOnClickCallback(OnButtonActivated);
                button.buttonInfo.OnSelectedEvent += OnUIElementSelected;
                mapCategoryButtons.Add(button);
            }
        }

        private void DrawWindowButtonsGroup() {
            references.doneButton.GetComponent<ButtonInfo>().text.text = _language.doneButtonLabel;
            references.restoreDefaultsButton.GetComponent<ButtonInfo>().text.text = _language.restoreDefaultsButtonLabel;
        }

        // Redraw / Refresh the layout

        private void Redraw(bool listsChanged, bool playTransitions) {
            RedrawPlayerGroup(playTransitions);
            RedrawControllerGroup();
            RedrawMapCategoriesGroup(playTransitions);
            RedrawInputGrid(listsChanged);

            // Make sure something is always selected so UI navigation is never lost with only joysticks
            if(currentUISelection == null || !currentUISelection.activeInHierarchy) RestoreLastUISelection();
        }

        private void RedrawPlayerGroup(bool playTransitions) {
            if(!showPlayers) return;

            for(int i = 0; i < playerButtons.Count; i++) {
                bool state = currentPlayerId != playerButtons[i].buttonInfo.intData;
                playerButtons[i].SetInteractible(state, playTransitions);
            }
        }

        private void RedrawControllerGroup() {

            int prevSelectedButtonJoyId = -1;

            // Reset config group
            references.controllerNameLabel.text = _language.none;
            UI.ControlMapper.UITools.SetInteractable(references.removeControllerButton, false, false);
            UI.ControlMapper.UITools.SetInteractable(references.assignControllerButton, false, false);
            UI.ControlMapper.UITools.SetInteractable(references.calibrateControllerButton, false, false);

            // Reset assigned controllers group
            if(ShowAssignedControllers()) {

                // Clear assigned controller buttons
                foreach(GUIButton button in assignedControllerButtons) {
                    if(button.gameObject == null) continue;
                    if(currentUISelection == button.gameObject) prevSelectedButtonJoyId = button.buttonInfo.intData; // store selection if this was the active UI element
                    Object.Destroy(button.gameObject);
                }
                assignedControllerButtons.Clear();

                // Enable placeholder
                assignedControllerButtonsPlaceholder.SetActive(true);
            }

            // Get the current player
            Player player = ReInput.players.GetPlayer(currentPlayerId);
            if(player == null) return;

            // Assigned controller buttons
            if(ShowAssignedControllers()) {

                if(player.controllers.joystickCount > 0) assignedControllerButtonsPlaceholder.SetActive(false); // disable the None placeholder

                // Create buttons for the assigned controllers
                foreach(Joystick joystick in player.controllers.Joysticks) {
                    GUIButton button = CreateButton(_language.GetControllerName(joystick), references.assignedControllersGroup.content, Vector2.zero);
                    button.SetButtonInfoData(buttonIdentifier_assignedControllerSelection, joystick.id);
                    button.SetOnClickCallback(OnButtonActivated);
                    button.buttonInfo.OnSelectedEvent += OnUIElementSelected;
                    assignedControllerButtons.Add(button);
                    if(joystick.id == currentJoystickId) button.SetInteractible(false, true); // disable the selected joystick
                }

                // Auto-select the first controller if none selected
                if(player.controllers.joystickCount > 0 && !isJoystickSelected) {
                    currentJoystickId = player.controllers.Joysticks[0].id;
                    assignedControllerButtons[0].SetInteractible(false, false);
                }

                // Restore UI selection if selected button was destroyed
                if(prevSelectedButtonJoyId >= 0) {
                    foreach(GUIButton button in assignedControllerButtons) {
                        if(button.buttonInfo.intData == prevSelectedButtonJoyId) {
                            SetUISelection(button.gameObject);
                            break;
                        }
                    }
                }

            } else { // just one controller per player

                // Auto-select the first controller if none selected
                if(player.controllers.joystickCount > 0 && !isJoystickSelected) {
                    currentJoystickId = player.controllers.Joysticks[0].id;
                }
            }

            // Set ineractible on controller config buttons if a controller is selected/assigned
            if(isJoystickSelected && player.controllers.joystickCount > 0) {

                // Remove Controller button
                references.removeControllerButton.interactable = true;

                // Controller name label
                references.controllerNameLabel.text = _language.GetControllerName(currentJoystick);

                // Calibrate Controller button
                if(currentJoystick.axisCount > 0) references.calibrateControllerButton.interactable = true;
            }

            // Assign Controller button
            int playerJoystickCount = player.controllers.joystickCount;
            int totalJoystickCount = ReInput.controllers.joystickCount;
            int maxJoysticksPerPlayer = GetMaxControllersPerPlayer();
            bool infiniteJoysticksPerPlayer = maxJoysticksPerPlayer == 0;

            if(totalJoystickCount > 0 &&
                playerJoystickCount < totalJoystickCount &&
                (maxJoysticksPerPlayer == 1 || infiniteJoysticksPerPlayer || playerJoystickCount < maxJoysticksPerPlayer)
                ) {
                UI.ControlMapper.UITools.SetInteractable(references.assignControllerButton, true, false);
            }
        }

        private void RedrawMapCategoriesGroup(bool playTransitions) {
            if(!showMapCategories) return;

            for(int i = 0; i < mapCategoryButtons.Count; i++) {
                bool state = currentMapCategoryId != mapCategoryButtons[i].buttonInfo.intData;
                mapCategoryButtons[i].SetInteractible(state, playTransitions);
            }
        }

        private void RedrawInputGrid(bool listsChanged) {
            if(listsChanged) RefreshInputGridStructure();
            PopulateInputFields();
            if(listsChanged) ResetInputGridScrollBar();
        }

        private void ForceRefresh() {
            if(windowManager.isWindowOpen) CloseAllWindows();
            else Redraw(false, false);
        }

        #endregion

        #region Create UI Objects

        private void CreateInputCategoryRow(ref int rowCount, InputCategory category) {
            CreateLabel(_language.GetMapCategoryName(category.id), references.inputGridActionColumn, new Vector2(0, rowCount * _inputRowHeight * -1.0f));
            rowCount++;
        }

        private GUILabel CreateLabel(string labelText, Transform parent, Vector2 offset) {
            return CreateLabel(prefabs.inputGridLabel, labelText, parent, offset);
        }
        private GUILabel CreateLabel(GameObject prefab, string labelText, Transform parent, Vector2 offset) {
            GameObject instance = InstantiateGUIObject(prefab, parent, offset);
            Text text = UnityTools.GetComponentInSelfOrChildren<Text>(instance);
            if(text == null) {
                Debug.LogError("Rewired Control Mapper: Label prefab is missing Text component!");
                return null;
            }
            text.text = labelText;
            return new GUILabel(instance);
        }

        private GUIButton CreateButton(string labelText, Transform parent, Vector2 offset) {
            GUIButton button = new GUIButton(InstantiateGUIObject(prefabs.button, parent, offset));
            button.SetLabel(labelText);
            return button;
        }

        private GUIButton CreateFitButton(string labelText, Transform parent, Vector2 offset) {
            GUIButton button = new GUIButton(InstantiateGUIObject(prefabs.fitButton, parent, offset));
            button.SetLabel(labelText);
            return button;
        }

        private GUIInputField CreateInputField(Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex) {
            GUIInputField field = CreateInputField(parent, offset);
            field.SetLabel("");
            field.SetFieldInfoData(actionId, axisRange, controllerType, fieldIndex);
            field.SetOnClickCallback(inputFieldActivatedDelegate);
            field.fieldInfo.OnSelectedEvent += OnUIElementSelected;
            return field;
        }
        private GUIInputField CreateInputField(Transform parent, Vector2 offset) {
            return new GUIInputField(InstantiateGUIObject(prefabs.inputGridFieldButton, parent, offset));
        }

        private GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex) {
            GUIToggle toggle = CreateToggle(prefab, parent, offset);
            toggle.SetToggleInfoData(actionId, axisRange, controllerType, fieldIndex);
            toggle.SetOnSubmitCallback(inputFieldInvertToggleStateChangedDelegate);
            toggle.toggleInfo.OnSelectedEvent += OnUIElementSelected;
            return toggle;
        }
        private GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset) {
            return new GUIToggle(InstantiateGUIObject(prefab, parent, offset));
        }

        private GameObject InstantiateGUIObject(GameObject prefab, Transform parent, Vector2 offset) {
            if(prefab == null) {
                Debug.LogError("Rewired Control Mapper: Prefab is null!");
                return null;
            }
            GameObject instance = (GameObject)Object.Instantiate(prefab);
            return InitializeNewGUIGameObject(instance, parent, offset);
        }

        private GameObject CreateNewGUIObject(string name, Transform parent, Vector2 offset) {
            GameObject instance = new GameObject();
            instance.name = name;
            instance.AddComponent<RectTransform>();
            return InitializeNewGUIGameObject(instance, parent, offset);
        }

        private GameObject InitializeNewGUIGameObject(GameObject gameObject, Transform parent, Vector2 offset) {
            if(gameObject == null) {
                Debug.LogError("Rewired Control Mapper: GameObject is null!");
                return null;
            }

            RectTransform instanceXform = gameObject.GetComponent<RectTransform>();
            if(instanceXform == null) {
                Debug.LogError("Rewired Control Mapper: GameObject does not have a RectTransform component!");
                return gameObject;
            }
            if(parent != null) {
                instanceXform.SetParent(parent, false);
            }
            //instanceXform.localScale = Vector3.one;
            //instanceXform.localPosition = Vector3.zero;
            instanceXform.anchoredPosition = offset;
            return gameObject;
        }

        private GameObject CreateNewColumnGroup(string name, Transform parent, int maxWidth) {
            GameObject group = CreateNewGUIObject(name, parent, Vector2.zero);
            inputGrid.AddGroup(group);
            LayoutElement layout = group.AddComponent<LayoutElement>();
            if(maxWidth >= 0) layout.preferredWidth = maxWidth;
            RectTransform rectXForm = group.GetComponent<RectTransform>();
            rectXForm.anchorMin = new Vector2(0, 0);
            rectXForm.anchorMax = new Vector2(1, 0);
            return group;
        }

        #endregion

        #region Popup Window Management

        private Window OpenWindow(bool closeOthers) {
            return OpenWindow(string.Empty, closeOthers);
        }
        private Window OpenWindow(string name, bool closeOthers) {
            if(closeOthers) windowManager.CancelAll();
            Window window = windowManager.OpenWindow(name, _defaultWindowWidth, _defaultWindowHeight);
            if(window == null) return null;
            ChildWindowOpened();
            return window;
        }
        private Window OpenWindow(GameObject windowPrefab, bool closeOthers) {
            return OpenWindow(windowPrefab, string.Empty, closeOthers);
        }
        private Window OpenWindow(GameObject windowPrefab, string name, bool closeOthers) {
            if(closeOthers) windowManager.CancelAll();
            Window window = windowManager.OpenWindow(windowPrefab, name);
            if(window == null) return null;
            ChildWindowOpened();
            return window;
        }

        private void OpenModal(string title, string message, string confirmText, System.Action<int> confirmAction, string cancelText, System.Action<int> cancelAction, bool closeOthers) {
            Window window = OpenWindow(closeOthers);
            if(window == null) return;
            window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, title);
            window.AddContentText(prefabs.windowContentText, UI.UIPivot.TopCenter, UI.UIAnchor.TopHStretch, new Vector2(0, -100), message);
            UnityAction cancelCallback = () => { OnWindowCancel(window.id); };
            window.cancelCallback = cancelCallback;
            window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomLeft, UI.UIAnchor.BottomLeft, Vector2.zero, confirmText, () => { OnRestoreDefaultsConfirmed(window.id); }, cancelCallback, false);
            window.CreateButton(prefabs.fitButton, UI.UIPivot.BottomRight, UI.UIAnchor.BottomRight, Vector2.zero, cancelText, cancelCallback, cancelCallback, true);
            windowManager.Focus(window);
        }

        private void CloseWindow(int windowId) {
            if(!windowManager.isWindowOpen) return;
            windowManager.CloseWindow(windowId);
            ChildWindowClosed();
        }

        private void CloseTopWindow() {
            if(!windowManager.isWindowOpen) return;
            windowManager.CloseTop();
            ChildWindowClosed();
        }

        private void CloseAllWindows() {
            if(!windowManager.isWindowOpen) return;
            windowManager.CancelAll();
            ChildWindowClosed();
            InputPollingStopped();
        }

        private void ChildWindowOpened() {
            if(!windowManager.isWindowOpen) return; // do nothing if a window is not open
            SetIsFocused(false);
            if(_PopupWindowOpenedEvent != null) _PopupWindowOpenedEvent();
            if(_onPopupWindowOpened != null) _onPopupWindowOpened.Invoke();
        }

        private void ChildWindowClosed() {
            if(windowManager.isWindowOpen) return; // do nothing if a window is still open
            SetIsFocused(true);
            if(_PopupWindowClosedEvent != null) _PopupWindowClosedEvent();
            if(_onPopupWindowClosed != null) _onPopupWindowClosed.Invoke();
        }

        #endregion

        #region Element Assignment

        private bool HasElementAssignmentConflicts(Player player, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers) {
            if(player == null || mapping == null) return false;

            ElementAssignmentConflictCheck conflictCheck;
            if(!CreateConflictCheck(mapping, assignment, out conflictCheck)) return false;

            if(skipOtherPlayers) { // skip other players, only check system and self
                if(ReInput.players.SystemPlayer.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck)) return true;
                if(player.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck)) return true;
                return false;
            } else { // check all players
                return ReInput.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck);
            }
        }

        private bool IsBlockingAssignmentConflict(InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers) {
            ElementAssignmentConflictCheck conflictCheck;
            if(!CreateConflictCheck(mapping, assignment, out conflictCheck)) return false;

            if(skipOtherPlayers) { // skip other players, only check system and self
                foreach(ElementAssignmentConflictInfo conflict in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck)) {
                    if(!conflict.isUserAssignable) return true;
                }
                foreach(ElementAssignmentConflictInfo conflict in currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck)) {
                    if(!conflict.isUserAssignable) return true;
                }
            } else { // check all players
                foreach(ElementAssignmentConflictInfo conflict in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck)) {
                    if(!conflict.isUserAssignable) return true;
                }
            }

            return false;
        }

        private IEnumerable<ElementAssignmentConflictInfo> ElementAssignmentConflicts(Player player, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers) {
            if(player == null || mapping == null) yield break;

            ElementAssignmentConflictCheck conflictCheck;
            if(!CreateConflictCheck(mapping, assignment, out conflictCheck)) yield break;

            if(skipOtherPlayers) { // skip other players, only check system and self
                foreach(ElementAssignmentConflictInfo conflict in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck)) {
                    if(!conflict.isUserAssignable) yield return conflict;
                }
                foreach(ElementAssignmentConflictInfo conflict in player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck)) {
                    if(!conflict.isUserAssignable) yield return conflict;
                }
            } else { // check all players
                foreach(ElementAssignmentConflictInfo conflict in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck)) {
                    if(!conflict.isUserAssignable) yield return conflict;
                }
            }
        }

        private bool CreateConflictCheck(InputMapping mapping, ElementAssignment assignment, out ElementAssignmentConflictCheck conflictCheck) {
            if(mapping == null || currentPlayer == null) {
                conflictCheck = new ElementAssignmentConflictCheck();
                return false;
            }

            conflictCheck = assignment.ToElementAssignmentConflictCheck();
            conflictCheck.playerId = currentPlayer.id;
            conflictCheck.controllerType = mapping.controllerType;
            conflictCheck.controllerId = mapping.controllerId;
            conflictCheck.controllerMapId = mapping.map.id;
            conflictCheck.controllerMapCategoryId = mapping.map.categoryId;
            if(mapping.aem != null) conflictCheck.elementMapId = mapping.aem.id;

            return true;
        }

        private void PollKeyboardForAssignment(out ControllerPollingInfo pollingInfo, out bool modifierKeyPressed, out ModifierKeyFlags modifierFlags, out string label) {
            pollingInfo = new ControllerPollingInfo();
            label = string.Empty;
            modifierKeyPressed = false;
            modifierFlags = ModifierKeyFlags.None;

            int modifierPressedCount = 0; // the number of modifier keys being pressed this cycle
            ControllerPollingInfo nonModifierKeyInfo = new ControllerPollingInfo();
            ControllerPollingInfo firstModifierKeyInfo = new ControllerPollingInfo();
            ModifierKeyFlags curModifiers = ModifierKeyFlags.None;

            // Check all keys being pressed at present so we can handle modifier keys
            foreach(ControllerPollingInfo info in ReInput.controllers.Keyboard.PollForAllKeys()) {
                KeyCode key = info.keyboardKey;
                if(key == KeyCode.AltGr) continue; // skip AltGr key because it gets fired when alt and control are held on some keyboards

                // determine if a modifier key is being pressed
                if(Keyboard.IsModifierKey(info.keyboardKey)) { // a modifier key is pressed
                    if(modifierPressedCount == 0) firstModifierKeyInfo = info; // store the polling info for the first modifier key pressed in case its the only key pressed

                    curModifiers |= Keyboard.KeyCodeToModifierKeyFlags(key); // add the key to the current modifier flags
                    modifierPressedCount += 1; // count how many modifier keys are pressed

                } else { // this is not a modifier key

                    if(nonModifierKeyInfo.keyboardKey != KeyCode.None) continue; // skip after the first one detected, we only need one non-modifier key press
                    nonModifierKeyInfo = info; // store the polling info
                }

            }

            // Commit immediately if a non-modifier key was pressed
            if(nonModifierKeyInfo.keyboardKey != KeyCode.None) { // a regular key was pressed

                // Make sure the primary button is in a down state or else we'll get immediate confirmation if using the keyboard to navigate
                if(!ReInput.controllers.Keyboard.GetKeyDown(nonModifierKeyInfo.keyboardKey)) return; // fail because key is not in a down state

                if(modifierPressedCount == 0) { // only the regular key was pressed
                    pollingInfo = nonModifierKeyInfo; // copy polling info into entry
                    return;

                } else { // one more more modifier keys was pressed too

                    pollingInfo = nonModifierKeyInfo; // copy polling info into entry
                    modifierFlags = curModifiers; // set the modifier key flags
                    return;

                }

            } else if(modifierPressedCount > 0) { // one or more modifier keys were pressed, but no regular keys
                modifierKeyPressed = true;

                if(modifierPressedCount == 1) { // only one modifier is pressed, allow assigning just the modifier key

                    // Assign the modifier key as the main key if the user holds it for 1 second
                    if(ReInput.controllers.Keyboard.GetKeyTimePressed(firstModifierKeyInfo.keyboardKey) > 1.0f) { // key was pressed for one second
                        pollingInfo = firstModifierKeyInfo; // copy polling info into entry
                        return;
                    }

                    // Show the key that is being pressed
                    label = Keyboard.GetKeyName(firstModifierKeyInfo.keyboardKey);

                } else { // more than one modifier key is pressed
                    // do nothing because we don't want to assign modified modifier key presses such as Control + Alt, but you could if you wanted to.

                    // Show the modifier keys being held
                    label = _language.ModifierKeyFlagsToString(curModifiers);

                }
            }
        }

        private bool GetFirstElementAssignmentConflict(ElementAssignmentConflictCheck conflictCheck, out ElementAssignmentConflictInfo conflict, bool skipOtherPlayers) {

            // Find the first conflict in all applicable Players always checking self first
            if(GetFirstElementAssignmentConflict(currentPlayer, conflictCheck, out conflict)) return true;

            // Check other applicable Players

            // Check System
            if(GetFirstElementAssignmentConflict(ReInput.players.SystemPlayer, conflictCheck, out conflict)) return true;

            // Check others
            if(!skipOtherPlayers) {
                IList<Player> players = ReInput.players.Players;
                for(int i = 0; i < players.Count; i++) {
                    Player player = players[i];
                    if(player == currentPlayer) continue; // already handled current above
                    if(GetFirstElementAssignmentConflict(player, conflictCheck, out conflict)) return true;
                }
            }

            return false;
        }

        private bool GetFirstElementAssignmentConflict(Player player, ElementAssignmentConflictCheck conflictCheck, out ElementAssignmentConflictInfo conflict) {
            foreach(ElementAssignmentConflictInfo c in player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck)) {
                conflict = c;
                return true;
            };
            conflict = new ElementAssignmentConflictInfo();
            return false;
        }

        #endregion

        #region Controller Calibration

        private void StartAxisCalibration(int axisIndex) {
            if(currentPlayer == null) return;
            if(currentPlayer.controllers.joystickCount == 0) return;
            Joystick joystick = currentJoystick;
            if(axisIndex < 0 || axisIndex >= joystick.axisCount) return;

            pendingAxisCalibration = new AxisCalibrator(joystick, axisIndex);
            ShowCalibrateAxisStep1Window();
        }

        private void EndAxisCalibration() {
            if(pendingAxisCalibration == null) return;
            pendingAxisCalibration.Commit();
            pendingAxisCalibration = null;
        }

        #endregion

        #region UI Selection

        private void SetUISelection(GameObject selection) {
            if(EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(selection);
        }

        private void RestoreLastUISelection() {
            if(lastUISelection == null || !lastUISelection.activeInHierarchy) {
                SetDefaultUISelection();
                return;
            }
            SetUISelection(lastUISelection);
        }

        private void SetDefaultUISelection() {
            if(!isOpen) return;
            if(references.defaultSelection == null) SetUISelection(null);
            else SetUISelection(references.defaultSelection.gameObject);
        }

        private void SelectDefaultMapCategory(bool redraw) {
            currentMapCategoryId = GetDefaultMapCategoryId();
            OnMapCategorySelected(currentMapCategoryId, redraw);

            if(!showMapCategories) return;

            for(int i = 0; i < _mappingSets.Length; i++) {
                InputMapCategory cat = ReInput.mapping.GetMapCategory(_mappingSets[i].mapCategoryId);
                if(cat == null) continue;
                currentMapCategoryId = _mappingSets[i].mapCategoryId;
                break;
            }
            if(currentMapCategoryId < 0) return; // invalid, no map cats found

            // Disable the selected button and enable others
            for(int i = 0; i < _mappingSets.Length; i++) {
                bool state = _mappingSets[i].mapCategoryId == currentMapCategoryId ? false : true;
                mapCategoryButtons[i].SetInteractible(state, false);
            }
        }

        private void CheckUISelection() {
            if(!isFocused) return;
            if(currentUISelection == null) RestoreLastUISelection();
        }

        private void OnUIElementSelected(GameObject selectedObject) {
            lastUISelection = selectedObject; // record the last selected UI element
        }

        #endregion

        #region Main Page

        private void SetIsFocused(bool state) {
            // Set interactible state on main page controls
            references.mainCanvasGroup.interactable = state;

            if(state) { // just received focus
                Redraw(false, false);
                RestoreLastUISelection();
                // Block input for a short time to prevent immediate button-down events when making a new assignment or assigning a controller
                blockInputOnFocusEndTime = Time.unscaledTime + blockInputOnFocusTimeout;
            }
        }

        public void Toggle() {
            if(isOpen) Close(true);
            else Open();
        }

        public void Open() {
            Open(false);
        }
        private void Open(bool force) {
            if(!initialized) Initialize();
            if(!initialized) return; // failed to init
            if(!force && isOpen) return;
            Clear();
            canvas.SetActive(true);
            OnPlayerSelected(0, false);
            SelectDefaultMapCategory(false);
            SetDefaultUISelection();
            Redraw(true, false);
            if(_ScreenOpenedEvent != null) _ScreenOpenedEvent();
            if(_onScreenOpened != null) _onScreenOpened.Invoke();
        }

        public void Close(bool save) {
            if(!initialized) return;
            if(!isOpen) return;
            if(save) { // save data before closing
                if(ReInput.userDataStore != null) ReInput.userDataStore.Save();
            }
            Clear();
            canvas.SetActive(false);
            SetUISelection(null); // deselect
            if(_ScreenClosedEvent != null) _ScreenClosedEvent();
            if(_onScreenClosed != null) _onScreenClosed.Invoke();
        }

        #endregion

        #region Clear / Reset

        private void Clear() {
            windowManager.CancelAll();
            lastUISelection = null;
            pendingInputMapping = null;
            pendingAxisCalibration = null;
            InputPollingStopped();
        }

        private void ClearCompletely() {
            ClearSpawnedObjects();
            ClearAllVars();
        }

        private void ClearSpawnedObjects() {

            windowManager.ClearCompletely();
            inputGrid.ClearAll();

            foreach(GUIButton item in playerButtons) {
                Object.Destroy(item.gameObject);
            }
            playerButtons.Clear();
            foreach(GUIButton item in mapCategoryButtons) {
                Object.Destroy(item.gameObject);
            }
            mapCategoryButtons.Clear();
            foreach(GUIButton item in assignedControllerButtons) {
                Object.Destroy(item.gameObject);
            }
            assignedControllerButtons.Clear();

            if(assignedControllerButtonsPlaceholder != null) {
                Object.Destroy(assignedControllerButtonsPlaceholder.gameObject);
                assignedControllerButtonsPlaceholder = null;
            }

            foreach(GameObject item in miscInstantiatedObjects) {
                Object.Destroy(item);
            }
            miscInstantiatedObjects.Clear();
        }

        private void ClearVarsOnPlayerChange() {
            currentJoystickId = -1;
        }

        private void ClearVarsOnJoystickChange() {
            currentJoystickId = -1;
        }

        private void ClearAllVars() {
            initialized = false;
            Instance = null; // clear singleton instance
            playerCount = 0;

            inputGrid = null;
            windowManager = null;
            currentPlayerId = -1;
            currentMapCategoryId = -1;
            playerButtons = null;
            mapCategoryButtons = null;
            miscInstantiatedObjects = null;
            canvas = null;
            lastUISelection = null;
            currentJoystickId = -1;

            pendingInputMapping = null;
            pendingAxisCalibration = null;

            inputFieldActivatedDelegate = null;
            inputFieldInvertToggleStateChangedDelegate = null;

            isPollingForInput = false;
        }

        public void Reset() {
            if(!initialized) return;
            ClearCompletely();
            Initialize();
            if(isOpen) Open(true);
        }

        #endregion

        #region Misc

        private void SetActionAxisInverted(bool state, ControllerType controllerType, int actionElementMapId) {
            if(currentPlayer == null) return;

            ControllerMapWithAxes map = GetControllerMap(controllerType) as ControllerMapWithAxes;
            if(map == null) return;

            ActionElementMap aem = map.GetElementMap(actionElementMapId);
            if(aem == null) return;

            aem.invert = state;
        }

        private ControllerMap GetControllerMap(ControllerType type) {
            if(currentPlayer == null) return null;
            int controllerId = 0;
            switch(type) {
                case ControllerType.Keyboard:
                    break;
                case ControllerType.Mouse:
                    break;
                case ControllerType.Joystick:
                    if(currentPlayer.controllers.joystickCount > 0) controllerId = currentJoystick.id;
                    else return null;
                    break;
                default: throw new System.NotImplementedException();
            }

            // We cannot support multiple layouts in the same category in this system, so just get the first map in the category we find
            return currentPlayer.controllers.maps.GetFirstMapInCategory(type, controllerId, currentMapCategoryId);
        }

        private ControllerMap GetControllerMapOrCreateNew(ControllerType controllerType, int controllerId, int layoutId) {
            ControllerMap map = GetControllerMap(controllerType);
            if(map == null) {
                currentPlayer.controllers.maps.AddEmptyMap(controllerType, controllerId, currentMapCategoryId, layoutId);
                map = currentPlayer.controllers.maps.GetMap(controllerType, controllerId, currentMapCategoryId, layoutId);
            }
            return map;
        }

        private int CountIEnumerable<T>(IEnumerable<T> enumerable) {
            if(enumerable == null) return 0;
            IEnumerator<T> enumerator = enumerable.GetEnumerator();
            if(enumerator == null) return 0;
            int count = 0;
            while(enumerator.MoveNext()) {
                count++;
            }
            return count;
        }

        private int GetDefaultMapCategoryId() {
            if(_mappingSets.Length == 0) return 0; // use Default

            // Get the first valid map category id we find
            for(int i = 0; i < _mappingSets.Length; i++) {
                if(ReInput.mapping.GetMapCategory(_mappingSets[i].mapCategoryId) == null) continue;
                return _mappingSets[i].mapCategoryId;
            }
            return 0;
        }

        private void SubscribeFixedUISelectionEvents() {
            if(references.fixedSelectableUIElements == null) return;
            foreach(GameObject go in references.fixedSelectableUIElements) {
                UIElementInfo elementInfo = UnityTools.GetComponent<UIElementInfo>(go);
                if(elementInfo == null) continue;
                elementInfo.OnSelectedEvent += OnUIElementSelected;
            }
        }

        private void SubscribeMenuControlInputEvents() {

            // Set up menu toggle event
            SubscribeRewiredInputEventAllPlayers(_screenToggleAction, OnScreenToggleActionPressed);

            // Set up player menu open event
            SubscribeRewiredInputEventAllPlayers(_screenOpenAction, OnScreenOpenActionPressed);

            // Set up player menu close event
            SubscribeRewiredInputEventAllPlayers(_screenCloseAction, OnScreenCloseActionPressed);

            // Set up universal cancel event
            SubscribeRewiredInputEventAllPlayers(_universalCancelAction, OnUniversalCancelActionPressed);
        }

        private void UnsubscribeMenuControlInputEvents() {

            // Remove  menu toggle event
            UnsubscribeRewiredInputEventAllPlayers(_screenToggleAction, OnScreenToggleActionPressed);

            // Remove  player menu open event
            UnsubscribeRewiredInputEventAllPlayers(_screenOpenAction, OnScreenOpenActionPressed);

            // Remove  player menu close event
            UnsubscribeRewiredInputEventAllPlayers(_screenCloseAction, OnScreenCloseActionPressed);

            // Remove universal cancel event
            UnsubscribeRewiredInputEventAllPlayers(_universalCancelAction, OnUniversalCancelActionPressed);
        }

        private void SubscribeRewiredInputEventAllPlayers(int actionId, System.Action<InputActionEventData> callback) {
            if(actionId < 0 || callback == null) return;

            if(ReInput.mapping.GetAction(actionId) == null) {
                Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
                return;
            }

            // Allow control by all players including system
            foreach(Player p in ReInput.players.AllPlayers) {
                p.AddInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionId);
            }
        }

        private void UnsubscribeRewiredInputEventAllPlayers(int actionId, System.Action<InputActionEventData> callback) {
            if(actionId < 0 || callback == null) return;
            if(!Rewired.ReInput.isReady) return; // Rewired may have been destroyed already

            if(ReInput.mapping.GetAction(actionId) == null) {
                Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
                return;
            }

            // Allow control by all players including system
            foreach(Player p in ReInput.players.AllPlayers) {
                p.RemoveInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionId);
            }
        }

        private int GetMaxControllersPerPlayer() {
            if(_rewiredInputManager.userData.ConfigVars.autoAssignJoysticks) return _rewiredInputManager.userData.ConfigVars.maxJoysticksPerPlayer;
            return _maxControllersPerPlayer;
        }

        private bool ShowAssignedControllers() {
            if(!_showControllers) return false;
            if(_showAssignedControllers) return true;
            if(GetMaxControllersPerPlayer() != 1) return true; // force display if Player can have more than 1 controller
            return false;
        }

        private void InspectorPropertyChanged(bool reset = false) {
            if(reset) Reset();
        }

        private void AssignController(Player player, int controllerId) {
            if(player == null) return;
            if(player.controllers.ContainsController(ControllerType.Joystick, controllerId)) return; // already assigned to this player

            // Remove any controllers first if player only allowed 1 at a time
            if(GetMaxControllersPerPlayer() == 1) {
                RemoveAllControllers(player);
                ClearVarsOnJoystickChange(); // redraw
            }

            // Manually remove the controller from all other Players first so the configs can be saved
            foreach(Player p in ReInput.players.Players) {
                if(p == player) continue; // skip self
                RemoveController(p, controllerId);
            }

            // Assign the controller to the Player
            player.controllers.AddController(ControllerType.Joystick, controllerId, false);

            // Load the controller data in case the user customized this controller before
            if(ReInput.userDataStore != null) ReInput.userDataStore.LoadControllerData(player.id, ControllerType.Joystick, controllerId);
        }

        private void RemoveAllControllers(Player player) {
            if(player == null) return;

            // Clear controllers from the Player manually so we can save them first
            IList<Joystick> joysticks = player.controllers.Joysticks;
            for(int i = joysticks.Count - 1; i >= 0; i--) {
                RemoveController(player, joysticks[i].id);
            }
        }

        private void RemoveController(Player player, int controllerId) {
            if(player == null) return;
            if(!player.controllers.ContainsController(ControllerType.Joystick, controllerId)) return;

            // Save the controller data before unassigning it
            if(ReInput.userDataStore != null) ReInput.userDataStore.SaveControllerData(player.id, ControllerType.Joystick, controllerId);

            // Unassign the controller
            player.controllers.RemoveController(ControllerType.Joystick, controllerId);
        }

        private bool IsAllowedAssignment(InputMapping pendingInputMapping, ControllerPollingInfo pollingInfo) {
            if(pendingInputMapping == null) return false;

            // Determine if user settings prevent this potential assignment

            // Handle user disabling of split-axis assignment fields
            if(pendingInputMapping.axisRange == AxisRange.Full) { // this is a full-axis assignment

                // If the user has disabled the split-axis fields, do not allow button or key assignments
                if(!_showSplitAxisInputFields && pollingInfo.elementType == ControllerElementType.Button) {
                    return false;
                }
            }

            return true;
        }

        private void InputPollingStarted() {
            bool prev = isPollingForInput;
            isPollingForInput = true;
            if(!prev) {
                if(_InputPollingStartedEvent != null) _InputPollingStartedEvent();
                if(_onInputPollingStarted != null) _onInputPollingStarted.Invoke();
            }
        }

        private void InputPollingStopped() {
            bool prev = isPollingForInput;
            isPollingForInput = false;
            if(prev) {
                if(_InputPollingEndedEvent != null) _InputPollingEndedEvent();
                if(_onInputPollingEnded != null) _onInputPollingEnded.Invoke();
            }
        }

        private int GetControllerInputFieldCount(ControllerType controllerType) {
            switch(controllerType) {
                case ControllerType.Keyboard: return _keyboardInputFieldCount;
                case ControllerType.Mouse: return _mouseInputFieldCount;
                case ControllerType.Joystick: return _controllerInputFieldCount;
                default: throw new System.NotImplementedException();
            }
        }

        private bool ShowSwapButton(int windowId, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers) {
            if(currentPlayer == null) return false;
            if(!_allowElementAssignmentSwap) return false;
            if(mapping == null || mapping.aem == null) return false;

            ElementAssignmentConflictCheck conflictCheck;
            if(!CreateConflictCheck(mapping, assignment, out conflictCheck)) {
                Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
                return false;
            }

            // Check for conflicts for swapping
            // Only consider this Player and System Player
            List<ElementAssignmentConflictInfo> conflicts = new List<ElementAssignmentConflictInfo>();
            conflicts.AddRange(currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck));
            conflicts.AddRange(ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck));
            if(conflicts.Count == 0) return false;

            ActionElementMap origAemToReplace = mapping.aem;

            // Get the first conflict in all applicable Players always checking self first
            ElementAssignmentConflictInfo firstConflict = conflicts[0];

            // Take the Action and the axis contribution from the conflict
            int swapActionId = firstConflict.elementMap.actionId;
            Pole swapAxisContribution = firstConflict.elementMap.axisContribution;

            // Take the axis range and element type from the original
            AxisRange swapAxisRange = origAemToReplace.axisRange;
            ControllerElementType swapElementType = origAemToReplace.elementType;

            if(swapElementType == firstConflict.elementMap.elementType && swapElementType == ControllerElementType.Axis) {
                if(swapAxisRange != firstConflict.elementMap.axisRange) {
                    if(swapAxisRange == AxisRange.Full) { // converting from full to split
                        swapAxisRange = AxisRange.Positive; // just use the positive side
                    } else if(firstConflict.elementMap.axisRange == AxisRange.Full) { // converting from split to full
                        // This is okay as long as there is space in the correct axis contribution field
                    }
                }
            } else if(swapElementType == ControllerElementType.Axis) {
                if(firstConflict.elementMap.elementType == ControllerElementType.Button || (firstConflict.elementMap.elementType == ControllerElementType.Axis && firstConflict.elementMap.axisRange != AxisRange.Full)) {
                    // Make sure Axis is split and only one side of it is bound to the Action
                    if(swapAxisRange == AxisRange.Full) {
                        swapAxisRange = AxisRange.Positive; // just bind the positive side of the Axis
                    }
                }
            }

            // Determine if there is space for the new swapped assignment
            // Prevent swap from creating a mapping that the user cannot see because the input fields
            // are already full. This can happen with a swap between a full-axis mapping and a button mapping.
            int usedFieldCount = 0;

            // If swapping into the same Controller Map, must consider the new mapping that would be created
            if(assignment.actionId == firstConflict.actionId && mapping.map == firstConflict.controllerMap) {
                Controller controller = ReInput.controllers.GetController(mapping.controllerType, mapping.controllerId);
                if(
                    SwapIsSameInputRange(
                        swapElementType,
                        swapAxisRange,
                        swapAxisContribution,
                        controller.GetElementById(assignment.elementIdentifierId).type,
                        assignment.axisRange,
                        assignment.axisContribution
                    )
                ) {
                    usedFieldCount++;
                }
            }

            // Count how many mappings already exist in the same mapping range on the target controller map to which we are swapping the mapping
            foreach(var aem in firstConflict.controllerMap.ElementMapsWithAction(swapActionId)) {
                if(aem.id == origAemToReplace.id) continue; // skip the original mapping
                if(conflicts.FindIndex(x => x.elementMapId == aem.id) >= 0) continue; // skip all conflicts because they will have been removed already
                if(
                    SwapIsSameInputRange(
                        swapElementType,
                        swapAxisRange,
                        swapAxisContribution,
                        aem.elementType,
                        aem.axisRange,
                        aem.axisContribution
                    )
                ) {
                    usedFieldCount++;
                }
            }

            return usedFieldCount < GetControllerInputFieldCount(mapping.controllerType); // there are unused fields, allow it
        }

        private bool SwapIsSameInputRange(ControllerElementType origElementType, AxisRange origAxisRange, Pole origAxisContribution, ControllerElementType conflictElementType, AxisRange conflictAxisRange, Pole conflictAxisContribution) {
            if((origElementType == ControllerElementType.Button ||
                    (origElementType == ControllerElementType.Axis && origAxisRange != AxisRange.Full)) && // the swap is a button or a split-axis
                    (conflictElementType == ControllerElementType.Button || (conflictElementType == ControllerElementType.Axis && conflictAxisRange != AxisRange.Full)) && // the existing mapping is a button or a split-axis
                    conflictAxisContribution == origAxisContribution) // the existing mapping has the same axis contribution
                {
                return true;
            } else if(origElementType == ControllerElementType.Axis && origAxisRange == AxisRange.Full && // the swap is a full-axis
                conflictElementType == ControllerElementType.Axis && conflictAxisRange == AxisRange.Full) // the existing mapping is a full-axis
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Editor Recompile

#if UNITY_EDITOR

        private bool recompiling;

        private void CheckEditorRecompile() {
            if(!recompiling) return;
            if(!ReInput.isReady) return;
            recompiling = false;
            PreInitialize();
            Initialize();
        }

        private void OnEditorRecompile() {
            recompiling = true;
            if(!initialized) return;
            Close(false);
            ClearCompletely();
        }

#endif

        #endregion

        #region Static Methods

        public static void ApplyTheme(ThemedElement.ElementInfo[] elementInfo) {
            if(Instance == null) return;
            if(Instance._themeSettings == null) return;
            if(!Instance._useThemeSettings) return; // themeing not allowed
            Instance._themeSettings.Apply(elementInfo);
        }

        public static UI.ControlMapper.LanguageDataBase GetLanguage() {
            if(Instance == null) return null;
            return Instance._language;
        }

        #endregion
    }
}