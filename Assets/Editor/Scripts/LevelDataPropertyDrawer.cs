using _Extensions;
using Game.Data.Levels;
using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LevelDataEntry))]
public class LevelDataPropertyDrawer : PropertyDrawer
{
    private VisualElement _randomLevelGroup;
    private VisualElement _fixedLevelGroup;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement
        {
            style =
            {
                marginLeft = 25f,
                marginTop = 10f
            }
        };
        
        container.Add(new UnityEditor.UIElements.PropertyField(property.FindPropertyRelative("NumberOfBoosts")));
        container.Add(new UnityEditor.UIElements.PropertyField(property.FindPropertyRelative("TimeInSeconds")));
        
        var typeEnum = property.FindPropertyRelative("Type");
        var typeField = new UnityEditor.UIElements.PropertyField(typeEnum);
        typeField.RegisterValueChangeCallback(evt => ShowContextFields(evt.changedProperty.enumValueIndex));
        container.Add(typeField);

        _randomLevelGroup = CreateRandomGroup(property);
        container.Add(_randomLevelGroup);
        
        _fixedLevelGroup = CreateFixedGroup(property);
        container.Add(_fixedLevelGroup);

        ShowContextFields(typeEnum.enumValueIndex);

        return container;
    }

    private VisualElement CreateRandomGroup(SerializedProperty property)
    {
        var container = new VisualElement();
        container.Add(new UnityEditor.UIElements.PropertyField(property.FindPropertyRelative("CustomersCount")));
        container.Add(new UnityEditor.UIElements.PropertyField(property.FindPropertyRelative("TotalMealsNumber")));
        container.Add(new UnityEditor.UIElements.PropertyField(property.FindPropertyRelative("MaxMealsInOneOrder")));
        return container;
    }
    
    private VisualElement CreateFixedGroup(SerializedProperty property)
    {
        var container = new VisualElement();
        container.Add(new UnityEditor.UIElements.PropertyField(property.FindPropertyRelative("_orders")));
        return container;
    }

    private void ShowContextFields(int enumValueIndex)
    {
        bool isFixed = enumValueIndex == (int)LevelDataEntry.LevelType.Fixed;
        _fixedLevelGroup.SetVisibility(isFixed);
        _randomLevelGroup.SetVisibility(!isFixed);
    }
}
#endif
