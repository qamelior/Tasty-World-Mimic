using System;
using System.Collections;
using _Extensions;
using TMPro;
using UnityEngine;
using Zenject;

namespace Restaurants.Customers
{
    public class Customer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _idLabelRef;
        private State _state;
        private Vector3 _spawnLocation;
        private Vector3 _spotLocation;
    
        private Action _onDestinationReached;
        private float _movementSpeed;
        private Settings _settings;

        [Inject]
        public void Construct(Settings settings)
        {
            _settings = settings;
        }

        public void Init(Vector3 destination, string id, Action onArrive)
        {
            _idLabelRef.text = $"{id}";
            _spawnLocation = transform.localPosition;
            _spotLocation = destination;
            _onDestinationReached += onArrive;
        
            _state = State.Entering;
            _movementSpeed = Vector3.Distance(_spawnLocation, _spotLocation) / _settings.MovementDuration;
        }

        private IEnumerator Move(Vector3 destination)
        {
            while (!Extensions.IsClose(transform.localPosition, destination))
            {
                transform.localPosition =
                    Vector3.Lerp(transform.localPosition, destination, _movementSpeed * Time.deltaTime);
                yield return null;
            }
            IterateState();
        }

        public void LeaveRestaurant()
        {
            if (_state != State.WaitingForOrder)
            {
                Debug.LogError("How did we end up here?");
                return;
            }
            IterateState();
        }
    

        private void IterateState()
        {
            switch (_state)
            {
                case State.Spawned:
                    StartCoroutine(Move(_spotLocation));
                    break;
                case State.Entering:
                    _onDestinationReached?.Invoke();
                    break;
                case State.WaitingForOrder:
                    StartCoroutine(Move(_spawnLocation));
                    break;
                case State.Leaving:
                    Destroy(gameObject);
                    return;
            }

            _state++;
        }

        private enum State
        {
            Spawned,
            Entering,
            WaitingForOrder,
            Leaving,
            Count,
        }
    
        
        [Serializable]
        public class Settings
        {
            public float MovementDuration = 5f;
        }

        public class Factory : PlaceholderFactory<Customer>
        {
        }
    }
}
