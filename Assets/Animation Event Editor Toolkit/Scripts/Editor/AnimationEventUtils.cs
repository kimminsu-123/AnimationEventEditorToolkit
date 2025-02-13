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
}