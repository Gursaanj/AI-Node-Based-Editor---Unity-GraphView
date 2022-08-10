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
        public Vector2 position;
        protected bool _hasValidConditions = true;
        
        private State _state = State.Running;
        private bool _started = false;
        private string _guid;

        public abstract string InspectorName { get; protected set; }
        
        public State NodeState => _state;
        public bool Started => _started;

        public string Guid
        {
            get 
        {
            if (string.IsNullOrEmpty(_guid))
            {
                Debug.LogError("invalid GUID, references in the GraphView might be lost");
            }
            return _guid;
        }
            set => _guid = value;
        }

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

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}
