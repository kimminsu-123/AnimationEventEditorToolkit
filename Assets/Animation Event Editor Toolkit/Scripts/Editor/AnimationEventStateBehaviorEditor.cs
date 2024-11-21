using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KMS.AnimationToolkit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnimationEventStateBehavior))]
    public class AnimationEventStateBehaviorEditor : Editor
    {
        private SerializedProperty _containerSoProperty;
        
        private AnimationEventDataContainer _container;
        private List<uint> _selectedEventIds;
        private ReorderableList _selectedEventIdReorderableList;

        private float _totalHeight;
        private int _selected;
        private bool _usePreview;
        private float _previewTime;

        private void OnEnable()
        {
            _containerSoProperty = serializedObject.FindProperty("container");
            
            AnimationEventStateBehavior sb = (AnimationEventStateBehavior)target;
            
            _container = sb.container;
            _selectedEventIds = sb.selectedEvents;
            
            _selectedEventIdReorderableList = new ReorderableList(_selectedEventIds, typeof(uint), true, true, false, false);
            _selectedEventIdReorderableList.multiSelect = true;
            _selectedEventIdReorderableList.drawElementCallback += DrawElement;
            _selectedEventIdReorderableList.drawHeaderCallback += (r) => EditorGUI.LabelField(r, "Selected Events");
            _selectedEventIdReorderableList.elementHeightCallback += _ => _totalHeight;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_containerSoProperty);
            
            if (_containerSoProperty.objectReferenceValue != null)
            {
                DrawAddButton();
                DrawRemoveButton();
                DrawClearButton();
                _selectedEventIdReorderableList.DoLayoutList();
                DrawPreviewOption();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAddButton()
        {
            string[] options = _container.AnimationEventDataList.Select(x => $"{x.title}[{x.id}]").ToArray();
            
            _selected = EditorGUILayout.Popup("Select ID/Titles", _selected, options);
            if (GUILayout.Button("Add Animation Event"))
            {
                OnAdd();   
            }
        }

        private void OnAdd()
        {
            AnimationEventData selectEvent = _container.AnimationEventDataList[_selected];
            uint id = selectEvent.id;
            string title = selectEvent.title;

            if (_selectedEventIds.Contains(id))
            {
                EditorUtility.DisplayDialog("Error", $"The animation event has already been added. {id}-{title}", "OK");
                return;
            }
            
            _selectedEventIds.Add(id);
        }

        private void DrawRemoveButton()
        {
            if (GUILayout.Button("Remove Animation Event"))
            {
                OnRemove();
            }
        }

        private void OnRemove()
        {
            if (_selectedEventIdReorderableList.selectedIndices.Count <= 0) return;
            
            int[] selectedIndices = _selectedEventIdReorderableList.selectedIndices.ToArray();
            
            Array.Reverse(selectedIndices);
            
            foreach (var idx in selectedIndices)
            {
                _selectedEventIds.Remove((uint)_selectedEventIdReorderableList.list[idx]);
            }
        }

        private void DrawClearButton()
        {
            if (GUILayout.Button("Clear Animation Events"))
            {
                _selectedEventIds.Clear();
            }
        }
        
        private void DrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element =
                _container.AnimationEventDataList.First(x =>
                    x.id.Equals(_selectedEventIdReorderableList.list[index])); 

            float originalLabelWidth = EditorGUIUtility.labelWidth;
            rect.y += 2;
            EditorGUIUtility.labelWidth = 20f;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), $"{element.title}[{element.id}]", EditorStyles.boldLabel);
            _totalHeight = EditorGUIUtility.singleLineHeight;

            // 애니메이션 시간에 맞춰서 해야함
            EditorGUI.LabelField(new Rect(rect.x, rect.y + _totalHeight, rect.width, EditorGUIUtility.singleLineHeight), $"{element.title}[{element.id}]", EditorStyles.boldLabel);
            _totalHeight += EditorGUIUtility.singleLineHeight;
            
            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        private void DrawPreviewOption()
        {
            float from = 0f, to = 1f;
            _previewTime = EditorGUILayout.Slider($"Preview Time [{from}:{to}]", _previewTime, from, to);

            string title = _usePreview ? "Enabled Preview" : "Disabled Preview";
            Color orgColor = GUI.backgroundColor;
            GUI.backgroundColor = _usePreview ? Color.green: Color.red;
            if (GUILayout.Button(title))
            {
                _usePreview = !_usePreview;
            }
            GUI.backgroundColor = orgColor;
        }
    }
}