using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace KMS.AnimationToolkit
{
	[CustomEditor(typeof(AnimationEventReceiver), true)]
	public class AnimationEventReceiverEditor : Editor
	{
		private AnimationEventReceiver _receiver;

		private SerializedProperty _containerProperty;
		private ReorderableList _reorderableList;

		private int _selected;
		private float _totalHeight;
		private bool _foldOut = false;
		
		private void OnEnable()
		{
			_receiver = target as AnimationEventReceiver;

			if (_receiver == null) return;

			_containerProperty = serializedObject.FindProperty("container");
			
			_reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("mappedEvents"), true, true, false, false);
			_reorderableList.drawElementCallback += DrawElement;
			_reorderableList.drawHeaderCallback += (r) => GUI.Label(r, "Registered Events");
			_reorderableList.elementHeightCallback += _ => _totalHeight;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_containerProperty);

			if (_containerProperty.objectReferenceValue != null)
			{
				ValidateReorderableList();
				DrawContainerIds();
				DrawAddButton();
				DrawRemoveButton();
				_reorderableList.DoLayoutList();
			}
			
			serializedObject.ApplyModifiedProperties();
		}

		private void ValidateReorderableList()
		{
			for (int i = _receiver.mappedEvents.Count - 1; i >= 0; i--)
			{
				var mappedEvent = _receiver.mappedEvents[i];
				var found = _receiver.container.AnimationEventDataList.FirstOrDefault(x => x.id.Equals(mappedEvent.id));
				if (found == null)
				{
					_reorderableList.serializedProperty.DeleteArrayElementAtIndex(i);
				}
			}
		}

		private void DrawContainerIds()
		{
			AnimationEventDataContainer container = _containerProperty.objectReferenceValue as AnimationEventDataContainer;
			if (container != null)
			{
				string[] options = container.AnimationEventDataList.Select(x => $"{x.title}[{x.id}]").ToArray();
				_selected = EditorGUILayout.Popup("Select ID/Titles", _selected, options);	
			}
		}

		private void DrawAddButton()
		{
			Color orgColor = GUI.backgroundColor;

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Register Event"))
			{
				uint id = _receiver.container.AnimationEventDataList[_selected].id;
				MappedEvent found = _receiver.mappedEvents.FirstOrDefault(x => x.id.Equals(id));
				if (found == null)
				{
					_receiver.mappedEvents.Add(new MappedEvent(id));
				}
				else
				{
					EditorUtility.DisplayDialog("Error", $"The animation event has already been added. {id}", "OK");
				}
			}
			
			GUI.backgroundColor = orgColor;
		}

		private void DrawRemoveButton()
		{
			Color orgColor = GUI.backgroundColor;

			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("UnRegister Event"))
			{
				foreach (int select in _reorderableList.selectedIndices)
				{
					_reorderableList.serializedProperty.DeleteArrayElementAtIndex(select);
				}
			}
			
			GUI.backgroundColor = orgColor;
		}
		
		private void DrawElement(Rect rect, int index, bool isactive, bool isfocused)
		{
			SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
			
			rect.y += 2;
			EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), $"ID: {element.FindPropertyRelative("id").uintValue}", EditorStyles.boldLabel);
			_totalHeight = EditorGUIUtility.singleLineHeight;
			
			AnimationEventData info = _receiver.container.AnimationEventDataList.FirstOrDefault();
			if (info != null)
			{
				EditorGUI.LabelField(new Rect(rect.x, rect.y + _totalHeight, rect.width, EditorGUIUtility.singleLineHeight), $"Title: {info.title}");
				_totalHeight += EditorGUIUtility.singleLineHeight;

				SerializedProperty callbackProperty = element.FindPropertyRelative("callback");
				_foldOut = EditorGUI.Foldout(new Rect(rect.x, rect.y + _totalHeight, rect.width, EditorGUIUtility.singleLineHeight), _foldOut, "Callbacks", true);
				if (_foldOut)
				{
					EditorGUI.PropertyField(new Rect(rect.x, rect.y + _totalHeight, rect.width, EditorGUIUtility.singleLineHeight), callbackProperty);
					_totalHeight += EditorGUI.GetPropertyHeight(callbackProperty);	
				}
				else
				{
					_totalHeight += EditorGUIUtility.singleLineHeight;	
				}
			}
		}
	}
}