using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace KMS.AnimationToolkit
{
	[CustomEditor(typeof(AnimationEventReceiver), true)]
	public class AnimationEventReceiverEditor : Editor
	{
		private SerializedProperty _containerProperty;
		private ReorderableList _reorderableList;

		private void OnEnable()
		{
			_containerProperty = serializedObject.FindProperty("container");
			
			_reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("mappedEvents"), true, true, true, true);
			_reorderableList.drawHeaderCallback += (r) => EditorGUI.LabelField(r, "Registered Events");
			_reorderableList.drawElementCallback += DrawElement;
			_reorderableList.elementHeightCallback += CalculateElementHeight;
			_reorderableList.onAddDropdownCallback += OnAddDropdown;
		}
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_containerProperty);

			if (_containerProperty.objectReferenceValue != null)
			{
				_reorderableList.DoLayoutList();
			}
			
			serializedObject.ApplyModifiedProperties();
		}
		
		private void DrawElement(Rect rect, int index, bool isactive, bool isfocused)
		{
			SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
			GUIPropertyElement idField = new GUIPropertyElement(element.FindPropertyRelative("id"))
			{
				Position = new Vector2(rect.x, rect.y + 2f),
				Width = rect.width,
				Readonly = true
			};
			GUIPropertyElement titleField = new GUIPropertyElement(element.FindPropertyRelative("title"))
			{
				Position = new Vector2(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 5f),
				Width = rect.width,
				Readonly = true
			};
			GUIPropertyElement callbackField = new GUIPropertyElement(element.FindPropertyRelative("callback"))
			{
				Position = new Vector2(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2f + 10f),
				Width = rect.width,
			};

			idField.Draw();
			titleField.Draw();
			callbackField.Draw();
		}
		
		private float CalculateElementHeight(int index)
		{
			SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
			SerializedProperty callbackProperty = element.FindPropertyRelative("callback");
    
			float baseHeight = EditorGUIUtility.singleLineHeight * 2f + 5f;
			float callbackHeight = EditorGUI.GetPropertyHeight(callbackProperty, true);
			float padding = 10f;
    
			return baseHeight + callbackHeight + padding;
		}
		
		private void OnAddDropdown(Rect buttonrect, ReorderableList list)
		{
			var container = _containerProperty.objectReferenceValue as AnimationEventDataContainer;
            
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
				SerializedProperty id = element.FindPropertyRelative("id");
				SerializedProperty title = element.FindPropertyRelative("title");
				SerializedProperty callback = element.FindPropertyRelative("callback");
				
				id.uintValue = data.Id;
				title.stringValue = data.Title;
				callback.FindPropertyRelative("m_PersistentCalls").FindPropertyRelative("m_Calls").ClearArray();
				
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}