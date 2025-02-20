using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace KMS.AnimationToolkit
{
    [CustomEditor(typeof(AnimationEventStateBehavior), true)]
    public class AnimationEventStateBehaviorEditor : Editor
    {
        private SerializedProperty _containerSoProperty;
        
        private ReorderableList _eventStateEnterList;
        private ReorderableList _eventStateExitList;
        private ReorderableList _eventStateReachedNormalizedTimeList;

        private int _selectedNormalizedEventIndex;

        private int _selected;
        private bool _usePreview;

        private void OnEnable()
        {
            _containerSoProperty = serializedObject.FindProperty("container");
            
            _eventStateEnterList = new ReorderableList(serializedObject, serializedObject.FindProperty("eventStateEnter"), true, true, true, true);
            _eventStateEnterList.drawHeaderCallback += (r) => EditorGUI.LabelField(r, "On Enter State");
            _eventStateEnterList.drawElementCallback += (rect, index, isactive, isfocused) => DrawElement(_eventStateEnterList, rect, index, isactive, isfocused, false);
            _eventStateEnterList.elementHeightCallback += _ => EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing;
            _eventStateEnterList.onAddDropdownCallback += OnAddDropdown;

            _eventStateExitList = new ReorderableList(serializedObject, serializedObject.FindProperty("eventStateExit"), true, true, true, true);
            _eventStateExitList.drawHeaderCallback += (r) => EditorGUI.LabelField(r, "On Exit State");
            _eventStateExitList.drawElementCallback += (rect, index, isactive, isfocused) => DrawElement(_eventStateExitList, rect, index, isactive, isfocused, false);
            _eventStateExitList.elementHeightCallback += _ => EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing;
            _eventStateExitList.onAddDropdownCallback += OnAddDropdown;

            _eventStateReachedNormalizedTimeList = new ReorderableList(serializedObject, serializedObject.FindProperty("eventReachedNormalizedTime"), true, true, true, true);
            _eventStateReachedNormalizedTimeList.drawHeaderCallback += (r) => EditorGUI.LabelField(r, "On Normalized Time");
            _eventStateReachedNormalizedTimeList.drawElementCallback += (rect, index, isactive, isfocused) => DrawElement(_eventStateReachedNormalizedTimeList, rect, index, isactive, isfocused, true);
            _eventStateReachedNormalizedTimeList.elementHeightCallback += _ => EditorGUIUtility.singleLineHeight * 4f + EditorGUIUtility.standardVerticalSpacing;
            _eventStateReachedNormalizedTimeList.onAddDropdownCallback += OnAddDropdown;
            _eventStateReachedNormalizedTimeList.onSelectCallback += OnSelectNormalizedTimeEvent;

            ValidateReorderableList(_eventStateEnterList);
            ValidateReorderableList(_eventStateExitList);
            ValidateReorderableList(_eventStateReachedNormalizedTimeList);
            
            EditorApplication.update += UpdatePreview;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdatePreview;
        }

        private void ValidateReorderableList(ReorderableList list)
        {
            var container = _containerSoProperty.objectReferenceValue as AnimationEventDataContainer;

            if (container != null)
            {
                var listProperty = list.serializedProperty;

                for (int i = listProperty.arraySize - 1; i >= 0; i--)
                {
                    var element = listProperty.GetArrayElementAtIndex(i);
                    var idProperty = element.FindPropertyRelative("id");
                    var found = container.AnimationEventDataList.Find(x => x.Id.Equals(idProperty.uintValue));
                    if (found == null)
                    {
                        listProperty.DeleteArrayElementAtIndex(i);
                    }
                }
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        
        private void UpdatePreview()
        {
            if (!_usePreview || _selectedNormalizedEventIndex == -1) return;
            
            AnimationEventUtils.GetCurrentAnimatorAndController(out AnimatorController ignore, out Animator animator);
            StateMachineBehaviourContext[] contexts = AnimatorController.FindStateMachineBehaviourContext((AnimationEventStateBehavior)target);
            foreach (StateMachineBehaviourContext context in contexts)
            {
                AnimatorState state = context.animatorObject as AnimatorState;
                if(state == null) continue;
                
                AnimationClip clip = AnimationEventUtils.GetFirstAvailableClip(state.motion);
                if(clip == null) continue;

                try
                {
                    SerializedProperty timeField =
                        _eventStateReachedNormalizedTimeList.serializedProperty.GetArrayElementAtIndex(
                            _selectedNormalizedEventIndex).FindPropertyRelative("normalizedTime");

                    AnimationMode.BeginSampling();
                    AnimationMode.SampleAnimationClip(animator.gameObject, clip, timeField.floatValue * clip.length);
                    AnimationMode.EndSampling();
                }
                catch (Exception)
                {
                    // ignore
                    _selectedNormalizedEventIndex = -1;
                }
            }
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_containerSoProperty);
            
            if (_containerSoProperty.objectReferenceValue != null)
            {
                _eventStateEnterList.DoLayoutList();
                _eventStateExitList.DoLayoutList();
                _eventStateReachedNormalizedTimeList.DoLayoutList();
                DrawPreviewOption();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawElement(ReorderableList list, Rect rect, int index, bool isactive, bool isfocused, bool dispTime)
        {   
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            GUIPropertyElement idField = new GUIPropertyElement(element.FindPropertyRelative("id"))
            {
                Position = new Vector2(rect.x, rect.y + 2f),
                Width = rect.width,
                Readonly = true
            };
            GUIPropertyElement titleField = new GUIPropertyElement(element.FindPropertyRelative("title"))
            {
                Position = new Vector2(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2f),
                Width = rect.width,
                Readonly = true
            };
            GUIPropertyElement loopField = new GUIPropertyElement(element.FindPropertyRelative("loop"))
            {
                Position = new Vector2(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2 + 2f),
                Width = rect.width
            };

            idField.Draw();
            titleField.Draw();
            loopField.Draw();
            
            if (dispTime)
            {
                if (isactive)
                {
                    _selectedNormalizedEventIndex = index;
                }
                
                GUISliderField normalizedTimeField = new GUISliderField(element.FindPropertyRelative("normalizedTime"))
                {
                    Position = new Vector2(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3 + 2f),
                    Width = rect.width,
                    Label = "Normalized Time",
                    Ratio = new Vector2(0.3f, 0.7f),
                    Min = 0f, Max = 1f
                };
                normalizedTimeField.Draw();
            }
        }

        private void OnAddDropdown(Rect buttonrect, ReorderableList list)
        {
            var container = _containerSoProperty.objectReferenceValue as AnimationEventDataContainer;
            
            if (container != null)
            {
                if (container.AnimationEventDataList.Count <= 0)
                {
                    EditorUtility.DisplayDialog("Error", $"연결된 Scriptable Object에 등록된 이벤트가 없습니다.", "OK");
                    return;
                }
                
                var menu = new GenericMenu();
                foreach (AnimationEventData data in container.AnimationEventDataList)
                {
                    menu.AddItem(new GUIContent(data.Title), false, obj => OnSelectDropdown(obj, list), data);
                }
                menu.DropDown(buttonrect);    
            }
        }

        private void OnSelectDropdown(object obj, ReorderableList list)
        {
            if (obj is AnimationEventData data)
            {
                SerializedProperty listProperty = list.serializedProperty;
                
                listProperty.arraySize++;
                
                SerializedProperty element = listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1);
                SerializedProperty title = element.FindPropertyRelative("title");
                SerializedProperty id = element.FindPropertyRelative("id");
                SerializedProperty loop = element.FindPropertyRelative("loop");
                SerializedProperty normalizedTime = element.FindPropertyRelative("normalizedTime");

                title.stringValue = data.Title;
                id.uintValue = data.Id;
                loop.boolValue = false;
                normalizedTime.floatValue = 0f;

                serializedObject.ApplyModifiedProperties();
            }
        }
        
        private void OnSelectNormalizedTimeEvent(ReorderableList list)
        {
            _selectedNormalizedEventIndex = list.index;
        }
        
        private void DrawPreviewOption()
        {
            string title = _usePreview ? "Enabled Preview" : "Disabled Preview";
            Color backgroundColor = _usePreview ? Color.green: Color.red;
            GUIButton previewButton = new GUIButton(title, () =>
            {
                _usePreview = !_usePreview;
                if(_usePreview) AnimationMode.StartAnimationMode();
                else AnimationMode.StopAnimationMode();
            }, backgroundColor);
            previewButton.Draw();
        }
    }
}