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
        private Action _onArrive;
        private Settings _settings;
        private Vector3 _spawnLocation;
        private Vector3 _spotLocation;
        private State _state;
        private bool _isMoving;
        [Inject]
        public void Construct(GameController gameController, Settings settings)
        {
            gameController.SubscribeToGameStateChange(UpdateMoveState);
            _settings = settings;
        }

        private void UpdateMoveState(GameController.GameStates gameState)
        {
            _isMoving = gameState == GameController.GameStates.Playing;
        }


        public void Init(string id, Transform parent, Vector3 origin, Vector3 destination, Action onArrive)
        {
            transform.SetParent(parent);
            transform.position = _spawnLocation = origin;
            _idLabelRef.text = $"{id}";
            _onArrive = onArrive;
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
                if (_isMoving)
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