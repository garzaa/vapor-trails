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

        private class WindowManager {

            private List<Window> windows;
            private GameObject windowPrefab;
            private Transform parent;
            private GameObject fader;

            private int idCounter = 0;

            public bool isWindowOpen {
                get {
                    for(int i = windows.Count - 1; i >= 0; i--) {
                        if(windows[i] == null) continue;
                        return true;
                    }
                    return false;
                }
            }
            public Window topWindow {
                get {
                    for(int i = windows.Count - 1; i >= 0; i--) {
                        if(windows[i] == null) continue;
                        return windows[i];
                    }
                    return null;
                }
            }


            public WindowManager(GameObject windowPrefab, GameObject faderPrefab, Transform parent) {
                this.windowPrefab = windowPrefab;
                this.parent = parent;
                windows = new List<Window>();
                fader = (GameObject)Object.Instantiate(faderPrefab);
                fader.transform.SetParent(parent, false);
                fader.GetComponent<RectTransform>().localScale = Vector2.one;
                SetFaderActive(false);
            }


            public Window OpenWindow(string name, int width, int height) {
                Window window = InstantiateWindow(name, width, height);
                UpdateFader();
                return window;
            }

            public Window OpenWindow(GameObject windowPrefab, string name) {
                if(windowPrefab == null) {
                    Debug.LogError("Rewired Control Mapper: Window Prefab is null!");
                    return null;
                }
                Window window = InstantiateWindow(name, windowPrefab);
                UpdateFader();
                return window;
            }

            public void CloseTop() {
                for(int i = windows.Count - 1; i >= 0; i--) {
                    if(windows[i] == null) {
                        windows.RemoveAt(i); // remove null entry
                        continue;
                    }
                    DestroyWindow(windows[i]);
                    windows.RemoveAt(i);
                    break;
                }
                UpdateFader();
            }

            public void CloseWindow(int windowId) {
                CloseWindow(GetWindow(windowId));
            }
            public void CloseWindow(Window window) {
                if(window == null) return;
                for(int i = windows.Count - 1; i >= 0; i--) {
                    if(windows[i] == null) {
                        windows.RemoveAt(i); // remove null entry
                        continue;
                    }
                    if(windows[i] != window) continue;
                    DestroyWindow(windows[i]);
                    windows.RemoveAt(i);
                    break;
                }
                UpdateFader();
                FocusTopWindow();
            }

            public void CloseAll() {
                SetFaderActive(false);
                for(int i = windows.Count - 1; i >= 0; i--) {
                    if(windows[i] == null) {
                        windows.RemoveAt(i); // remove null entry
                        continue;
                    }
                    DestroyWindow(windows[i]);
                    windows.RemoveAt(i);
                }
                UpdateFader();
            }

            public void CancelAll() {
                if(!isWindowOpen) return;
                for(int i = windows.Count - 1; i >= 0; i--) {
                    if(windows[i] == null) continue;
                    windows[i].Cancel();
                }
                CloseAll();
            }

            public Window GetWindow(int windowId) {
                if(windowId < 0) return null;
                for(int i = windows.Count - 1; i >= 0; i--) {
                    if(windows[i] == null) continue;
                    if(windows[i].id != windowId) continue;
                    return windows[i];
                }
                return null;
            }

            public bool IsFocused(int windowId) {
                if(windowId < 0) return false;
                if(topWindow == null) return false;
                return topWindow.id == windowId;
            }

            public void Focus(int windowId) {
                Focus(GetWindow(windowId));
            }
            public void Focus(Window window) {
                if(window == null) return;
                window.TakeInputFocus();
                DefocusOtherWindows(window.id);
            }

            private void DefocusOtherWindows(int focusedWindowId) {
                if(focusedWindowId < 0) return;
                for(int i = windows.Count - 1; i >= 0; i--) {
                    if(windows[i] == null) continue;
                    if(windows[i].id == focusedWindowId) continue; // skip focused window
                    windows[i].Disable();
                }
            }

            private void UpdateFader() {
                if(!isWindowOpen) {
                    SetFaderActive(false);
                    return;
                }

                // Activate the fader and move it behind the top window in the hierarchy
                Transform windowParent = topWindow.transform.parent;
                if(windowParent == null) return;

                SetFaderActive(true); // activate fader

                fader.transform.SetAsLastSibling(); // move to last place
                int topWindowIndex = topWindow.transform.GetSiblingIndex(); // get index of the top window which should always be next-to-last
                fader.transform.SetSiblingIndex(topWindowIndex); // move to next-to-last place

            }

            private void FocusTopWindow() {
                if(topWindow == null) return;
                topWindow.TakeInputFocus();
            }

            private void SetFaderActive(bool state) {
                fader.SetActive(state);
            }

            private Window InstantiateWindow(string name, int width, int height) {
                if(string.IsNullOrEmpty(name)) name = "Window";
                GameObject instance = UI.ControlMapper.UITools.InstantiateGUIObject<Window>(windowPrefab, parent, name);
                if(instance == null) return null;
                Window window = instance.GetComponent<Window>();
                if(window != null) {
                    window.Initialize(GetNewId(), IsFocused);
                    windows.Add(window);
                    window.SetSize(width, height);
                }
                return window;
            }
            private Window InstantiateWindow(string name, GameObject windowPrefab) {
                if(string.IsNullOrEmpty(name)) name = "Window";
                if(windowPrefab == null) return null;
                GameObject instance = UI.ControlMapper.UITools.InstantiateGUIObject<Window>(windowPrefab, parent, name);
                if(instance == null) return null;
                Window window = instance.GetComponent<Window>();
                if(window != null) {
                    window.Initialize(GetNewId(), IsFocused);
                    windows.Add(window);
                }
                return window;
            }

            private void DestroyWindow(Window window) {
                if(window == null) return;
                Object.Destroy(window.gameObject);
            }

            private int GetNewId() {
                int id = idCounter;
                idCounter++;
                return id;
            }

            public void ClearCompletely() {
                CloseAll();
                if(fader != null) Object.Destroy(fader);
            }

        }
    }
}
