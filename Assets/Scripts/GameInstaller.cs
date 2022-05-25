using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Container.Bind<string>().FromInstance("Hello World!");
        // Container.Bind<Greeter>().AsSingle().NonLazy();
    }
}
