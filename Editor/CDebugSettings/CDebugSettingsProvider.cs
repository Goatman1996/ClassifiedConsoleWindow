using System.Collections.Generic;
using UnityEditor;

namespace ClassifiedConsole.Editor
{
    internal class CDebugSettingsProvider : SettingsProvider
    {
        public CDebugSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        public static SettingsProvider GetSettings()
        {
            return new CDebugSettingsProvider("Project/CDebugSettings", SettingsScope.Project);
        }

        public override void OnGUI(string searchContext)
        {
            var instance = ClassifiedConsole.CDebugSettings.Instance;

            SerializedObject m_SerializedObject = new SerializedObject(instance);
            m_SerializedObject.Update();
            SerializedProperty m_SerializedProperty = m_SerializedObject.GetIterator();

            m_SerializedProperty.NextVisible(true);
            UnityEngine.GUI.enabled = false;
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.PropertyField(rect, m_SerializedProperty);
            UnityEngine.GUI.enabled = true;

            while (m_SerializedProperty.NextVisible(false))
            {
                EditorGUILayout.PropertyField(m_SerializedProperty);
            }

            m_SerializedObject.ApplyModifiedProperties();
        }
    }
}
