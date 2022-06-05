using System;
using Game.Data.Levels;
using Restaurants.Customers;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            
            Container.Bind<Restaurants.Restaurant>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.Bind<LevelManager>().AsSingle();
        }
    }
}
