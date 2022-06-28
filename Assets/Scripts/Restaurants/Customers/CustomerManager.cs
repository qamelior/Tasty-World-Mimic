using Game.Data.Levels;

namespace Restaurants.Customers
{
    public class CustomerManager
    {
        private int _customerUID;
        public string GetCustomerUID() { return $"{_customerUID++}";}

        public CustomerManager(LevelManager levelManager)
        {
            levelManager.OnLevelStarted += ResetCounter;
        }

        private void ResetCounter(LevelDataEntry obj)
        {
            _customerUID = 1;
        }
        
    }
}