using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteRendererExtension
{
    public static void ScreenFlip(this SpriteRenderer sprite, Vector3 targetPosition, bool mirrorFilp = false)
    {
        var selfScreenPosition = Camera.main.WorldToScreenPoint(sprite.transform.position);
        var targetScreenPosition = Camera.main.WorldToScreenPoint(targetPosition);

        if (targetScreenPosition.x != selfScreenPosition.x)
        {
            var flipx = targetScreenPosition.x > selfScreenPosition.x;
            if (mirrorFilp)
                flipx = !flipx;

            sprite.flipX = flipx;
        }
    }
}
