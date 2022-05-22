using UnityEngine;

namespace Gbt
{
    /// <summary>
    /// Adds delay (in seconds) for a wait action
    /// </summary>
    public class WaitNode : ActionNode
    {
        [Tooltip("The length of delay in seconds")]
        public float duration = 1.0f;
        
        private float _startTime;
        
        protected override void OnStart()
        {
            if (duration < 0.0f)
            {
                _hasValidConditions = false;
                Debug.LogWarning("WaitNode Duration cannot be less than 0.0 seconds, returning with Failure");
                return;
            }
            
            _startTime = Time.time;
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            if (Time.time - _startTime > duration)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
}
