using System;
using UnityEngine;

namespace TastyWorld.Levels
{
    [Serializable]
    public class LevelData
    {
        public int CustomersCount;
        public int DishesCount;
        public int TimeToComplete;
        public int MaxDishesInOneOrder;
    }
}