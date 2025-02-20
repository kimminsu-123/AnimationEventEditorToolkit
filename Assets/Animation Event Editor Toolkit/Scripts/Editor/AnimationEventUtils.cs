using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace KMS.AnimationToolkit
{
	public static class AnimationEventUtils
	{
		public static void GetCurrentAnimatorAndController(out AnimatorController controller, out Animator animator) {
			Type animatorWindowType = Type.GetType("UnityEditor.Graphs.AnimatorControllerTool, UnityEditor.Graphs");
			var window = EditorWindow.GetWindow(animatorWindowType);

			var animatorField = animatorWindowType.GetField("m_PreviewAnimator", BindingFlags.Instance | BindingFlags.NonPublic);
			animator = animatorField.GetValue(window) as Animator;

			var controllerField = animatorWindowType.GetField("m_AnimatorController", BindingFlags.Instance | BindingFlags.NonPublic);
			controller = controllerField.GetValue(window) as AnimatorController;
		}
        
		public static AnimationClip GetFirstAvailableClip(Motion motion) {
			AnimationClip clip = motion as AnimationClip;
			if (clip != null)
				return clip;

			BlendTree tree = motion as BlendTree;
			if (tree != null) {
				foreach (ChildMotion childMotion in tree.children) {
					var child = childMotion.motion;
					BlendTree childTree = child as BlendTree;
					if (childTree != null) {
						var childClip = GetFirstAvailableClip(childTree);
						if (childClip != null)
							return childClip;
					}
					else {
						AnimationClip childClip = child as AnimationClip;
						if (childClip != null)
							return childClip;
					}
				}
			}

			return null;
		}
	}

	public abstract class GUILayoutElement
	{
		public abstract void Draw();
	}

	public class GUIPropertyElement
	{
		private readonly SerializedProperty _property;
		private readonly GUIContent _content;

		public float LabelWidth { get; set; } = 40f;
		public Vector2 Position { get; set; } = Vector2.zero;
		public float Width { get; set; }
		public bool Readonly { get; set; } = false;
		public SerializedProperty Property => _property;
		public GUIContent Content => _content;
		
		public GUIPropertyElement(SerializedProperty property)
		{
			_property = property;
			_content = new GUIContent(property.displayName, property.tooltip);
		}

		public virtual void Draw()
		{
			GUI.enabled = !Readonly;
			float org = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = LabelWidth;

			float height = EditorGUI.GetPropertyHeight(Property, true); 
			Rect rect = new Rect(Position, new Vector2(Width, height));
			EditorGUI.PropertyField(rect, Property, _content, true);

			EditorGUIUtility.labelWidth = org;
			GUI.enabled = true;
		}
	}

	public class GUIHelpLabel : GUILayoutElement
	{
		private readonly string _text;
		
		public GUIHelpLabel(string txt)
		{
			_text = txt;
		}
		
		public override void Draw()
		{
			GUILayout.Label(_text, EditorStyles.helpBox);
		}
	}
	
	public class GUIButton : GUILayoutElement
	{
		private readonly string _title;
		private readonly Action _callback;
		private readonly Color _backgroundColor;
		
		public GUIButton(string title, Action callback, Color backgroundColor = default)
		{
			_title = title;
			_callback = callback;
			_backgroundColor = backgroundColor;
		}
		
		public override void Draw()
		{
			Color org = GUI.backgroundColor;
			GUI.backgroundColor = _backgroundColor;
			if (GUILayout.Button(_title))
			{
				_callback?.Invoke();
			}
			GUI.backgroundColor = org;
		}
	}
	
	public class GUISliderField : GUIPropertyElement
	{
		public string Label { get; set; }
		public Vector2 Ratio { get; set; }
		public float Min { get; set; }
		public float Max { get; set; }
		
		public GUISliderField(SerializedProperty property) : base(property)
		{
		}
		
		public override void Draw()
		{
			float org = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = LabelWidth;

			float height = EditorGUI.GetPropertyHeight(Property, true);
			
			var labelSize = Width * Ratio.x;
			EditorGUI.LabelField(new Rect(Position, new Vector2(labelSize, height)), Label);
			
			var sliderPosition = Position;
			sliderPosition.x += labelSize;
			
			var sliderSize = Width * Ratio.y;
			Property.floatValue = EditorGUI.Slider(new Rect(sliderPosition, new Vector2(sliderSize, height)), Property.floatValue, Min, Max);
			EditorGUIUtility.labelWidth = org;
		}
	}
}