using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonHover : Button
{
    [SerializeField] private Image hoverImage;

    [SerializeField] public UnityEvent<bool> onHover;

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        hoverImage?.gameObject.SetActive(false);
        onHover?.Invoke(false);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        hoverImage?.gameObject.SetActive(true);
        onHover?.Invoke(true);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        hoverImage?.gameObject.SetActive(true);
        onHover?.Invoke(true);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        hoverImage?.gameObject.SetActive(false);
        onHover?.Invoke(false);
    }

    protected override void Start()
    {
        base.Start();
        hoverImage?.gameObject.SetActive(false);
        onHover?.Invoke(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        hoverImage?.gameObject.SetActive(false);
        onHover?.Invoke(false);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ButtonHover))]
public class ButtonHoverEditor : UnityEditor.UI.ButtonEditor
{
    SerializedProperty m_hoverImage;
    SerializedProperty m_OnHover;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_hoverImage = serializedObject.FindProperty("hoverImage");
        m_OnHover = serializedObject.FindProperty("onHover");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_hoverImage);
        EditorGUILayout.PropertyField(m_OnHover);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif