using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "IdImagePair Scriptable", menuName = "ScriptableObject/IdImagePair")]
public class IdImagePair : ScriptableObject
{
    public string imageId;
    public Sprite imageSprite;
}