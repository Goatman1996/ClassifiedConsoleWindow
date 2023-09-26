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
            var settings = new CDebugSettingsProvider("Project/CDebugSettings", SettingsScope.Project);
            settings.instance = ClassifiedConsole.CDebugSettings.Instance;
            settings.m_SerializedObject = new SerializedObject(settings.instance);
            return settings;
        }

        ClassifiedConsole.CDebugSettings instance;
        SerializedObject m_SerializedObject;

        public override void OnGUI(string searchContext)
        {
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
