using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class RectSizeOverride : MonoBehaviour
{
    [SerializeField] bool x, y;
    [SerializeField] RectTransform targetRect;


    RectTransform rectTransform;


    private void Update()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        if (targetRect != null && rectTransform != null)
        {
            Vector2 newSize = rectTransform.sizeDelta;
            if (x) newSize.x = targetRect.sizeDelta.x;
            if (y) newSize.y = targetRect.sizeDelta.y;

            rectTransform.sizeDelta = newSize;
        }
    }


    public void SetTargetRect(RectTransform rect)
    {
        targetRect = rect;
    }
}
