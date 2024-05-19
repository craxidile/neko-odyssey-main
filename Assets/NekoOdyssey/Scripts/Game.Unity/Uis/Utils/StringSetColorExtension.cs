using UnityEngine;

public static class StringSetColorExtension
{
    public static string SetColor(this string text, Color color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGBA(color);
        return $"<color=#{hexColor}>{text}</color>";
    }

    public static string Translate(this string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            var adjustLanguageText = LoadUiLanguageFromCSV.GetUiLanguageText(text);
            return adjustLanguageText;
        }
        else
        {
            return text;
        }
    }
}
