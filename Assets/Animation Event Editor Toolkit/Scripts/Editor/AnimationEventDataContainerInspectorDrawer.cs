using System;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace KMS.AnimationToolkit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnimationEventDataContainer), true)]
    public class AnimationEventDataContainerInspectorDrawer : Editor
    {
        private AnimationEventDataContainer _target;
        private SerializedProperty _dataListProperty;
        
        private ReorderableList _reorderableList;

        private void OnEnable()
        {
            _target = target as AnimationEventDataContainer;
            _dataListProperty = serializedObject.FindProperty("animationEventDataList");
            
            _reorderableList = new ReorderableList(serializedObject, _dataListProperty, true, true, false, false)
                {
                    multiSelect = true
                };
            _reorderableList.drawElementCallback += DrawElement;
            _reorderableList.elementHeightCallback += (i) => EditorGUIUtility.singleLineHeight * 2;
        }
        
        private void DrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = _dataListProperty.GetArrayElementAtIndex(index);

            float originalLabelWidth = EditorGUIUtility.labelWidth;
            rect.y += 2;

            SerializedProperty property = element.FindPropertyRelative("id");
            GUIContent content = new GUIContent(property.displayName, property.tooltip);
            EditorGUIUtility.labelWidth = 20f;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), property, content);
            
            property = element.FindPropertyRelative("timeType");
            content.text = property.displayName;
            content.tooltip = property.tooltip;
            EditorGUIUtility.labelWidth = 80f;
            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), property, content);

            switch (property.enumValueIndex)
            {
                case (int) TimeType.Normalized:
                    property = element.FindPropertyRelative("normalizedTime");
                    content.text = property.displayName;
                    content.tooltip = property.tooltip;
                    EditorGUIUtility.labelWidth = 100f;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight), property, content);
                    break;
                case (int) TimeType.Fixed:
                    property = element.FindPropertyRelative("fixedTime");
                    content.text = property.displayName;
                    content.tooltip = property.tooltip;
                    EditorGUIUtility.labelWidth = 80f;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight), property, content);
                    break;
            }
            
            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;
            
            serializedObject.Update();

            CreateEventData();
            RemoveEventData();

            _reorderableList.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateEventData()
        {
            if (GUILayout.Button("Add Animation Event Data"))
            {
                _target.AnimationEventDataList.Add(new AnimationEventData());
            }
        }

        private void RemoveEventData()
        {
            if (GUILayout.Button("Remove Animation Event Data"))
            {
                foreach (int idx in _reorderableList.selectedIndices)
                {
                    _target.AnimationEventDataList.RemoveAt(idx);
                }
            }
        }
    }
}