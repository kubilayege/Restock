using MonoObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(FridgeData))]
    public class FridgeDataEditor : UnityEditor.Editor
    {
        public FridgeSection fridgeSection;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var cellular = (FridgeData) target;
            fridgeSection = (FridgeSection) EditorGUILayout.EnumPopup("Fridge Section: ", fridgeSection);

            if (GUILayout.Button("Open"))
            {
                cellular.Open(fridgeSection);
            }
            
            
            if (GUILayout.Button("Close"))
            {
                cellular.CloseAll();
            }
        }
    }
}