using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace _Extensions
{
    public static class UIToolkitExtensions
    {
        public static bool IsVisible(this VisualElement el) { return el.style.display == DisplayStyle.Flex; }
        public static void SetVisibility(this VisualElement el, bool mode)
        {
            if (el.IsVisible() == mode)
                return;
            el.style.display = mode ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static void Show(this VisualElement el) { el.SetVisibility(true); }
        public static void Hide(this VisualElement el) { el.SetVisibility(true); }
        
        public static T FindVisualElement<T>(this VisualElement root, string elementName) where T : VisualElement
        {
            var elem = root.Q<T>(elementName);
            if (elem == null)
                Debug.LogError($"[UI] Incorrect ID for UI element: {elementName} in {root}.");
            return elem;
        }
        public static void SetText(this TextField f, string text) { f.value = text; }
        public static void SetType(this ObjectField field, Type type) { field.objectType = type; }
        
        public static void SetWorldPosition(this VisualElement el, Vector3 worldPosition, Camera camera = null)
        {
            if (camera == null) camera = Camera.main;
            var rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(el.panel, worldPosition, Vector2.one, camera);
            el.transform.position = rect.position;
        }
    }

    
    public static class Extensions
    {
        public static Vector2Int ChangeX(this Vector2Int v2, int value) { return new Vector2Int(v2.x + value, v2.y); }

        public static void DestroyChildren(this Transform root)
        {
            bool isRuntime = Application.isPlaying;
            for (int c = root.childCount - 1; c >= 0; c--)
            {
                var child = root.GetChild(c);
                if (!child) continue;
                if (isRuntime)
                    Object.Destroy(child.gameObject);
                else
                    Object.DestroyImmediate(child.gameObject);
            }
        }
    }
}
