// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using Rewired;

    [CustomEditor(typeof(CustomToggle), true)]
    [CanEditMultipleObjects]
    public class CustomToggleInspector : UnityEditor.UI.ToggleEditor {

        private SerializedProperty disabledHighlightedSprite;
        private SerializedProperty disabledHighlightedColor;
        private SerializedProperty disabledHighlightedTrigger;

        private SerializedProperty autoNavUp;
        private SerializedProperty autoNavDown;
        private SerializedProperty autoNavLeft;
        private SerializedProperty autoNavRight;

        protected override void OnEnable() {
            base.OnEnable();

            disabledHighlightedSprite = serializedObject.FindProperty("_disabledHighlightedSprite");
            disabledHighlightedColor = serializedObject.FindProperty("_disabledHighlightedColor");
            disabledHighlightedTrigger = serializedObject.FindProperty("_disabledHighlightedTrigger");
            autoNavUp = serializedObject.FindProperty("_autoNavUp");
            autoNavDown = serializedObject.FindProperty("_autoNavDown");
            autoNavLeft = serializedObject.FindProperty("_autoNavLeft");
            autoNavRight = serializedObject.FindProperty("_autoNavRight");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(disabledHighlightedSprite);
            EditorGUILayout.PropertyField(disabledHighlightedColor);
            EditorGUILayout.PropertyField(disabledHighlightedTrigger);

            EditorGUILayout.PropertyField(autoNavUp);
            EditorGUILayout.PropertyField(autoNavDown);
            EditorGUILayout.PropertyField(autoNavRight);
            EditorGUILayout.PropertyField(autoNavLeft);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
