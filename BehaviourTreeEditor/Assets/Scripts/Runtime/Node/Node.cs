using UnityEngine;

namespace Gbt
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        public string guid;

        protected bool _hasValidConditions = true;
        
        private State _state = State.Running;
        private bool _started = false;

        public abstract string InspectorName { get; protected set; }
        
        public State NodeState => _state;
        public bool Started => _started;

        public Vector2 Position { get; set; }

        public State Update()
        {
            if (!_started)
            {
                OnStart();
                _started = true;
            }

            if (!_hasValidConditions)
            {
                _state = State.Failure;
                OnStop();
                _started = false;
                _hasValidConditions = true;
                return _state;
            }

            _state = OnUpdate();

            if (_state == State.Failure || _state == State.Success)
            {
                OnStop();
                _started = false;
            }

            return _state;
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}
