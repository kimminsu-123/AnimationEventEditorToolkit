using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace KimsEditor.Animation
{
    public class AnimEventEditor : EditorWindow
    {
        private AnimationClip _selectedClip;
        
        [MenuItem("Window/Animation Event Toolkit")]
        public static void Init()
        {
            ShowWindow();
        }
        
        private static void ShowWindow()
        {
            AnimEventEditor window = GetWindow<AnimEventEditor>("Animation Event Toolkit");

            float halfWidth = Screen.currentResolution.width * 0.5f;
            float halfHeight = Screen.currentResolution.height * 0.5f;
            Vector2 minSize = new Vector2(halfWidth, halfHeight);
            
            window.minSize = minSize; 
            window.position = new Rect(
                minSize.x - halfWidth * 0.5f,
                minSize.y - halfHeight * 0.5f,
                0f,
                0f);
            
            window.Show();
        }

        private void OnGUI()
        {
            DrawTopMenu();
        }

        private void DrawTopMenu()
        {
            GUIStyle style = new GUIStyle();
            style.padding = new RectOffset(10, 10, 10, 10);

            GUILayout.BeginArea(new Rect(0, 0, position.width, 100));
            GUILayout.BeginHorizontal(style);
            _selectedClip = EditorGUILayout.ObjectField(new GUIContent("Animation Clip"), _selectedClip, typeof(AnimationClip), false, GUILayout.Width(300)) as AnimationClip;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Test", GUILayout.Width(40)))
            {
                Debug.Log("Test1");
            }
            if (GUILayout.Button("Test", GUILayout.Width(40)))
            {
                Debug.Log("Test2");
            }
            if (GUILayout.Button("Test", GUILayout.Width(40)))
            {
                Debug.Log("Test3");
            }
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Add Event", GUILayout.Width(100)))
            {
                Debug.Log("Test3");
            }
            if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                Debug.Log("Test3");
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawInheritDropdown()
        {
            GUILayoutOption[] options = new GUILayoutOption[3];

            if (!EditorGUI.DropdownButton(new Rect(20f, 20f, 100, 10), new GUIContent("123"), FocusType.Keyboard))
            {
                return;
            }

            Type[] types = typeof(AnimationEvent).Assembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(AnimationEvent))).ToArray();
            
            
            GenericMenu menu = new GenericMenu();
            foreach (var t in types)
            {
                menu.AddItem(new GUIContent(t.GetMethods()[0].Name), false, data => {Debug.Log(data);}, $"{t.FullName}");
            }
            menu.DropDown(new Rect(20f, 20f, 100, 10));
        }
    }
}