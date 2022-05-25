using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public void Init(Transform root)
    {
        transform.position = root.position;
        
    }
}
