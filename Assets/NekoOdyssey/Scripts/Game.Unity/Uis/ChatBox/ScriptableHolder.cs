using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScriptableHolder : MonoBehaviour
{


    public static ScriptableHolder Instance;
    private void Awake()
    {
        Instance = this;
    }


    public List<NpcAssets> npcAssets = new List<NpcAssets>();
    public NpcAssets GetNpcAsset(string npcId)
    {
        return npcAssets.Find(npcAsset => npcAsset.npcId == npcId);
    }


    public List<IdImagePair> imagePairsList = new List<IdImagePair>();
    public Sprite GetImage(string imageId)
    {
        return imagePairsList.FirstOrDefault(imagePair => imagePair.imageId == imageId).imageSprite;
    }


    private void OnValidate()
    {
        //check and remove duplicate scriptable object
        var cloneImagePairsList = new List<IdImagePair>();
        for (int i = 0; i < imagePairsList.Count; i++)
        {
            var imagePair = imagePairsList[i];

            if (imagePair == null) continue;
            if (cloneImagePairsList.Contains(imagePair) && i != imagePairsList.Count - 1) //do not check the last element
            {
                Debug.Log($"IdImagePair List duplicate item removed [object name : {imagePair.name}] [object image id : {imagePair.imageId}]");
            }
            else
            {
                cloneImagePairsList.Add(imagePair);
            }
        }
        imagePairsList = cloneImagePairsList;
    }
}
