using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace mitaywalle.Physical2D.QualityOfLife.Editor
{
	[UsedImplicitly]
	[Overlay(typeof(SceneView), "Colliders 2D", true)]
	public class SceneViewColliders2DOverlay : Overlay
	{
		private SaveContext saveContext;
		private EnumFlagsField flags;
		private EditorToolbarFloatField slider;
		private EditorToolbarFloatField slider2;

		public override VisualElement CreatePanelContent()
		{
			saveContext ??= ScriptableObject.CreateInstance<SaveContext>();
			const string path = "ProjectSettings/Physics2DSettings.asset";
			SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(path)[0]);
			SerializedProperty property = manager.FindProperty("m_GizmoOptions");

			GizmoOptions value = (GizmoOptions)property.enumValueFlag;

			var root = new VisualElement() { name = "My Toolbar Root" };
			flags = new EnumFlagsField(value);
			root.Add(flags);

			flags.RegisterCallback<ChangeEvent<Enum>>((context) =>
			{
				const string settingsPath = "ProjectSettings/Physics2DSettings.asset";
				SerializedObject myManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(settingsPath)[0]);
				SerializedProperty foundProperty = myManager.FindProperty("m_GizmoOptions");
				saveContext.GizmoOptions = (GizmoOptions)context.newValue;
				foundProperty.enumValueFlag = (int)(GizmoOptions)context.newValue;
				myManager.ApplyModifiedProperties();
			});

			slider = new EditorToolbarFloatField();
			slider.label = "Fill";

			slider.tooltip = "Collider Awake Filled Alpha";
			saveContext.FillAlpha = slider.value = EditorPrefs.GetFloat("UnityEditor.U2D.Physics/ColliderAwakeFilledColor_a");
			root.Add(slider);

			slider.RegisterCallback<ChangeEvent<float>>(context =>
			{
				float value = Mathf.Clamp01(context.newValue);
				slider.SetValueWithoutNotify(value);
				saveContext.FillAlpha = context.newValue;
				EditorPrefs.SetFloat("UnityEditor.U2D.Physics/ColliderAwakeFilledColor_a", context.newValue);
			});

			slider2 = new EditorToolbarFloatField();
			slider2.label = "Outline";
			slider2.tooltip = "Collider Awake Outline Alpha";
			slider2.value = EditorPrefs.GetFloat("UnityEditor.U2D.Physics/ColliderAwakeOutlineColor_a");
			root.Add(slider2);

			slider2.RegisterCallback<ChangeEvent<float>>(context =>
			{
				float newValue = Mathf.Clamp01(context.newValue);
				slider2.SetValueWithoutNotify(newValue);
				saveContext.OutlineAlpha = context.newValue;
				EditorPrefs.SetFloat("UnityEditor.U2D.Physics/ColliderAwakeOutlineColor_a", context.newValue);
			});

			return root;
		}

		private void OnUndo()
		{
			flags.value = saveContext.GizmoOptions;
			flags.MarkDirtyRepaint();
			slider.value = saveContext.FillAlpha;
			slider2.value = saveContext.OutlineAlpha;
		}
	}

	[Flags]
	public enum GizmoOptions
	{
		AllColliders = 1,
		CollidersOutlined = 2,
		CollidersFilled = 4,
		CollidersSleeping = 8,
		ColliderContacts = 16, // 0x00000010
		ColliderBounds = 32, // 0x00000020
	}

	public class SaveContext : ScriptableObject
	{
		public GizmoOptions GizmoOptions;
		public float FillAlpha;
		public float OutlineAlpha;
	}
}