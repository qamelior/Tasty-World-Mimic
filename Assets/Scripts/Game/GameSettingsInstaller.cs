using Game.Data.Levels;
using Restaurants;
using Restaurants.Customers;
using UnityEngine;
using Zenject;

namespace Game;

[CreateAssetMenu(menuName = "Data/Game Settings")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    public LevelManager.Settings Levels;
    public Restaurant.Settings Restaurant;
    public Customer.Settings Customer;
    public MealSource.Settings MealSource;
    public RestaurantInstaller.FactoryPrefabs Prefabs;

    public override void InstallBindings()
    {
        Container.BindInstance(Levels);
        Container.BindInstance(Restaurant);
        Container.BindInstance(Customer);
        Container.BindInstance(MealSource);
        Container.BindInstance(Prefabs);
    }
}