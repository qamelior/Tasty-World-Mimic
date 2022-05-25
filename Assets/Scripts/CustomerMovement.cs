using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class CustomerMovement : MonoBehaviour
{
    private ReactiveProperty<Vector3> _destination;
    private readonly Settings _settings;
    
    public CustomerMovement(Settings settings)
    {
        _settings = settings;
    }
    
    private void Awake()
    {
        _destination.Subscribe(x => StartMove());
    }



    public void SetDestination(Vector3 destination)
    {
        _destination.Value = destination;
    }

    private void StartMove()
    {
        StopAllCoroutines();
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (!Extensions.IsClose(transform.position, _destination.Value))
        {
            transform.position =
                Vector3.Lerp(transform.position, _destination.Value, _settings.MoveSpeed * Time.deltaTime);
            yield return null;
        }
    }
    
    [Serializable]
    public class Settings
    {
        public float MoveSpeed;
    }
}
