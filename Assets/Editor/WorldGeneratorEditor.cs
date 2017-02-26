using UnityEditor;
using UnityEngine;

namespace Sandbox
{
    [CustomEditor(typeof(WorldGenerator))]
    public class WorldGeneratorEditor : Editor
    {
        private SerializedProperty player;
        private WorldGenerator script;
        void OnEnable()
        {
            player = serializedObject.FindProperty("player");
            script = (WorldGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck() || GUILayout.Button("Rebuild"))
            {
                script.Rebuild();
            }
        }
    }
}