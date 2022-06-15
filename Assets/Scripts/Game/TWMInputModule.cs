using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TWMInputModule : StandaloneInputModule
{
    [Header("Extensions")]
    public bool ShowHoverableObject;
    public GameObject CurrentlyFocusedObject;

    public override void UpdateModule()
    {
        base.UpdateModule();
        if (!ShowHoverableObject)
            return;
        CurrentlyFocusedObject = GetCurrentFocusedGameObject();
    }
}
