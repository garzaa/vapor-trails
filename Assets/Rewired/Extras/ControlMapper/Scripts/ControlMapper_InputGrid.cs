// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Rewired;

    public partial class ControlMapper {

        private class InputGrid {

            private InputGridEntryList list;
            private List<GameObject> groups;

            public InputGrid() {
                list = new InputGridEntryList();
                groups = new List<GameObject>();
            }

            public void AddMapCategory(int mapCategoryId) {
                list.AddMapCategory(mapCategoryId);
            }

            public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange) {
                list.AddAction(mapCategoryId, action, axisRange);
            }

            public void AddActionCategory(int mapCategoryId, int actionCategoryId) {
                list.AddActionCategory(mapCategoryId, actionCategoryId);
            }

            public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer) {
                list.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, fieldSetContainer);
            }

            public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, GUIInputField inputField) {
                list.AddInputField(mapCategoryId, action, axisRange, controllerType, fieldIndex, inputField);
            }

            public void AddGroup(GameObject group) {
                groups.Add(group);
            }

            public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, GUILabel label) {
                list.AddActionLabel(mapCategoryId, actionId, axisRange, label);
            }

            public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, GUILabel label) {
                list.AddActionCategoryLabel(mapCategoryId, actionCategoryId, label);
            }

            public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex) {
                return list.Contains(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
            }

            public GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex) {
                return list.GetGUIInputField(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
            }

            public IEnumerable<InputActionSet> GetActionSets(int mapCategoryId) {
                return list.GetActionSets(mapCategoryId);
            }

            public void SetColumnHeight(int mapCategoryId, float height) {
                list.SetColumnHeight(mapCategoryId, height);
            }

            public float GetColumnHeight(int mapCategoryId) {
                return list.GetColumnHeight(mapCategoryId);
            }

            public void SetFieldsActive(int mapCategoryId, bool state) {
                list.SetFieldsActive(mapCategoryId, state);
            }

            public void SetFieldLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label) {
                list.SetLabel(mapCategoryId, actionId, axisRange, controllerType, index, label);
            }

            public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert) {
                list.PopulateField(mapCategoryId, actionId, axisRange, controllerType, controllerId, index, actionElementMapId, label, invert);
            }

            public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId) {
                list.SetFixedFieldData(mapCategoryId, actionId, axisRange, controllerType, controllerId);
            }

            public void InitializeFields(int mapCategoryId) {
                list.InitializeFields(mapCategoryId);
            }

            public void Show(int mapCategoryId) {
                list.Show(mapCategoryId);
            }

            public void HideAll() {
                list.HideAll();
            }

            public void ClearLabels(int mapCategoryId) {
                list.ClearLabels(mapCategoryId);
            }

            private void ClearGroups() {
                for(int i = 0; i < groups.Count; i++) {
                    if(groups[i] == null) continue;
                    Object.Destroy(groups[i]);
                }
            }

            public void ClearAll() {
                ClearGroups();
                list.Clear();
            }

            
        }

        private class InputGridEntryList {

            private IndexedDictionary<int, MapCategoryEntry> entries;

            public InputGridEntryList() {
                entries = new IndexedDictionary<int, MapCategoryEntry>();
            }

            public void AddMapCategory(int mapCategoryId) {
                if(mapCategoryId < 0) return;
                if(entries.ContainsKey(mapCategoryId)) return; // already used
                entries.Add(mapCategoryId, new MapCategoryEntry());
            }

            public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange) {
                AddActionEntry(mapCategoryId, action, axisRange);
            }

            private ActionEntry AddActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange) {
                if(action == null) return null;
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return null;
                return entry.AddAction(action, axisRange);
            }

            public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, GUILabel label) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return;

                ActionEntry actionEntry = entry.GetActionEntry(actionId, axisRange);
                if(actionEntry == null) return;

                actionEntry.SetLabel(label);
            }

            public void AddActionCategory(int mapCategoryId, int actionCategoryId) {
                AddActionCategoryEntry(mapCategoryId, actionCategoryId);
            }

            private ActionCategoryEntry AddActionCategoryEntry(int mapCategoryId, int actionCategoryId) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return null;
                return entry.AddActionCategory(actionCategoryId);
            }

            public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, GUILabel label) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return;

                ActionCategoryEntry actionCategoryEntry = entry.GetActionCategoryEntry(actionCategoryId);
                if(actionCategoryEntry == null) return;

                actionCategoryEntry.SetLabel(label);
            }

            public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer) {
                ActionEntry actionEntry = GetActionEntry(mapCategoryId, action, axisRange);
                if(actionEntry == null) return;
                actionEntry.AddInputFieldSet(controllerType, fieldSetContainer);
            }

            public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, GUIInputField inputField) {
                ActionEntry actionEntry = GetActionEntry(mapCategoryId, action, axisRange);
                if(actionEntry == null) return;
                actionEntry.AddInputField(controllerType, fieldIndex, inputField);
            }

            public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange) {
                return GetActionEntry(mapCategoryId, actionId, axisRange) != null;
            }
            public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex) {
                ActionEntry actionEntry = GetActionEntry(mapCategoryId, actionId, axisRange);
                if(actionEntry == null) return false;
                return actionEntry.Contains(controllerType, fieldIndex);
            }

            public void SetColumnHeight(int mapCategoryId, float height) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return;
                entry.columnHeight = height;
            }

            public float GetColumnHeight(int mapCategoryId) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return 0.0f;
                return entry.columnHeight;
            }

            public GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex) {
                ActionEntry actionEntry = GetActionEntry(mapCategoryId, actionId, axisRange);
                if(actionEntry == null) return null;
                return actionEntry.GetGUIInputField(controllerType, fieldIndex);
            }

            private ActionEntry GetActionEntry(int mapCategoryId, int actionId, AxisRange axisRange) {
                if(actionId < 0) return null;

                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return null;

                ActionEntry actionEntry = entry.GetActionEntry(actionId, axisRange);
                return actionEntry;
            }
            private ActionEntry GetActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange) {
                if(action == null) return null;
                return GetActionEntry(mapCategoryId, action.id, axisRange);
            }

            public IEnumerable<InputActionSet> GetActionSets(int mapCategoryId) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) yield break;

                List<ActionEntry> list = entry.actionList;
                int count = list != null ? list.Count : 0;
                for(int i = 0; i < count; i++) {
                    yield return list[i].actionSet;
                }
            }

            public void SetFieldsActive(int mapCategoryId, bool state) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return;

                List<ActionEntry> list = entry.actionList;
                int count = list != null ? list.Count : 0;
                for(int i = 0; i < count; i++) {
                    list[i].SetFieldsActive(state);
                }
            }

            public void SetLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label) {
                ActionEntry entry = GetActionEntry(mapCategoryId, actionId, axisRange);
                if(entry == null) return;
                entry.SetFieldLabel(controllerType, index, label);
            }

            public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert) {
                ActionEntry entry = GetActionEntry(mapCategoryId, actionId, axisRange);
                if(entry == null) return;
                entry.PopulateField(controllerType, controllerId, index, actionElementMapId, label, invert);
            }

            public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId) {
                ActionEntry entry = GetActionEntry(mapCategoryId, actionId, axisRange);
                if(entry == null) return;
                entry.SetFixedFieldData(controllerType, controllerId);
            }

            public void InitializeFields(int mapCategoryId) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return;

                List<ActionEntry> list = entry.actionList;
                int count = list != null ? list.Count : 0;
                for(int i = 0; i < count; i++) {
                    list[i].Initialize();
                }
            }

            public void Show(int mapCategoryId) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return;
                entry.SetAllActive(true);
            }

            public void HideAll() {
                for(int i = 0; i < entries.Count; i++) {
                    entries[i].SetAllActive(false);
                }
            }

            public void ClearLabels(int mapCategoryId) {
                MapCategoryEntry entry;
                if(!entries.TryGet(mapCategoryId, out entry)) return;

                List<ActionEntry> list = entry.actionList;
                int count = list != null ? list.Count : 0;
                for(int i = 0; i < count; i++) {
                    list[i].ClearLabels();
                }
            }

            public void Clear() {
                entries.Clear();
            }

            private class MapCategoryEntry {

                private List<ActionEntry> _actionList;
                private IndexedDictionary<int, ActionCategoryEntry> _actionCategoryList;
                private float _columnHeight;

                public List<ActionEntry> actionList { get { return _actionList; } }
                public IndexedDictionary<int, ActionCategoryEntry> actionCategoryList { get { return _actionCategoryList; } }
                public float columnHeight { get { return _columnHeight; } set { _columnHeight = value; } }

                public MapCategoryEntry() {
                    _actionList = new List<ActionEntry>();
                    _actionCategoryList = new IndexedDictionary<int, ActionCategoryEntry>();
                }

                public ActionEntry GetActionEntry(int actionId, AxisRange axisRange) {
                    int index = IndexOfActionEntry(actionId, axisRange);
                    if(index < 0) return null;
                    return _actionList[index];
                }

                public int IndexOfActionEntry(int actionId, AxisRange axisRange) {
                    int count = _actionList.Count;
                    for(int i = 0; i < count; i++) {
                        if(_actionList[i].Matches(actionId, axisRange)) return i;
                    }
                    return -1;
                }

                public bool ContainsActionEntry(int actionId, AxisRange axisRange) {
                    return IndexOfActionEntry(actionId, axisRange) >= 0;
                }

                public ActionEntry AddAction(InputAction action, AxisRange axisRange) {
                    if(action == null) return null;
                    if(ContainsActionEntry(action.id, axisRange)) return null; // already used
                    _actionList.Add(new ActionEntry(action, axisRange));
                    return _actionList[_actionList.Count - 1];
                }

                public ActionCategoryEntry GetActionCategoryEntry(int actionCategoryId) {
                    if(!_actionCategoryList.ContainsKey(actionCategoryId)) return null;
                    return _actionCategoryList.Get(actionCategoryId);
                }

                public ActionCategoryEntry AddActionCategory(int actionCategoryId) {
                    if(actionCategoryId < 0) return null;
                    if(_actionCategoryList.ContainsKey(actionCategoryId)) return null; // already used
                    _actionCategoryList.Add(actionCategoryId, new ActionCategoryEntry(actionCategoryId));
                    return _actionCategoryList.Get(actionCategoryId);
                }

                public void SetAllActive(bool state) {
                    for(int i = 0; i < _actionCategoryList.Count; i++) {
                        _actionCategoryList[i].SetActive(state);
                    }
                    for(int i = 0; i < _actionList.Count; i++) {
                        _actionList[i].SetActive(state);
                    }
                }

            }

            private class ActionEntry {

                private IndexedDictionary<int, FieldSet> fieldSets;

                public GUILabel label;
                public readonly InputAction action;
                public readonly AxisRange axisRange;

                public readonly InputActionSet actionSet;

                public ActionEntry(InputAction action, AxisRange axisRange) {
                    this.action = action;
                    this.axisRange = axisRange;
                    this.actionSet = new InputActionSet(action.id, axisRange);
                    fieldSets = new IndexedDictionary<int, FieldSet>();
                }

                public void SetLabel(GUILabel label) {
                    this.label = label;
                }

                public bool Matches(int actionId, AxisRange axisRange) {
                    if(this.action.id != actionId) return false;
                    if(this.axisRange != axisRange) return false;
                    return true;
                }

                public void AddInputFieldSet(ControllerType controllerType, GameObject fieldSetContainer) {
                    if(fieldSets.ContainsKey((int)controllerType)) return;
                    fieldSets.Add((int)controllerType, new FieldSet(fieldSetContainer));
                }

                public void AddInputField(ControllerType controllerType, int fieldIndex, GUIInputField inputField) {
                    if(!fieldSets.ContainsKey((int)controllerType)) return;
                    FieldSet fieldSet = fieldSets.Get((int)controllerType);
                    if(fieldSet.fields.ContainsKey(fieldIndex)) return; // already used
                    fieldSet.fields.Add(fieldIndex, inputField);
                }

                public GUIInputField GetGUIInputField(ControllerType controllerType, int fieldIndex) {
                    if(!fieldSets.ContainsKey((int)controllerType)) return null;
                    if(!fieldSets.Get((int)controllerType).fields.ContainsKey(fieldIndex)) return null;
                    return fieldSets.Get((int)controllerType).fields.Get(fieldIndex);
                }

                public bool Contains(ControllerType controllerType, int fieldId) {
                    if(!fieldSets.ContainsKey((int)controllerType)) return false;
                    if(!fieldSets.Get((int)controllerType).fields.ContainsKey(fieldId)) return false;
                    return true;
                }

                public void SetFieldLabel(ControllerType controllerType, int index, string label) {
                    if(!fieldSets.ContainsKey((int)controllerType)) return;
                    if(!fieldSets.Get((int)controllerType).fields.ContainsKey(index)) return;
                    fieldSets.Get((int)controllerType).fields.Get(index).SetLabel(label);
                }

                public void PopulateField(ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert) {
                    if(!fieldSets.ContainsKey((int)controllerType)) return;
                    if(!fieldSets.Get((int)controllerType).fields.ContainsKey(index)) return;

                    GUIInputField field = fieldSets.Get((int)controllerType).fields.Get(index);
                    field.SetLabel(label); // set the label
                    field.actionElementMapId = actionElementMapId; // store the element map id
                    field.controllerId = controllerId;
                    if(field.hasToggle) {
                        field.toggle.SetInteractible(true, false);
                        field.toggle.SetToggleState(invert);
                        field.toggle.actionElementMapId = actionElementMapId; // store the element map id
                    }
                }

                public void SetFixedFieldData(ControllerType controllerType, int controllerId) {
                    if(!fieldSets.ContainsKey((int)controllerType)) return;
                    var setEntries = fieldSets.Get((int)controllerType);
                    int count = setEntries.fields.Count;
                    for(int i = 0; i < count; i++) {
                        setEntries.fields[i].controllerId = controllerId; // store the controller id
                    }
                }

                public void Initialize() {
                    for(int i = 0; i < fieldSets.Count; i++) {
                        var fieldSet = fieldSets[i];
                        int count = fieldSet.fields.Count;
                        for(int j = 0; j < count; j++) {
                            GUIInputField field = fieldSet.fields[j];
                            if(field.hasToggle) {
                                field.toggle.SetInteractible(false, false); // disable toggle
                                field.toggle.SetToggleState(false); // unset toggle
                                field.toggle.actionElementMapId = -1;
                            }
                            field.SetLabel("");
                            field.actionElementMapId = -1;
                            field.controllerId = -1;
                        }
                    }
                }

                public void SetActive(bool state) {
                    if(label != null) label.SetActive(state);
                    int count = fieldSets.Count;
                    for(int i = 0; i < count; i++) {
                        fieldSets[i].groupContainer.SetActive(state);
                    }
                }

                public void ClearLabels() {
                    for(int i = 0; i < fieldSets.Count; i++) {
                        var fieldSet = fieldSets[i];
                        int count = fieldSet.fields.Count;
                        for(int j = 0; j < count; j++) {
                            GUIInputField field = fieldSet.fields[j];
                            field.SetLabel("");
                        }
                    }
                }

                public void SetFieldsActive(bool state) {
                    for(int i = 0; i < fieldSets.Count; i++) {
                        var fieldSet = fieldSets[i];
                        int count = fieldSet.fields.Count;
                        for(int j = 0; j < count; j++) {
                            GUIInputField field = fieldSet.fields[j];
                            field.SetInteractible(state, false);
                            if(field.hasToggle) {
                                if(!state || field.toggle.actionElementMapId >= 0) { // only enable toggle if something is mapped. Disable it regardless.
                                    field.toggle.SetInteractible(state, false); // set toggle state
                                }
                            }
                        }
                    }
                }

            }

            private class FieldSet {
                public readonly GameObject groupContainer;
                public readonly IndexedDictionary<int, GUIInputField> fields;

                public FieldSet(GameObject groupContainer) {
                    this.groupContainer = groupContainer;
                    fields = new IndexedDictionary<int, GUIInputField>();
                }
            }

            private class ActionCategoryEntry {

                public readonly int actionCategoryId;
                public GUILabel label;

                public ActionCategoryEntry(int actionCategoryId) {
                    this.actionCategoryId = actionCategoryId;
                }

                public void SetLabel(GUILabel label) {
                    this.label = label;
                }

                public void SetActive(bool state) {
                    if(label != null) label.SetActive(state);
                }

            }

        }
    }
}
