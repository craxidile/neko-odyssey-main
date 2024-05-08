#if UNITY_EDITOR    
using UnityEditor;

[CustomEditor(typeof(DialogueClip))]
[CanEditMultipleObjects]
public class DialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}

#endif