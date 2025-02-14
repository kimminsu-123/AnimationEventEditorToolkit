using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

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

            EditorApplication.update += UpdatePreview;
        }
        
        private void OnDisable()
        {
            EditorApplication.update -= UpdatePreview;
        }
        
        private void UpdatePreview()
        {
            if (!_usePreview) return;
            
            AnimationEventUtils.GetCurrentAnimatorAndController(out AnimatorController ignore, out Animator animator);
            StateMachineBehaviourContext[] contexts = AnimatorController.FindStateMachineBehaviourContext((AnimationEventStateBehavior)target);
            foreach (StateMachineBehaviourContext context in contexts)
            {
                AnimatorState state = context.animatorObject as AnimatorState;
                if(state == null) continue;
                
                AnimationClip clip = AnimationEventUtils.GetFirstAvailableClip(state.motion);
                if(clip == null) continue;
                
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(animator.gameObject, clip, _previewTime * clip.length);
                AnimationMode.EndSampling();
            }
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_containerSoProperty);
            
            ValidateReorderableList();
            
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

        private void ValidateReorderableList()
        {
            for (int i = _selectedEventIds.Count - 1; i >= 0; i--)
            {
                var id = _selectedEventIds[i];
                var found = _container.AnimationEventDataList.FirstOrDefault(x => x.id.Equals(id));
                if (found == null)
                {
                    _selectedEventIds.Remove(id);
                }
            }
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

            EditorGUI.LabelField(new Rect(rect.x, rect.y + _totalHeight, rect.width, EditorGUIUtility.singleLineHeight), $"{element.timeType}   ");
            switch (element.timeType)
            {
                case TimeType.Entered:
                case TimeType.Exited:
                    break;
                case TimeType.Normalized:
                    element.time = EditorGUI.Slider(new Rect(rect.x + rect.width * 0.3f, rect.y + _totalHeight, rect.width * 0.7f, EditorGUIUtility.singleLineHeight), element.time, 0f, 1f);
                    break;
            }
            _totalHeight += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(new Rect(rect.x, rect.y + _totalHeight, rect.width, EditorGUIUtility.singleLineHeight), $"loop : ");
            element.loop = EditorGUI.Toggle(new Rect(rect.x + rect.width * 0.3f, rect.y + _totalHeight, rect.width * 0.7f, EditorGUIUtility.singleLineHeight), element.loop);
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
                if(_usePreview) AnimationMode.StartAnimationMode();
                else AnimationMode.StopAnimationMode();
            }
            GUI.backgroundColor = orgColor;
        }
    }
}