// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {
    using System;
    using UnityEngine;
    using Rewired;

    [Serializable]
    public abstract class LanguageDataBase : ScriptableObject {
        public abstract void Initialize();
        public abstract string GetCustomEntry(string key);
        public abstract bool ContainsCustomEntryKey(string key);
        public abstract string yes { get; }
        public abstract string no { get; }
        public abstract string add { get; }
        public abstract string replace { get; }
        public abstract string remove { get; }
        public abstract string swap { get; }
        public abstract string cancel { get; }
        public abstract string none { get; }
        public abstract string okay { get; }
        public abstract string done { get; }
        public abstract string default_ { get; }
        public abstract string assignControllerWindowTitle { get; }
        public abstract string assignControllerWindowMessage { get; }
        public abstract string controllerAssignmentConflictWindowTitle { get; }
        public abstract string elementAssignmentPrePollingWindowMessage { get; }
        public abstract string elementAssignmentConflictWindowMessage { get; }
        public abstract string mouseAssignmentConflictWindowTitle { get; }
        public abstract string calibrateControllerWindowTitle { get; }
        public abstract string calibrateAxisStep1WindowTitle { get; }
        public abstract string calibrateAxisStep2WindowTitle { get; }
        public abstract string inputBehaviorSettingsWindowTitle { get; }
        public abstract string restoreDefaultsWindowTitle { get; }
        public abstract string actionColumnLabel { get; }
        public abstract string keyboardColumnLabel { get; }
        public abstract string mouseColumnLabel { get; }
        public abstract string controllerColumnLabel { get; }
        public abstract string removeControllerButtonLabel { get; }
        public abstract string calibrateControllerButtonLabel { get; }
        public abstract string assignControllerButtonLabel { get; }
        public abstract string inputBehaviorSettingsButtonLabel { get; }
        public abstract string doneButtonLabel { get; }
        public abstract string restoreDefaultsButtonLabel { get; }
        public abstract string controllerSettingsGroupLabel { get; }
        public abstract string playersGroupLabel { get; }
        public abstract string assignedControllersGroupLabel { get; }
        public abstract string settingsGroupLabel { get; }
        public abstract string mapCategoriesGroupLabel { get; }
        public abstract string restoreDefaultsWindowMessage { get; }
        public abstract string calibrateWindow_deadZoneSliderLabel { get; }
        public abstract string calibrateWindow_zeroSliderLabel { get; }
        public abstract string calibrateWindow_sensitivitySliderLabel { get; }
        public abstract string calibrateWindow_invertToggleLabel { get; }
        public abstract string calibrateWindow_calibrateButtonLabel { get; }

        public abstract string GetControllerAssignmentConflictWindowMessage(string joystickName, string otherPlayerName, string currentPlayerName);
        public abstract string GetJoystickElementAssignmentPollingWindowMessage(string actionName);
        public abstract string GetJoystickElementAssignmentPollingWindowMessage_FullAxisFieldOnly(string actionName);
        public abstract string GetKeyboardElementAssignmentPollingWindowMessage(string actionName);
        public abstract string GetMouseElementAssignmentPollingWindowMessage(string actionName);
        public abstract string GetMouseElementAssignmentPollingWindowMessage_FullAxisFieldOnly(string actionName);
        public abstract string GetElementAlreadyInUseBlocked(string elementName);
        public abstract string GetElementAlreadyInUseCanReplace(string elementName, bool allowConflicts);
        public abstract string GetMouseAssignmentConflictWindowMessage(string otherPlayerName, string thisPlayerName);
        public abstract string GetCalibrateAxisStep1WindowMessage(string axisName);
        public abstract string GetCalibrateAxisStep2WindowMessage(string axisName);

        // Translation of Rewired core items

        public abstract string GetPlayerName(int playerId);
        public abstract string GetControllerName(Controller controller);
        public abstract string GetElementIdentifierName(ActionElementMap actionElementMap);
        public abstract string GetElementIdentifierName(Controller controller, int elementIdentifierId, AxisRange axisRange);
        public abstract string GetElementIdentifierName(KeyCode keyCode, ModifierKeyFlags modifierKeyFlags);
        public abstract string GetActionName(int actionId);
        public abstract string GetActionName(int actionId, AxisRange axisRange);
        public abstract string GetMapCategoryName(int id);
        public abstract string GetActionCategoryName(int id);
        public abstract string GetLayoutName(ControllerType controllerType, int id);
        public abstract string ModifierKeyFlagsToString(ModifierKeyFlags flags);
    }
}