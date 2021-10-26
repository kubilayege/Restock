using MonoObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Cellular), true)]
    [CanEditMultipleObjects]
    public class CellularEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var cellular = (Cellular) target;
            if (GUILayout.Button("Create"))
            {
                cellular.CreateGrid();
            }
        }
    }
}