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
    [RequireComponent(typeof(CanvasGroup))]
    public class Window : MonoBehaviour {

        public Image backgroundImage;
        public GameObject content;

        // Working vars

        private bool _initialized;
        private int _id = -1;
        private RectTransform _rectTransform;
        private Text _titleText;
        private List<Text> _contentText;
        private GameObject _defaultUIElement;
        private System.Action<int> _updateCallback;
        private System.Func<int, bool> _isFocusedCallback;
        private Timer _timer;
        private CanvasGroup _canvasGroup;
        public UnityAction cancelCallback;
        private GameObject lastUISelection;

        // Properties

        public bool hasFocus { get { return _isFocusedCallback != null ? _isFocusedCallback(_id) : false; } }
        public int id { get { return _id; } }
        public RectTransform rectTransform {
            get {
                if(_rectTransform == null) _rectTransform = gameObject.GetComponent<RectTransform>();
                return _rectTransform;
            }
        }
        public Text titleText { get { return _titleText; } }
        public List<Text> contentText { get { return _contentText; } }
        public GameObject defaultUIElement { get { return _defaultUIElement; } set { _defaultUIElement = value; } }
        public System.Action<int> updateCallback { get { return _updateCallback; } set { _updateCallback = value; } }
        public Timer timer { get { return _timer; } }
        public int width {
            get {
                return (int)rectTransform.sizeDelta.x;
            }
            set {
                Vector2 size = rectTransform.sizeDelta;
                size.x = value;
                rectTransform.sizeDelta = size;
            }
        }
        public int height {
            get {
                return (int)rectTransform.sizeDelta.y;
            }
            set {
                Vector2 size = rectTransform.sizeDelta;
                size.y = value;
                rectTransform.sizeDelta = size;
            }
        }
        protected bool initialized { get { return _initialized; } }

        // Unity Events

        void OnEnable() {
            StartCoroutine("OnEnableAsync"); // Use coroutine to get around issue with OnEnable being called before GUI is ready and button not highlighting
        }

        protected virtual void Update() {
            if(!_initialized) return;
            if(!hasFocus) return;
            CheckUISelection();
            if(_updateCallback != null) _updateCallback(_id);
        }

        // Public Methods

        public virtual void Initialize(int id, System.Func<int, bool> isFocusedCallback) {
            if(_initialized) {
                Debug.LogError("Window is already initialized!");
                return;
            }
            _id = id;
            _isFocusedCallback = isFocusedCallback;
            _timer = new Timer();
            _contentText = new List<Text>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _initialized = true;
        }

        public void SetSize(int width, int height) {
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        public void CreateTitleText(GameObject prefab, Vector2 offset) {
            CreateText(prefab, ref _titleText, "Title Text", UIPivot.TopCenter, UIAnchor.TopHStretch, offset);
        }
        public void CreateTitleText(GameObject prefab, Vector2 offset, string text) {
            CreateTitleText(prefab, offset);
            SetTitleText(text);
        }

        public void AddContentText(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset) {
            Text text = null;
            CreateText(prefab, ref text, "Content Text", pivot, anchor, offset);
            _contentText.Add(text);
        }
        public void AddContentText(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset, string text) {
            AddContentText(prefab, pivot, anchor, offset);
            SetContentText(text, _contentText.Count - 1);
        }

        public void AddContentImage(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset) {
            CreateImage(prefab, "Image", pivot, anchor, offset);
        }
        public void AddContentImage(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset, string text) {
            AddContentImage(prefab, pivot, anchor, offset);
        }

        public void CreateButton(GameObject prefab, UIPivot pivot, UIAnchor anchor, Vector2 offset, string buttonText, UnityAction confirmCallback, UnityAction cancelCallback, bool setDefault) {
            if(prefab == null) return;
            ButtonInfo buttonInfo;
            GameObject instance = CreateButton(prefab, "Button", anchor, pivot, offset, out buttonInfo);
            if(instance == null) return;
            Button button = instance.GetComponent<Button>();
            if(confirmCallback != null) button.onClick.AddListener(confirmCallback);

            // Create a UI cancel event for this button
            CustomButton customButton = button as CustomButton;
            if(cancelCallback != null && customButton != null) customButton.CancelEvent += cancelCallback;

            if(buttonInfo.text != null) buttonInfo.text.text = buttonText;
            if(setDefault) _defaultUIElement = instance;
        }
        
        public string GetTitleText(string text) {
            if(_titleText == null) return string.Empty;
            return _titleText.text;
        }

        public void SetTitleText(string text) {
            if(_titleText == null) return;
            _titleText.text = text;
        }

        public string GetContentText(int index) {
            if(_contentText == null || _contentText.Count <= index || _contentText[index] == null) return string.Empty;
            return _contentText[index].text;
        }

        public float GetContentTextHeight(int index) {
            if(_contentText == null || _contentText.Count <= index || _contentText[index] == null) return 0.0f;
            return _contentText[index].rectTransform.sizeDelta.y;
        }

        public void SetContentText(string text, int index) {
            if(_contentText == null || _contentText.Count <= index || _contentText[index] == null) return;
            _contentText[index].text = text;
        }

        public void SetUpdateCallback(System.Action<int> callback) {
            this.updateCallback = callback;
        }

        public virtual void TakeInputFocus() {
            if(EventSystem.current == null) return;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_defaultUIElement);
            Enable();
        }

        public virtual void Enable() {
            _canvasGroup.interactable = true;
        }

        public virtual void Disable() {
            _canvasGroup.interactable = false;
        }

        public virtual void Cancel() {
            if(!initialized) return;
            if(cancelCallback != null) cancelCallback();
        }

        // Private Methods

        private void CreateText(GameObject prefab, ref Text textComponent, string name, UIPivot pivot, UIAnchor anchor, Vector2 offset) {
            if(prefab == null || content == null) return;
            if(textComponent != null) {
                Debug.LogError("Window already has " + name + "!");
                return;
            }

            GameObject instance = UITools.InstantiateGUIObject<Text>(
                prefab,
                content.transform,
                name,
                pivot,
                anchor.min,
                anchor.max,
                offset
            );
            if(instance == null) return;
            textComponent = instance.GetComponent<Text>();
        }

        private void CreateImage(GameObject prefab, string name, UIPivot pivot, UIAnchor anchor, Vector2 offset) {
            if(prefab == null || content == null) return;

            UITools.InstantiateGUIObject<Image>(
                prefab,
                content.transform,
                name,
                pivot,
                anchor.min,
                anchor.max,
                offset
            );
        }

        private GameObject CreateButton(GameObject prefab, string name, UIAnchor anchor, UIPivot pivot, Vector2 offset, out ButtonInfo buttonInfo) {
            buttonInfo = null;
            if(prefab == null) return null;
            
            GameObject instance = UITools.InstantiateGUIObject<ButtonInfo>(
                prefab,
                content.transform,
                name,
                pivot,
                anchor.min,
                anchor.max,
                offset
            );
            if(instance == null) return null;

            buttonInfo = instance.GetComponent<ButtonInfo>();
            Button button = instance.GetComponent<Button>();
            if(button == null) {
                Debug.Log("Button prefab is missing Button component!");
                return null;
            }
            if(buttonInfo == null) {
                Debug.Log("Button prefab is missing ButtonInfo component!");
                return null;
            }
            return instance;
        }

        private IEnumerator OnEnableAsync() {
            yield return 1;
            // Set the default selection when modal is enabled
            if(EventSystem.current == null) yield break;
            if(defaultUIElement != null) EventSystem.current.SetSelectedGameObject(defaultUIElement); // select default UI element
            else EventSystem.current.SetSelectedGameObject(null); // deselect
        }

        private void CheckUISelection() {
            if(!hasFocus) return;
            if(EventSystem.current == null) return;
            if(EventSystem.current.currentSelectedGameObject == null) RestoreDefaultOrLastUISelection(); // nothing is selected, restore default or last selection
            lastUISelection = EventSystem.current.currentSelectedGameObject; // store current selection as last
        }

        private void RestoreDefaultOrLastUISelection() {
            if(!hasFocus) return;
            if(lastUISelection == null || !lastUISelection.activeInHierarchy) {
                SetUISelection(_defaultUIElement);
                return;
            }
            SetUISelection(lastUISelection);
        }

        private void SetUISelection(GameObject selection) {
            if(EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(selection);
        }

        public class Timer {

            private bool _started;
            private float end;

            public bool started { get { return _started; } }
            public bool finished {
                get {
                    if(!started) return false;
                    if(Time.realtimeSinceStartup < end) return false;
                    _started = false;
                    return true;
                }
            }
            public float remaining {
                get {
                    if(!_started) return 0.0f;
                    return end - Time.realtimeSinceStartup;
                }
            }

            public Timer() {

            }

            public void Start(float length) {
                end = Time.realtimeSinceStartup + length;
                _started = true;
            }

            public void Stop() {
                _started = false;
            }
        }
    }
}
