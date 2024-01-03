using UnityEditor;
using System.IO;
using UnityEditor.Build.Pipeline;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/StreamingAssets/SwitchAssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        CompatibilityBuildPipeline.BuildAssetBundles(assetBundleDirectory,
            BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget);
        // CompatibilityBuildPipeline.BuildAssetBundles(assetBundleDirectory,
        // BuildAssetBundleOptions.DeterministicAssetBundle,
        // BuildTarget.Switch);
    }
    
    [MenuItem("Assets/Build Editor AssetBundles")]
    static void BuildEditorAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/EditorAssetBundles",
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows);
    }
    
}