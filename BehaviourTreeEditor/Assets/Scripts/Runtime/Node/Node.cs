using System;
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

        [HideInInspector] public Vector2 position;
        [HideInInspector] public string guid;
        [HideInInspector] public State state = State.Running;
        [HideInInspector] public bool hasStarted = false;
        [TextArea] public string description = String.Empty;
        
        protected bool _hasValidConditions = true;

        public abstract string InspectorName { get; protected set; }

        public State Update()
        {
            if (!hasStarted)
            {
                OnStart();
                hasStarted = true;
            }

            if (!_hasValidConditions)
            {
                state = State.Failure;
                OnStop();
                hasStarted = false;
                _hasValidConditions = true;
                return state;
            }

            state = OnUpdate();

            if (state == State.Failure || state == State.Success)
            {
                OnStop();
                hasStarted = false;
            }

            return state;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }
        
        #region DragAndDropDataInjection

        public virtual void InjectData(object data)
        {
            if (data == null)
            {
                Debug.LogError("null Data");
                return;
            }
        }
        
        #endregion

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}
