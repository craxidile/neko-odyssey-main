using System.Collections.Generic;
using System.IO;
using Codice.Client.BaseCommands.Fileinfo;
using UnityEngine;
using UnityEditor;

public class SpriteExporter
{
    [MenuItem("Assets/Export Sub-Sprites %q")]
    public static void DoExportSubSprites()
    {
        // var folder = EditorUtility.OpenFolderPanel("Export subsprites into what folder?", null, null);
        foreach (var obj in Selection.objects)
        {
            var sprite = obj as Sprite;
            if (sprite == null) continue;
            var path = AssetDatabase.GetAssetPath(sprite);
            SetTextureReadable(path, true);
            var extracted = ExtractAndName(sprite);
            var outputPath = SaveSubSprite(extracted, new System.IO.FileInfo(path).Directory?.FullName);
            SetTextureReadable(path, false);
            AssetDatabase.Refresh();
            // SetTextureReady(outputPath);
        }
    }

    [MenuItem("Assets/Export Sub-Sprites %q", true)]
    private static bool CanExportSubSprites()
    {
        return Selection.activeObject is Sprite;
    }

    [MenuItem("Assets/Prepare 2D Sprite %w")]
    public static void DoPrepareTexture()
    {
        foreach (var obj in Selection.objects)
        {
            var texture = obj as Texture2D;
            if (texture == null) continue;
            var path = AssetDatabase.GetAssetPath(texture);
            SetTextureReady(path);
        }
    }

    [MenuItem("Assets/Prepare 2D Sprite %w", true)]
    public static bool CanPrepareTexture()
    {
        return Selection.activeObject is Texture2D;
    }

    private static void SetTextureReadable(string spritePath, bool readable)
    {
        var currentStatus = readable ? "0" : "1";
        var nextStatus = readable ? "1" : "0";

        var metadataPath = $"{spritePath}.meta";
        if (!File.Exists(metadataPath)) return;
        var newFile = new List<string>();
        var lines = File.ReadAllLines(metadataPath);

        foreach (var line in lines)
        {
            var newLine = line;
            if (newLine.Contains($"isReadable: {currentStatus}"))
                newLine = newLine.Replace($"isReadable: {currentStatus}", $"isReadable: {nextStatus}");
            newFile.Add(newLine);
        }

        File.WriteAllLines(metadataPath, newFile.ToArray());
        AssetDatabase.Refresh();
    }

    private static void SetTextureReady(string spritePath)
    {
        var metadataPath = $"{spritePath}.meta";
        if (!File.Exists(metadataPath)) return;
        var newFile = new List<string>();
        var lines = File.ReadAllLines(metadataPath);

        foreach (var line in lines)
        {
            var newLine = line;

            if (newLine.Contains($"enableMipMap: 1"))
                newLine = newLine.Replace($"enableMipMap: 1", $"enableMipMap: 0");
            if (newLine.Contains($"filterMode: 1"))
                newLine = newLine.Replace($"filterMode: 1", $"filterMode: 0");
            if (newLine.Contains($"wrapU: 0"))
                newLine = newLine.Replace($"wrapU: 0", $"wrapU: 1");
            if (newLine.Contains($"wrapV: 0"))
                newLine = newLine.Replace($"wrapV: 0", $"wrapV: 1");
            if (newLine.Contains($"alphaIsTransparency: 0"))
                newLine = newLine.Replace($"alphaIsTransparency: 0", $"alphaIsTransparency: 1");
            if (newLine.Contains($"textureType: 0"))
                newLine = newLine.Replace($"textureType: 0", $"textureType: 8");
            if (newLine.Contains($"textureFormat: -1"))
                newLine = newLine.Replace($"textureFormat: -1", $"textureFormat: 4");
            newFile.Add(newLine);
        }

        File.WriteAllLines(metadataPath, newFile.ToArray());
        AssetDatabase.Refresh();
    }

    // Since a sprite may exist anywhere on a tex2d, this will crop out the sprite's claimed region and return a new, cropped, tex2d.
    private static Texture2D ExtractAndName(Sprite sprite)
    {
        var output = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        // var r = sprite.textureRect;
        var r = sprite.rect;
        var pixels = sprite.texture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
        output.SetPixels(pixels);
        output.Apply();
        output.name = $"{sprite.texture.name} {sprite.name}";
        return output;
    }

    private static string SaveSubSprite(Texture2D tex, string saveToDirectory)
    {
        if (!System.IO.Directory.Exists(saveToDirectory)) System.IO.Directory.CreateDirectory(saveToDirectory);
        var path = System.IO.Path.Combine(saveToDirectory, $"{tex.name}.png");
        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        return path;
    }
}