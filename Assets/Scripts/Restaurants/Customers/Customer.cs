using System;
using System.Collections;
using Game;
using TMPro;
using UnityEngine;
using Zenject;

namespace Restaurants.Customers
{
    public class Customer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _idLabelRef;
        private GameController _gameController;
        private Action _onArrive;
        private Settings _settings;
        private Vector3 _spawnLocation;
        private Vector3 _spotLocation;
        private State _state;

        public event Action OnArrive { add => _onArrive += value; remove => _onArrive -= value; }

        [Inject]
        public void Construct(GameController gameController, Settings settings)
        {
            _gameController = gameController;
            _settings = settings;
        }

        public void Init(Vector3 destination, string id)
        {
            _idLabelRef.text = $"{id}";
            _spawnLocation = transform.position;
            _spotLocation = destination;
            _state = State.Spawned;
            IterateState();
        }

        private IEnumerator Move(Vector3 destination)
        {
            var t = 0f;
            float moveDuration = _settings.MovementDuration;
            float lerpSpeedMod = 1f / moveDuration;
            var origin = transform.position;
            while (t < moveDuration)
            {
                if (!_gameController.GameIsPaused)
                {
                    transform.position = Vector3.Lerp(origin, destination, lerpSpeedMod * t);
                    t += Time.deltaTime;
                }

                yield return null;
            }

            transform.position = destination;
            IterateState();
        }

        public void MoveToExit()
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
                    _onArrive?.Invoke();
                    break;
                case State.WaitingForOrder:
                    StartCoroutine(Move(_spawnLocation));
                    break;
                case State.Leaving:
                    Destroy(gameObject);
                    return;
            }

            _state++;
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            if (GameController.ShowDebugLogs)
                Debug.Log($"Customer {_idLabelRef.text} is now {_state}");
        }

        private enum State
        {
            Spawned,
            Entering,
            WaitingForOrder,
            Leaving,
            Count
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