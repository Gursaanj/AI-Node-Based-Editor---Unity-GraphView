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

        private State _state = State.Running;
        private bool _started = false;

        public State NodeState => _state;
        public bool Started => _started;

        public State Update()
        {
            if (!_started)
            {
                OnStart();
                _started = true;
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
