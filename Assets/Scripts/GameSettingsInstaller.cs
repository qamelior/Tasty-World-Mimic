using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Data/Game Settings")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    public OrderSettings Orders;
    public CustomerSettings Customer;
    [SerializeField] private int _test;

    public override void InstallBindings()
    {
        Container.BindInstance(Customer.Movement);
    }

    [Serializable]
    public class CustomerSettings
    {
        public CustomerMovement.Settings Movement;
    }
    
    [Serializable]
    public class OrderSettings
    {
        public int MaxNumberOfCustomers;
        public GameObject FoodOrderPrefab;
    }
}
