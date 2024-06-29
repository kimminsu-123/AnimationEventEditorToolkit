using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace KimsEditor.Animation
{
    public class AnimEventEditor : EditorWindow
    {
        [MenuItem("Window/Animation Event Toolkit")]
        public static void ShowWindow()
        {
            AnimEventEditor window = GetWindow<AnimEventEditor>("Animation Event Toolkit");

            float halfWidth = Screen.currentResolution.width * 0.5f;
            float halfHeight = Screen.currentResolution.height * 0.5f;
            Vector2 minSize = new Vector2(halfWidth, halfHeight);
            
            window.position = new Rect(
                minSize.x - halfWidth * 0.5f,
                minSize.y - halfHeight * 0.5f,
                0f,
                0f);

            window.minSize = minSize; 
            
            window.Show();
        }

        private void OnGUI()
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
        
        public static Type[] FindInheritedTypes(
            Type parentType, Assembly assembly)
        {
            Type[] allTypes = assembly.GetTypes();
            ArrayList avTypesAL = new ArrayList();

            return allTypes.Where(
                t => parentType.IsAssignableFrom(t) && t != parentType).ToArray();
        }

    }
}
