using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KMS.AnimationToolkit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnimationEventDataContainer), true)]
    public class AnimationEventDataContainerEditor : Editor
    {
        private AnimationEventDataContainer _target;
        private SerializedProperty _dataListProperty;
        
        private ReorderableList _reorderableList;
        private float _totalHeight;
        
        private readonly string _helpTextEng = @"If you want to modify correctly, please add AnimationEventStateBehavior Component in the State Machine Behavior and check with preview.";
        private readonly string _helpTextKor = @"만약 올바르게 수정하고 싶다면 State Machine Behavior 에 AnimationEventStateBehavior Component 를 추가하여 프리뷰를 보면서 작업하세요";

        private void OnEnable()
        {
            _target = target as AnimationEventDataContainer;
            _dataListProperty = serializedObject.FindProperty("animationEventDataList");
            
            _reorderableList = new ReorderableList(serializedObject, _dataListProperty, true, true, false, false)
                {
                    multiSelect = true
                };
            _reorderableList.drawElementCallback += DrawElement;
            _reorderableList.drawHeaderCallback += (r) => GUI.Label(r, "Animation Data");
            _reorderableList.elementHeightCallback += _ => _totalHeight;
        }
        
        private void DrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = _dataListProperty.GetArrayElementAtIndex(index);

            float originalLabelWidth = EditorGUIUtility.labelWidth;
            rect.y += 2;

            EditorGUIUtility.labelWidth = 20f;
            SerializedProperty property = element.FindPropertyRelative("id");
            GUIContent content = new GUIContent(property.displayName, property.tooltip);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), property, content);
            
            EditorGUIUtility.labelWidth = 80f;
            property = element.FindPropertyRelative("timeType");
            content.text = property.displayName;
            content.tooltip = property.tooltip;
            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), property, content);
            _totalHeight = EditorGUIUtility.singleLineHeight;
            
            EditorGUIUtility.labelWidth = 100f;
            switch (property.enumValueIndex)
            {
                case (int) TimeType.Normalized:
                    property = element.FindPropertyRelative("time");
                    content.text = "normalized Time";
                    break;
                case (int) TimeType.Fixed:
                    property = element.FindPropertyRelative("time");
                    content.text = "fixed Time";
                    break;
            }
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + _totalHeight + 3, rect.width, EditorGUIUtility.singleLineHeight), property, content);
            _totalHeight += EditorGUIUtility.singleLineHeight + 3;
            
            EditorGUIUtility.labelWidth = 50;
            property = element.FindPropertyRelative("title");
            content.text = property.displayName;
            content.tooltip = property.tooltip;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + _totalHeight + 3, rect.width, EditorGUIUtility.singleLineHeight), property, content);
            _totalHeight += EditorGUIUtility.singleLineHeight + 3;
            
            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;
            
            serializedObject.Update();

            DrawInformation();
            CreateEventData();
            RemoveEventData();

            _reorderableList.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInformation()
        {
            GUILayout.Label(_helpTextEng, EditorStyles.helpBox);
            GUILayout.Space(10);
            GUILayout.Label(_helpTextKor, EditorStyles.helpBox);
            GUILayout.Space(10);
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