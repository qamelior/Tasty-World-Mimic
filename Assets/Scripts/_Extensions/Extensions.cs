using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace _Extensions
{
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
    
        public static T FindVisualElement<T>(this VisualElement root, string elementName) where T : VisualElement
        {
            var elem = root.Q<T>(elementName);
            if (elem == null)
                Debug.LogError($"[UI] Incorrect ID for UI element: {elementName} in {root}.");
            return elem;
        }

        public static Vector2Int ChangeX(this Vector2Int v2, int value)
        {
            return new Vector2Int(v2.x + value, v2.y);
        }

        public static void SetWorldPosition(this VisualElement el, Vector3 worldPosition, Camera camera = null)
        {
            if (camera == null) camera = Camera.main;
            var rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(el.panel, worldPosition, Vector2.one, camera);
            el.transform.position = rect.position;
        }

        public static void SetType(this ObjectField field, Type type)
        {
            field.objectType = type;
        }
    
        public static void DestroyChildren(this Transform root)
        {
            var isRuntime = Application.isPlaying;
            for (int c = root.childCount - 1; c >= 0; c--)
            {
                Transform child = root.GetChild(c);
                if (!child)
                    continue;
                if (isRuntime)
                    Object.Destroy(child.gameObject);
                else
                    Object.DestroyImmediate(child.gameObject);
            }
        }

    }
}
