using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class Extensions
{
    public static string InspectorExtensionsFolderPath = "Assets/Scripts/Editor/";
    public static bool IsClose(Vector3 p1, Vector3 p2, float precision = 0.1f)
    {
        return Vector3.SqrMagnitude(p1 - p2) < precision;
    }

    public static void SetText(this TextField f, string text)
    {
        f.value = text;
    }

    public static void SetVisibility(this VisualElement el, bool mode)
    {
        el.style.display = mode ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
