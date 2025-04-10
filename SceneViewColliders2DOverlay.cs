using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Game.Plugins.mitaywalle.Physical2D.QualityOfLife.Editor
{
	[UsedImplicitly]
	[Overlay(typeof(SceneView), "Colliders 2D", true)]
	public class SceneViewColliders2DOverlay : Overlay
	{
		public override VisualElement CreatePanelContent()
		{
			const string path = "ProjectSettings/Physics2DSettings.asset";
			SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(path)[0]);
			SerializedProperty property = manager.FindProperty("m_GizmoOptions");

			GizmoOptions value = (GizmoOptions)property.enumValueFlag;

			var root = new VisualElement() { name = "My Toolbar Root" };
			var flags = new EnumFlagsField(value);
			root.Add(flags);

			flags.RegisterCallback<ChangeEvent<Enum>>((context) =>
			{
				const string path = "ProjectSettings/Physics2DSettings.asset";
				SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(path)[0]);
				SerializedProperty property = manager.FindProperty("m_GizmoOptions");

				property.enumValueFlag = (int)(GizmoOptions)context.newValue;
				manager.ApplyModifiedProperties();
			});

			var slider = new Slider("Fill", 0.0f, 1.0f,pageSize:100);
			slider.tooltip = "Collider Awake Filled Alpha";
			slider.value = EditorPrefs.GetFloat("UnityEditor.U2D.Physics/ColliderAwakeFilledColor_a");
			root.Add(slider);
			
			slider.RegisterCallback<ChangeEvent<float>>(context =>
			{
				EditorPrefs.SetFloat("UnityEditor.U2D.Physics/ColliderAwakeFilledColor_a", context.newValue);
			});  
			
			slider = new Slider("Outline", 0.0f, 1.0f,pageSize:100);
			slider.tooltip = "Collider Awake Outline Alpha";
			slider.value = EditorPrefs.GetFloat("UnityEditor.U2D.Physics/ColliderAwakeOutlineColor_a");
			root.Add(slider);
			
			slider.RegisterCallback<ChangeEvent<float>>(context =>
			{
				EditorPrefs.SetFloat("UnityEditor.U2D.Physics/ColliderAwakeOutlineColor_a", context.newValue);
			});
			return root;
		}

		[Flags]
		private enum GizmoOptions
		{
			AllColliders = 1,
			CollidersOutlined = 2,
			CollidersFilled = 4,
			CollidersSleeping = 8,
			ColliderContacts = 16,// 0x00000010
			ColliderBounds = 32,// 0x00000020
		}
	}
}
