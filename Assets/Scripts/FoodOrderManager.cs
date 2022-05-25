using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOrderManager : MonoBehaviour
{

    public FoodOrderManager()
    {
        //for (var i = 0; i< )
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha0))
            SpawnCustomer();    
    }

    private void SpawnCustomer()
    {
        
    }
}
