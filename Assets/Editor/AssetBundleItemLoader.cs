using System;
using System.IO;
using System.Linq;
using NekoOdyssey.Scripts.Constants;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetBundleItemLoader
{
    public static void Load()
    {
        var args = Environment.GetCommandLineArgs();
        var outputPath = args.Skip(1).First();
        var assetBundle = args.Skip(2).First();

        var bundlePath = Path.Combine(
            Application.streamingAssetsPath,
            AppConstants.AssetBundlesFolder,
            assetBundle
        );

        if (!File.Exists(bundlePath))
        {
            File.Create(outputPath);
            return;
        }

        var request = AssetBundle.LoadFromFile(bundlePath);

        using (var stream = new FileStream(outputPath, FileMode.CreateNew))
        using (var writer = new StreamWriter(stream))
        {
            foreach (var item in request.LoadAllAssets())
            {
                writer.WriteLine($"{item.name}");
            }
        }
    }
}