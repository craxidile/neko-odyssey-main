#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatiumInteractive.Libraries.Unity.Platform.CustomEditor.Inspector
{
    /// <summary>
    /// base class for all inspector-based editor extensions 
    /// </summary>
    /// <typeparam name="T">Class for which we're building the editor class</typeparam>

    public abstract class InspectorEditorBase<T> : Editor
    {
        protected Dictionary<string, SerializedProperty> Properties = new Dictionary<string, SerializedProperty>();
        protected List<Property> TimingProperties = new List<Property>();

        public void OnEnable()
        {
            Properties.Clear();
            SerializedProperty property = serializedObject.GetIterator();

            while (property.NextVisible(true))
            {
                Properties[property.name] = property.Copy();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            TimingProperties.Clear();

            GUIStyle boldStyle = new GUIStyle();
            boldStyle.fontStyle = FontStyle.Bold;
        }

        protected void DisplayRegularField(Property property)
        {
            EditorGUILayout.PropertyField(Properties[property.name], new GUIContent(property.text), true);
        }
    }

}

#endif
