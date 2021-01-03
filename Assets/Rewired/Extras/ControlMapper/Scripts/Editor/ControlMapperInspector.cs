// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Rewired;
    using Rewired.Data;
    using Rewired.Utils;

    [CustomEditor(typeof(ControlMapper))]
    public class ControlMapperInspector : UnityEditor.Editor {

        #region Inspector Variable Name Consts

        private const string c_rewiredInputManager = "_rewiredInputManager";
        private const string c_dontDestroyOnLoad = "_dontDestroyOnLoad";

        private const string c_openOnStart = "_openOnStart";

        private const string c_keyboardMapDefaultLayout = "_keyboardMapDefaultLayout";
        private const string c_mouseMapDefaultLayout = "_mouseMapDefaultLayout";
        private const string c_joystickMapDefaultLayout = "_joystickMapDefaultLayout";

        private const string c_mappingSets = "_mappingSets";

        private const string c_showPlayers = "_showPlayers";
        private const string c_showControllers = "_showControllers";
        private const string c_showKeyboard = "_showKeyboard";
        private const string c_showMouse = "_showMouse";

        private const string c_maxControllersPerPlayer = "_maxControllersPerPlayer";

        private const string c_showActionCategoryLabels = "_showActionCategoryLabels";

        private const string c_keyboardInputFieldCount = "_keyboardInputFieldCount";
        private const string c_mouseInputFieldCount = "_mouseInputFieldCount";
        private const string c_controllerInputFieldCount = "_controllerInputFieldCount";

        private const string c_showFullAxisInputFields = "_showFullAxisInputFields";
        private const string c_showSplitAxisInputFields = "_showSplitAxisInputFields";

        private const string c_allowElementAssignmentConflicts = "_allowElementAssignmentConflicts";
        private const string c_allowElementAssignmentSwap = "_allowElementAssignmentSwap";

        private const string c_actionLabelWidth = "_actionLabelWidth";
        private const string c_keyboardColMaxWidth = "_keyboardColMaxWidth";
        private const string c_mouseColMaxWidth = "_mouseColMaxWidth";
        private const string c_controllerColMaxWidth = "_controllerColMaxWidth";

        private const string c_inputRowHeight = "_inputRowHeight";
        private const string c_inputRowPadding = "_inputRowPadding";
        private const string c_inputRowFieldSpacing = "_inputRowFieldSpacing";
        private const string c_inputColumnSpacing = "_inputColumnSpacing";
        private const string c_inputRowCategorySpacing = "_inputRowCategorySpacing";
        private const string c_invertToggleWidth = "_invertToggleWidth";

        private const string c_defaultWindowWidth = "_defaultWindowWidth";
        private const string c_defaultWindowHeight = "_defaultWindowHeight";

        private const string c_controllerAssignmentTimeout = "_controllerAssignmentTimeout";
        private const string c_inputAssignmentTimeout = "_inputAssignmentTimeout";
        private const string c_preInputAssignmentTimeout = "_preInputAssignmentTimeout";
        private const string c_axisCalibrationTimeout = "_axisCalibrationTimeout";

        private const string c_ignoreMouseX = "_ignoreMouseXAxisAssignment";
        private const string c_ignoreMouseY = "_ignoreMouseYAxisAssignment";

        private const string c_screenToggleAction = "_screenToggleAction";
        private const string c_screenOpenAction = "_screenOpenAction";
        private const string c_screenCloseAction = "_screenCloseAction";
        private const string c_universalCancelAction = "_universalCancelAction";
        private const string c_universalCancelClosesScreen = "_universalCancelClosesScreen";

        private const string c_showInputBehaviorSettings = "_showInputBehaviorSettings";
        private const string c_inputBehaviorSettings = "_inputBehaviorSettings";

        private const string c_useThemeSettings = "_useThemeSettings";
        private const string c_themeSettings = "_themeSettings";

        private const string c_language = "_language";

        private const string c_prefabs = "prefabs";
        private const string c_references = "references";

        private const string c_showPlayersGroupLabel = "_showPlayersGroupLabel";
        private const string c_showControllerGroupLabel = "_showControllerGroupLabel";
        private const string c_showAssignedControllersGroupLabel = "_showAssignedControllersGroupLabel";
        private const string c_showSettingsGroupLabel = "_showSettingsGroupLabel";
        private const string c_showMapCategoriesGroupLabel = "_showMapCategoriesGroupLabel";

        private const string c_showControllerNameLabel = "_showControllerNameLabel";
        private const string c_showAssignedControllers = "_showAssignedControllers";

        private const string c_onScreenClosed = "_onScreenClosed";
        private const string c_onScreenOpened = "_onScreenOpened";
        private const string c_onPopupWindowClosed = "_onPopupWindowClosed";
        private const string c_onPopupWindowOpened = "_onPopupWindowOpened";
        private const string c_onInputPollingStarted = "_onInputPollingStarted";
        private const string c_onInputPollingEnded = "_onInputPollingEnded";

        #endregion

        #region Working Vars

        private Dictionary<string, SerializedProperty> properties;
        private bool foldout_internalData;
        private bool stylesCreated;
        private GUIStyle style_sectionBkg;
        private GUIStyle style_mapSetBkg;
        private GUIStyle style_sectionLabel;

        #endregion

        #region Properties

        private InputManager inputManager { get { return properties[c_rewiredInputManager].objectReferenceValue as InputManager; } }
        private UserData userData { get { return inputManager != null ? inputManager.userData : null; } }

        #endregion

        #region MonoBehaviour Events

        protected virtual void OnEnable() {
            properties = new Dictionary<string, SerializedProperty>();

            AddProperty(c_rewiredInputManager);
            AddProperty(c_dontDestroyOnLoad);

            AddProperty(c_openOnStart);

            AddProperty(c_keyboardMapDefaultLayout);
            AddProperty(c_mouseMapDefaultLayout);
            AddProperty(c_joystickMapDefaultLayout);

            AddProperty(c_mappingSets);

            AddProperty(c_showPlayers);
            AddProperty(c_showControllers);
            AddProperty(c_showKeyboard);
            AddProperty(c_showMouse);

            AddProperty(c_maxControllersPerPlayer);

            AddProperty(c_showActionCategoryLabels);

            AddProperty(c_keyboardInputFieldCount);
            AddProperty(c_mouseInputFieldCount);
            AddProperty(c_controllerInputFieldCount);

            AddProperty(c_showFullAxisInputFields);
            AddProperty(c_showSplitAxisInputFields);

            AddProperty(c_allowElementAssignmentConflicts);
            AddProperty(c_allowElementAssignmentSwap);

            AddProperty(c_actionLabelWidth);
            AddProperty(c_keyboardColMaxWidth);
            AddProperty(c_mouseColMaxWidth);
            AddProperty(c_controllerColMaxWidth);

            AddProperty(c_inputRowHeight);
            AddProperty(c_inputRowPadding);
            AddProperty(c_inputRowFieldSpacing);
            AddProperty(c_inputColumnSpacing);
            AddProperty(c_inputRowCategorySpacing);
            AddProperty(c_invertToggleWidth);

            AddProperty(c_defaultWindowWidth);
            AddProperty(c_defaultWindowHeight);

            AddProperty(c_controllerAssignmentTimeout);
            AddProperty(c_inputAssignmentTimeout);
            AddProperty(c_preInputAssignmentTimeout);
            AddProperty(c_axisCalibrationTimeout);

            AddProperty(c_ignoreMouseX);
            AddProperty(c_ignoreMouseY);

            AddProperty(c_screenToggleAction);
            AddProperty(c_screenOpenAction);
            AddProperty(c_screenCloseAction);
            AddProperty(c_universalCancelAction);
            AddProperty(c_universalCancelClosesScreen);

            AddProperty(c_showInputBehaviorSettings);
            AddProperty(c_inputBehaviorSettings);

            AddProperty(c_useThemeSettings);
            AddProperty(c_themeSettings);

            AddProperty(c_language);

            AddProperty(c_prefabs);
            AddProperty(c_references);

            AddProperty(c_showPlayersGroupLabel);
            AddProperty(c_showControllerGroupLabel);
            AddProperty(c_showAssignedControllersGroupLabel);
            AddProperty(c_showSettingsGroupLabel);
            AddProperty(c_showMapCategoriesGroupLabel);

            AddProperty(c_showControllerNameLabel);
            AddProperty(c_showAssignedControllers);

            AddProperty(c_onScreenClosed);
            AddProperty(c_onScreenOpened);
            AddProperty(c_onPopupWindowClosed);
            AddProperty(c_onPopupWindowOpened);
            AddProperty(c_onInputPollingStarted);
            AddProperty(c_onInputPollingEnded);
        }

        public override void OnInspectorGUI() {
            if(Application.isPlaying) {
                EditorGUILayout.HelpBox("Settings cannot be edited in Play mode.", MessageType.Info);
                return;
            }
            
            if(!stylesCreated) CreateStyles();

            serializedObject.Update();

            EditorGUILayout.Space();
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.PropertyField(properties[c_rewiredInputManager]);
                EditorGUILayout.PropertyField(properties[c_dontDestroyOnLoad]);
            }

            DrawLayout();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            //base.DrawDefaultInspector();
        }

        #endregion

        #region Layout

        private void DrawLayout() {

            // Check for Rewired Input Manager reference
            if(userData == null) {
                EditorGUILayout.HelpBox("A Rewired Input Manager must be linked in the inspector.", MessageType.Error);
                return;
            }

            // Screen options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Screen Options:", "Various options for the control mapper screen."), style_sectionLabel);
                EditorGUILayout.PropertyField(properties[c_openOnStart]);
            }

            // Player options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Player Options:", "Various options for Players."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_showPlayers]);
                if(properties[c_showPlayers].boolValue) {
                    EditorGUILayout.PropertyField(properties[c_showPlayersGroupLabel]);
                }
            }

            // Controller options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Controller Options:", "Various options for controllers."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_showControllers]);
                if(properties[c_showControllers].boolValue) {

                    if(!userData.ConfigVars.autoAssignJoysticks) { // only show max controllers per player if auto-assignment is disabled, otherwise it will be driven by auto-assignment settings
                        DrawIntProperty(properties[c_maxControllersPerPlayer], 0, 1000);
                    }
                    DrawIntProperty(properties[c_controllerInputFieldCount], 1, 5);
                    
                    // Show assigned controllers group and label
                    bool forceEnable = userData.ConfigVars.autoAssignJoysticks && userData.ConfigVars.maxJoysticksPerPlayer != 1;
                    if(forceEnable && !properties[c_showAssignedControllers].boolValue) properties[c_showAssignedControllers].boolValue = true;
                    bool guiEnabledPrev = GUI.enabled;
                    if(guiEnabledPrev && forceEnable) GUI.enabled = false;
                    EditorGUILayout.PropertyField(properties[c_showAssignedControllers]);
                    if(GUI.enabled != guiEnabledPrev) GUI.enabled = guiEnabledPrev;
                    if(properties[c_showAssignedControllers].boolValue) {
                        EditorGUILayout.PropertyField(properties[c_showAssignedControllersGroupLabel]);
                    }

                    // Labels
                    EditorGUILayout.PropertyField(properties[c_showControllerGroupLabel]);
                    EditorGUILayout.PropertyField(properties[c_showControllerNameLabel]);
                }
            }

            // Keyboard options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Keyboard Options:", "Various options for the Keyboard."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_showKeyboard]);
                if(properties[c_showKeyboard].boolValue) {
                    DrawIntProperty(properties[c_keyboardInputFieldCount], 1, 5);
                }
            }

            // Mouse options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Mouse Options:", "Various options for the Mouse."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_showMouse]);
                if(properties[c_showMouse].boolValue) {
                    DrawIntProperty(properties[c_mouseInputFieldCount], 1, 5);
                    EditorGUILayout.PropertyField(properties[c_ignoreMouseX]);
                    EditorGUILayout.PropertyField(properties[c_ignoreMouseY]);
                }
            }        

            // Input field options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Input Field Options:", "Various options for the input field grid."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_showFullAxisInputFields]);
                EditorGUILayout.PropertyField(properties[c_showSplitAxisInputFields]);
                if(!properties[c_showFullAxisInputFields].boolValue) {
                    if(!properties[c_showSplitAxisInputFields].boolValue) { // both are disabled
                        EditorGUILayout.HelpBox("No axis input fields will be displayed! The user will be unable to make any assignments to axis-type Actions.", MessageType.Error);
                    } else {
                        EditorGUILayout.HelpBox(
                            "Full-axis input fields will not be displayed. This field is required if you have made any full-axis assignments " +
                            "in the Rewired Input Manager or in saved XML user data. Disabling this field when you have full-axis assignments will result in the " +
                            "inability for the user to view, remove, or modify these full-axis assignments. In addition, these assignments may cause conflicts when " + 
                            "trying to remap the same axes to Actions.",
                            MessageType.Warning
                        );
                    }
                } else if(!properties[c_showSplitAxisInputFields].boolValue) {
                    EditorGUILayout.HelpBox(
                        "Split-axis input fields will not be displayed. These fields are required to assign buttons, keyboard keys, and hat or D-Pad directions to axis-type Actions. " +
                        "If you have made any split-axis assignments or button/key/D-pad assignments to axis-type Actions in the Rewired Input Manager or " +
                        "in saved XML user data, disabling these fields will result in the inability for the user to view, remove, or modify these assignments. " +
                        "In addition, these assignments may cause conflicts when trying to remap the same elements to Actions.",
                        MessageType.Warning
                    );
                }
            }

            // Mapping sets
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Map Categories and Actions:", "Options for the Map Categories and Actions displayed to the user for input mapping."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_showActionCategoryLabels]);
                if(properties[c_mappingSets].arraySize > 1) {
                    EditorGUILayout.PropertyField(properties[c_showMapCategoriesGroupLabel]);    
                }
                DrawMappingSet(properties[c_mappingSets]);
            }

            // Layouts
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Default Layouts:", "The controller map Layout that will be shown to the user. This remapping system only supports a single fixed Layout per controller type."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("This Layout defined here is only a suggestion. If a ControllerMap with this Layout is found in the Player, it will be used. Otherwise, the first ControllerMap in the correct category found will be used regardless of the Layout id.", MessageType.Info);
                EditorGUILayout.Space();
                DrawKeyboardMapDefaultLayoutProperty();
                DrawMouseMapDefaultLayoutProperty();
                DrawJoystickMapDefaultLayoutProperty();
            }

            // Input Behaviors
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Input Behaviors:", "Settings for user-modifiable Input Behaviors. Only certain properties are supported."), style_sectionLabel);
                EditorGUILayout.PropertyField(properties[c_showInputBehaviorSettings]);
                if(properties[c_showInputBehaviorSettings].boolValue) {
                    EditorGUILayout.PropertyField(properties[c_showSettingsGroupLabel]);
                    DrawInputBehaviorSettings(properties[c_inputBehaviorSettings]);
                }
            }

            // Element Assignment options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Element Assignment Options:", "Various options for the element assignment."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_allowElementAssignmentConflicts]);
                EditorGUILayout.PropertyField(properties[c_allowElementAssignmentSwap]);
            }

            // Timer options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Timer Options:", "Various timer options."), style_sectionLabel);
                EditorGUILayout.Space();
                DrawFloatProperty(properties[c_preInputAssignmentTimeout], 0.0f, 1000.0f);
                DrawFloatProperty(properties[c_inputAssignmentTimeout], 0.0f, 1000.0f);
                DrawFloatProperty(properties[c_controllerAssignmentTimeout], 0.0f, 1000.0f);
                DrawFloatProperty(properties[c_axisCalibrationTimeout], 0.0f, 1000.0f);
            }

            // Input grid layout options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Input Grid Layout Options:", "Dimensions for various elements in the input grid."), style_sectionLabel);
                EditorGUILayout.Space();
                DrawIntProperty(properties[c_actionLabelWidth], 0, 10000);
                DrawIntProperty(properties[c_keyboardColMaxWidth], 0, 10000);
                DrawIntProperty(properties[c_mouseColMaxWidth], 0, 10000);
                DrawIntProperty(properties[c_controllerColMaxWidth], 0, 10000);
                DrawIntProperty(properties[c_inputRowHeight], 0, 10000);
                EditorGUILayout.PropertyField(properties[c_inputRowPadding], true);
                DrawIntProperty(properties[c_inputRowFieldSpacing], 0, 10000);
                DrawIntProperty(properties[c_inputColumnSpacing], 0, 10000);
                DrawIntProperty(properties[c_inputRowCategorySpacing], 0, 10000);
                DrawIntProperty(properties[c_invertToggleWidth], 0, 10000);
            }

            // Popup window options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Popup Window Options:", "Various options for popup windows."), style_sectionLabel);
                EditorGUILayout.Space();
                DrawIntProperty(properties[c_defaultWindowWidth], 0, 10000);
                DrawIntProperty(properties[c_defaultWindowHeight], 0, 10000);
            }

            // Menu control Actions
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Screen Control Actions:", "Actions that control the mapping screen."), style_sectionLabel);
                EditorGUILayout.Space();
                DrawRewiredActionProperty(properties[c_screenOpenAction]);
                DrawRewiredActionProperty(properties[c_screenCloseAction]);
                DrawRewiredActionProperty(properties[c_screenToggleAction]);
                DrawRewiredActionProperty(properties[c_universalCancelAction]);
                EditorGUILayout.PropertyField(properties[c_universalCancelClosesScreen]);
            }

            // Theme options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Theme Options:", "UI theme options."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_useThemeSettings]);
                if(properties[c_useThemeSettings].boolValue) EditorGUILayout.PropertyField(properties[c_themeSettings], true);
            }

            // Language options
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Language Options:", "Language options."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_language]);
            }

            // Unity Events
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Events:", "Events."), style_sectionLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(properties[c_onScreenOpened]);
                EditorGUILayout.PropertyField(properties[c_onScreenClosed]);
                EditorGUILayout.PropertyField(properties[c_onPopupWindowOpened]);
                EditorGUILayout.PropertyField(properties[c_onPopupWindowClosed]);
                EditorGUILayout.PropertyField(properties[c_onInputPollingStarted]);
                EditorGUILayout.PropertyField(properties[c_onInputPollingEnded]);
            }

            // Advanced settings
            using(new EditorGUILayoutSection(true, style_sectionBkg)) {
                EditorGUILayout.LabelField(new GUIContent("Advanced Settings:", "These are internal settings like references to GameObject and Prefabs. You should not have to change these."), style_sectionLabel);
                foldout_internalData = EditorGUILayout.Foldout(foldout_internalData, "Internal Data");
                if(foldout_internalData) {
                    EditorGUILayout.PropertyField(properties[c_prefabs], true);
                    EditorGUILayout.PropertyField(properties[c_references], true);
                }
            }
        }

        #endregion

        #region Draw Specific Properties

        private void DrawKeyboardMapDefaultLayoutProperty() {
            DrawPopupProperty(
                userData.GetKeyboardLayoutIds(),
                userData.GetKeyboardLayoutNames(),
                properties[c_keyboardMapDefaultLayout]
            );
        }

        private void DrawMouseMapDefaultLayoutProperty() {
            DrawPopupProperty(
                userData.GetMouseLayoutIds(),
                userData.GetMouseLayoutNames(),
                properties[c_mouseMapDefaultLayout]
            );
        }

        private void DrawJoystickMapDefaultLayoutProperty() {
            DrawPopupProperty(
                userData.GetJoystickLayoutIds(),
                userData.GetJoystickLayoutNames(),
                properties[c_joystickMapDefaultLayout]
            );
        }

        private void DrawMappingSet(SerializedProperty mapSetsArray) {
            if(mapSetsArray == null || !mapSetsArray.isArray) return;

            int[] mapCategoryIds = userData.GetMapCategoryIds();
            string[] mapCategoryNames = userData.GetMapCategoryNames();
            int[] actionCategoryIds = userData.GetActionCategoryIds();
            string[] actionCategoryNames = userData.GetActionCategoryNames();
            int[] actionIds = userData.GetActionIds();
            string[] actionNames = userData.GetActionNames();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "You can define a list of Map Categories to be shown to the user for remapping.\n\n" +
                "You can choose to show one or multiple Map Categories. If you define multiple Map Categories here, " +
                "a new row of buttons will be shown that will allow the user to select a Map Category for which to create input assignments. " +
                "You must define a list of assignable Actions to displayed to the user for each Map Category. You can choose to show the user a list of all user-assignable Actions " +
                "contained in an one or more Action Categories or a customized list of individual Actions."
            , MessageType.Info);

            EditorGUILayout.Space();

            int count = mapSetsArray.arraySize;
            if(count == 0) {
                EditorGUILayout.HelpBox("You must have at least one map category!", MessageType.Error);
            }
            for(int i = 0; i < count; i++) {

                using(new EditorGUILayoutSection(true, style_mapSetBkg)) {

                    SerializedProperty mapSet = mapSetsArray.GetArrayElementAtIndex(i);

                    GUILayout.Space(20f);
                    SerializedProperty mapCategoryId = mapSet.FindPropertyRelative("_mapCategoryId");
                    DrawPopupProperty(new GUIContent("Map Category", "The Map Category that will be displayed to the user for mapping."), mapCategoryIds, mapCategoryNames, mapCategoryId); // NOTE: mapCategoryId tool tip from Attribute is always NULL!
                    int selectedMapCategoryIndex = System.Array.IndexOf<int>(mapCategoryIds, mapCategoryId.intValue);
                    if(selectedMapCategoryIndex < 0) continue;

                    SerializedProperty actionListMode = mapSet.FindPropertyRelative("_actionListMode");
                    EditorGUILayout.PropertyField(actionListMode);

                    EditorGUILayout.Space();

                    if((ControlMapper.MappingSet.ActionListMode)actionListMode.intValue == ControlMapper.MappingSet.ActionListMode.ActionCategory) {

                        EditorGUILayout.LabelField(new GUIContent("Action Categories:", "List each Action Category you want to be displayed for this map category. This will list all user-assignable Actions in that category allowing the user to make assignments for each of these Actions."));
                        EditorGUILayout.Space();

                        SerializedProperty actionCategoryIdsArray = mapSet.FindPropertyRelative("_actionCategoryIds");
                        DrawEditableSerializedPropertyArray(actionCategoryIdsArray, "Action Category", actionCategoryIds, actionCategoryNames);

                    } else {

                        EditorGUILayout.LabelField(new GUIContent("Actions:", "List each Action you want to be displayed for this map category. This will allow the user to make assignments for each of these Actions."));
                        EditorGUILayout.Space();

                        SerializedProperty actionIdsArray = mapSet.FindPropertyRelative("_actionIds");
                        DrawEditableSerializedPropertyArray(actionIdsArray, "Action", actionIds, actionNames);
                    }

                    // Array control butons
                    GUILayout.Space(20f);
                    using(new EditorGUILayoutSection(false)) {
                        GUILayout.FlexibleSpace();
                        bool guiEnabled = GUI.enabled;
                        if(i == 0) GUI.enabled = false; // don't allow deleting the first entry
                        if(GUILayout.Button("Delete", GUILayout.ExpandWidth(false), GUILayout.Width(100f))) {
                            mapSetsArray.DeleteArrayElementAtIndex(i);
                            break; // exit now to avoid issues
                        }
                        if(i == 0) GUI.enabled = guiEnabled;
                    }
                }

                GUILayout.Space(20f);
            }

            EditorGUILayout.Space();
            if(GUILayout.Button("+ Add Map Category")) {
                mapSetsArray.InsertArrayElementAtIndex(mapSetsArray.arraySize);
            }
            EditorGUILayout.Space();
        }

        private void DrawInputBehaviorSettings(SerializedProperty settingsArray) {
            if(settingsArray == null || !settingsArray.isArray) return;

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "You can define a list of Input Behaviors to be shown to the user for modification. If enabled, new controls will be displayed so the user can modify these settings. " +
                "This is useful if you need to allow the user to set certain per-Action sensitivity levels such as Mouse Look Sensitivity."
            , MessageType.Info);

            EditorGUILayout.Space();

            int[] inputBehaviorIds = userData.GetInputBehaviorIds();
            string[] inputBehaviorNames = userData.GetInputBehaviorNames();

            int count = settingsArray.arraySize;
            for(int i = 0; i < count; i++) {

                using(new EditorGUILayoutSection(true, style_mapSetBkg)) {

                    SerializedProperty setting = settingsArray.GetArrayElementAtIndex(i);
                    if(setting == null) continue;

                    GUILayout.Space(20f);
                    SerializedProperty inputBehaviorId = setting.FindPropertyRelative("_inputBehaviorId");
                    DrawPopupProperty(new GUIContent("Input Behavior", "The Input Behavior that will be displayed to the user for modification."), inputBehaviorIds, inputBehaviorNames, inputBehaviorId); // NOTE: mapCategoryId tool tip from Attribute is always NULL!
                    int selectedIndex = System.Array.IndexOf<int>(inputBehaviorIds, inputBehaviorId.intValue);
                    if(selectedIndex < 0) continue;

                    // Display settings

                    SerializedProperty labelLanguageKey = setting.FindPropertyRelative("_labelLanguageKey");

                    SerializedProperty showJoystickAxisSensitivity = setting.FindPropertyRelative("_showJoystickAxisSensitivity");
                    SerializedProperty showMouseXYAxisSensitivity = setting.FindPropertyRelative("_showMouseXYAxisSensitivity");

                    SerializedProperty joystickAxisSensitivityLabelLanguageKey = setting.FindPropertyRelative("_joystickAxisSensitivityLabelLanguageKey");
                    SerializedProperty mouseXYAxisSensitivityLabelLanguageKey = setting.FindPropertyRelative("_mouseXYAxisSensitivityLabelLanguageKey");

                    SerializedProperty joystickAxisSensitivityIcon = setting.FindPropertyRelative("_joystickAxisSensitivityIcon");
                    SerializedProperty mouseXYAxisSensitivityIcon = setting.FindPropertyRelative("_mouseXYAxisSensitivityIcon");

                    SerializedProperty joystickAxisSensitivityMin = setting.FindPropertyRelative("_joystickAxisSensitivityMin");
                    SerializedProperty joystickAxisSensitivityMax = setting.FindPropertyRelative("_joystickAxisSensitivityMax");
                    SerializedProperty mouseXYAxisSensitivityMin = setting.FindPropertyRelative("_mouseXYAxisSensitivityMin");
                    SerializedProperty mouseXYAxisSensitivityMax = setting.FindPropertyRelative("_mouseXYAxisSensitivityMax");

                    EditorGUILayout.PropertyField(labelLanguageKey);
                    
                    EditorGUILayout.PropertyField(showJoystickAxisSensitivity);
                    if(showJoystickAxisSensitivity.boolValue) {
                        EditorGUILayout.PropertyField(joystickAxisSensitivityLabelLanguageKey);
                        EditorGUILayout.PropertyField(joystickAxisSensitivityIcon);
                        DrawFloatProperty(joystickAxisSensitivityMin, 0f, 10000f);
                        DrawFloatProperty(joystickAxisSensitivityMax, 0f, 10000f);
                    }

                    EditorGUILayout.PropertyField(showMouseXYAxisSensitivity);
                    if(showMouseXYAxisSensitivity.boolValue) {
                        EditorGUILayout.PropertyField(mouseXYAxisSensitivityLabelLanguageKey);
                        EditorGUILayout.PropertyField(mouseXYAxisSensitivityIcon);
                        DrawFloatProperty(mouseXYAxisSensitivityMin, 0f, 10000f);
                        DrawFloatProperty(mouseXYAxisSensitivityMax, 0f, 10000f);
                    }
                    
                    EditorGUILayout.Space();

                    // Array control butons
                    GUILayout.Space(20f);
                    using(new EditorGUILayoutSection(false)) {
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button("Delete", GUILayout.ExpandWidth(false), GUILayout.Width(100f))) {
                            settingsArray.DeleteArrayElementAtIndex(i);
                            break; // exit now to avoid issues
                        }
                    }
                }

                GUILayout.Space(20f);
            }

            EditorGUILayout.Space();
            if(GUILayout.Button("+ Add Input Behavior")) {
                settingsArray.InsertArrayElementAtIndex(settingsArray.arraySize);
                SerializedProperty setting = settingsArray.GetArrayElementAtIndex(settingsArray.arraySize - 1);
                // Clear to defaults
                setting.FindPropertyRelative("_inputBehaviorId").intValue = 0;
                setting.FindPropertyRelative("_labelLanguageKey").stringValue = string.Empty;
                setting.FindPropertyRelative("_showJoystickAxisSensitivity").boolValue = false;
                setting.FindPropertyRelative("_showMouseXYAxisSensitivity").boolValue = false;
                setting.FindPropertyRelative("_joystickAxisSensitivityLabelLanguageKey").stringValue = string.Empty;
                setting.FindPropertyRelative("_mouseXYAxisSensitivityLabelLanguageKey").stringValue = string.Empty;
                setting.FindPropertyRelative("_joystickAxisSensitivityIcon").objectReferenceValue = (Sprite)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("0c9ce4e64fb83764aa394faeeed56210"), typeof(Sprite));
                setting.FindPropertyRelative("_mouseXYAxisSensitivityIcon").objectReferenceValue = (Sprite)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath("fc88ce24a47f4014cb0ad237abc8d1dd"), typeof(Sprite));
                setting.FindPropertyRelative("_joystickAxisSensitivityMin").floatValue = 0.0f;
                setting.FindPropertyRelative("_joystickAxisSensitivityMax").floatValue = 2.0f;
                setting.FindPropertyRelative("_mouseXYAxisSensitivityMin").floatValue = 0.0f;
                setting.FindPropertyRelative("_mouseXYAxisSensitivityMax").floatValue = 2.0f;
            }
            EditorGUILayout.Space();
        }

        #endregion

        #region Draw Property Types
        
        private void DrawPopupProperty(int[] values, string[] names, SerializedProperty serializedProperty) {
            DrawPopupProperty(new GUIContent(serializedProperty.displayName, serializedProperty.tooltip), values, names, serializedProperty);
        }
        private void DrawPopupProperty(GUIContent label, int[] values, string[] names, SerializedProperty serializedProperty) {
            int valueCount = values != null ? values.Length : 0;
            int nameCount = names != null ? names.Length : 0;
            if(valueCount != nameCount) throw new System.Exception("values.Length must equal names.Length!");

            int selectedIndex = valueCount > 0 ? System.Array.IndexOf<int>(values, serializedProperty.intValue) : -1;

            int newIndex = EditorGUILayout.Popup(label, selectedIndex, ToGUIContentArray(names));
            if(newIndex != selectedIndex) { // 
                serializedProperty.intValue = values[newIndex];
            }
        }

        private void DrawIntProperty(SerializedProperty serializedProperty, int min, int max) {
            if(serializedProperty.intValue < min) serializedProperty.intValue = min;
            if(serializedProperty.intValue > max) serializedProperty.intValue = max;
            EditorGUILayout.PropertyField(serializedProperty);
        }

        private void DrawFloatProperty(SerializedProperty serializedProperty, float min, float max) {
            if(serializedProperty.floatValue < min) serializedProperty.floatValue = min;
            if(serializedProperty.floatValue > max) serializedProperty.floatValue = max;
            EditorGUILayout.PropertyField(serializedProperty);
        }

        private void DrawRewiredActionProperty(SerializedProperty serializedProperty) {
            if(serializedProperty == null) return;
            DrawPopupProperty(
                ArrayTools.Insert(userData.GetActionIds(), 0, -1),
                ArrayTools.Insert(userData.GetActionNames(), 0, "[None]"),
                serializedProperty
            );
        }

        private void DrawEditableSerializedPropertyArray(SerializedProperty serializedPropertyArray, string label, int[] values, string[] names) {
            if(serializedPropertyArray == null || !serializedPropertyArray.isArray) return;

            for(int i = 0; i < serializedPropertyArray.arraySize; i++) {
                SerializedProperty prop = serializedPropertyArray.GetArrayElementAtIndex(i);

                using(new EditorGUILayoutSection(false)) {
                    DrawPopupProperty(new GUIContent(label + " " + i), values, names, prop);
                    GUILayout.Space(10f);
                    if(GUILayout.Button("Del", GUILayout.Width(50f))) {
                        serializedPropertyArray.DeleteArrayElementAtIndex(i);
                    }
                }
            }

            EditorGUILayout.Space();
            if(GUILayout.Button("Add " + label)) { // insert new one at end
                serializedPropertyArray.InsertArrayElementAtIndex(serializedPropertyArray.arraySize);
            }
        }

        #endregion

        #region Misc

        private void AddProperty(string name) {
            properties.Add(name, serializedObject.FindProperty(name));
        }

        private GUIContent[] ToGUIContentArray(string[] array) {
            if(array == null) return null;
            GUIContent[] retVal = new GUIContent[array.Length];
            for(int i = 0; i < array.Length; i++) {
                retVal[i] = new GUIContent(array[i]);
            }
            return retVal;
        }

        private void CreateStyles() {
            if(stylesCreated) return;

            style_mapSetBkg = new GUIStyle(GUI.skin.window);
            style_mapSetBkg.padding = new RectOffset(10, 10, 15, 15);

            style_sectionBkg = new GUIStyle(GUI.skin.box);
            style_sectionBkg.padding = new RectOffset(10, 10, 10, 10);
            style_sectionBkg.margin = new RectOffset(0, 0, 10, 10);

            style_sectionLabel = new GUIStyle(GUI.skin.label);
            style_sectionLabel.fontStyle = FontStyle.Bold;
            stylesCreated = true;
        }

        #endregion

        #region Private Classes

        private class EditorGUILayoutSection : System.IDisposable {

            private readonly bool vertical;

            public EditorGUILayoutSection(bool vertical) : this(vertical, null) {

            }
            public EditorGUILayoutSection(bool vertical, GUIStyle style) {
                this.vertical = vertical;
                if(vertical) {
                    if(style != null) EditorGUILayout.BeginVertical(style);
                    else EditorGUILayout.BeginVertical();
                } else {
                    if(style != null) EditorGUILayout.BeginHorizontal(style);
                    else EditorGUILayout.BeginHorizontal();
                }
            }

            #region IDisposable Implementation

            private bool _disposed;

            public virtual void Dispose() {
                Dispose(true);
                System.GC.SuppressFinalize(this);
            }

            ~EditorGUILayoutSection() {
                Dispose(false);
            }

            protected virtual void Dispose(bool disposing) {
                if(_disposed) return;

                if(disposing) {
                    // free other managed objects
                    if(vertical) EditorGUILayout.EndVertical();
                    else EditorGUILayout.EndHorizontal();
                }

                // release any unmanaged objects

                _disposed = true;
            }

            #endregion
        }

        #endregion
    }
}
