using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KMS.AnimationToolkit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnimationEventDataContainer), true)]
    public class AnimationEventDataContainerEditor : Editor
    {
        private readonly GUIHelpLabel _helpLabel = new(
            @"만약 올바르게 수정하고 싶다면 State Machine Behavior 에 AnimationEventStateBehavior Component 를 추가하여 
프리뷰를 보면서 작업하세요
Id 값은 중복될 수 없습니다. 중복된 Id 값이 존재한다면 수정하세요!");

        private ReorderableList _reorderableList;
        private float _totalHeight;

        private void OnEnable()
        {
            _reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("animationEventDataList"), true, true, true, true);
            _reorderableList.multiSelect = true;
            _reorderableList.drawElementCallback += DrawElement;
            _reorderableList.drawHeaderCallback += (r) => GUI.Label(r, "Animation Data");
            _reorderableList.elementHeightCallback += _ => _totalHeight;
            _reorderableList.onAddCallback += OnAdd;
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;

            serializedObject.Update();

            _helpLabel.Draw();
            _reorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            GUIPropertyElement idField = new GUIPropertyElement(element.FindPropertyRelative("id"))
            {
                Position = new Vector2(rect.x, rect.y + 4f),
                Size = new Vector2(rect.width, EditorGUIUtility.singleLineHeight)
            };
            GUIPropertyElement titleField = new GUIPropertyElement(element.FindPropertyRelative("title"))
            {
                Position = new Vector2(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 6f),
                Size = new Vector2(rect.width, EditorGUIUtility.singleLineHeight)
            };
            GUIPropertyElement loopField = new GUIPropertyElement(element.FindPropertyRelative("loop"))
            {
                Position = new Vector2(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2 + 6f),
                Size = new Vector2(rect.width, EditorGUIUtility.singleLineHeight)
            };

            idField.Draw();
            titleField.Draw();
            loopField.Draw();

            _totalHeight = EditorGUIUtility.singleLineHeight * 3.5f;
        }

        private void OnAdd(ReorderableList list)
        {
            list.serializedProperty.arraySize++;
            SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(list.count - 1);
            
            property.FindPropertyRelative("id").uintValue = GetNextAvailableId(list);
            property.FindPropertyRelative("title").stringValue = string.Empty;
            property.FindPropertyRelative("loop").boolValue = false;
        }

        private uint GetNextAvailableId(ReorderableList list)
        {
            uint maxId = 0;
            for (int i = 0; i < list.count; i++)
            {
                var idProperty = list.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("id");
                if (idProperty.uintValue > maxId)
                {
                    maxId = idProperty.uintValue;
                }
            }
            return maxId + 1;
        }
    }
}