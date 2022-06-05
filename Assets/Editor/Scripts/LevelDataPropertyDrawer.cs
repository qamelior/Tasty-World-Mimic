using _Extensions;
using Game.Data.Levels;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LevelData))]
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

        //container.style.backgroundColor = new StyleColor(new Color(0.56f, 0.56f, 0.56f, 0.35f));
        //container.Add(new Label($""));
        //container.Add(new PropertyField(property.FindPropertyRelative("_displayName")));
        container.Add(new PropertyField(property.FindPropertyRelative("TimeInSeconds")));
        
        //type
        var typeEnum = property.FindPropertyRelative("Type");
        var typeField = new PropertyField(typeEnum);
        typeField.RegisterValueChangeCallback(OnTypeChanged);
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
        container.Add(new PropertyField(property.FindPropertyRelative("CustomersCount")));
        container.Add(new PropertyField(property.FindPropertyRelative("TotalMealsNumber")));
        container.Add(new PropertyField(property.FindPropertyRelative("MaxMealsInOneOrder")));
        return container;
    }
    
    private VisualElement CreateFixedGroup(SerializedProperty property)
    {
        var container = new VisualElement();
        container.Add(new PropertyField(property.FindPropertyRelative("_orders")));
        return container;
    }

    private void ShowContextFields(int enumValueIndex)
    {
        var isFixed = enumValueIndex == (int)LevelData.LevelType.Fixed;
        _fixedLevelGroup.SetVisibility(isFixed);
        _randomLevelGroup.SetVisibility(!isFixed);
    }

    private void OnTypeChanged(SerializedPropertyChangeEvent evt)
    {
        ShowContextFields(evt.changedProperty.enumValueIndex);
    }
}
#endif
